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
public class RatingController : ControllerBase
{
    private readonly IRatingService _ratingService;
    private readonly IValidator<RatingDto> _ratingValidator;

    private readonly IValidator<CreateRatingDTO> _createRatingValidator;
    private readonly ICurrentUserService _currentUserService;
    public RatingController(IRatingService ratingService, IValidator<RatingDto> ratingValidator, IValidator<CreateRatingDTO> createRatingValidator, ICurrentUserService currentUserService)
    {
        _ratingService = ratingService;
        _ratingValidator = ratingValidator;
        _createRatingValidator = createRatingValidator;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RatingDto>>> GetAllRatings()
    {
        var ratings = await _ratingService.GetAllRatingsAsync();
        return Ok(ratings);
    }

    [HttpGet("my")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<RatingDto>>> GetMyRatings()
    {
        var userId = _currentUserService.UserId
                 ?? throw new UnauthorizedException("User is not authenticated.");
        var ratings = await _ratingService.GetRatingsByUserIdAsync(userId);
        return Ok(ratings);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RatingDto>> GetRatingById(int id)
    {
        var rating = await _ratingService.GetRatingByIdAsync(id);

        return Ok(rating);
    }

    [HttpGet("game/{gameId}")]
    public async Task<ActionResult<IEnumerable<RatingDto>>> GetRatingsByGameId(int gameId)
    {
        var ratings = await _ratingService.GetRatingsByGameIdAsync(gameId);
        return Ok(ratings);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<RatingDto>> CreateRating([FromBody] CreateRatingDTO createRatingDto)
    {
        var validationResult = await _createRatingValidator.ValidateAsync(createRatingDto);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var userId = _currentUserService.UserId
                 ?? throw new UnauthorizedException("User is not authenticated.");

        var createdRating = await _ratingService.CreateRatingAsync(createRatingDto, userId);

        return CreatedAtAction(
            nameof(GetRatingById),
            new { id = createdRating.Id },
            createdRating);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<ActionResult<RatingDto>> UpdateRating(int id, [FromBody] RatingDto ratingDto)
    {
        var validationResult = await _ratingValidator.ValidateAsync(ratingDto);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }


        var updatedRating = await _ratingService.UpdateRatingAsync(id, ratingDto);

        return Ok(updatedRating);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteRating(int id)
    {
        var deleted = await _ratingService.DeleteRatingAsync(id);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}

