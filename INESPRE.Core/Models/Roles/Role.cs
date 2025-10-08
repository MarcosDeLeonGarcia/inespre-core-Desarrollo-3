using System;

namespace INESPRE.Core.Models.Roles
{
    public class Role
    {
        public int RoleId { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

