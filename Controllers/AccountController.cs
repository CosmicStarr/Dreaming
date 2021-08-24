using System.Threading.Tasks;
using AutoMapper;
using Dreaming.Extensions;
using Dreaming.Repositories;
using DreamMore.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.DTOs;

namespace Dreaming.Controllers
{
  
    public class AccountController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IApplicationUserRepository _applicationUserRepo;

        public AccountController(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager, IApplicationUserRepository applicationUser)
        {
            _applicationUserRepo = applicationUser;
            _userManager = userManager;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("Email")]
        public async Task<ActionResult<LoginModelDTO>> GetUser(string Email)
        {
            var user = await _userManager.FindByEmailAsync(Email);
            var MappedUser = _mapper.Map<LoginModelDTO>(user);
            return new LoginModelDTO
            {
                Email = user.Email,
                token = _applicationUserRepo.Token(MappedUser)
            };

        }

        [HttpPost("Register")]
        public async Task<ActionResult> Register([FromBody] RegisterModelDTO registerModeldto)
        {
            if (await UserExist(registerModeldto.Email)) return BadRequest("Email already exist!");
            var registerModel = new RegisterModel();
            var appMap = _mapper.Map<RegisterModel,RegisterModelDTO>(registerModel);
            var appUser = await _applicationUserRepo.SignUpAsync(registerModeldto);
            //Add User Roles & Username property in the future!
            if (!appUser.Succeeded) return BadRequest(appUser.Errors);

            return Ok(appUser.Succeeded);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<LoginModelDTO>> Login([FromBody]LoginModelDTO loginModelDTO)
        {
            var userResult = await _applicationUserRepo.LoginAsync(loginModelDTO);
            var Log = new LoginModel();
            var MappedUser = _mapper.Map<LoginModel,LoginModelDTO>(Log);
            if(string.IsNullOrEmpty(userResult))
            {
                return Unauthorized();
            }
            return new LoginModelDTO
            {
                Email = loginModelDTO.Email,
                Password = loginModelDTO.Password,
                token = userResult
            };
        }

        private async Task<bool> UserExist(string Username)
        {
            return await _userManager.Users.AnyAsync(u => u.UserName == Username.ToLower());
        }
    }
}