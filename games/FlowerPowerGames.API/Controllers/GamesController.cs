using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
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

        if (game is null)
        {
            return NotFound();
        }

        return Ok(game);
    }

    [HttpPost]
    public async Task<ActionResult<GameDto>> CreateGame([FromBody] GameDto gameDto)
    {
        var validationResult = await _gameValidator.ValidateAsync(gameDto);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        try
        {
            var createdGame = await _gameService.CreateGameAsync(gameDto);

            return CreatedAtAction(
                nameof(GetGameById),
                new { id = createdGame.Id },
                createdGame);
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

    [HttpPut("{id:int}")]
    public async Task<ActionResult<GameDto>> Updategame(int id, [FromBody] GameDto gameDto)
    {
        var validationResult = await _gameValidator.ValidateAsync(gameDto);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        try
        {
            var updatedGame = await _gameService.UpdateGameAsync(id, gameDto);

            if (updatedGame is null)
            {
                return NotFound();
            }

            return Ok(updatedGame);
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
    
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteGame(int id)
    {
        var deleted = await _gameService.DeleteGameAsync(id);

        if(!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}

