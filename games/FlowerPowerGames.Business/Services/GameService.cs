using AutoMapper;
using FlowerPowerGames.Business.DTOs;
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
        var games = await _context.Games.ToListAsync();

        return _mapper.Map<List<GameDto>>(games);
    }

    public async Task<GameDto?> GetGameByIdAsync(int id)
    {
        var game = await _context.Games.FindAsync(id);

        if (game is null)
        {
            return null;
        }

        return _mapper.Map<GameDto>(game);
    }

    public async Task<GameDto> CreateGameAsync(GameDto gameDto)
    {
        if (gameDto.MaxPlayers < gameDto.MinPlayers)
        {
            throw new ArgumentException(
                "Maximum number of players cannot be lower than minimum number of players.");
        }

        var game = _mapper.Map<Game>(gameDto);

        _context.Games.Add(game);
        await _context.SaveChangesAsync();

        return _mapper.Map<GameDto>(game);
    }

    public async Task<GameDto?> UpdateGameAsync(int id, GameDto gameDto)
    {
        var game = await _context.Games.FindAsync(id);

        if (game is null)
        {
            return null;
        }

        _mapper.Map(gameDto, game);
        
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

}