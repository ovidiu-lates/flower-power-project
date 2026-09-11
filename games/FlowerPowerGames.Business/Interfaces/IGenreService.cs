using FlowerPowerGames.Business.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlowerPowerGames.Business.Interfaces;

public interface IGenreService
{
    Task<List<GenreDto>> GetAllGenresAsync();

    Task<GenreDto?> GetGenreByIdAsync(int id);

    Task<GenreDto> CreateGenreAsync(GenreDto genreDto);

    Task<GenreDto?> UpdateGenreAsync(int id, GenreDto genreDto);
}
