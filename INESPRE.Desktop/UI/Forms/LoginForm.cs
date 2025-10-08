using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using INESPRE.Desktop.Api;
using INESPRE.Desktop.Core;

namespace INESPRE.Desktop.UI.Forms
{
    public partial class LoginForm : Form
    {
        private static readonly HttpClient _http = new();

        public LoginForm()
        {
            InitializeComponent();
            btnLogin.Click += async (_, __) => await DoLoginAsync();
        }

        private async Task DoLoginAsync()
        {
            try
            {
                lblError.Text = "";
                btnLogin.Enabled = false;

                var res = await _http.PostAsJsonAsync(
                    $"{Session.ApiBase}/api/auth/login",
                    new LoginRequest { username = txtUser.Text.Trim(), password = txtPass.Text }
                );

                if (!res.IsSuccessStatusCode)
                {
                    lblError.Text = "Usuario o contraseña inválidos";
                    return;
                }

                var data = await res.Content.ReadFromJsonAsync<LoginResponse>();
                if (data is null)
                {
                    lblError.Text = "Respuesta inválida del servidor";
                    return;
                }

                // 1) Si llega token (formato con JWT)
                if (!string.IsNullOrEmpty(data.token))
                {
                    var role = !string.IsNullOrWhiteSpace(data.role) ? data.role! : MapRoleId(data.roleId);
                    Session.Set(data.token!, data.userId ?? 0, txtUser.Text.Trim(), role);
                }
                // 2) Si NO hay token pero isValid == true (tu formato actual)
                else if (data.isValid == true)
                {
                    var role = MapRoleId(data.roleId);
                    // sin token -> no se agrega Authorization en ApiClient (y está bien si tu API no lo requiere)
                    Session.Set(token: "", userId: data.userId ?? 0, username: txtUser.Text.Trim(), role: role);
                }
                else
                {
                    lblError.Text = "Credenciales inválidas";
                    return;
                }

                Hide();
                var main = new MainForm();
                main.FormClosed += (_, __) => Close();
                main.Show();
            }
            catch (Exception ex)
            {
                lblError.Text = "Error: " + ex.Message;
            }
            finally { btnLogin.Enabled = true; }
        }

        // Mapea roleId (ajusta si tus IDs son otros)
        private static string MapRoleId(int? roleId) => roleId switch
        {
            1 => "ADMIN",
            2 => "CAJA",
            3 => "COMERCIAL",
            4 => "AUDITORIA",
            _ => "PUBLICO"
        };
    
    }
}
