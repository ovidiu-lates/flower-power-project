using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Exceptions;
using FlowerPowerGames.Business.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace FlowerPowerGames.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserPreferenceController : ControllerBase
{
    private readonly IUserPreferenceService _userPreferenceService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IValidator<CreateUserPreferenceDto> _createUserPreferenceValidator;

    public UserPreferenceController(IUserPreferenceService userPreferenceService, IValidator<CreateUserPreferenceDto> createUserPreferenceValidator, ICurrentUserService currentUserService)
    {
        _userPreferenceService = userPreferenceService;
        _createUserPreferenceValidator = createUserPreferenceValidator;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<UserPreferenceDto>> GetUserPreferenceByUserId()
    {
        var userId = _currentUserService.UserId
                 ?? throw new UnauthorizedException("User is not authenticated.");

        var userPreference = await _userPreferenceService.GetUserPreferenceByUserIdAsync(userId);

        return Ok(userPreference);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<UserPreferenceDto>> CreateUserPreference(CreateUserPreferenceDto createUserPreferenceDto)
    {
        var validationResult = await _createUserPreferenceValidator.ValidateAsync(createUserPreferenceDto);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var userId = _currentUserService.UserId
                 ?? throw new UnauthorizedException("User is not authenticated.");

        var createdUserPreference = await _userPreferenceService.CreateUserPreferenceAsync(createUserPreferenceDto, userId);

        return CreatedAtAction(
            nameof(GetUserPreferenceByUserId), 
            new { userId = createdUserPreference.UserId }, 
            createdUserPreference);
    }

    [HttpPut]
    [Authorize]
    public async Task<ActionResult<UserPreferenceDto>> UpdateUserPreference(CreateUserPreferenceDto createUserPreferenceDto)
    {
        var userId = _currentUserService.UserId
                 ?? throw new UnauthorizedException("User is not authenticated.");

        var validationResult = await _createUserPreferenceValidator.ValidateAsync(createUserPreferenceDto);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var updatedUserPreference = await _userPreferenceService.UpdateUserPreferenceAsync(userId, createUserPreferenceDto);

        return Ok(updatedUserPreference);
        
    }
}
