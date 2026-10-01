using AutoMapper;
using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Exceptions;
using FlowerPowerGames.Business.Helpers;
using FlowerPowerGames.Business.Interfaces;
using FlowerPowerGames.Data;
using FlowerPowerGames.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlowerPowerGames.Business.Services;

public class GameTypeService : IGameTypeService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public GameTypeService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<GameTypeDto>> GetAllGameTypesAsync()
    {
        var types = await _context.GameTypes
            .AsNoTracking()
            .OrderBy(type => type.Name)
            .ToListAsync();

        return _mapper.Map<List<GameTypeDto>>(types);
    }

    public async Task<GameTypeDto> GetGameTypeByIdAsync(int id)
    {
        var type = await _context.GameTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(type => type.Id == id);

        if (type is null)
        {
            throw new NotFoundException($"Game type with id {id} was not found.");
        }

        return _mapper.Map<GameTypeDto>(type);
    }

    public async Task<GameTypeDto> CreateGameTypeAsync(GameTypeDto gameTypeDto)
    {
        var name = HelpersImplementation.CleanName(gameTypeDto.Name);

        ValidateName(name);

        await EnsureNameIsUniqueAsync(name);

        var type = _mapper.Map<GameType>(gameTypeDto);

        type.Name = name;

        _context.GameTypes.Add(type);

        await _context.SaveChangesAsync();

        return _mapper.Map<GameTypeDto>(type);

    }

    public async Task<GameTypeDto> UpdateGameTypeAsync(int id, GameTypeDto gameTypeDto)
    {
        var type = await _context.GameTypes.FindAsync(id);

        if (type is null)
        {
            throw new NotFoundException($"Game type with id {id} was not found.");
        }

        var name = HelpersImplementation.CleanName(gameTypeDto.Name);

        ValidateName(name);

        await EnsureNameIsUniqueAsync(name, id);

        type.Name = name;

        await _context.SaveChangesAsync();

        return _mapper.Map<GameTypeDto>(type);
    }

    public async Task<bool> DeleteGameTypeAsync(int id)
    {
        var type = await _context.GameTypes
            .Include(type => type.Games)
            .FirstOrDefaultAsync(type => type.Id == id);

        if (type is null)
        {
            throw new NotFoundException($"Game type with id {id} was not found.");
        }

        if (type.Games.Count > 0)
        {
            throw new ConflictException("Game type cannot be deleted because it is assigned to one or more games.");
        }

        _context.GameTypes.Remove(type);

        await _context.SaveChangesAsync();

        return true;
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Type name cannot be empty.");
        }
    }

    private async Task EnsureNameIsUniqueAsync(string name, int? excludedId = null)
    {
        var comparisonName = name.ToLower();

        var exists = await _context.GameTypes
            .AnyAsync(type => type.Name.ToLower() == comparisonName
                && (!excludedId.HasValue || type.Id != excludedId.Value));

        if (exists)
        {
            throw new ConflictException($"A game type named '{name}' already exists.");
        }
    }
}
