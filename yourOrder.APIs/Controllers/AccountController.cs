using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using yourOrder.APIs.DTOs;
using yourOrder.Core.Entity.Identity;
using yourOrder.Core.Services;

namespace yourOrder.APIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        public AccountController(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager,ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }


        
        [HttpPost("login")] 
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
                return Unauthorized("Invalid email");

            var PasswordResult = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!PasswordResult.Succeeded)
                return Unauthorized("Invalid password");

            var roles = await _userManager.GetRolesAsync(user); 
            return new UserDto
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = _tokenService.CreateToken(user , roles)
            };
        }


        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            var user = new AppUser
            {
                DisplayName = registerDto.DisplayName,
                Email = registerDto.Email,
                UserName = registerDto.Email,
                PhoneNumber = registerDto.PhoneNumber,
                FirstName = registerDto.FirstName,
                SecondName = registerDto.SecondName,

                Addresses = new List<Address>
                {
                    new Address{
                        City = registerDto.City,
                        Country = registerDto.Country,
                        Street = registerDto.Street
                    },
                    new Address{
                        City = registerDto.City,
                        Country = registerDto.Country,
                        Street = registerDto.Street
                    }
                }
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
                return BadRequest("Error creating user");

            
            await _userManager.AddToRoleAsync(user, "Customer");


            var roles = await _userManager.GetRolesAsync(user);
            return new UserDto
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = _tokenService.CreateToken(user, roles)
            };
        }






        

    }
}
