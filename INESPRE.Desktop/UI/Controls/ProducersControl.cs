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
    public class ProducersControl : UserControl
    {
        private readonly ProducersApi api = new();
        private readonly BindingSource bs = new();
        private readonly DataGridView grid = new()
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AutoGenerateColumns = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AllowUserToAddRows = false,
        };

        // filtros
        private readonly TextBox txtFilter = new() { PlaceholderText = "Filtrar por nombre", Width = 220 };
        private readonly CheckBox chkActivos = new() { Text = "Activos", Checked = true, AutoSize = true };
        private readonly Button btnBuscar = new() { Text = "Buscar" };
        private readonly Button btnLimpiar = new() { Text = "Limpiar" };

        // editor
        private readonly TextBox txtName = new() { PlaceholderText = "Nombre", Width = 180 };
        private readonly TextBox txtPhone = new() { PlaceholderText = "Teléfono", Width = 120 };
        private readonly TextBox txtAddress = new() { PlaceholderText = "Dirección", Width = 220 };
        private readonly CheckBox chkActive = new() { Text = "Activo", Checked = true, AutoSize = true };
        private readonly Button btnCreate = new() { Text = "Crear", Width = 90 };
        private readonly Button btnUpdate = new() { Text = "Actualizar", Width = 90 };
        private readonly Button btnDelete = new() { Text = "Borrar", Width = 90 };

        private List<ProducerDto> _all = new();
        private int? _id = null;

        public ProducersControl()
        {
            // top bar
            var tools = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 38, Padding = new Padding(8, 6, 8, 4), BackColor = Color.WhiteSmoke };
            tools.Controls.AddRange(new Control[] { txtFilter, chkActivos, btnBuscar, btnLimpiar });
            Controls.Add(grid);
            Controls.Add(tools);

            // editor
            var editor = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 40, Padding = new Padding(8, 6, 8, 4), BackColor = Color.White };
            editor.Controls.AddRange(new Control[] { txtName, txtPhone, txtAddress, chkActive, btnCreate, btnUpdate, btnDelete });
            Controls.Add(editor);

            // permisos
            btnCreate.Visible = btnUpdate.Visible = btnDelete.Visible = Session.IsAdmin;

            // grid
            grid.DataSource = bs;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 252);
            BuildColumns();
            grid.SelectionChanged += (_, __) => FromSelection();

            // events
            btnBuscar.Click += (_, __) => ApplyFilter();
            btnLimpiar.Click += async (_, __) => { txtFilter.Text = ""; chkActivos.Checked = true; await LoadData(); };
            btnCreate.Click += async (_, __) => await CreateAsync();
            btnUpdate.Click += async (_, __) => await UpdateAsync();
            btnDelete.Click += async (_, __) => await DeleteAsync();

            bool loaded = false;
            VisibleChanged += async (_, __) =>
            {
                if (Visible && !loaded) { loaded = true; await LoadData(); }
            };
        }

        private void BuildColumns()
        {
            grid.Columns.Clear();
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ProducerId", HeaderText = "Id", Width = 60 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Name", HeaderText = "Nombre", Width = 180 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Phone", HeaderText = "Teléfono", Width = 120 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Address", HeaderText = "Dirección", Width = 260 });
            grid.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = "Active", HeaderText = "Activo", Width = 60 });
        }

        private async Task LoadData()
        {
            try
            {
                _all = await api.GetAllAsync();

                // Failsafe opcional: si viniera algún status irregular
                foreach (var x in _all)
                {
                    if (!x.Active && !string.IsNullOrWhiteSpace(x.Status))
                        x.Active = x.Status.Trim().ToUpperInvariant() == "ACTIVO";
                }

                ApplyFilter();
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void ApplyFilter()
        {
            IEnumerable<ProducerDto> q = _all;
            var s = txtFilter.Text.Trim();
            if (!string.IsNullOrWhiteSpace(s))
                q = q.Where(x => (x.Name ?? "").IndexOf(s, StringComparison.OrdinalIgnoreCase) >= 0);
            if (chkActivos.Checked)
                q = q.Where(x => x.Active);
            bs.DataSource = q.ToList();
        }

        private void FromSelection()
        {
            _id = null;
            if (grid.CurrentRow?.DataBoundItem is ProducerDto d)
            {
                _id = d.ProducerId;
                txtName.Text = d.Name ?? "";
                txtPhone.Text = d.Phone ?? "";
                txtAddress.Text = d.Address ?? "";
                chkActive.Checked = d.Active;
            }
        }

        private async Task CreateAsync()
        {
            try
            {
                await api.CreateAsync(new ProducerCreateRequest
                {
                    Name = txtName.Text.Trim(),
                    Phone = txtPhone.Text.Trim(),
                    Address = txtAddress.Text.Trim(),
                    Active = chkActive.Checked
                });
                await LoadData();
                MessageBox.Show("Productor creado.");
            }
            catch (Exception ex) { MessageBox.Show("Error al crear: " + ex.Message); }
        }

        private async Task UpdateAsync()
        {
            if (_id is null) { MessageBox.Show("Selecciona un productor."); return; }
            try
            {
                await api.UpdateAsync(_id.Value, new ProducerUpdateRequest
                {
                    ProducerId = _id.Value,
                    Name = txtName.Text.Trim(),
                    Phone = txtPhone.Text.Trim(),
                    Address = txtAddress.Text.Trim(),
                    Active = chkActive.Checked
                });
                await LoadData();
                MessageBox.Show("Productor actualizado.");
            }
            catch (Exception ex) { MessageBox.Show("Error al actualizar: " + ex.Message); }
        }

        private async Task DeleteAsync()
        {
            if (_id is null) { MessageBox.Show("Selecciona un productor."); return; }
            if (MessageBox.Show("¿Borrar productor?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try { await api.DeleteAsync(_id.Value); await LoadData(); }
            catch (Exception ex) { MessageBox.Show("Error al borrar: " + ex.Message); }
        }
    }
}
