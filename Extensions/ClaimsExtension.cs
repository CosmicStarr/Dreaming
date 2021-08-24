using System.Linq;
using System.Security.Claims;

namespace Dreaming.Extensions
{
    public static class ClaimsExtension
    {
        public static string RetrieveEmail(this ClaimsPrincipal User)
        {
            return User.FindFirstValue(ClaimTypes.Email);
        }
    }
}