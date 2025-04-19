using Microsoft.AspNetCore.Identity;
using System;

namespace buoi2_PhamDucThang_2122110019.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Đánh dấu FullName là nullable (có thể null)
        public string? FullName { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public DateTime? DateModified { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
