using AutoMapper;
using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Interfaces;
using FlowerPowerGames.Data;
using FlowerPowerGames.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using FlowerPowerGames.Business.Exceptions;

namespace FlowerPowerGames.Business.Services;

public class FavoriteService : IFavoriteService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public FavoriteService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<FavoriteDTO>> GetAllFavoritesAsync()
    {
        var favorites = await _context.Favorites
            .Include(f => f.Game)
            .ToListAsync();
        return _mapper.Map<List<FavoriteDTO>>(favorites);
    }

    public async Task<List<FavoriteDTO>> GetFavoritesByUserIdAsync(int userId)
    {
        var favorites = await _context.Favorites
            .Include(f => f.Game)
            .Where(f => f.UserId == userId)
            .ToListAsync();

        if (favorites is null)
        {
            throw new NotFoundException($"Favorites for the user with id {userId} were not found.");
        }

        return _mapper.Map<List<FavoriteDTO>>(favorites);
    }



    public async Task<FavoriteDTO> GetFavoriteByIdAsync(int id)
    {
        var favorite = await _context.Favorites
            .Include(f => f.Game)
            .FirstOrDefaultAsync(f => f.Id == id);
        if (favorite is null)
        {
            throw new NotFoundException($"Favorite with id {id} was not found.");
        }
        return _mapper.Map<FavoriteDTO>(favorite);
    }

    public async Task<FavoriteDTO> CreateFavoriteAsync(FavoriteDTO favoriteDto)
    {
        var gameExists = await _context.Games
            .AnyAsync(g => g.Id == favoriteDto.GameId);

        if (!gameExists)
        {
            throw new NotFoundException(
                $"Game with id {favoriteDto.GameId} does not exist.");
        }

        var userExists = await _context.Users
            .AnyAsync(u => u.Id == favoriteDto.UserId);

        if (!userExists)
        {
            throw new NotFoundException( $"User with id {favoriteDto.UserId} does not exist.");
        }

        var favoriteExists = await _context.Favorites
            .AnyAsync(f =>
                f.UserId == favoriteDto.UserId &&
                f.GameId == favoriteDto.GameId);

        if (favoriteExists)
        {
            throw new ConflictException(
                "This game is already in the user's favorites.");
        }

        var favorite = _mapper.Map<Favorite>(favoriteDto);

        _context.Favorites.Add(favorite);
        await _context.SaveChangesAsync();

        return _mapper.Map<FavoriteDTO>(favorite);
    }

    public async Task<bool> DeleteFavoriteAsync(int id)
    {
        var favorite = await _context.Favorites.FindAsync(id);
        if (favorite is null)
        {
            throw new NotFoundException("Favorite not found.");
        }
        _context.Favorites.Remove(favorite);
        await _context.SaveChangesAsync();
        return true;
    }
}
