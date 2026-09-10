using FlowerPowerGames.Data;
using FlowerPowerGames.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlowerPowerGames.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RolesController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Role>>> GetRoles()
    {
        var roles = await dbContext.Roles
            .AsNoTracking()
            .ToListAsync();

        return Ok(roles);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Role>> GetRole(int id)
    {
        var role = await dbContext.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(role => role.RoleId == id);

        if (role is null)
        {
            return NotFound();
        }

        return Ok(role);
    }

    [HttpPost]
    public async Task<ActionResult<Role>> CreateRole(Role role)
    {
        var roleNameExists = await dbContext.Roles
            .AnyAsync(existingRole => existingRole.Name == role.Name);

        if (roleNameExists)
        {
            return Conflict($"A role named '{role.Name}' already exists.");
        }

        dbContext.Roles.Add(role);
        await dbContext.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetRole),
            new { id = role.RoleId },
            role);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateRole(int id, Role updatedRole)
    {
        if (id != updatedRole.RoleId)
        {
            return BadRequest("The route ID does not match the role ID.");
        }

        var existingRole = await dbContext.Roles
            .FirstOrDefaultAsync(role => role.RoleId == id);

        if (existingRole is null)
        {
            return NotFound();
        }

        var nameExists = await dbContext.Roles
            .AnyAsync(role =>
                role.RoleId != id &&
                role.Name == updatedRole.Name);

        if (nameExists)
        {
            return Conflict($"A role named '{updatedRole.Name}' already exists.");
        }

        existingRole.Name = updatedRole.Name;

        await dbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteRole(int id)
    {
        var role = await dbContext.Roles
            .FirstOrDefaultAsync(role => role.RoleId == id);

        if (role is null)
        {
            return NotFound();
        }

        dbContext.Roles.Remove(role);
        await dbContext.SaveChangesAsync();

        return NoContent();
    }
}
