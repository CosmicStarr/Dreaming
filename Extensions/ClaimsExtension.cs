using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Dreaming.Extensions
{
    public static class ClaimsExtension
    {
        public static string RetrieveEmail(this ClaimsPrincipal User)
        {
            return User.FindFirstValue(ClaimTypes.Email);
        }
        public async static Task<ApplicationUser> UserAddress(this UserManager<ApplicationUser> appUser, ClaimsPrincipal User)
        {
            var UserEmail = User.FindFirstValue(ClaimTypes.Email);
            return await appUser.Users.FirstOrDefaultAsync(x => x.Email == UserEmail);
        }
    }
}