
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyWebAPI.Models;
using MyWebAPI.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyWebAPI.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<DefaultUser> _userManager;

        public AdminController(RoleManager<IdentityRole> roleManager, UserManager<DefaultUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpGet("roles")]
        public IActionResult GetRoles()
        {
            var roles = _roleManager.Roles.ToList();
            return Ok(roles);
        }

        [HttpPost("roles")]
        public async Task<IActionResult> AddRole([FromBody] AddRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole role = new() { Name = model.RoleName };
                var result = await _roleManager.CreateAsync(role);
                if (result.Succeeded) return Ok(role);
                return BadRequest(result.Errors);
            }
            return BadRequest(ModelState);
        }

        [HttpPut("roles/{id}")]
        public async Task<IActionResult> EditRole(string id, [FromBody] EditRoleViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) return NotFound($"No role with Id '{id}' found.");

            role.Name = model.RoleName;
            var result = await _roleManager.UpdateAsync(role);
            if (result.Succeeded) return Ok(role);

            return BadRequest(result.Errors);
        }

        [HttpDelete("roles/{id}")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) return NotFound($"No role with Id '{id}' found.");

            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded) return Ok(new { Message = "Role deleted successfully" });

            return BadRequest(result.Errors);
        }

        [HttpGet("roles/{id}/users")]
        public async Task<IActionResult> GetUsersInRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) return NotFound($"No role with Id '{id}' found.");

            var users = _userManager.Users.Where(u => _userManager.IsInRoleAsync(u, role.Name).Result)
                                          .Select(u => new { u.Id, u.UserName })
                                          .ToList();

            return Ok(users);
        }

        [HttpPost("roles/{id}/users")]
        public async Task<IActionResult> UpdateUsersInRole(string id, [FromBody] List<UserRoleViewModel> model)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) return NotFound($"No role with Id '{id}' found.");

            foreach (var userRole in model)
            {
                var user = await _userManager.FindByIdAsync(userRole.Id);
                if (userRole.IsSelected && !(await _userManager.IsInRoleAsync(user, role.Name)))
                {
                    await _userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!userRole.IsSelected && await _userManager.IsInRoleAsync(user, role.Name))
                {
                    await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
            }
            return Ok(new { Message = "User roles updated successfully" });
        }
    }
}
