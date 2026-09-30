using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using FlowerPowerGames.Business.Authentication;
using Microsoft.AspNetCore.Authorization;
namespace FlowerPowerGames.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserPreferenceController : ControllerBase
{
    private readonly IUserPreferenceService _userPreferenceService;
    public UserPreferenceController(IUserPreferenceService userPreferenceService)
    {
        _userPreferenceService = userPreferenceService;
    }

    [HttpGet("{userId}")]
    [Authorize]
    public async Task<ActionResult<UserPreferenceDTO>> GetUserPreferenceByUserId(int userId)
    {
        var userPreference = await _userPreferenceService.GetUserPreferenceByUserIdAsync(userId);
        if (userPreference == null)
        {
            return NotFound();
        }
        return Ok(userPreference);
    }

    [HttpPost]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<ActionResult<UserPreferenceDTO>> CreateUserPreference(UserPreferenceDTO userPreferenceDto)
    {
        try
        {
            var createdUserPreference = await _userPreferenceService.CreateUserPreferenceAsync(userPreferenceDto);

            return CreatedAtAction(
                nameof(GetUserPreferenceByUserId), 
                new { userId = createdUserPreference.UserId }, 
                createdUserPreference);

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

    [HttpPut("{userId}")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<ActionResult<UserPreferenceDTO>> UpdateUserPreference(int userId, UserPreferenceDTO userPreferenceDto)
    {
        try
        {
            var updatedUserPreference = await _userPreferenceService.UpdateUserPreferenceAsync(userId, userPreferenceDto);
            if (updatedUserPreference == null)
            {
                return NotFound();
            }
            return Ok(updatedUserPreference);
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
}
