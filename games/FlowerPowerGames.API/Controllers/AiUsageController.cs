using FlowerPowerGames.Business.Authentication;
using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Exceptions;
using FlowerPowerGames.Business.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowerPowerGames.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AiUsageController : ControllerBase
{
    private readonly IAiUsageService _aiUsageService;
    private readonly IValidator<AiUsageDTO> _aiUsageValidator;
    private readonly IValidator<CreateAiUsageDTO> _createAiUsageValidator;

    private readonly IValidator<UpdateAiUsageDTO> _updateAiUsageValidator;
    private readonly ICurrentUserService _currentUserService;

    public AiUsageController(IAiUsageService aiUsageService, IValidator<AiUsageDTO> aiUsageValidator, IValidator<CreateAiUsageDTO> createAiUsageValidator, IValidator<UpdateAiUsageDTO> updateAiUsageValidator, ICurrentUserService currentUserService)
    {
        _aiUsageService = aiUsageService;
        _aiUsageValidator = aiUsageValidator;
        _createAiUsageValidator = createAiUsageValidator;
        _updateAiUsageValidator = updateAiUsageValidator;
        _currentUserService = currentUserService;
    }

    
    [HttpGet("user/{userId}")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> GetAiUsageByUserId(int userId)
    {
        var aiUsage = await _aiUsageService.GetAiUsageByUserIdAsync(userId);
        return Ok(aiUsage);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> GetAiUsageById(int id)
    {
        var aiUsage = await _aiUsageService.GetAiUsageByIdAsync(id);
        return Ok(aiUsage);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateAiUsage([FromBody] CreateAiUsageDTO createAiUsageDto)
    {
        var validationResult = await _createAiUsageValidator.ValidateAsync(createAiUsageDto);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var userId = _currentUserService.UserId
                     ?? throw new UnauthorizedException("User is not authenticated.");

        var createdAiUsage = await _aiUsageService.CreateAiUsageAsync(createAiUsageDto, userId);

        return CreatedAtAction(
            nameof(GetAiUsageById),
            new { id = createdAiUsage.Id },
            createdAiUsage);
    }


    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateAiUsage(int id, [FromBody] AiUsageDTO aiUsageDto)
    {
        var validationResult = await _aiUsageValidator.ValidateAsync(aiUsageDto);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var updatedAiUsage = await _aiUsageService.UpdateAiUsageAsync(id, aiUsageDto);
        return Ok(updatedAiUsage);

    }

    [HttpPut("admin/{id}")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> UpdateAiUsageByAdmin(int id, [FromBody] UpdateAiUsageDTO updateAiUsageDto)
    {
        var validationResult = await _updateAiUsageValidator.ValidateAsync(updateAiUsageDto);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }
        var updatedAiUsage = await _aiUsageService.UpdateAiUsageAdminAsync(id, updateAiUsageDto);
        return Ok(updatedAiUsage);
    }
}