using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Interfaces;
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
    public async Task<ActionResult<IEnumerable<RoleDto>>> GetAllRoles()
    {
        var roles = await _roleService.GetAllRolesAsync();

        return Ok(roles);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RoleDto>> GetRoleById(int id)
    {
        var role = await _roleService.GetRoleByIdAsync(id);

        if (role is null)
        {
            return NotFound();
        }

        return Ok(role);
    }

    [HttpPost]
    public async Task<ActionResult<RoleDto>> CreateRole([FromBody] RoleDto roleDto)
    {
        try
        {
            var createdRole = await _roleService.CreateRoleAsync(roleDto);

            return CreatedAtAction(
                nameof(GetRoleById),
                new { id = createdRole.RoleId },
                createdRole);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<RoleDto>> UpdateRole(int id, [FromBody] RoleDto roleDto)
    {
        try
        {
            var updatedRole = await _roleService.UpdateRoleAsync(id, roleDto);

            if (updatedRole is null)
            {
                return NotFound();
            }

            return Ok(updatedRole);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteRole(int id)
    {
        var deleted = await _roleService.DeleteRoleAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}