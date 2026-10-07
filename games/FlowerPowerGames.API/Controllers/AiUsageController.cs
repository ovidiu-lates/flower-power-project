using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace FlowerPowerGames.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AiUsageController : ControllerBase
{
    private readonly IAiUsageService _aiUsageService;
    private readonly IValidator<AiUsageDTO> _aiUsageValidator;

    public AiUsageController(IAiUsageService aiUsageService, IValidator<AiUsageDTO> aiUsageValidator)
    {
        _aiUsageService = aiUsageService;
        _aiUsageValidator = aiUsageValidator;
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetAiUsageByUserId(int userId)
    {
        var aiUsage = await _aiUsageService.GetAiUsageByUserIdAsync(userId);
        return Ok(aiUsage);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAiUsageById(int id)
    {
        var aiUsage = await _aiUsageService.GetAiUsageByIdAsync(id);
        return Ok(aiUsage);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAiUsage([FromBody] AiUsageDTO aiUsageDto)
    {
        var validationResult = await _aiUsageValidator.ValidateAsync(aiUsageDto);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var createdAiUsage = await _aiUsageService.CreateAiUsageAsync(aiUsageDto);

        return CreatedAtAction(
            nameof(GetAiUsageById),
            new { id = createdAiUsage.Id },
            createdAiUsage);
    }


    [HttpPut("{id}")]
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
}