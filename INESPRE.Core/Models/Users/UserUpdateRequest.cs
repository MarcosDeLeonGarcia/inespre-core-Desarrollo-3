namespace INESPRE.Core.Models.Users
{
    public class UserUpdateRequest
    {
        public int UserId { get; set; }  // Agregamos UserId para usarlo en la actualización
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public int RoleId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
