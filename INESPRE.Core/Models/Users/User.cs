using System;

namespace INESPRE.Core.Models.Users
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; } = default!;
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public int RoleId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        // Deliberadamente NO exponemos PasswordSalt/Hash en el modelo público
    }
}
