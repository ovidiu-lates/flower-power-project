using Microsoft.AspNetCore.Mvc;
using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Interfaces;

namespace FlowerPowerGames.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GenresController : ControllerBase
{
    private readonly IGenreService _genreService;

    public GenresController(IGenreService genreService)
    {
        _genreService = genreService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GenreDto>>> GetAllGenres()
    {
        var genres = await _genreService.GetAllGenresAsync();

        return Ok(genres);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GenreDto>> GetGenreById(int id)
    {
        var genre = await _genreService.GetGenreByIdAsync(id);
        
        if (genre == null)
        {
            return NotFound();
        }
        return Ok(genre);
    }

    [HttpPost]
    public async Task<ActionResult<GenreDto>> CreateGenre([FromBody] GenreDto genreDto)
    {
        try
        {
            var createdGenre = await _genreService.CreateGenreAsync(genreDto);

            return CreatedAtAction(
                nameof(GetGenreById),
                new { id = createdGenre.Id },
                createdGenre);
        }
        catch(ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<GenreDto>> UpdateGenre(int id, [FromBody] GenreDto genreDto)
    {
        try
        {
            var updatedGenre = await _genreService.UpdateGenreAsync(id, genreDto);

            if (updatedGenre == null)
            {
                return NotFound();
            }

            return Ok(updatedGenre);
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
