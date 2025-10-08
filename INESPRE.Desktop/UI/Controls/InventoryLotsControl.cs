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
    public class InventoryLotsControl : UserControl
    {
        private sealed class Option<T>
        {
            public T? Id { get; set; }
            public string Name { get; set; } = "";
        }

        private readonly InventoryLotsApi api = new();
        private readonly ProductsApi productsApi = new();
        private readonly EventsApi eventsApi = new();

        private readonly BindingSource bs = new();
        private readonly DataGridView grid = new()
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AutoGenerateColumns = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            AllowUserToAddRows = false
        };

        // FILTRO
        private readonly ComboBox cmbFilterProduct = new()
        {
            Width = 240,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        private readonly Button btnBuscar = new() { Text = "Buscar" };
        private readonly Button btnLimpiar = new() { Text = "Limpiar" };

        // EDITOR
        private readonly ComboBox cmbProduct = new()
        {
            Width = 240,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        private readonly NumericUpDown numQty = new() { DecimalPlaces = 2, Minimum = -100000, Maximum = 100000, Width = 90, Value = 0 };
        private readonly NumericUpDown numCost = new() { DecimalPlaces = 2, Minimum = 0, Maximum = 1000000, Width = 90, Value = 0 };
        private readonly ComboBox cmbEvent = new()
        {
            Width = 220,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        // Ubicación (requerida por el backend)
        private readonly TextBox txtLocation = new() { PlaceholderText = "Ubicación (requerido)", Width = 150, Text = "ALMACEN" };
        private readonly TextBox txtLocationRef = new() { PlaceholderText = "Referencia (opc.)", Width = 140 };

        private readonly Button btnCreate = new() { Text = "Crear", Width = 90 };
        private readonly Button btnUpdate = new() { Text = "Actualizar", Width = 90 };
        private readonly Button btnDelete = new() { Text = "Borrar", Width = 90 };

        private List<InventoryLotDto> _all = new();
        private List<ProductDto> _products = new();
        private List<EventDto> _events = new();
        private int? _id = null;

        public InventoryLotsControl()
        {
            // TOOLS
            var tools = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 38, Padding = new Padding(8, 6, 8, 4), BackColor = Color.WhiteSmoke };
            tools.Controls.AddRange(new Control[]
            {
                new Label { Text = "Filtrar por Producto:", AutoSize = true, Padding = new Padding(0, 6, 8, 0) },
                cmbFilterProduct, btnBuscar, btnLimpiar
            });
            Controls.Add(grid);
            Controls.Add(tools);

            // EDITOR
            var editor = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 40, Padding = new Padding(8, 6, 8, 4), BackColor = Color.White };
            editor.Controls.AddRange(new Control[]
            {
                cmbProduct, numQty, numCost,
                new Label { Text = "Evento (opcional):", AutoSize = true, Padding = new Padding(8, 6, 0, 0) },
                cmbEvent,
                txtLocation, txtLocationRef,
                btnCreate, btnUpdate, btnDelete
            });
            Controls.Add(editor);

            btnCreate.Visible = btnUpdate.Visible = btnDelete.Visible = Session.IsAdmin;

            // GRID
            grid.DataSource = bs;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 252);
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "LotId", HeaderText = "LotId", Width = 60 });

            // Columna calculada: nombre del producto
            var colProdName = new DataGridViewTextBoxColumn { Name = "Producto", HeaderText = "Producto", Width = 220 };
            grid.Columns.Add(colProdName);

            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ProductId", HeaderText = "ProductId", Width = 80 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Quantity", HeaderText = "Cantidad", Width = 90, DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" } });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "UnitCost", HeaderText = "Costo", Width = 90, DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" } });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "EventId", HeaderText = "EventId", Width = 80 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Location", HeaderText = "Ubicación", Width = 120 });
            grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "LocationRef", HeaderText = "Ref.", Width = 120 });

            grid.CellFormatting += (_, e) =>
            {
                if (grid.Columns[e.ColumnIndex].Name == "Producto")
                {
                    if (grid.Rows[e.RowIndex].DataBoundItem is InventoryLotDto lot)
                        e.Value = _products.FirstOrDefault(p => p.ProductId == lot.ProductId)?.Name ?? "";
                }
            };

            grid.SelectionChanged += (_, __) => FromSelection();
            btnBuscar.Click += (_, __) => ApplyFilter();
            btnLimpiar.Click += async (_, __) =>
            {
                if (cmbFilterProduct.Items.Count > 0) cmbFilterProduct.SelectedIndex = 0;
                await LoadLots();
            };

            btnCreate.Click += async (_, __) => await CreateAsync();
            btnUpdate.Click += async (_, __) => await UpdateAsync();
            btnDelete.Click += async (_, __) => await DeleteAsync();

            bool loaded = false;
            VisibleChanged += async (_, __) =>
            {
                if (Visible && !loaded) { loaded = true; await InitializeAsync(); }
            };
        }

        // -------------------- Init --------------------
        private async Task InitializeAsync()
        {
            // Productos
            try
            {
                _products = await productsApi.GetAllAsync();

                var filterData = new List<Option<int>> { new Option<int> { Id = 0, Name = "(Todos)" } };
                filterData.AddRange(_products
                    .OrderBy(p => p.Name ?? string.Empty)
                    .Select(p => new Option<int> { Id = p.ProductId, Name = p.Name ?? $"#{p.ProductId}" }));

                cmbFilterProduct.DisplayMember = "Name";
                cmbFilterProduct.ValueMember = "Id";
                cmbFilterProduct.DataSource = filterData;
                cmbFilterProduct.SelectedIndex = 0;

                var editorData = _products
                    .OrderBy(p => p.Name ?? string.Empty)
                    .Select(p => new Option<int> { Id = p.ProductId, Name = p.Name ?? $"#{p.ProductId}" })
                    .ToList();

                if (editorData.Count == 0)
                    editorData.Add(new Option<int> { Id = 0, Name = "(Sin productos)" });

                cmbProduct.DisplayMember = "Name";
                cmbProduct.ValueMember = "Id";
                cmbProduct.DataSource = editorData;
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron cargar los productos.\n" + ex.Message, "Productos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbFilterProduct.DataSource = new List<Option<int>> { new Option<int> { Id = 0, Name = "(No disponibles)" } };
                cmbProduct.DataSource = new List<Option<int>> { new Option<int> { Id = 0, Name = "(No disponibles)" } };
            }

            // Eventos
            try
            {
                _events = await eventsApi.GetAllAsync();

                var evData = new List<Option<int?>> { new Option<int?> { Id = null, Name = "(Sin evento)" } };
                evData.AddRange(_events
                    .OrderBy(e => e.EventDateTime)
                    .Select(e => new Option<int?> { Id = e.EventId, Name = $"{e.Name} - {e.EventDateTime:yyyy-MM-dd HH:mm}" }));

                cmbEvent.DisplayMember = "Name";
                cmbEvent.ValueMember = "Id";
                cmbEvent.DataSource = evData;
                cmbEvent.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron cargar los eventos.\n" + ex.Message, "Eventos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbEvent.DataSource = new List<Option<int?>> { new Option<int?> { Id = null, Name = "(No disponibles)" } };
            }

            await LoadLots();
        }

        private async Task LoadLots()
        {
            try
            {
                _all = await api.GetAllAsync() ?? new List<InventoryLotDto>();
                ApplyFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudieron cargar los lotes.\n" + ex.Message, "Lotes", MessageBoxButtons.OK, MessageBoxIcon.Error);
                bs.DataSource = new List<InventoryLotDto>();
            }
        }

        private void ApplyFilter()
        {
            if (cmbFilterProduct.SelectedValue is int pid && pid != 0)
                bs.DataSource = _all.Where(x => x.ProductId == pid).ToList();
            else
                bs.DataSource = _all.ToList();
        }

        private void FromSelection()
        {
            _id = null;
            if (grid.CurrentRow?.DataBoundItem is InventoryLotDto d)
            {
                _id = d.LotId;

                if (cmbProduct.DataSource != null)
                    cmbProduct.SelectedValue = d.ProductId;

                numQty.Value  = (decimal)d.Quantity;
                numCost.Value = (decimal)d.UnitCost; // <-- usar UnitCost

                if (cmbEvent.DataSource != null)
                    cmbEvent.SelectedValue = d.EventId.HasValue ? (object)d.EventId.Value : null;

                txtLocation.Text    = d.Location    ?? "ALMACEN";
                txtLocationRef.Text = d.LocationRef ?? "";
            }
        }

        private bool TryGetSelectedProductId(out int productId)
        {
            productId = 0;
            if (cmbProduct.SelectedValue is int id && id > 0) { productId = id; return true; }
            return false;
        }

        private int? GetSelectedEventId()
        {
            if (cmbEvent.SelectedValue is int id) return id;
            if (cmbEvent.SelectedValue == null) return null;
            return int.TryParse(cmbEvent.SelectedValue.ToString(), out var parsed) ? parsed : (int?)null;
        }

        private async Task CreateAsync()
        {
            try
            {
                if (!TryGetSelectedProductId(out var pid))
                {
                    MessageBox.Show("Selecciona un producto.");
                    return;
                }

                var location = string.IsNullOrWhiteSpace(txtLocation.Text) ? "ALMACEN" : txtLocation.Text.Trim();

                await api.CreateAsync(new InventoryLotCreateRequest
                {
                    ProductId    = pid,
                    Quantity     = numQty.Value,
                    AvailableQty = numQty.Value,
                    UnitCost     = numCost.Value, // <-- enviar UnitCost
                    EventId      = GetSelectedEventId(),
                    Location     = location,
                    LocationRef  = string.IsNullOrWhiteSpace(txtLocationRef.Text) ? null : txtLocationRef.Text.Trim()
                });

                await LoadLots();
                MessageBox.Show("Lote creado.");
            }
            catch (Exception ex) { MessageBox.Show("Error al crear: " + ex.Message); }
        }

        private async Task UpdateAsync()
        {
            if (_id is null) { MessageBox.Show("Selecciona un lote."); return; }
            try
            {
                if (!TryGetSelectedProductId(out var pid))
                {
                    MessageBox.Show("Selecciona un producto.");
                    return;
                }

                var location = string.IsNullOrWhiteSpace(txtLocation.Text) ? "ALMACEN" : txtLocation.Text.Trim();

                await api.UpdateAsync(_id.Value, new InventoryLotUpdateRequest
                {
                    LotId        = _id.Value,
                    ProductId    = pid,
                    Quantity     = numQty.Value,
                    AvailableQty = numQty.Value,
                    UnitCost     = numCost.Value, // <-- enviar UnitCost
                    EventId      = GetSelectedEventId(),
                    Location     = location,
                    LocationRef  = string.IsNullOrWhiteSpace(txtLocationRef.Text) ? null : txtLocationRef.Text.Trim()
                });

                await LoadLots();
                MessageBox.Show("Lote actualizado.");
            }
            catch (Exception ex) { MessageBox.Show("Error al actualizar: " + ex.Message); }
        }

        private async Task DeleteAsync()
        {
            if (_id is null) { MessageBox.Show("Selecciona un lote."); return; }
            if (MessageBox.Show("¿Borrar lote?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            try
            {
                await api.DeleteAsync(_id.Value);
                await LoadLots();
            }
            catch (Exception ex) { MessageBox.Show("Error al borrar: " + ex.Message); }
        }
    }
}
