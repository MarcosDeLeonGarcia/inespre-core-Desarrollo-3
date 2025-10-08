#nullable enable
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using INESPRE.Desktop.Core;
using INESPRE.Desktop.UI.Controls;

namespace INESPRE.Desktop.UI.Forms
{
    public partial class MainForm : Form
    {
        // =======================
        // Vistas persistentes
        // =======================
        private readonly EventsControl eventsCtl = new();
        private readonly ProductsControl productsCtl = new();
        private readonly UsersControl usersCtl = new();

        // Admin
        private readonly ProducersControl producersCtl = new();
        private readonly RolesControl rolesCtl = new();
        private readonly InventoryLotsControl inventoryCtl = new();
        private readonly PurchaseOrdersControl purchasesCtl = new();
        private readonly PaymentsControl paymentsCtl = new();
        private readonly SalesControl salesCtl = new();

        // Bloque Admin
        private const string AdminHostName = "panelAdminHost";
        private FlowLayoutPanel? _adminHost;

        // =======================
        // Constructor
        // =======================
        public MainForm()
        {
            InitializeComponent();

            // Guardas mínimas para evitar nulls en runtime
            ArgumentNullException.ThrowIfNull(panelHeader);
            ArgumentNullException.ThrowIfNull(panelBody);
            ArgumentNullException.ThrowIfNull(lblUser);
            ArgumentNullException.ThrowIfNull(btnDashboard);
            ArgumentNullException.ThrowIfNull(btnEventos);
            ArgumentNullException.ThrowIfNull(btnProductos);
            ArgumentNullException.ThrowIfNull(btnUsuarios);
            ArgumentNullException.ThrowIfNull(btnLogout);

            // Info de usuario (arriba derecha)
            lblUser!.Text = $"{Session.Username} · {Session.Role}";

            // Navegación base
            btnLogout.Click     += (_, __) => { Session.Clear(); Application.Restart(); };
            btnDashboard.Click  += (_, __) => LoadView(new LabelView("Bienvenido a INESPRE"));
            btnEventos.Click    += (_, __) => LoadView(eventsCtl);
            btnProductos.Click  += (_, __) => LoadView(productsCtl);
            btnUsuarios.Click   += (_, __) => LoadView(usersCtl);

            // Estilo uniforme del menú lateral y creación del bloque Admin
            StyleBaseMenu();
            CreateAdminMenu();   // <- limpia botones viejos y construye el bloque ADMINISTRACIÓN
            ApplyPermissions();

            // Vista inicial
            LoadView(new LabelView("Bienvenido a INESPRE"));

            // Botón "Cambiar contraseña" en Header
            var btnChangePwd = new Button
            {
                Text = "Cambiar contraseña",
                Width = 150,
                Height = 28,
                Left = panelHeader.Width - 1000,
                Top  = 20,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnChangePwd.Click += (_, __) =>
            {
                using var dlg = new ChangePasswordForm();
                dlg.ShowDialog(this);
            };
            panelHeader!.Controls.Add(btnChangePwd);
        }

        // =========================================================
        // ESTÉTICA DEL MENÚ LATERAL (botones principales)
        // =========================================================
        private void StyleBaseMenu()
        {
            var side = btnDashboard!.Parent
                       ?? throw new InvalidOperationException("No se encontró el contenedor del menú lateral.");

            // Ancho mínimo razonable
            if (side.Width < 210) side.Width = 210;

            var fnt = new Font("Segoe UI", 9.5f, FontStyle.Regular);

            void StyleButton(Button b)
            {
                b.Height = 36;
                b.Dock   = DockStyle.Top;
                b.FlatStyle = FlatStyle.Flat;
                b.FlatAppearance.BorderSize = 0;
                b.TextAlign = ContentAlignment.MiddleLeft;
                b.Padding   = new Padding(14, 0, 10, 0);
                b.BackColor = Color.White;
                b.Font      = fnt;
                b.Cursor    = Cursors.Hand;
            }

            foreach (var b in new[] { btnDashboard, btnEventos, btnProductos, btnUsuarios })
                StyleButton(b!);

            // Asegurar orden de arriba hacia abajo: Dashboard, Eventos, Productos, Usuarios
            side.Controls.SetChildIndex(btnUsuarios!, 0);
            side.Controls.SetChildIndex(btnProductos!, 0);
            side.Controls.SetChildIndex(btnEventos!, 0);
            side.Controls.SetChildIndex(btnDashboard!, 0);
        }

        // =========================================================
        // BLOQUE ADMIN – crea el bloque bonito y limpia lo antiguo
        // =========================================================
        private void CreateAdminMenu()
        {
            var side = btnDashboard!.Parent
                ?? throw new InvalidOperationException("No se encontró el contenedor del menú lateral.");

            // 1) Eliminar cualquier rastro de controles viejos del diseñador
            RemoveLegacyAdminMenuItems(side);

            // 2) Construir host del bloque ADMIN
            //    (si hubiera uno previo, también lo eliminamos)
            foreach (Control c in side.Controls.Find(AdminHostName, true))
            {
                side.Controls.Remove(c);
                c.Dispose();
            }

            _adminHost = new FlowLayoutPanel
            {
                Name = AdminHostName,
                FlowDirection = FlowDirection.TopDown,
                WrapContents  = false,
                AutoSize      = true,
                AutoSizeMode  = AutoSizeMode.GrowAndShrink,
                Dock          = DockStyle.Top,
                BackColor     = Color.White,
                Margin        = new Padding(0),
                Padding       = new Padding(0, 8, 0, 8)
            };

            // Insertar justo debajo de Usuarios
            side.Controls.Add(_adminHost);
            var idxUsuarios = side.Controls.GetChildIndex(btnUsuarios!);
            side.Controls.SetChildIndex(_adminHost, idxUsuarios);

            // Encabezado
            _adminHost.Controls.Add(BuildAdminHeader());

            // Botones del bloque
            _adminHost.Controls.Add(BuildAdminButton("Roles", (_, __) => LoadView(rolesCtl), "admBtnRoles"));
            _adminHost.Controls.Add(BuildAdminButton("Productores", (_, __) => LoadView(producersCtl), "admBtnProducers"));
            _adminHost.Controls.Add(BuildAdminButton("Inventario", (_, __) => LoadView(inventoryCtl), "admBtnInventory"));
            _adminHost.Controls.Add(BuildAdminButton("Órdenes compra", (_, __) => LoadView(purchasesCtl), "admBtnPurchases"));
            _adminHost.Controls.Add(BuildAdminButton("Pagos", (_, __) => LoadView(paymentsCtl), "admBtnPayments"));
            _adminHost.Controls.Add(BuildAdminButton("Ventas", (_, __) => LoadView(salesCtl), "admBtnSales"));
        }

        private Control BuildAdminHeader()
        {
            var header = new Panel
            {
                Height = 28,
                Width  = (_adminHost?.Width ?? 200),
                BackColor = Color.Gainsboro,
                Margin = new Padding(0, 0, 0, 6)
            };

            var lbl = new Label
            {
                Text = "ADMINISTRACIÓN",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.Black,
                BackColor = Color.Transparent
            };

            header.Controls.Add(lbl);
            return header;
        }

        private Button BuildAdminButton(string text, EventHandler click, string name)
        {
            var b = new Button
            {
                Name  = name,
                Text  = text,
                Width = (_adminHost?.Width ?? 200) - 8,
                Height = 32,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                Margin = new Padding(4, 2, 4, 0),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding   = new Padding(16, 0, 10, 0),
                Cursor    = Cursors.Hand
            };
            b.FlatAppearance.BorderSize = 0;
            b.Click += click;
            return b;
        }

        // =========================================================
        // PERMISOS POR ROL
        // =========================================================
        private void ApplyPermissions()
        {
            if (Session.IsAdmin)
            {
                btnDashboard!.Visible = true;
                btnEventos!.Visible   = true;
                btnProductos!.Visible = true;
                btnUsuarios!.Visible  = true;
                if (_adminHost != null) _adminHost.Visible = true;
            }
            else
            {
                // Usuarios no admin: oculta bloque admin
                btnUsuarios!.Visible  = false;
                if (_adminHost != null) _adminHost.Visible = false;
            }
        }

        // =========================================================
        // CARGA DE VISTAS EN EL PANEL CENTRAL
        // =========================================================
        private void LoadView(Control c)
        {
            panelBody!.Controls.Clear();
            c.Dock = DockStyle.Fill;
            panelBody.Controls.Add(c);
            c.BringToFront();
        }

        // =========================================================
        // UTILIDADES – limpieza de controles "viejos" del diseñador
        // =========================================================
        private static string Normalize(string s)
        {
            s ??= string.Empty;
            var t = s.Trim().ToLowerInvariant();
            t = t.Replace("á", "a").Replace("é", "e").Replace("í", "i")
                 .Replace("ó", "o").Replace("ú", "u").Replace("ñ", "n");
            return t;
        }

        private static readonly string[] AdminTexts =
        {
            "roles", "productores", "inventario",
            "ordenes compra", "ordenes de compra", "ordenes",
            "pagos", "ventas"
        };

        private static bool IsLegacyAdminButton(Button b)
        {
            var txt = Normalize(b.Text);
            return AdminTexts.Any(k => txt == k);
        }

        /// <summary>
        /// Elimina del panel lateral cualquier botón/label heredado del diseñador
        /// que represente el antiguo menú de administración (para evitar duplicados).
        /// </summary>
        private void RemoveLegacyAdminMenuItems(Control side)
        {
            // Conserva solo los 4 botones base del menú
            var keep = new[] { btnDashboard, btnEventos, btnProductos, btnUsuarios }
                        .Where(x => x != null).Cast<Control>().ToHashSet();

            // Labels tipo "Usuarios (ADMIN)" u otros separadores antiguos
            foreach (var lab in side.Controls.OfType<Label>().ToList())
            {
                var t = Normalize(lab.Text);
                if (t.Contains("usuarios") && t.Contains("admin"))
                {
                    side.Controls.Remove(lab);
                    lab.Dispose();
                }
            }

            // Botones del admin "viejo" (eliminarlos)
            foreach (var b in side.Controls.OfType<Button>().ToList())
            {
                if (keep.Contains(b)) continue; // no tocar los 4 base
                if (IsLegacyAdminButton(b))
                {
                    side.Controls.Remove(b);
                    b.Dispose();
                }
            }

            // Host antiguos con el mismo nombre
            foreach (Control c in side.Controls.Find(AdminHostName, true))
            {
                side.Controls.Remove(c);
                c.Dispose();
            }
        }

        // =========================================================
        // Vista placeholder para Dashboard
        // =========================================================
        private sealed class LabelView : Panel
        {
            public LabelView(string text)
            {
                BackColor = Color.White;
                var lbl = new Label
                {
                    Text = text,
                    Font = new Font("Segoe UI", 14f),
                    Location = new Point(20, 20),
                    AutoSize = true
                };
                Controls.Add(lbl);
            }
        }
    }
}
