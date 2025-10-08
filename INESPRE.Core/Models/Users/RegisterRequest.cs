namespace INESPRE.Core.Models.Users
{
    public class RegisterRequest
    {
        public string Username { get; set; } = default!;
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public int RoleId { get; set; } = 1;
        public string Password { get; set; } = default!;
    }
}
