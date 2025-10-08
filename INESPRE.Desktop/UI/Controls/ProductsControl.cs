using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using INESPRE.Desktop.Api;

namespace INESPRE.Desktop.UI.Controls
{
    public sealed class ProductsControl : UserControl
    {
        private readonly ProductsApi _api = new();
        private readonly BindingSource _bs = new();

        // cache actual de productos
        private List<ProductDto> _all = new();

        // Filtros
        private readonly TextBox _txtFName = new() { PlaceholderText = "Filtrar por nombre", Width = 180 };
        private readonly TextBox _txtFSku = new() { PlaceholderText = "SKU", Width = 100 };
        private readonly ComboBox _cboFType = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 120 };
        private readonly CheckBox _chkFActive = new() { ThreeState = true, Text = "Activos" };
        private readonly Button _btnSearch = new() { Text = "Buscar", Width = 80 };
        private readonly Button _btnClear = new() { Text = "Limpiar", Width = 80 };

        // Edición / Alta
        private readonly TextBox _txtName = new() { PlaceholderText = "Nombre", Width = 180 };
        private readonly TextBox _txtSku = new() { PlaceholderText = "SKU", Width = 100 };
        private readonly TextBox _txtUnit = new() { PlaceholderText = "Unidad (und, kg…)", Width = 120, Text = "und" };
        private readonly ComboBox _cboType = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 120 };
        private readonly CheckBox _chkPerishable = new() { Text = "Perecedero" };
        private readonly NumericUpDown _numPrice = new() { DecimalPlaces = 2, Maximum = 1000000000, Width = 100 };
        private readonly CheckBox _chkActive = new() { Text = "Activo", Checked = true };
        private readonly Button _btnCreate = new() { Text = "Crear", Width = 90 };
        private readonly Button _btnUpdate = new() { Text = "Actualizar", Width = 90, Enabled = false };
        private readonly Button _btnDelete = new() { Text = "Borrar", Width = 90, Enabled = false };

        private readonly DataGridView _grid = new()
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AutoGenerateColumns = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false
        };

        private bool _loaded;

        public ProductsControl()
        {
            Dock = DockStyle.Fill;
            BackColor = Color.White;

            // combos
            _cboType.Items.AddRange(new[] { "SIMPLE", "COMBO" });
            _cboType.SelectedIndex = 0;

            _cboFType.Items.Add("(Todos)");
            _cboFType.Items.AddRange(new[] { "SIMPLE", "COMBO" });
            _cboFType.SelectedIndex = 0;

            // layout filtros
            var pFilters = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 40,
                Padding = new Padding(10),
                BackColor = Color.FromArgb(246, 246, 246)
            };
            pFilters.Controls.AddRange(new Control[] { _txtFName, _txtFSku, _cboFType, _chkFActive, _btnSearch, _btnClear });

            // layout editor
            var pEdit = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 40,
                Padding = new Padding(10),
                BackColor = Color.White
            };
            pEdit.Controls.AddRange(new Control[]
            {
                _txtName, _txtSku, _txtUnit, _cboType, _chkPerishable,
                new Label { Text = "Precio:", AutoSize = true, Padding = new Padding(10, 8, 0, 0) },
                _numPrice, _chkActive, _btnCreate, _btnUpdate, _btnDelete
            });

            BuildGrid();

            Controls.Add(_grid);
            Controls.Add(pEdit);
            Controls.Add(pFilters);

            // eventos
            _btnSearch.Click += async (_, __) => await LoadDataAsync();
            _btnClear.Click  += async (_, __) => { ClearFilters(); await LoadDataAsync(); };

            _btnCreate.Click += async (_, __) => await CreateAsync();
            _btnUpdate.Click += async (_, __) => await UpdateAsync();
            _btnDelete.Click += async (_, __) => await DeleteAsync();

            _grid.SelectionChanged += (_, __) => SyncSelectionToEditor();
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (DesignMode || _loaded) return;
            _loaded = true;
            await LoadDataAsync();
        }

        private void BuildGrid()
        {
            _grid.DataSource = _bs;

            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ProductId", HeaderText = "Id", Width = 60 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Name", HeaderText = "Nombre", Width = 180 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SKU", HeaderText = "SKU", Width = 100 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Unit", HeaderText = "Unidad", Width = 80 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ProductType", HeaderText = "Tipo", Width = 100 });
            _grid.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = "Perishable", HeaderText = "Perec.", Width = 60 });

            var priceCol = new DataGridViewTextBoxColumn
            {
                DataPropertyName = "DefaultSalePrice",
                HeaderText = "Precio",
                Width = 90,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Format = "N2",
                    NullValue = ""   // muestra vacío cuando es null
                }
            };
            _grid.Columns.Add(priceCol);

            _grid.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = "Active", HeaderText = "Activo", Width = 60 });
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var type = _cboFType.SelectedIndex <= 0 ? null : _cboFType.SelectedItem?.ToString();
                bool? active = _chkFActive.CheckState switch
                {
                    CheckState.Checked => true,
                    CheckState.Unchecked => false,
                    _ => (bool?)null
                };

                var list = await _api.GetAllAsync(
                    name: _txtFName.Text.Trim().Length == 0 ? null : _txtFName.Text.Trim(),
                    sku: _txtFSku.Text.Trim().Length  == 0 ? null : _txtFSku.Text.Trim(),
                    productType: type,
                    active: active
                );

                _all = list ?? new List<ProductDto>();
                _bs.DataSource = _all;
                _btnUpdate.Enabled = _btnDelete.Enabled = _grid.CurrentRow != null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar productos: " + ex.Message);
            }
        }

        private async Task CreateAsync()
        {
            try
            {
                var dto = new ProductCreateRequest
                {
                    Name = _txtName.Text.Trim(),
                    SKU  = _txtSku.Text.Trim(),
                    Unit = string.IsNullOrWhiteSpace(_txtUnit.Text) ? "und" : _txtUnit.Text.Trim(),
                    ProductType = _cboType.SelectedItem?.ToString() ?? "SIMPLE",
                    Perishable = _chkPerishable.Checked,
                    // Si quieres que 0 signifique "sin precio", cambia a: DefaultSalePrice = _numPrice.Value == 0 ? null : _numPrice.Value,
                    DefaultSalePrice = _numPrice.Value,
                    Active = _chkActive.Checked
                };

                if (string.IsNullOrWhiteSpace(dto.Name))
                {
                    MessageBox.Show("El nombre es obligatorio.");
                    return;
                }
                if (string.IsNullOrWhiteSpace(dto.SKU))
                {
                    MessageBox.Show("El SKU es obligatorio.");
                    return;
                }
                // Chequeo de SKU duplicado (cliente)
                if (_all.Any(p => string.Equals(p.SKU ?? "", dto.SKU!, StringComparison.OrdinalIgnoreCase)))
                {
                    MessageBox.Show("El SKU ya existe. Usa otro o actualiza el producto existente.");
                    return;
                }

                await _api.CreateAsync(dto);
                ClearEditor();
                await LoadDataAsync();
                MessageBox.Show("Producto creado.");
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("UQ_") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("No se pudo crear: el SKU ya existe.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al crear: " + ex.Message);
            }
        }

        private async Task UpdateAsync()
        {
            if (_grid.CurrentRow?.DataBoundItem is not ProductDto row)
            {
                MessageBox.Show("Selecciona un producto.");
                return;
            }

            try
            {
                var dto = new ProductUpdateRequest
                {
                    ProductId = row.ProductId,
                    Name = _txtName.Text.Trim(),
                    SKU  = _txtSku.Text.Trim(),
                    Unit = string.IsNullOrWhiteSpace(_txtUnit.Text) ? "und" : _txtUnit.Text.Trim(),
                    ProductType = _cboType.SelectedItem?.ToString() ?? "SIMPLE",
                    Perishable = _chkPerishable.Checked,
                    DefaultSalePrice = _numPrice.Value,
                    Active = _chkActive.Checked
                };

                if (string.IsNullOrWhiteSpace(dto.Name))
                {
                    MessageBox.Show("El nombre es obligatorio.");
                    return;
                }
                if (string.IsNullOrWhiteSpace(dto.SKU))
                {
                    MessageBox.Show("El SKU es obligatorio.");
                    return;
                }
                // Si cambió el SKU, valida que no choque con otro producto
                if (_all.Any(p => p.ProductId != dto.ProductId &&
                                  string.Equals(p.SKU ?? "", dto.SKU!, StringComparison.OrdinalIgnoreCase)))
                {
                    MessageBox.Show("El SKU ya existe en otro producto.");
                    return;
                }

                // *** CORRECCIÓN: UpdateAsync requiere el id ***
                await _api.UpdateAsync(dto.ProductId, dto);
                await LoadDataAsync();
                MessageBox.Show("Producto actualizado.");
            }
            catch (HttpRequestException ex) when (ex.Message.Contains("UQ_") || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("No se pudo actualizar: el SKU ya existe.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar: " + ex.Message);
            }
        }

        private async Task DeleteAsync()
        {
            if (_grid.CurrentRow?.DataBoundItem is not ProductDto row) return;

            var accion = row.Active ? "desactivar" : "reactivar";
            if (MessageBox.Show($"¿Deseas {accion} el producto #{row.ProductId} ({row.Name})?",
                    "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                // Soft delete: solo cambiamos el flag Active
                var dto = new ProductUpdateRequest
                {
                    ProductId        = row.ProductId,
                    Name             = row.Name ?? "",
                    SKU              = row.SKU ?? "",
                    Unit             = string.IsNullOrWhiteSpace(row.Unit) ? "und" : row.Unit!,
                    ProductType      = string.IsNullOrWhiteSpace(row.ProductType) ? "SIMPLE" : row.ProductType!,
                    Perishable       = row.Perishable,
                    DefaultSalePrice = row.DefaultSalePrice ?? 0m,  // si tu DTO es nullable
                    Active           = !row.Active ? true : false    // si quieres alternar; o simplemente false para “borrar”
                };

                await _api.UpdateAsync(dto.ProductId, dto);
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cambiar estado: " + ex.Message);
            }
        }


        private void SyncSelectionToEditor()
        {
            if (_grid.CurrentRow?.DataBoundItem is not ProductDto row)
            {
                _btnUpdate.Enabled = _btnDelete.Enabled = false;
                return;
            }

            _txtName.Text = row.Name ?? "";
            _txtSku.Text  = row.SKU ?? "";
            _txtUnit.Text = row.Unit ?? "und";
            _cboType.SelectedItem =
                string.Equals(row.ProductType ?? "SIMPLE", "COMBO", StringComparison.OrdinalIgnoreCase) ? "COMBO" : "SIMPLE";
            _chkPerishable.Checked = row.Perishable;

            // Precio puede ser null
            try { _numPrice.Value = Convert.ToDecimal(row.DefaultSalePrice); }
            catch { _numPrice.Value = 0m; }

            _chkActive.Checked = row.Active;
            _btnUpdate.Enabled = _btnDelete.Enabled = true;
        }

        private void ClearEditor()
        {
            _txtName.Clear();
            _txtSku.Clear();
            _txtUnit.Text = "und";
            _cboType.SelectedIndex = 0;
            _chkPerishable.Checked = false;
            _numPrice.Value = 0;
            _chkActive.Checked = true;
        }

        private void ClearFilters()
        {
            _txtFName.Clear();
            _txtFSku.Clear();
            _cboFType.SelectedIndex = 0;
            _chkFActive.CheckState = CheckState.Indeterminate;
        }
    }
}
