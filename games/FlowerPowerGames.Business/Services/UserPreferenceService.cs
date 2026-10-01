using AutoMapper;
using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Interfaces;
using FlowerPowerGames.Data;
using FlowerPowerGames.Data;
using FlowerPowerGames.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlowerPowerGames.Business.Services;

public class UserPreferenceService : IUserPreferenceService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public UserPreferenceService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<UserPreferenceDTO> GetUserPreferenceByUserIdAsync(int userId)
    {
        var userPreference = await _context.UserPreferences
            .AsNoTracking()
            .Include(game => game.Genres)
            .Include(game => game.Types)
            .FirstOrDefaultAsync(pref => pref.UserId == userId);

        if(userPreference == null)
        {
            return null;
        }

        return _mapper.Map<UserPreferenceDTO>(userPreference);
    }

    public async Task<UserPreferenceDTO> CreateUserPreferenceAsync(UserPreferenceDTO userPreferenceDto)
    {
        await CheckExistenceOfUserAsync(userPreferenceDto);

        CheckExistenceOfUserPreference(await _context.UserPreferences.FirstOrDefaultAsync(pref => pref.UserId == userPreferenceDto.UserId), userPreferenceDto.UserId);

        ValidateUserPreference(userPreferenceDto);

        var genreIds = userPreferenceDto.GenreIds.Distinct().ToList();

        var typeIds = userPreferenceDto.TypeIds.Distinct().ToList();

        var genres = await GetAndValidateGenresAsync(genreIds);

        var types = await GetAndValidateTypesAsync(typeIds);

        var userPreference = _mapper.Map<UserPreference>(userPreferenceDto);

        userPreference.Genres = genres;
        userPreference.Types = types;



        _context.UserPreferences.Add(userPreference);
        await _context.SaveChangesAsync();
        return _mapper.Map<UserPreferenceDTO>(userPreference);
    }

    public async Task<UserPreferenceDTO> UpdateUserPreferenceAsync(int userId, UserPreferenceDTO userPreferenceDto)
    { 

        var userPreference = await _context.UserPreferences
           .Include(game => game.Genres)
           .Include(game => game.Types)
           .FirstOrDefaultAsync(pref => pref.UserId == userId);

        if (userPreference == null)
        {
            return null;
        }

        ValidateUserPreference(userPreferenceDto);

        var genreIds = userPreferenceDto.GenreIds.Distinct().ToList();
        var typeIds = userPreferenceDto.TypeIds.Distinct().ToList();
        var genres = await GetAndValidateGenresAsync(genreIds);
        var types = await GetAndValidateTypesAsync(typeIds);

        _mapper.Map(userPreferenceDto, userPreference);

        userPreference.Genres.Clear();

        foreach (var genre in genres)
        {
            userPreference.Genres.Add(genre);
        }


        userPreference.Types.Clear();

        foreach (var type in types)
        {
            userPreference.Types.Add(type);
        }

        await _context.SaveChangesAsync();

        return _mapper.Map<UserPreferenceDTO>(userPreference);
    }

    private async Task CheckExistenceOfUserAsync(UserPreferenceDTO userPreferenceDto)
    {
        // DTO is assumed correct — just check DB for the user existence
        var userId = userPreferenceDto.UserId;

        var userExists = await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Id == userId);

        if (!userExists)
        {
            throw new InvalidOperationException($"User with id '{userId}' does not exist.");
        }
    }

    private static void CheckExistenceOfUserPreference(UserPreference userPreference, int userId)
    {
        if (userPreference != null)
        {
            throw new InvalidOperationException($"User preference for user ID {userId} already exist.");
        }
    }
    private static void ValidateUserPreference(UserPreferenceDTO userPreferenceDto)
    {
        
        if (userPreferenceDto.MinBudget > userPreferenceDto.MaxBudget)
        {
            throw new ArgumentException("Minimum budget cannot be greater than maximum budget.");
        }
        if (userPreferenceDto.MinPlayTime > userPreferenceDto.MaxPlayTime)
        {
            throw new ArgumentException("Minimum play time cannot be greater than maximum play time.");
        }
        if (userPreferenceDto.MinPlayers > userPreferenceDto.MaxPlayers)
        {
            throw new ArgumentException("Minimum players cannot be greater than maximum players.");
        }
    }

    private async Task<List<Genre>> GetAndValidateGenresAsync(List<int> genreIds)
    {
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
