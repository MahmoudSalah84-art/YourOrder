using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using yourOrder.Core.Entity.Identity;

namespace yourOrder.APIs.Controllers
{
    public class RolesController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateRole([FromBody] string roleName)
        {
            if (await _roleManager.RoleExistsAsync(roleName))
                return BadRequest("Role already exists");

            await _roleManager.CreateAsync(new IdentityRole(roleName));
            return Ok($"Role '{roleName}' created successfully");
        }
    }
}
