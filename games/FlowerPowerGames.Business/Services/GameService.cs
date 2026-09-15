using AutoMapper;
using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Helpers;
using FlowerPowerGames.Business.Interfaces;
using FlowerPowerGames.Data;
using FlowerPowerGames.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlowerPowerGames.Business.Services;

public class GameService : IGameService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public GameService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<GameDto>> GetAllGamesAsync()
    {
        var games = await _context.Games
            .AsNoTracking()
            .Include(game => game.Genres)
            .Include(game => game.Types)
            .ToListAsync();

        return _mapper.Map<List<GameDto>>(games);
    }

    public async Task<GameDto?> GetGameByIdAsync(int id)
    {
        var game = await _context.Games
            .AsNoTracking()
            .Include(game => game.Genres)
            .Include(game => game.Types)
            .FirstOrDefaultAsync(game => game.Id == id);

        if (game is null)
        {
            return null;
        }

        return _mapper.Map<GameDto>(game);
    }

    public async Task<GameDto> CreateGameAsync(GameDto gameDto)
    {
        var name = HelpersImplementation.CleanName(gameDto.Name);

        ValidateGame(gameDto, name);

        await EnsureGameNameIsUniqueAsync(name);

        var genreIds = gameDto.GenreIds.Distinct().ToList();

        var typeIds = gameDto.TypeIds.Distinct().ToList();

        var genres = await GetAndValidateGenresAsync(genreIds);

        var types = await GetAndValidateTypesAsync(typeIds);

        var game = _mapper.Map<Game>(gameDto);

        game.Name = name;
        game.Genres = genres;
        game.Types = types;

        _context.Games.Add(game);

        await _context.SaveChangesAsync();

        return _mapper.Map<GameDto>(game);
    }

    public async Task<GameDto?> UpdateGameAsync(int id, GameDto gameDto)
    {
        var game = await _context.Games
            .Include(game => game.Genres)
            .Include(game => game.Types)
            .FirstOrDefaultAsync(game => game.Id == id);

        if (game is null)
            return null;

        var name = HelpersImplementation.CleanName(gameDto.Name);

        ValidateGame(gameDto, name);

        await EnsureGameNameIsUniqueAsync(name, id);

        var genreIds = gameDto.GenreIds.Distinct().ToList();
        var typeIds = gameDto.TypeIds.Distinct().ToList();

        var genres = await GetAndValidateGenresAsync(genreIds);
        var types = await GetAndValidateTypesAsync(typeIds);

        _mapper.Map(gameDto, game);

        game.Name = name;

        game.Genres.Clear();
        foreach (var genre in genres)
        {
            game.Genres.Add(genre);
        }

        game.Types.Clear();
        foreach(var type in types)
        {
            game.Types.Add(type);
        }

        await _context.SaveChangesAsync();

        return _mapper.Map<GameDto>(game);
    }


    public async Task<bool> DeleteGameAsync(int id)
    {
        var game = await _context.Games.FindAsync(id);

        if (game is null)
        {
            return false;
        }

        _context.Games.Remove(game);
        await _context.SaveChangesAsync();

        return true;
    }

    private static void ValidateGame (GameDto gameDto, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Game name cannot be empty.");
        }

        if (gameDto.MaxPlayers < gameDto.MinPlayers)
        {
            throw new ArgumentException("Maximum number of players cannot be lower than minimum number of players.");
        }

        if (gameDto.GenreIds.Count == 0)
        {
            throw new ArgumentException("A game must have at least one genre.");
        }

        if (gameDto.TypeIds.Count == 0)
        {
            throw new ArgumentException("A game must have at least one type.");
        }
    }

    private async Task EnsureGameNameIsUniqueAsync(string name, int? excludedId = null)
    {
        var comparisonName = name.ToLower();

        var exists = await _context.Games
            .AnyAsync( game => game.Name.ToLower() == comparisonName 
            && (!excludedId.HasValue || game.Id != excludedId));

        if (exists)
        {
            throw new InvalidOperationException($"A game named '{name}' aready exists.");
        }
    }

    // Most likely will need a different service when Preferences is implemented
    private async Task<List<Genre>> GetAndValidateGenresAsync(List<int> genreIds)
    {
        if (genreIds.Count == 0)
        {
            throw new ArgumentException("A game must have at leat one genere.");
        }

        if (genreIds.Any(id => id <= 0))
        {
            throw new ArgumentException("Genre Ids must be positive.");
        }

        var genres = await _context.Genres
            .Where(genre => genreIds.Contains(genre.Id))
            .ToListAsync();

        if (genres.Count != genreIds.Count)
        {
            var existingIds = genres.Select(genre => genre.Id).ToHashSet();

            var missingIds = genreIds.Where(id => !existingIds.Contains(id));

            throw new ArgumentException($"The following genre Ids do not exist: " + $"{string.Join(",", missingIds)}.");
        }

        return genres;
    }

    private async Task<List<GameType>> GetAndValidateTypesAsync(List<int> typeIds)
    {
        if (typeIds.Count == 0)
        {
            throw new ArgumentException("A game must have at least one type.");
        }

        if (typeIds.Any(id => id <= 0))
        {
            throw new ArgumentException("Type Ids must be positive");
        }

        var types = await _context.GameTypes
            .Where(type => typeIds.Contains(type.Id))
            .ToListAsync();

        if (types.Count != typeIds.Count)
        {
            var existingIds = types.Select(type => type.Id).ToHashSet();

            var missingIds = typeIds.Where(id => !existingIds.Contains(id));

            throw new ArgumentException($"The following type Ids do not exist: " + $"{string.Join(", ", missingIds)}.");
        }

        return types;
    }

}