using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FlowerPowerGames.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GamesController : ControllerBase
{
    private readonly IGameService _gameService;

    public GamesController(IGameService gameService)
    {
        _gameService = gameService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GameDto>>> GetAllGames()
    {
        var games = await _gameService.GetAllGamesAsync();
        return Ok(games);
    }

    [HttpGet("{id}")]
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
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<GameDto>> Updategame(int id, [FromBody] GameDto gameDto)
    {
        var updatedGame = await _gameService.UpdateGameAsync(id, gameDto);

        if (updatedGame is null)
        {
            return NotFound();
        }

        return Ok(updatedGame);
    }

    [HttpDelete("{id}")]
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

