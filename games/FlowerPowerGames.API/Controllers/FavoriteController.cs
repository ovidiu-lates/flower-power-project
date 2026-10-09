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
public class FavoriteController: ControllerBase
{
    private readonly IFavoriteService _favoriteService;
    private readonly IValidator<CreateFavoriteDto> _createFavoriteValidator;

    private readonly ICurrentUserService _currentUserService;

    public FavoriteController(IFavoriteService favoriteService, IValidator<CreateFavoriteDto> createFavoriteValidator, ICurrentUserService currentUserService)
    {
        _favoriteService = favoriteService;
        _createFavoriteValidator = createFavoriteValidator;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<ActionResult<IEnumerable<FavoriteDto>>> GetAllFavorites()
    {
        var favorites = await _favoriteService.GetAllFavoritesAsync();
        return Ok(favorites);
    }

    [HttpGet("my")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<FavoriteDto>>> GetMyFavorites()
    {
        var userId = _currentUserService.UserId
                     ?? throw new UnauthorizedException("User is not authenticated.");

        var favorites = await _favoriteService.GetFavoritesByUserIdAsync(userId);
        return Ok(favorites);
    }



    [HttpGet("{id}")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<ActionResult<FavoriteDto>> GetFavoriteById(int id)
    {
        var favorite = await _favoriteService.GetFavoriteByIdAsync(id);

        return Ok(favorite);
    }

    [HttpGet("user/{userId}")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<ActionResult<IEnumerable<FavoriteDto>>> GetFavoritesByUserId(int userId)
    {
        var favorites = await _favoriteService.GetFavoritesByUserIdAsync(userId);

        return Ok(favorites);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<FavoriteDto>> CreateFavorite([FromBody] CreateFavoriteDto createFavoriteDto)
    {
        var validationResult = await _createFavoriteValidator.ValidateAsync(createFavoriteDto);
        if (!validationResult.IsValid) return BadRequest(validationResult.Errors);

        var userId = _currentUserService.UserId
                     ?? throw new UnauthorizedException("User is not authenticated.");

        var created = await _favoriteService.CreateFavoriteAsync(createFavoriteDto, userId);

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
