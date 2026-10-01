using FlowerPowerGames.Business.Authentication;
using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowerPowerGames.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<ActionResult<IEnumerable<RoleDto>>> GetAllRoles()
    {
        var roles = await _roleService.GetAllRolesAsync();

        return Ok(roles);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<ActionResult<RoleDto>> GetRoleById(int id)
    {
        var role = await _roleService.GetRoleByIdAsync(id);

        return Ok(role);
    }

    [HttpPost]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<ActionResult<RoleDto>> CreateRole([FromBody] RoleDto roleDto)
    {
        var createdRole = await _roleService.CreateRoleAsync(roleDto);

        return CreatedAtAction(
            nameof(GetRoleById),
            new { id = createdRole.RoleId },
            createdRole);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<ActionResult<RoleDto>> UpdateRole(int id, [FromBody] RoleDto roleDto)
    {
        var updatedRole = await _roleService.UpdateRoleAsync(id, roleDto);

        return Ok(updatedRole);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> DeleteRole(int id)
    {
        var deleted = await _roleService.DeleteRoleAsync(id);

        return NoContent();
    }
}