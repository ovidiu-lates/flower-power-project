using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FlowerPowerGames.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FavoriteController: ControllerBase
{
    private readonly IFavoriteService _favoriteService;

    public FavoriteController(IFavoriteService favoriteService)
    {
        _favoriteService = favoriteService;
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

        if (favorite is null)
        {
            return NotFound();
        }

        return Ok(favorite);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<FavoriteDTO>>> GetFavoritesByUserId(int userId)
    {
        var favorites = await _favoriteService.GetFavoritesByUserIdAsync(userId);
        return Ok(favorites);
    }

    [HttpPost]
    public async Task<ActionResult<FavoriteDTO>> CreateFavorite([FromBody] FavoriteDTO favorite)
    {
        try
        {
            var createdFavorite = await _favoriteService.CreateFavoriteAsync(favorite);
            return CreatedAtAction(nameof(GetFavoriteById), new { id = createdFavorite.Id }, createdFavorite);
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

    [HttpPut("{id}")]
    public async Task<ActionResult<FavoriteDTO>> UpdateFavorite(int id, [FromBody] FavoriteDTO favorite)
    {
        try
        {
            var updatedFavorite = await _favoriteService.UpdateFavoriteAsync(id, favorite);
            return Ok(updatedFavorite);
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

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteFavorite(int id)
    {
        try
        {
            await _favoriteService.DeleteFavoriteAsync(id);
            return NoContent();
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
