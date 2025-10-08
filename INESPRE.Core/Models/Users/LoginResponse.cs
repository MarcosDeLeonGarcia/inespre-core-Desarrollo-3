namespace INESPRE.Core.Models.Users
{
    public class LoginResponse
    {
        public bool IsValid { get; set; }
        public int? UserId { get; set; }
        public int? RoleId { get; set; }
    }
}
