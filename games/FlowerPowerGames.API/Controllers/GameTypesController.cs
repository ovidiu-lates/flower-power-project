using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FlowerPowerGames.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameTypesController : ControllerBase
{
    private readonly IGameTypeService _gameTypeService;

    public GameTypesController(IGameTypeService gameTypeService)
    {
        _gameTypeService = gameTypeService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GameTypeDto>>> GetAllGameTypes()
    {
        var types = await _gameTypeService.GetAllGameTypesAsync();

        return Ok(types);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GameTypeDto>> GetGameTypeById(int id)
    {
        var type = await _gameTypeService.GetGameTypeByIdAsync(id);

        if (type == null)
        {
            return NotFound();
        }

        return Ok(type);
    }

    [HttpPost]
    public async Task<ActionResult<GameTypeDto>> CreateGameTypes([FromBody] GameTypeDto gameTypeDto)
    {
        try
        {
            var createdType = await _gameTypeService.CreateGameTypeAsync(gameTypeDto);

            return CreatedAtAction(
                nameof(GetGameTypeById),
                new { id = createdType.Id },
                createdType);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch(InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<GameTypeDto>> UpdateGameType(int id, [FromBody]GameTypeDto gameTypeDto)
    {
        try
        {
            var updatedType = await _gameTypeService.UpdateGameTypeAsync(id, gameTypeDto);

            if (updatedType == null)
            {
                return NotFound();
            }

            return Ok(updatedType);
        }
        catch(ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch(InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

   
}