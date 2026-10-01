using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Interfaces;
using FlowerPowerGames.Data.Models;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowerPowerGames.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FavoriteController: ControllerBase
{
    private readonly IFavoriteService _favoriteService;
    private readonly IValidator<FavoriteDTO> _favoriteValidator;

    public FavoriteController(IFavoriteService favoriteService, IValidator<FavoriteDTO> favoriteValidator)
    {
        _favoriteService = favoriteService;
        _favoriteValidator = favoriteValidator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FavoriteDTO>>> GetAllFavorites()
    {
        var favorites = await _favoriteService.GetAllFavoritesAsync();
        return Ok(favorites);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<FavoriteDTO>> GetFavoriteById(int id)
    {
        var favorite = await _favoriteService.GetFavoriteByIdAsync(id);

        return Ok(favorite);
    }

    [HttpGet("user/{userId}")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<FavoriteDTO>>> GetFavoritesByUserId(int userId)
    {
        var favorites = await _favoriteService.GetFavoritesByUserIdAsync(userId);

        return Ok(favorites);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<FavoriteDTO>> CreateFavorite([FromBody] FavoriteDTO favorite)
    {
        var validationResult = await _favoriteValidator.ValidateAsync(favorite);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var createdFavorite = await _favoriteService.CreateFavoriteAsync(favorite);

        return CreatedAtAction(nameof(GetFavoriteById), new { id = createdFavorite.Id }, createdFavorite);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> DeleteFavorite(int id)
    {
        await _favoriteService.DeleteFavoriteAsync(id);
        return NoContent();
    }

}
