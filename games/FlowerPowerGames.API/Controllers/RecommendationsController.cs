using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Exceptions;
using FlowerPowerGames.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowerPowerGames.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RecommendationsController : ControllerBase
{
    private readonly IRecommendationService _recommendationService;

    private readonly ICurrentUserService _currentUserService;

    public RecommendationsController(IRecommendationService recommendationService, ICurrentUserService currentUserService)
    {
        _recommendationService = recommendationService;
        _currentUserService = currentUserService;
    }

    [HttpPost]
    public async Task<ActionResult<RecommendationResponseDto>> GetRecommendations([FromBody] RecommendationRequestDto request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId ?? throw new UnauthorizedException("User is not authenticated.");

        var recommendations =
            await _recommendationService.GetRecommendationsAsync( userId, request, cancellationToken: cancellationToken);

        return Ok(recommendations);
    }
}
