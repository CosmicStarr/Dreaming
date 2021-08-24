using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Models;
using Models.DTOs;

namespace Dreaming.Repositories
{
    public interface IApplicationUserRepository
    {
        Task<IdentityResult> SignUpAsync(RegisterModelDTO registerModel);
        Task<string> LoginAsync(LoginModelDTO loginModelDTO);
        string Token(LoginModelDTO loginModelDTO);

    }
}