using Microsoft.AspNetCore.Identity;

namespace DayDoc.Web.Models
{
    public class AppUser : IdentityUser
    {
        public string? RefreshToken { get; set; }
    }
}
