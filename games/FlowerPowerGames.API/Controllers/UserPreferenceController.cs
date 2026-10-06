using AutoMapper;
using FlowerPowerGames.Business.Authentication;
using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Exceptions;
using FlowerPowerGames.Business.Interfaces;
using FlowerPowerGames.Business.Services;
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
    private readonly IValidator<CreateUserPreferenceDTO> _createUserPreferenceValidator;
    private readonly IValidator<UserPreferenceDTO> _userPreferenceValidator;

    public UserPreferenceController(IUserPreferenceService userPreferenceService, IValidator<UserPreferenceDTO> userPreferenceValidator, IValidator<CreateUserPreferenceDTO> createUserPreferenceValidator, ICurrentUserService currentUserService)
    {
        _userPreferenceService = userPreferenceService;
        _userPreferenceValidator = userPreferenceValidator;
        _createUserPreferenceValidator = createUserPreferenceValidator;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<UserPreferenceDTO>> GetUserPreferenceByUserId()
    {
        var userId = _currentUserService.UserId
                 ?? throw new UnauthorizedException("User is not authenticated.");

        var userPreference = await _userPreferenceService.GetUserPreferenceByUserIdAsync(userId);

        return Ok(userPreference);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<UserPreferenceDTO>> CreateUserPreference(CreateUserPreferenceDTO createUserPreferenceDto)
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
    public async Task<ActionResult<UserPreferenceDTO>> UpdateUserPreference(CreateUserPreferenceDTO createUserPreferenceDto)
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
