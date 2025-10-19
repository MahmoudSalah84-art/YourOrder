using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using yourOrder.APIs.DTOs;
using yourOrder.APIs.DTOs.Admin;
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
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        public AccountController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ITokenService tokenService,
            IEmailService emailService,
            IConfiguration configuration,
            IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _emailService = emailService;
            _config = configuration;
            _mapper = mapper;
        }

        //[HttpPost("login")] 
        //public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        //{
        //    var user = await _userManager.FindByEmailAsync(loginDto.Email);
        //    if (user == null)
        //        return Unauthorized("Invalid email");

        //    var PasswordResult = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
        //    if (!PasswordResult.Succeeded)
        //        return Unauthorized("Invalid password");

        //    var roles = await _userManager.GetRolesAsync(user); 
        //    return new UserDto
        //    {
        //        DisplayName = user.DisplayName,
        //        Email = user.Email,
        //        Token = _tokenService.CreateToken(user , roles)
        //    };
        //}

        //more secure
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
                return Unauthorized("Invalid email");

            var PasswordResult = await _signInManager.PasswordSignInAsync(user, loginDto.Password, false, lockoutOnFailure: true);
            if (PasswordResult.IsLockedOut)
                return BadRequest("Account is locked due to multiple failed login attempts. Try again later.");
            if (!PasswordResult.Succeeded)
                return Unauthorized("Invalid password");

            var refreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshTokens.Add(refreshToken);
            await _userManager.UpdateAsync(user);

            
            var roles = await _userManager.GetRolesAsync(user);

            // in http only cookie store refresh token for xss attack
            Response.Cookies.Append("refreshToken", refreshToken.Token, new CookieOptions 
            {
                HttpOnly = true,
                Secure = true, // HTTPS فقط
                SameSite = SameSiteMode.Strict,
                Expires = refreshToken.ExpiresOn
            });

            return new UserDto
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = _tokenService.CreateToken(user, roles),
            };
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register( RegisterDto registerDto)
        {
            var user = _mapper.Map<AppUser>(registerDto);
            
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
                return BadRequest("Error creating user");
            await _userManager.AddToRoleAsync(user, "Customer");
            var roles = await _userManager.GetRolesAsync(user);

            var Emailtoken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmUrl = $"{_config["FrontendUrl"]}/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(Emailtoken)}";

            await _emailService.SendEmailAsync(user.Email, "Confirm your email",
                $"<h3>مرحبا {user.FirstName +" "+ user.SecondName}</h3><p>اضغط على الرابط لتأكيد حسابك:</p><a href='{confirmUrl}'>تأكيد البريد</a>");

            return new UserDto
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = _tokenService.CreateToken(user, roles)
            };
        }
        


        [Authorize]
        [HttpGet("me")] // GET: api/Account/me
        public async Task<IActionResult> Me()
        {
            var email = User.FindFirstValue(ClaimTypes.Email); 
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return NotFound();

            return Ok(new
            {
                user.Id,
                user.DisplayName,
                user.FirstName,
                user.SecondName,
                user.PhoneNumber,
                user.Email,
                Roles = await _userManager.GetRolesAsync(user)
            });
        }



        [HttpGet("confirm-email")] // GET: api/Account/confirm-email?userId=...&token=...
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId , [FromQuery] string token)
        {
            var user = await _userManager.FindByIdAsync (userId);
            if (user == null) 
                return BadRequest("Invalid user!!!");

            var result = await _userManager.ConfirmEmailAsync (user, token);
            if (!result.Succeeded) 
                return BadRequest("Email confirmation failed !!!!!");

            return Ok("Email confirmation is successed");

        }

        
        [HttpPost("forgot-password")] // POST: api/Account/forgot-password
        public async Task<IActionResult> ForgotPassword([FromQuery] ForgotPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync (model.Email);
            if (user == null) 
                return Ok(new { message = "If the email exists, a reset link has been sent." }); //enumeration attack

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetUrl = $"{_config["FrontendUrl"]}/reset-password?email={user.Email}&token={Uri.EscapeDataString(token)}";

            await _emailService.SendEmailAsync(user.Email, "Reset your password",
                $"<h3>استعادة كلمة المرور</h3><p>اضغط الرابط لتغيير كلمة المرور:</p><a href='{resetUrl}'>إعادة التعيين</a>");
            return Ok(new { message = "Password reset link sent." });
        }

      


        [HttpPost("reset-password")] // POST: api/Account/reset-password
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return BadRequest("Invalid email.");

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            return Ok("Password has been reset successfully ");
        }


        [HttpPost("refresh")] // POST: api/Account/refresh
        public async Task<IActionResult> Refresh(TokenRequestDto model)
        {
            var user = _userManager.Users
                .SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == model.RefreshToken));

            if (user == null)
                 return Unauthorized("Invalid refresh token");

            var refreshToken = user.RefreshTokens.Single(x => x.Token == model.RefreshToken);

            if (!refreshToken.IsActive)
                return Unauthorized("Inactive refresh token");

            // New Access Token 
            var newJwt = _tokenService.CreateToken(user, await _userManager.GetRolesAsync(user));
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            refreshToken.RevokedOn = DateTime.UtcNow;
            refreshToken.ReplacedByToken = newRefreshToken.Token;

            user.RefreshTokens.Add(newRefreshToken);
            await _userManager.UpdateAsync(user);

            Response.Cookies.Append("refreshToken", newRefreshToken.Token, new CookieOptions //for xss attack . JavaScript can't access HttpOnly cookies
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = newRefreshToken.ExpiresOn
            });

            return Ok(new
            {
                AccessToken = newJwt
            });
        }


        [HttpPost("revoke")] // POST: api/Account/revoke
        public async Task<IActionResult> Revoke(TokenRequestDto model)
        {
            var user = _userManager.Users
                .SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == model.RefreshToken));

            if (user == null)
                return NotFound("User not found");

            var Refreshtoken = user.RefreshTokens.Single(x => x.Token == model.RefreshToken);

            if (!Refreshtoken.IsActive)
                return BadRequest("Token is already inactive");

            Refreshtoken.RevokedOn = DateTime.UtcNow;
            Refreshtoken.RevokedByIp = HttpContext.Connection.RemoteIpAddress?.ToString();

            await _userManager.UpdateAsync(user);
            return Ok("Refresh token revoked successfully");
        }



    }
}
