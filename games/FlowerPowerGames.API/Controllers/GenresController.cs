using FlowerPowerGames.Business.Authentication;
using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowerPowerGames.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GenresController : ControllerBase
{
    private readonly IGenreService _genreService;
    private readonly IValidator<GenreDto> _genreValidator;

    public GenresController(IGenreService genreService, IValidator<GenreDto> genreValidator)
    {
        _genreService = genreService;
        _genreValidator = genreValidator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<GenreDto>>> GetAllGenres()
    {
        var genres = await _genreService.GetAllGenresAsync();

        return Ok(genres);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<GenreDto>> GetGenreById(int id)
    {
        var genre = await _genreService.GetGenreByIdAsync(id);
        
        return Ok(genre);
    }

    [HttpPost]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<ActionResult<GenreDto>> CreateGenre([FromBody] GenreDto genreDto)
    {
        var validationResult = await _genreValidator.ValidateAsync(genreDto);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }


        var createdGenre = await _genreService.CreateGenreAsync(genreDto);

        return CreatedAtAction(
            nameof(GetGenreById),
            new { id = createdGenre.Id },
            createdGenre);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<ActionResult<GenreDto>> UpdateGenre(int id, [FromBody] GenreDto genreDto)
    {
        var validationResult = await _genreValidator.ValidateAsync(genreDto);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }


        var updatedGenre = await _genreService.UpdateGenreAsync(id, genreDto);

        return Ok(updatedGenre);
     
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> DeleteGenre(int id)
    {
        var deleted = await _genreService.DeleteGenreAsync(id);

        return NoContent();
    }
}
