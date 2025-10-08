using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using INESPRE.Desktop.Api;
using INESPRE.Desktop.Core;

namespace INESPRE.Desktop.UI.Controls
{
    public class SalesControl : UserControl
    {
        private readonly SalesApi api = new();
        private readonly EventsApi evApi = new();
        private readonly UsersApi usersApi = new();

        private readonly BindingSource bs = new();
        private readonly DataGridView grid = new()
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AutoGenerateColumns = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AllowUserToAddRows = false
        };

        // Filtros
        private readonly DateTimePicker dtFrom = new() { Format = DateTimePickerFormat.Custom, CustomFormat = "yyyy-MM-dd", Width = 110 };
        private readonly DateTimePicker dtTo = new() { Format = DateTimePickerFormat.Custom, CustomFormat = "yyyy-MM-dd", Width = 110 };
        private readonly ComboBox cbStatusF = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 120 };
        private readonly Button btnBuscar = new() { Text = "Buscar" };
        private readonly Button btnLimpiar = new() { Text = "Limpiar" };

        // Editor
        private readonly DateTimePicker dtSale = new() { Format = DateTimePickerFormat.Custom, CustomFormat = "yyyy-MM-dd", Width = 110 };
        private readonly ComboBox cbEvent = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 200 };
        private readonly ComboBox cbUser = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 180 };
        private readonly ComboBox cbPayMethod = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 140 };
        private readonly TextBox txtAmount = new() { PlaceholderText = "Monto (0.00)", Width = 110 };
        private readonly ComboBox cbStatus = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 120 };
        private readonly Button btnCreate = new() { Text = "Crear", Width = 90 };
        private readonly Button btnUpdate = new() { Text = "Actualizar", Width = 90 };
        private readonly Button btnDelete = new() { Text = "Borrar", Width = 90 };

        // Datos en memoria
        private List<SaleDto> _all = new();
        private List<EventDto> _events = new();
        private List<UserDto> _users = new();
        private int? _id = null;

        public SalesControl()
        {
            // combos de estado
            cbStatusF.Items.AddRange(new[] { "(Todos)", "ABIERTA", "CERRADA", "ANULADA" }); cbStatusF.SelectedIndex = 0;
            cbStatus.Items.AddRange(new[] { "ABIERTA", "CERRADA", "ANULADA" }); cbStatus.SelectedIndex = 0;

            // métodos de pago (puedes ajustar la lista)
            cbPayMethod.Items.AddRange(new[] { "EFECTIVO", "TRANSFERENCIA", "TARJETA", "CHEQUE" });
            cbPayMethod.SelectedIndex = 0;

            // barra de filtros
            var tools = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 38, Padding = new Padding(8, 6, 8, 4), BackColor = Color.WhiteSmoke };
            tools.Controls.AddRange(new Control[] {
                new Label{Text="Desde",AutoSize=true,Padding=new Padding(0,6,0,0)}, dtFrom,
                new Label{Text="Hasta",AutoSize=true,Padding=new Padding(8,6,0,0)}, dtTo,
                cbStatusF, btnBuscar, btnLimpiar
            });
            Controls.Add(grid);
            Controls.Add(tools);

            // barra editor
            var editor = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 40, Padding = new Padding(8, 6, 8, 4), BackColor = Color.White };
            editor.Controls.AddRange(new Control[] {
                dtSale,
                new Label{Text="Evento",AutoSize=true,Padding=new Padding(8,6,0,0)}, cbEvent,
                new Label{Text="Usuario",AutoSize=true,Padding=new Padding(8,6,0,0)}, cbUser,
                new Label{Text="Método",AutoSize=true,Padding=new Padding(8,6,0,0)}, cbPayMethod,
                txtAmount,
                cbStatus, btnCreate, btnUpdate, btnDelete
            });
            Controls.Add(editor);

            btnCreate.Visible = btnUpdate.Visible = btnDelete.Visible = Session.IsAdmin;

            // grid
            grid.DataSource = bs;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 252);

            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SaleId", HeaderText = "Id", Width = 50 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SaleDate", HeaderText = "Fecha", Width = 110, DefaultCellStyle = new DataGridViewCellStyle { Format = "yyyy-MM-dd" } });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "EventName", HeaderText = "Evento", Width = 180 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "UserName", HeaderText = "Usuario", Width = 140 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "PaymentMethod", HeaderText = "Método", Width = 110 }); // NUEVO
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Total", HeaderText = "Total", Width = 90, DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" } });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Status", HeaderText = "Status", Width = 100 });

            // eventos UI
            grid.SelectionChanged += (_, __) => FromSelection();
            btnBuscar.Click +=     (_, __) => ApplyFilter();

            btnLimpiar.Click += async (_, __) =>
            {
                cbStatusF.SelectedIndex = 0;
                dtFrom.Value = DateTime.Today.AddMonths(-1);
                dtTo.Value   = DateTime.Today;
                await LoadData();
            };

            btnCreate.Click += async (_, __) => await CreateAsync();
            btnUpdate.Click += async (_, __) => await UpdateAsync();
            btnDelete.Click += async (_, __) => await DeleteAsync();

            bool loaded = false;
            VisibleChanged += async (_, __) => { if (Visible && !loaded) { loaded = true; await LoadData(); } };
        }

        // item p/combos
        private sealed record Item(int Id, string Text) { public override string ToString() => Text; }

        private async Task LoadData()
        {
            var salesTask = api.GetAllAsync();
            var eventsTask = evApi.GetAllAsync();
            var usersTask = usersApi.GetAllAsync();

            await Task.WhenAll(salesTask, eventsTask, usersTask);

            _all    = salesTask.Result ?? new();
            _events = eventsTask.Result ?? new();
            _users  = usersTask.Result  ?? new();

            // mapear nombres para el grid
            var evById = _events.ToDictionary(e => e.EventId, e => e.Name ?? $"Evento {e.EventId}");
            var usById = _users.ToDictionary(u => u.UserId, u => string.IsNullOrWhiteSpace(u.FullName) ? u.UserName : u.FullName);

            foreach (var s in _all)
            {
                s.EventName = (s.EventId.HasValue && evById.TryGetValue(s.EventId.Value, out var en)) ? en : "(Sin evento)";
                s.UserName  = usById.TryGetValue(s.UserId, out var un) ? un : $"Usuario {s.UserId}";
            }

            // combos editor (evento/usuario)
            var evItems = new List<Item> { new Item(0, "(Sin evento)") };
            evItems.AddRange(_events.Select(e => new Item(e.EventId, string.IsNullOrWhiteSpace(e.Name) ? $"Evento {e.EventId}" : e.Name!)));
            cbEvent.DataSource = evItems; cbEvent.SelectedIndex = 0;

            var userItems = _users.Select(u => new Item(u.UserId, string.IsNullOrWhiteSpace(u.FullName) ? u.UserName : u.FullName!)).ToList();
            cbUser.DataSource = userItems;

            // rango por datos
            if (_all.Count > 0)
            {
                var min = _all.Min(x => x.SaleDate).Date;
                var max = _all.Max(x => x.SaleDate).Date;
                dtFrom.Value = min; dtTo.Value = max;
            }
            else
            {
                dtFrom.Value = DateTime.Today.AddMonths(-1);
                dtTo.Value   = DateTime.Today;
            }

            ApplyFilter();
        }

        private void ApplyFilter()
        {
            IEnumerable<SaleDto> q = _all;

            var from = dtFrom.Value.Date; var to = dtTo.Value.Date;
            if (from > to) (from, to) = (to, from);
            q = q.Where(x => x.SaleDate.Date >= from && x.SaleDate.Date <= to);

            if (cbStatusF.SelectedIndex > 0)
                q = q.Where(x => string.Equals(x.Status, cbStatusF.SelectedItem!.ToString(), StringComparison.OrdinalIgnoreCase));

            bs.DataSource = q.OrderByDescending(x => x.SaleId).ToList();
        }

        private void FromSelection()
        {
            _id = null;
            if (grid.CurrentRow?.DataBoundItem is SaleDto d)
            {
                _id = d.SaleId;

                dtSale.Value = d.SaleDate;

                // evento
                var evItem = cbEvent.Items.Cast<Item>().FirstOrDefault(i => i.Id == (d.EventId ?? 0));
                if (evItem != null) cbEvent.SelectedItem = evItem; else cbEvent.SelectedIndex = 0;

                // usuario
                var usItem = cbUser.Items.Cast<Item>().FirstOrDefault(i => i.Id == d.UserId);
                if (usItem != null) cbUser.SelectedItem = usItem;

                // método y monto
                cbPayMethod.SelectedItem = d.PaymentMethod?.ToUpperInvariant();
                txtAmount.Text = d.Total.ToString("0.00");

                cbStatus.SelectedItem = d.Status.ToUpperInvariant();
            }
        }

        private async Task CreateAsync()
        {
            try
            {
                int? eventId = null;
                if (cbEvent.SelectedItem is Item ei && ei.Id > 0) eventId = ei.Id;

                var userId = (cbUser.SelectedItem as Item)?.Id ?? 0;
                if (userId <= 0) { MessageBox.Show("Selecciona un usuario."); return; }

                if (!decimal.TryParse(txtAmount.Text, out var amount) || amount < 0)
                {
                    MessageBox.Show("Monto inválido."); return;
                }

                var method = cbPayMethod.SelectedItem?.ToString() ?? "EFECTIVO";

                await api.CreateAsync(new SaleCreateRequest
                {
                    SaleDate = dtSale.Value.Date,
                    EventId = eventId,
                    UserId = userId,
                    PaymentMethod = method,
                    Total = amount,
                    Status = cbStatus.SelectedItem!.ToString()!
                });

                await LoadData();
                MessageBox.Show("Venta creada.");
            }
            catch (Exception ex) { MessageBox.Show("Error al crear: " + ex.Message); }
        }

        private async Task UpdateAsync()
        {
            if (_id is null) { MessageBox.Show("Selecciona una venta."); return; }
            try
            {
                int? eventId = null;
                if (cbEvent.SelectedItem is Item ei && ei.Id > 0) eventId = ei.Id;

                var userId = (cbUser.SelectedItem as Item)?.Id ?? 0;
                if (userId <= 0) { MessageBox.Show("Selecciona un usuario."); return; }

                if (!decimal.TryParse(txtAmount.Text, out var amount) || amount < 0)
                {
                    MessageBox.Show("Monto inválido."); return;
                }

                var method = cbPayMethod.SelectedItem?.ToString() ?? "EFECTIVO";

                await api.UpdateAsync(_id.Value, new SaleUpdateRequest
                {
                    SaleId = _id.Value,
                    SaleDate = dtSale.Value.Date,
                    EventId = eventId,
                    UserId = userId,
                    PaymentMethod = method,
                    Total = amount,
                    Status = cbStatus.SelectedItem!.ToString()!
                });

                await LoadData();
                MessageBox.Show("Venta actualizada.");
            }
            catch (Exception ex) { MessageBox.Show("Error al actualizar: " + ex.Message); }
        }

        private async Task DeleteAsync()
        {
            if (_id is null) { MessageBox.Show("Selecciona una venta."); return; }
            if (MessageBox.Show("¿Borrar venta?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try { await api.DeleteAsync(_id.Value); await LoadData(); }
            catch (Exception ex) { MessageBox.Show("Error al borrar: " + ex.Message); }
        }
    }
}
