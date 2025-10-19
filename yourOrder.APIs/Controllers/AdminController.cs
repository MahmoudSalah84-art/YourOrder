using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.EntityFrameworkCore;
using yourOrder.APIs.DTOs;
using yourOrder.APIs.DTOs.Admin;
using yourOrder.Core.Entity;
using yourOrder.Core.Entity.Identity;


namespace yourOrder.APIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public AdminController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager , IMapper mapper ) 
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            
        }

        [HttpPost("roles")] // POST: api/admin/roles
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto dto)
        {
            if (await _roleManager.RoleExistsAsync(dto.RoleName))
                return BadRequest(new { message = "Role already exists." });

            var result = await _roleManager.CreateAsync(new IdentityRole(dto.RoleName));

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = $"Role '{dto.RoleName}' created successfully." });
        }

        
        [HttpGet("roles")] // GET: api/admin/roles
        public async Task<IActionResult> GetRoles()
        {
            var roles =await  _roleManager.Roles.Select( r => r.Name).ToListAsync();
            return Ok(roles);
        }

        
        [HttpPost("assign-role")] // POST: api/admin/assign-role
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.UserId);
            if (user == null)
                return NotFound(new { message = "User not found." });
            if (!await _roleManager.RoleExistsAsync(dto.RoleName))
                return BadRequest(new { message = "Role does not exist." });
            var result = await _userManager.AddToRoleAsync(user, dto.RoleName);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            

            return Ok(new { message = $"Role '{dto.RoleName}' assigned to user '{user.Email}'." });
        }


        // dont forget filtaring 
        [HttpGet("users")] // GET: api/admin/users
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var result = _mapper.Map<List<AppUser>, List<UserInfoDto>>(users);
            return Ok(result);
        }


        
        [HttpPut("deactivate/{id}")]
        public async Task<IActionResult> DeactivateUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();
            user.IsDeleted = true;
            await _userManager.UpdateAsync(user);
            return Ok(new { message = $"User {user.Email} deactivated." });
        }

        
        [HttpPut("activate/{id}")]
        public async Task<IActionResult> ActivateUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();
            user.IsDeleted = false;
            await _userManager.UpdateAsync(user);
            return Ok(new { message = $"User {user.Email} activated." });
        }


        
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();
            await _userManager.DeleteAsync(user);
            return Ok(new { message = $"User {user.Email} permanently deleted." });
        }



        [HttpGet("users/search")]
        public async Task<IActionResult> SearchUsers([FromQuery] string keyword)
        {
            var users = await _userManager.Users
                .Where(u => u.Email.Contains(keyword) || u.FirstName.Contains(keyword) || u.SecondName.Contains(keyword))
                .ToListAsync();
            return Ok(users);
        }



    }
}
