using FlowerPowerGames.Business.Services;
using FlowerPowerGames.Data;
using FlowerPowerGames.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlowerPowerGames.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RolesController(IRoleService roleService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Role>>> GetRoles()
    {
        var roles = await roleService.GetAllAsync();

        return Ok(roles);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Role>> GetRole(int id)
    {
        var role = await roleService.GetByIdAsync(id);

        if (role is null)
        {
            return NotFound();
        }

        return Ok(role);
    }

    [HttpPost]
    public async Task<ActionResult<Role>> CreateRole(Role role)
    {
        var createdRole = await roleService.CreateAsync(role);

        if (createdRole is null)
        {
            return Conflict($"A role named '{role.Name}' already exists.");
        }

        return CreatedAtAction(
            nameof(GetRole),
            new { id = createdRole.RoleId },
            createdRole);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateRole(int id, Role role)
    {
        if (id != role.RoleId)
        {
            return BadRequest("The route ID does not match the role ID.");
        }

        var result = await roleService.UpdateAsync(role);

        return result switch
        {
            RoleUpdateResult.NotFound => NotFound(),

            RoleUpdateResult.DuplicateName =>
                Conflict($"A role named '{role.Name}' already exists."),

            RoleUpdateResult.Success => NoContent(),

            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteRole(int id)
    {
        var deleted = await roleService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
