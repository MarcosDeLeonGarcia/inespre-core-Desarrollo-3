using System;

namespace INESPRE.Desktop.Core
{
    public static class Session
    {
        public static string ApiBase { get; set; } = "https://localhost:7197";

        // Alias para compatibilidad con archivos que usaban ApiBaseUrl
        public static string ApiBaseUrl
        {
            get => ApiBase;
            set => ApiBase = value;
        }

        public static string? Token { get; private set; }
        public static int UserId { get; private set; }
        public static string Username { get; private set; } = "";
        public static string Role { get; private set; } = "PUBLICO";

        public static bool IsAdmin => Role.Equals("ADMIN", StringComparison.OrdinalIgnoreCase);
        public static bool IsCaja => Role.Equals("CAJA", StringComparison.OrdinalIgnoreCase);

        public static void Set(string token, int userId, string username, string role)
        {
            Token = token;
            UserId = userId;
            Username = username;
            Role = role?.ToUpperInvariant() ?? "PUBLICO";
        }

        public static void Clear()
        {
            Token = null;
            UserId = 0;
            Username = "";
            Role = "PUBLICO";
        }
    }
}
