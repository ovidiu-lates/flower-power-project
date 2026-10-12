using FlowerPowerGames.Business.DTOs;

namespace FlowerPowerGames.Business.Interfaces;

public interface IGenreService
{
    Task<List<GenreDto>> GetAllGenresAsync();

    Task<GenreDto> GetGenreByIdAsync(int id);

    Task<GenreDto> CreateGenreAsync(GenreDto genreDto);

    Task<GenreDto> UpdateGenreAsync(int id, GenreDto genreDto);

    Task<bool> DeleteGenreAsync(int id);

}
