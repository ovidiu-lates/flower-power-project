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

        return Ok(userPreference);
    }

    [HttpPost]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<ActionResult<UserPreferenceDTO>> CreateUserPreference(UserPreferenceDTO userPreferenceDto)
    {
        var createdUserPreference = await _userPreferenceService.CreateUserPreferenceAsync(userPreferenceDto);

        return CreatedAtAction(
            nameof(GetUserPreferenceByUserId), 
            new { userId = createdUserPreference.UserId }, 
            createdUserPreference);
    }

    [HttpPut("{userId}")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<ActionResult<UserPreferenceDTO>> UpdateUserPreference(int userId, UserPreferenceDTO userPreferenceDto)
    {
        var updatedUserPreference = await _userPreferenceService.UpdateUserPreferenceAsync(userId, userPreferenceDto);

        return Ok(updatedUserPreference);        
    }
}
