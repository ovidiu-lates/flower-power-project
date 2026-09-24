using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace FlowerPowerGames.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RatingController : ControllerBase
{
    private readonly IRatingService _ratingService;
    private readonly IValidator<RatingDto> _ratingValidator;

    public RatingController(IRatingService ratingService, IValidator<RatingDto> ratingValidator)
    {
        _ratingService = ratingService;
        _ratingValidator = ratingValidator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RatingDto>>> GetAllRatings()
    {
        var ratings = await _ratingService.GetAllRatingsAsync();
        return Ok(ratings);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RatingDto>> GetRatingById(int id)
    {
        var rating = await _ratingService.GetRatingByIdAsync(id);

        if (rating is null)
        {
            return NotFound();
        }

        return Ok(rating);
    }

    [HttpGet("game/{gameId}")]
    public async Task<ActionResult<IEnumerable<RatingDto>>> GetRatingsByGameId(int gameId)
    {
        var ratings = await _ratingService.GetRatingsByGameIdAsync(gameId);
        return Ok(ratings);
    }

    [HttpPost]
    public async Task<ActionResult<RatingDto>> CreateRating([FromBody] RatingDto ratingDto)
    {
        var validationResult = await _ratingValidator.ValidateAsync(ratingDto);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        try
        {
            var createdRating = await _ratingService.CreateRatingAsync(ratingDto);

            return CreatedAtAction(
                nameof(GetRatingById),
                new { id = createdRating.Id },
                createdRating);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<RatingDto>> UpdateRating(int id, [FromBody] RatingDto ratingDto)
    {
        var validationResult = await _ratingValidator.ValidateAsync(ratingDto);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        try
        {
            var updatedRating = await _ratingService.UpdateRatingAsync(id, ratingDto);

            if (updatedRating is null)
            {
                return NotFound();
            }

            return Ok(updatedRating);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRating(int id)
    {
        try
        {
            var deleted = await _ratingService.DeleteRatingAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}

