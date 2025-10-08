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
    public class RolesControl : UserControl
    {
        private readonly RolesApi api = new();
        private readonly BindingSource bs = new();
        private readonly DataGridView grid = new()
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AutoGenerateColumns = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AllowUserToAddRows = false
        };

        private readonly TextBox txtFilter = new() { PlaceholderText = "Filtrar rol", Width = 220 };
        private readonly Button btnBuscar = new() { Text = "Buscar" };
        private readonly Button btnLimpiar = new() { Text = "Limpiar" };

        private readonly TextBox txtName = new() { PlaceholderText = "Nombre del rol", Width = 240 };
        private readonly Button btnCreate = new() { Text = "Crear", Width = 90 };
        private readonly Button btnUpdate = new() { Text = "Actualizar", Width = 90 };
        private readonly Button btnDelete = new() { Text = "Borrar", Width = 90 };

        private List<RoleDto> _all = new();
        private int? _id = null;

        public RolesControl()
        {
            var tools = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 38, Padding = new Padding(8, 6, 8, 4), BackColor = Color.WhiteSmoke };
            tools.Controls.AddRange(new Control[] { txtFilter, btnBuscar, btnLimpiar });
            Controls.Add(grid);
            Controls.Add(tools);

            var editor = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 40, Padding = new Padding(8, 6, 8, 4), BackColor = Color.White };
            editor.Controls.AddRange(new Control[] { txtName, btnCreate, btnUpdate, btnDelete });
            Controls.Add(editor);

            btnCreate.Visible = btnUpdate.Visible = btnDelete.Visible = Session.IsAdmin;

            grid.DataSource = bs;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 252);
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "RoleId", HeaderText = "Id", Width = 60 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Name", HeaderText = "Nombre", Width = 240 });

            grid.SelectionChanged += (_, __) => FromSelection();
            btnBuscar.Click += (_, __) => ApplyFilter();
            btnLimpiar.Click += async (_, __) => { txtFilter.Text = ""; await LoadData(); };

            btnCreate.Click += async (_, __) => await CreateAsync();
            btnUpdate.Click += async (_, __) => await UpdateAsync();
            btnDelete.Click += async (_, __) => await DeleteAsync();

            bool loaded = false;
            VisibleChanged += async (_, __) => { if (Visible && !loaded) { loaded = true; await LoadData(); } };
        }

        private async Task LoadData()
        {
            try { _all = await api.GetAllAsync(); ApplyFilter(); }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void ApplyFilter()
        {
            var s = txtFilter.Text.Trim();
            bs.DataSource = string.IsNullOrWhiteSpace(s) ? _all : _all.Where(r => r.Name.Contains(s, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        private void FromSelection()
        {
            _id = null;
            if (grid.CurrentRow?.DataBoundItem is RoleDto r)
            {
                _id = r.RoleId;
                txtName.Text = r.Name;
            }
        }

        private async Task CreateAsync()
        {
            try { await api.CreateAsync(new RoleCreateRequest { Name = txtName.Text.Trim() }); await LoadData(); MessageBox.Show("Rol creado."); }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private async Task UpdateAsync()
        {
            if (_id is null) { MessageBox.Show("Selecciona un rol."); return; }
            try { await api.UpdateAsync(_id.Value, new RoleUpdateRequest { RoleId = _id.Value, Name = txtName.Text.Trim() }); await LoadData(); MessageBox.Show("Rol actualizado."); }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private async Task DeleteAsync()
        {
            if (_id is null) { MessageBox.Show("Selecciona un rol."); return; }
            if (MessageBox.Show("¿Borrar rol?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try { await api.DeleteAsync(_id.Value); await LoadData(); }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }
    }
}
