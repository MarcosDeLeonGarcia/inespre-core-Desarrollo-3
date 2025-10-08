using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using INESPRE.Desktop.Api;

namespace INESPRE.Desktop.UI.Controls
{
    public sealed class UsersControl : UserControl
    {
        private readonly UsersApi _api = new();
        private readonly ApiClient _http = new(); // para /api/auth/register
        private readonly BindingSource _bs = new();

        // Filtros
        private readonly TextBox _txtFUsername = new() { PlaceholderText = "Usuario", Width = 160 };
        private readonly TextBox _txtFEmail = new() { PlaceholderText = "Email", Width = 180 };
        private readonly NumericUpDown _numFRole = new() { Minimum = 0, Maximum = 999999, Width = 80 };
        private readonly CheckBox _chkFActive = new() { ThreeState = true, Text = "Activo" };
        private readonly Button _btnSearch = new() { Text = "Buscar", Width = 80 };
        private readonly Button _btnClear = new() { Text = "Limpiar", Width = 80 };

        // Editor / Update
        private readonly TextBox _txtUserName = new() { PlaceholderText = "Usuario", Width = 160 };
        private readonly TextBox _txtEmail = new() { PlaceholderText = "Email", Width = 180 };
        private readonly TextBox _txtFullName = new() { PlaceholderText = "Nombre completo", Width = 220 };
        private readonly TextBox _txtPhone = new() { PlaceholderText = "Teléfono", Width = 120 };
        private readonly NumericUpDown _numRole = new() { Minimum = 1, Maximum = 999999, Width = 80 };
        private readonly CheckBox _chkActive = new() { Text = "Activo", Checked = true };
        private readonly Button _btnUpdate = new() { Text = "Actualizar", Width = 90, Enabled = false };
        private readonly Button _btnDelete = new() { Text = "Borrar", Width = 90, Enabled = false };

        // Alta (vía /api/auth/register)
        private readonly TextBox _txtNewUser = new() { PlaceholderText = "Usuario (nuevo)", Width = 150 };
        private readonly TextBox _txtNewMail = new() { PlaceholderText = "Email (nuevo)", Width = 180 };
        private readonly TextBox _txtNewName = new() { PlaceholderText = "Nombre completo (nuevo)", Width = 220 };
        private readonly TextBox _txtNewPhone = new() { PlaceholderText = "Teléfono (nuevo)", Width = 120 };
        private readonly NumericUpDown _numNewRole = new() { Minimum = 1, Maximum = 999999, Width = 80, Value = 1 };
        private readonly TextBox _txtNewPwd = new() { PlaceholderText = "Password", Width = 120, UseSystemPasswordChar = true };
        private readonly Button _btnCreate = new() { Text = "Crear usuario", Width = 110 };

        private readonly DataGridView _grid = new()
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AutoGenerateColumns = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false
        };

        private bool _loaded;

        public UsersControl()
        {
            Dock = DockStyle.Fill;
            BackColor = Color.White;

            // Sección de filtros
            var pFilters = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 40,
                Padding = new Padding(10),
                BackColor = Color.FromArgb(246, 246, 246)
            };
            pFilters.Controls.AddRange(new Control[] { _txtFUsername, _txtFEmail, new Label { Text = "Rol:", AutoSize = true, Padding = new Padding(10, 8, 0, 0) }, _numFRole, _chkFActive, _btnSearch, _btnClear });

            // Sección de edición
            var pEdit = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 40,
                Padding = new Padding(10),
                BackColor = Color.White
            };
            pEdit.Controls.AddRange(new Control[] { _txtUserName, _txtEmail, _txtFullName, _txtPhone, new Label { Text = "Rol:", AutoSize = true, Padding = new Padding(10, 8, 0, 0) }, _numRole, _chkActive, _btnUpdate, _btnDelete });

            // Sección de creación de usuario
            var pCreate = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 40,
                Padding = new Padding(10),
                BackColor = Color.White
            };
            pCreate.Controls.AddRange(new Control[] { _txtNewUser, _txtNewMail, _txtNewName, _txtNewPhone, new Label { Text = "Rol:", AutoSize = true, Padding = new Padding(10, 8, 0, 0) }, _numNewRole, _txtNewPwd, _btnCreate });

            // Construir y agregar la grilla
            BuildGrid();

            Controls.Add(_grid);
            Controls.Add(pCreate);
            Controls.Add(pEdit);
            Controls.Add(pFilters);

            // Eventos
            _btnSearch.Click += async (_, __) => await LoadDataAsync();
            _btnClear.Click += async (_, __) => { ClearFilters(); await LoadDataAsync(); };
            _btnUpdate.Click += async (_, __) => await UpdateAsync();
            _btnDelete.Click += async (_, __) => await DeleteAsync();
            _btnCreate.Click += async (_, __) => await CreateUserAsync();
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

            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "UserId", HeaderText = "Id", Width = 60 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "UserName", HeaderText = "Usuario", Width = 150 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Email", HeaderText = "Email", Width = 200 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "FullName", HeaderText = "Nombre", Width = 200 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Phone", HeaderText = "Teléfono", Width = 120 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "RoleId", HeaderText = "Rol", Width = 80 });
            _grid.Columns.Add(new DataGridViewCheckBoxColumn { DataPropertyName = "IsActive", HeaderText = "Activo", Width = 60 });
        }

        private async Task LoadDataAsync()
        {
            try
            {
                bool? active = _chkFActive.CheckState switch
                {
                    CheckState.Checked => true,
                    CheckState.Unchecked => false,
                    _ => (bool?)null
                };
                int? role = _numFRole.Value > 0 ? (int)_numFRole.Value : null;

                var list = await _api.GetAllAsync(
                    username: string.IsNullOrWhiteSpace(_txtFUsername.Text) ? null : _txtFUsername.Text.Trim(),
                    email: string.IsNullOrWhiteSpace(_txtFEmail.Text) ? null : _txtFEmail.Text.Trim(),
                    roleId: role,
                    isActive: active
                );

                _bs.DataSource = list;
                _btnUpdate.Enabled = _btnDelete.Enabled = _grid.CurrentRow != null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar usuarios: " + ex.Message);
            }
        }

        private async Task UpdateAsync()
        {
            if (_grid.CurrentRow?.DataBoundItem is not UserDto row) return;

            try
            {
                var dto = new UserUpdateRequest
                {
                    UserId = row.UserId, // Asegúrate de enviar el UserId correcto
                    UserName = _txtUserName.Text.Trim(),
                    Email = _txtEmail.Text.Trim(),
                    FullName = _txtFullName.Text.Trim(),
                    Phone = _txtPhone.Text.Trim(),
                    RoleId = (int)_numRole.Value,
                    IsActive = _chkActive.Checked
                };

                // Asegúrate de pasar el UserId en la URL y el cuerpo de la solicitud
                await _api.UpdateAsync(row.UserId, dto);
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar: " + ex.Message);
            }
        }

        private async Task DeleteAsync()
        {
            if (_grid.CurrentRow?.DataBoundItem is not UserDto row) return;

            if (MessageBox.Show($"¿Eliminar usuario #{row.UserId} ({row.UserName})?",
                    "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            try
            {
                await _api.DeleteAsync(row.UserId);
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al borrar: " + ex.Message);
            }
        }

        private async Task CreateUserAsync()
        {
            try
            {
                var body = new
                {
                    username = _txtNewUser.Text.Trim(),
                    email = _txtNewMail.Text.Trim(),
                    fullName = _txtNewName.Text.Trim(),
                    phone = _txtNewPhone.Text.Trim(),
                    roleId = (int)_numNewRole.Value,
                    password = _txtNewPwd.Text
                };

                if (string.IsNullOrWhiteSpace(body.username) ||
                    string.IsNullOrWhiteSpace(body.password))
                {
                    MessageBox.Show("Usuario y Password son obligatorios.");
                    return;
                }

                await _http.PostAsync("/api/auth/register", body);

                // limpiar y recargar
                _txtNewUser.Clear(); _txtNewMail.Clear(); _txtNewName.Clear();
                _txtNewPhone.Clear(); _txtNewPwd.Clear(); _numNewRole.Value = 1;

                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al crear usuario: " + ex.Message);
            }
        }

        private void SyncSelectionToEditor()
        {
            if (_grid.CurrentRow?.DataBoundItem is not UserDto row)
            {
                _btnUpdate.Enabled = _btnDelete.Enabled = false;
                return;
            }

            _txtUserName.Text = row.UserName ?? "";
            _txtEmail.Text = row.Email ?? "";
            _txtFullName.Text = row.FullName ?? "";
            _txtPhone.Text = row.Phone ?? "";

            // NumericUpDown.Value es decimal: convertimos sin '??'
            _numRole.Value = row.RoleId <= 0 ? 1 : row.RoleId;

            _chkActive.Checked = row.IsActive;
            _btnUpdate.Enabled = _btnDelete.Enabled = true;
        }

        private void ClearFilters()
        {
            _txtFUsername.Clear();
            _txtFEmail.Clear();
            _numFRole.Value = 0;
            _chkFActive.CheckState = CheckState.Indeterminate;
        }
    }
}
