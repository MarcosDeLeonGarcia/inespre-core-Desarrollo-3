using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace INESPRE.Desktop.Api
{
    public sealed class UsersApi : ApiClient
    {
        public new void RefreshAuth() => base.RefreshAuth();
        private new readonly ApiClient _http = new();

        // GET /api/users?username=..&email=..&roleId=..&isActive=..
        public async Task<List<UserDto>> GetAllAsync(
            string? username = null,
            string? email = null,
            int? roleId = null,
            bool? isActive = null)
        {
            var qs = BuildQuery(new Dictionary<string, string?>
            {
                ["username"] = username,
                ["email"]    = email,
                ["roleId"]   = roleId?.ToString(),
                ["isActive"] = isActive?.ToString()?.ToLowerInvariant()
            });

            return await _http.GetAsync<List<UserDto>>($"/api/users{qs}");
        }

        // GET /api/users/{id}
        public Task<UserDto> GetByIdAsync(int id)
            => _http.GetAsync<UserDto>($"/api/users/{id}");

        // PUT /api/users/{id}
        public Task UpdateAsync(int id, UserUpdateRequest dto)
            => _http.PutAsync($"/api/users/{id}", dto);

        // DELETE /api/users/{id}
        public Task DeleteAsync(int id)
            => _http.DeleteAsync($"/api/users/{id}");

        // ----------------- helpers -----------------
        private static string BuildQuery(Dictionary<string, string?> pairs)
        {
            var first = true;
            var sb = new StringBuilder();

            foreach (var kv in pairs)
            {
                var v = kv.Value;
                if (string.IsNullOrWhiteSpace(v)) continue;

                sb.Append(first ? '?' : '&');
                first = false;

                sb.Append(Uri.EscapeDataString(kv.Key));
                sb.Append('=');
                sb.Append(Uri.EscapeDataString(v));
            }
            return sb.ToString();
        }
    }
}
