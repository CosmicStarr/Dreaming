using System.Threading.Tasks;
using AutoMapper;
using Dreaming.Errors;
using Dreaming.Extensions;
using Dreaming.Repositories;
using DreamMore.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.DTOs;
using Models.Order;

namespace Dreaming.Controllers
{
  
    public class AccountController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IApplicationUserRepository _applicationUserRepo;
        private readonly SignInManager<ApplicationUser> _signManager;

        public AccountController(IUnitOfWork unitOfWork,
         IMapper mapper, 
         UserManager<ApplicationUser> userManager, 
         IApplicationUserRepository applicationUser,
         SignInManager<ApplicationUser> signInManager)
        {
            _applicationUserRepo = applicationUser;
            _userManager = userManager;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _signManager = signInManager;
        }

        [HttpGet("Email")]
        public async Task<ActionResult<LoginModelDTO>> GetUser(string Email)
        {
            var user = await _userManager.FindByEmailAsync(Email);
            return _mapper.Map<LoginModelDTO>(user);
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
            var user = await _applicationUserRepo.LoginAsync(loginModelDTO);
            var logUser = new LoginModel();
            var appUser = _mapper.Map<LoginModel,LoginModelDTO>(logUser);
            if(appUser == null)   return Unauthorized();
            return new LoginModelDTO
            {
              Email = loginModelDTO.Email,
              Password = loginModelDTO.Password,
              token = _applicationUserRepo.Token(loginModelDTO)  
            };

        }

        [HttpGet("Address")]
        [Authorize]
        public async Task<ActionResult<AddressDTO>> GetAddressAsync(int id)
        {           
            var userAdd = HttpContext.User.RetrieveEmail();
            var obj = await _unitOfWork.Repository<Address>().GetFirstOrDefault(x => x.AddressId == id);
            if(obj == null) return NotFound();
            return _mapper.Map<Address,AddressDTO>(obj);
        }

        private async Task<bool> UserExist(string Username)
        {
            return await _userManager.Users.AnyAsync(u => u.UserName == Username.ToLower());
        }
    }
}