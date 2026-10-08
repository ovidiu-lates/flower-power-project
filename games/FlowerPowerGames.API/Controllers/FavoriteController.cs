using FlowerPowerGames.Business.Authentication;
using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Exceptions;
using FlowerPowerGames.Business.Interfaces;
using FlowerPowerGames.Business.Services;
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
    private readonly IValidator<CreateFavoriteDTO> _createFavoriteValidator;

    private readonly ICurrentUserService _currentUserService;

    public FavoriteController(IFavoriteService favoriteService, IValidator<CreateFavoriteDTO> createFavoriteValidator, ICurrentUserService currentUserService)
    {
        _favoriteService = favoriteService;
        _createFavoriteValidator = createFavoriteValidator;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<ActionResult<IEnumerable<FavoriteDTO>>> GetAllFavorites()
    {
        var favorites = await _favoriteService.GetAllFavoritesAsync();
        return Ok(favorites);
    }

    [HttpGet("my")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<FavoriteDTO>>> GetMyFavorites()
    {
        var userId = _currentUserService.UserId
                     ?? throw new UnauthorizedException("User is not authenticated.");

        var favorites = await _favoriteService.GetFavoritesByUserIdAsync(userId);
        return Ok(favorites);
    }



    [HttpGet("{id}")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<ActionResult<FavoriteDTO>> GetFavoriteById(int id)
    {
        var favorite = await _favoriteService.GetFavoriteByIdAsync(id);

        return Ok(favorite);
    }

    [HttpGet("user/{userId}")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<ActionResult<IEnumerable<FavoriteDTO>>> GetFavoritesByUserId(int userId)
    {
        var favorites = await _favoriteService.GetFavoritesByUserIdAsync(userId);

        return Ok(favorites);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<FavoriteDTO>> CreateFavorite([FromBody] CreateFavoriteDTO createFavoriteDTO)
    {
        var validationResult = await _createFavoriteValidator.ValidateAsync(createFavoriteDTO);
        if (!validationResult.IsValid) return BadRequest(validationResult.Errors);

        var userId = _currentUserService.UserId
                     ?? throw new UnauthorizedException("User is not authenticated.");

        var created = await _favoriteService.CreateFavoriteAsync(createFavoriteDTO, userId);

        return CreatedAtAction(nameof(GetFavoriteById), new { id = created.Id }, created);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> DeleteFavorite(int id)
    {
        var userId = _currentUserService.UserId
                     ?? throw new UnauthorizedException("User is not authenticated.");

        var role = _currentUserService.Role
                      ?? throw new UnauthorizedException("User role is not authenticated.");

        await _favoriteService.DeleteFavoriteAsync(id, userId, role);
        return NoContent();
    }

}
