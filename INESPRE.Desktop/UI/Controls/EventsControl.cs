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
    public class EventsControl : UserControl
    {
        private readonly EventsApi eventsApi = new();
        private readonly UsersApi usersApi = new();
        private readonly Dictionary<int, string> _userNameCache = new(); // userId -> nombre

        private readonly BindingSource bs = new();

        private readonly DataGridView grid = new()
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AutoGenerateColumns = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false
        };

        // Barra de filtros
        private readonly TextBox txtFilter = new() { PlaceholderText = "Filtrar por nombre", Width = 220 };
        private readonly TextBox txtProvinceFilter = new() { PlaceholderText = "Provincia", Width = 90 };
        private readonly Button btnSearch = new() { Text = "Buscar", Width = 70 };
        private readonly Button btnClear = new() { Text = "Limpiar", Width = 70 };

        // Editor / Altas
        private readonly TextBox txtName = new() { PlaceholderText = "Nombre", Width = 150 };
        private readonly ComboBox cbType = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 120 };
        private readonly DateTimePicker dtp = new() { Format = DateTimePickerFormat.Custom, CustomFormat = "yyyy-MM-dd HH:mm:ss", Width = 160 };
        private readonly TextBox txtProv = new() { PlaceholderText = "Provincia", Text = "SD", Width = 60 };
        private readonly TextBox txtMun = new() { PlaceholderText = "Municipio", Text = "DN", Width = 60 };
        private readonly TextBox txtVenue = new() { PlaceholderText = "Lugar", Width = 140 };
        private readonly TextBox txtAddress = new() { PlaceholderText = "Dirección", Width = 180 };
        private readonly ComboBox cbStatus = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 140 };

        private readonly Button btnCreate = new() { Text = "Crear", Width = 90 };
        private readonly Button btnUpdate = new() { Text = "Actualizar", Width = 90 };
        private readonly Button btnDelete = new() { Text = "Borrar", Width = 90 };

        private List<EventDto> _all = new();
        private int? _selectedId = null;

        public EventsControl()
        {
            // Fila de filtros
            var tools = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 38, Padding = new Padding(8, 6, 8, 4), BackColor = Color.WhiteSmoke };
            tools.Controls.AddRange(new Control[] { txtFilter, btnSearch, btnClear, new Label { Text = "Provincia", AutoSize = true, Padding = new Padding(12, 6, 0, 0) }, txtProvinceFilter });
            Controls.Add(grid);
            Controls.Add(tools);

            // Editor (altas/edición)
            cbType.Items.AddRange(new[] { "OPERATIVO", "VENTA", "FERIA", "OTRO" });
            cbType.SelectedIndex = 0;
            cbStatus.Items.AddRange(new[] { "PLANIFICADO", "EN_PROCESO", "CERRADO", "CANCELADO" });
            cbStatus.SelectedIndex = 0;

            var editor = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 40, Padding = new Padding(8, 6, 8, 4), BackColor = Color.White };
            editor.Controls.AddRange(new Control[] { txtName, cbType, dtp, txtProv, txtMun, txtVenue, txtAddress, cbStatus, btnCreate, btnUpdate, btnDelete });
            Controls.Add(editor);

            // Permisos
            btnCreate.Visible = Session.IsAdmin;
            btnUpdate.Visible = Session.IsAdmin;
            btnDelete.Visible = Session.IsAdmin;

            // Grid
            grid.DataSource = bs;
            BuildColumns();
            grid.SelectionChanged += (_, __) => PopulateFromSelection();
            grid.CellFormatting += Grid_CellFormatting;

            // Eventos
            btnSearch.Click += (_, __) => ApplyFilter();
            btnClear.Click += async (_, __) => { txtFilter.Text = ""; txtProvinceFilter.Text = ""; await LoadData(); };

            btnCreate.Click += async (_, __) => await CreateAsync();
            btnUpdate.Click += async (_, __) => await UpdateAsync();
            btnDelete.Click += async (_, __) => await DeleteAsync();

            // Cargar al entrar
            bool loaded = false;
            VisibleChanged += async (_, __) =>
            {
                if (Visible && !loaded)
                {
                    loaded = true;
                    await LoadData();
                }
            };
        }

        private void BuildColumns()
        {
            grid.Columns.Clear();

            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "EventId", HeaderText = "EventId", Width = 70 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Name", HeaderText = "Name", Width = 160 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "EventType", HeaderText = "EventType", Width = 100 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "EventDateTime", HeaderText = "EventDateTime", Width = 145, DefaultCellStyle = new DataGridViewCellStyle { Format = "yyyy-MM-dd HH:mm" } });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Province", HeaderText = "Province", Width = 90 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Municipality", HeaderText = "Municipality", Width = 100 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Venue", HeaderText = "Venue", Width = 120 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Address", HeaderText = "Address", Width = 160 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Status", HeaderText = "Status", Width = 110 });

            // Columna "Creado por" (se llenará con CreatedByName; si no, con el Id)
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "CreadoPor", HeaderText = "Creado por", Width = 160 });
        }

        private void Grid_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var col = grid.Columns[e.ColumnIndex].Name;
            if (col == "CreadoPor")
            {
                if (grid.Rows[e.RowIndex].DataBoundItem is EventDto dto)
                {
                    e.Value = string.IsNullOrWhiteSpace(dto.CreatedByName)
                        ? (dto.CreatedBy?.ToString() ?? "")
                        : dto.CreatedByName;
                    e.FormattingApplied = true;
                }
            }
        }

        private async Task LoadData()
        {
            try
            {
                eventsApi.RefreshAuth();
                _all = await eventsApi.GetAllAsync();        // traemos todos
                await ResolveCreatorsAsync(_all);            // resolvemos nombres
                ApplyFilter();                               // aplicamos filtro actual
            }
            catch (Exception ex) { MessageBox.Show("Error al cargar eventos: " + ex.Message); }
        }

        private void ApplyFilter()
        {
            var name = txtFilter.Text.Trim();
            var prov = txtProvinceFilter.Text.Trim();

            IEnumerable<EventDto> q = _all;

            if (!string.IsNullOrWhiteSpace(name))
                q = q.Where(e => (e.Name ?? "").IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0);

            if (!string.IsNullOrWhiteSpace(prov))
                q = q.Where(e => string.Equals(e.Province ?? "", prov, StringComparison.OrdinalIgnoreCase));

            bs.DataSource = q.ToList();
        }

        private async Task ResolveCreatorsAsync(IEnumerable<EventDto> items)
        {
            // IDs de usuario distintos presentes en la lista
            var ids = items
                .Select(e => e.CreatedBy)
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .Distinct()
                .ToList();

            // Qué ids me faltan en caché
            var missing = ids.Where(id => !_userNameCache.ContainsKey(id)).ToList();
            foreach (var id in missing)
            {
                try
                {
                    usersApi.RefreshAuth();
                    var u = await usersApi.GetByIdAsync(id);
                    var display = !string.IsNullOrWhiteSpace(u?.FullName) ? u!.FullName!
                                 : !string.IsNullOrWhiteSpace(u?.UserName) ? u!.UserName!
                                 : id.ToString();
                    _userNameCache[id] = display;
                }
                catch
                {
                    _userNameCache[id] = id.ToString();
                }
            }

            // Asigna el nombre resuelto a cada evento
            foreach (var e in items)
            {
                if (e.CreatedBy.HasValue && _userNameCache.TryGetValue(e.CreatedBy.Value, out var name))
                    e.CreatedByName = name;
            }
        }

        private void PopulateFromSelection()
        {
            _selectedId = null;
            if (grid.CurrentRow?.DataBoundItem is EventDto e)
            {
                _selectedId = e.EventId;
                txtName.Text = e.Name ?? "";
                cbType.SelectedItem = (e.EventType ?? "OPERATIVO").ToUpperInvariant();
                dtp.Value = e.EventDateTime == default ? DateTime.UtcNow : e.EventDateTime.ToLocalTime();
                txtProv.Text = (e.Province ?? "SD").ToUpperInvariant();
                txtMun.Text = (e.Municipality ?? "DN").ToUpperInvariant();
                txtVenue.Text = e.Venue ?? "";
                txtAddress.Text = e.Address ?? "";
                cbStatus.SelectedItem = (e.Status ?? "PLANIFICADO").ToUpperInvariant();
            }
        }

        private async Task CreateAsync()
        {
            try
            {
                eventsApi.RefreshAuth();
                var dto = new EventCreateRequest
                {
                    Name = txtName.Text.Trim(),
                    EventType = cbType.SelectedItem!.ToString()!.ToUpperInvariant(),
                    EventDateTime = DateTime.SpecifyKind(dtp.Value, DateTimeKind.Utc),
                    Province = txtProv.Text.Trim().ToUpperInvariant(),
                    Municipality = txtMun.Text.Trim().ToUpperInvariant(),
                    Venue = txtVenue.Text.Trim(),
                    Address = txtAddress.Text.Trim(),
                    Status = cbStatus.SelectedItem!.ToString()!.ToUpperInvariant(),
                    CreatedBy = Session.UserId
                };

                await eventsApi.CreateAsync(dto);
                await LoadData();
                MessageBox.Show("Evento creado.");
            }
            catch (Exception ex) { MessageBox.Show("Error al crear evento: " + ex.Message); }
        }

        private async Task UpdateAsync()
        {
            if (_selectedId is null) { MessageBox.Show("Selecciona un evento de la lista."); return; }
            try
            {
                eventsApi.RefreshAuth();
                var dto = new EventUpdateRequest
                {
                    EventId = _selectedId.Value,
                    Name = txtName.Text.Trim(),
                    EventType = cbType.SelectedItem!.ToString()!.ToUpperInvariant(),
                    EventDateTime = DateTime.SpecifyKind(dtp.Value, DateTimeKind.Utc),
                    Province = txtProv.Text.Trim().ToUpperInvariant(),
                    Municipality = txtMun.Text.Trim().ToUpperInvariant(),
                    Venue = txtVenue.Text.Trim(),
                    Address = txtAddress.Text.Trim(),
                    Status = cbStatus.SelectedItem!.ToString()!.ToUpperInvariant(),
                    UpdatedBy = Session.UserId
                };

                await eventsApi.UpdateAsync(_selectedId.Value, dto);  // PUT 204 No Content (no leemos JSON)
                await LoadData();
                MessageBox.Show("Evento actualizado.");
            }
            catch (Exception ex) { MessageBox.Show("Error al actualizar: " + ex.Message); }
        }

        private async Task DeleteAsync()
        {
            if (_selectedId is null) { MessageBox.Show("Selecciona un evento de la lista."); return; }
            if (MessageBox.Show("¿Eliminar el evento seleccionado?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                eventsApi.RefreshAuth();
                await eventsApi.DeleteAsync(_selectedId.Value);
                await LoadData();
                _selectedId = null;
                MessageBox.Show("Evento eliminado.");
            }
            catch (Exception ex) { MessageBox.Show("Error al eliminar: " + ex.Message); }
        }
    }
}
