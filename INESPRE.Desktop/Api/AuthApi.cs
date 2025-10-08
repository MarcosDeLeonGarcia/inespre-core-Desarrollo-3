using System.Net.Http.Headers;
using System.Net.Http.Json;
using INESPRE.Desktop.Core;

namespace INESPRE.Desktop.Api
{
    public class AuthApi
    {
        private readonly HttpClient _http = new() { BaseAddress = new Uri(Session.ApiBase) };
        // Si prefieres:
        // private readonly HttpClient _http = new() { BaseAddress = new Uri(Session.ApiBaseUrl) };

        private void AttachAuth()
        {
            _http.DefaultRequestHeaders.Authorization = null;
            if (!string.IsNullOrWhiteSpace(Session.Token))
                _http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", Session.Token);
        }

        public async Task<LoginResponse?> LoginAsync(string username, string password)
        {
            var res = await _http.PostAsJsonAsync("/api/auth/login",
                new LoginRequest { username = username, password = password });

            if (!res.IsSuccessStatusCode) return null;
            return await res.Content.ReadFromJsonAsync<LoginResponse>();
        }

        // ✅ Cambiar contraseña (el endpoint espera un string JSON con la nueva contraseña)
        public async Task<bool> ChangePasswordAsync(int userId, string newPassword)
        {
            AttachAuth(); // agrega Authorization si hay token
            var res = await _http.PostAsJsonAsync($"/api/auth/{userId}/change-password", newPassword);
            return res.IsSuccessStatusCode; // 204/200 -> true
        }
    }
}
