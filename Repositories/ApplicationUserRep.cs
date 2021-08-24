using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Models;
using Models.DTOs;

namespace Dreaming.Repositories
{
    public class ApplicationUserRep : IApplicationUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;

        public ApplicationUserRep(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager, IMapper mapper, IConfiguration configuration)
        {
            _configuration = configuration;
            _signInManager = signInManager;
            _mapper = mapper;
            _userManager = userManager;

        }
        public async Task<IdentityResult> SignUpAsync(RegisterModelDTO registerModel)
        {
            var NewUser = new ApplicationUser
            {
                FirstName = registerModel.FirstName,
                LastName = registerModel.LastName,
                Email = registerModel.Email,
                UserName = registerModel.Email
            };

            return await _userManager.CreateAsync(NewUser, registerModel.Password);
        }

        public async Task<string> LoginAsync(LoginModelDTO loginModelDTO)
        {
            var result = await _signInManager.PasswordSignInAsync(loginModelDTO.Email, loginModelDTO.Password, isPersistent: false, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                return null;
            }
            //Who the Application User Claims to be!
            return Token(loginModelDTO);     
        }

        public string Token(LoginModelDTO loginModelDTO)
        {
              var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,loginModelDTO.Email),
                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
            };
            //Secret Key for the Server to Use.
            var authKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:SecretKey"]));
            //Token Description
            var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:VaildIssuer"],
                    audience: _configuration["JWT:VaildAudience"],
                    expires: DateTime.UtcNow.AddDays(1),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authKey,SecurityAlgorithms.HmacSha512Signature)
                );

            return new JwtSecurityTokenHandler().WriteToken(token);    
           
        }
    }
}