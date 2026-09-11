using AutoMapper;
using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Helpers;
using FlowerPowerGames.Business.Interfaces;
using FlowerPowerGames.Data;
using FlowerPowerGames.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FlowerPowerGames.Business.Services;

public class GenreService : IGenreService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public GenreService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<GenreDto>> GetAllGenresAsync()
    {
        var genres = await _context.Genres
            .AsNoTracking()
            .OrderBy(genre => genre.Name)
            .ToListAsync();

        return _mapper.Map<List<GenreDto>>(genres);
    }

    public async Task<GenreDto?> GetGenreByIdAsync(int id)
    {
        var genre = await _context.Genres
            .AsNoTracking()
            .FirstOrDefaultAsync(genre => genre.Id == id);

        return genre is null ? null : _mapper.Map<GenreDto>(genre);
    }

    public async Task<GenreDto> CreateGenreAsync(GenreDto genreDto)
    {
        var name = HelpersImplementation.CleanName(genreDto.Name);

        ValidateName(name);

        await EnsureNameIsUniqueAsync(name);

        var genre = _mapper.Map<Genre>(genreDto);

        genre.Name = name;

        _context.Genres.Add(genre);

        await _context.SaveChangesAsync();

        return _mapper.Map<GenreDto>(genre);

    }

    public async Task<GenreDto?> UpdateGenreAsync(int id, GenreDto genreDto)
    {
        var genre = await _context.Genres.FindAsync(id);

        if (genre == null)
        {
            return null;
        }

        var name = HelpersImplementation.CleanName(genreDto.Name);

        ValidateName(name);

        await EnsureNameIsUniqueAsync(name, id);

        genre.Name = name;

        await _context.SaveChangesAsync();

        return _mapper.Map<GenreDto>(genre);
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Genre name cannot be empty.");
        }
    }

    private async Task EnsureNameIsUniqueAsync(string name, int? excludedId = null)
    {
        var comparisonName = name.ToLower();
        
        var exists = await _context.Genres
            .AnyAsync(genre => genre.Name.ToLower() == comparisonName
                && (!excludedId.HasValue || genre.Id != excludedId.Value));

        if (exists)
        {
            throw new InvalidOperationException($"A genre named '{name}' already exists");
        }
    }
}
