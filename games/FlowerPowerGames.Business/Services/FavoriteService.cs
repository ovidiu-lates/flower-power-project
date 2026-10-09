using AutoMapper;
using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Interfaces;
using FlowerPowerGames.Data;
using FlowerPowerGames.Data.Models;
using Microsoft.EntityFrameworkCore;
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

    public async Task<List<FavoriteDto>> GetAllFavoritesAsync()
    {
        var favorites = await _context.Favorites
            .Include(f => f.Game)
            .ToListAsync();
        return _mapper.Map<List<FavoriteDto>>(favorites);
    }

    public async Task<List<FavoriteDto>> GetFavoritesByUserIdAsync(int userId)
    {
        var favorites = await _context.Favorites
            .Include(f => f.Game)
            .Where(f => f.UserId == userId)
            .ToListAsync();

        if (favorites is null)
        {
            throw new NotFoundException($"Favorites for the user with id {userId} were not found.");
        }

        return _mapper.Map<List<FavoriteDto>>(favorites);
    }



    public async Task<FavoriteDto> GetFavoriteByIdAsync(int id)
    {
        var favorite = await _context.Favorites
            .Include(f => f.Game)
            .FirstOrDefaultAsync(f => f.Id == id);
        if (favorite is null)
        {
            throw new NotFoundException($"Favorite with id {id} was not found.");
        }
        return _mapper.Map<FavoriteDto>(favorite);
    }

    public async Task<FavoriteDto> CreateFavoriteAsync(CreateFavoriteDto favoriteDto, int userId)
    {
        var gameExists = await _context.Games
            .AnyAsync(g => g.Id == favoriteDto.GameId);

        if (!gameExists)
        {
            throw new NotFoundException(
                $"Game with id {favoriteDto.GameId} does not exist.");
        }

        var userExists = await _context.Users
            .AnyAsync(u => u.Id == userId);

        if (!userExists)
        {
            throw new NotFoundException( $"User with id {userId} does not exist.");
        }

        var favoriteExists = await _context.Favorites
            .AnyAsync(f =>
                f.UserId == userId &&
                f.GameId == favoriteDto.GameId);

        if (favoriteExists)
        {
            throw new ConflictException(
                "This game is already in the user's favorites.");
        }

        var favorite = _mapper.Map<Favorite>(favoriteDto);
        favorite.UserId = userId;

        _context.Favorites.Add(favorite);
        await _context.SaveChangesAsync();

        return _mapper.Map<FavoriteDto>(favorite);
    }

    public async Task<bool> DeleteFavoriteAsync(int id, int userId, string role)
    {
        var favorite = await _context.Favorites.FindAsync(id);
        if (favorite is null)
        {
            throw new NotFoundException("Favorite not found.");
        }


        if (favorite.UserId != userId && role != "Admin")
        {
            throw new UnauthorizedException(
                "You are not authorized to delete this favorite.");
        }

        _context.Favorites.Remove(favorite);
        await _context.SaveChangesAsync();
        return true;
    }
}
