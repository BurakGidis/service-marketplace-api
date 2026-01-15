using Microsoft.AspNetCore.Identity;

namespace ServiceMarketAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = null!;
        public bool IsProvider { get; set; } = false;
    }
}