using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using FlowerPowerGames.Business.Authentication;
using Microsoft.AspNetCore.Authorization;
using FluentValidation;
namespace FlowerPowerGames.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GamesController : ControllerBase
{
    private readonly IGameService _gameService;
    private readonly IValidator<GameDto> _gameValidator;

    public GamesController(IGameService gameService, IValidator<GameDto> gameValidator)
    {
        _gameService = gameService;
        _gameValidator = gameValidator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GameDto>>> GetAllGames()
    {
        var games = await _gameService.GetAllGamesAsync();
        return Ok(games);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<GameDto>> GetGameById(int id)
    {
        var game = await _gameService.GetGameByIdAsync(id);

        return Ok(game);
    }

    [HttpPost]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<ActionResult<GameDto>> CreateGame([FromBody] GameDto gameDto)
    {
        var validationResult = await _gameValidator.ValidateAsync(gameDto);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }


        var createdGame = await _gameService.CreateGameAsync(gameDto);

        return CreatedAtAction(
            nameof(GetGameById),
            new { id = createdGame.Id },
            createdGame);   
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<ActionResult<GameDto>> Updategame(int id, [FromBody] GameDto gameDto)
    {
        var validationResult = await _gameValidator.ValidateAsync(gameDto);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var updatedGame = await _gameService.UpdateGameAsync(id, gameDto);

        return Ok(updatedGame);
    }
    
    [HttpDelete("{id:int}")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> DeleteGame(int id)
    {
        await _gameService.DeleteGameAsync(id);

        return NoContent();
    }
}

