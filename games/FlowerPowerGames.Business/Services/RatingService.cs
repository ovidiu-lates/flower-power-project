using AutoMapper;
using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Interfaces;
using FlowerPowerGames.Data;
using FlowerPowerGames.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using FlowerPowerGames.Business.Exceptions;

namespace FlowerPowerGames.Business.Services;

public class RatingService : IRatingService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public RatingService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<RatingDto>> GetAllRatingsAsync()
    {
        var ratings = await _context.Ratings
            .Include(r => r.Game)
            .ToListAsync();

        return _mapper.Map<List<RatingDto>>(ratings);
    }

    public async Task<List<RatingDto>> GetRatingsByGameIdAsync(int id)
    {
        var ratings = await _context.Ratings
            .Include(r => r.Game)
            .Where(r => r.GameId == id)
            .ToListAsync();

        return _mapper.Map<List<RatingDto>>(ratings);
    }
    public async Task<RatingDto> GetRatingByIdAsync(int id)
    {
        var rating = await _context.Ratings
            .Include(r => r.Game)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (rating is null)
        {
            throw new NotFoundException($"Rating with id {id} was not found.");
        }
        return _mapper.Map<RatingDto>(rating);
    }

    public async Task<List<RatingDto>> GetRatingsByUserIdAsync(int userId)
    {
        var ratings = await _context.Ratings
            .Include(r => r.Game)
            .Where(r => r.UserId == userId)
            .ToListAsync();
        if (ratings is null || !ratings.Any())
        {
            throw new NotFoundException($"Ratings for the user with id {userId} were not found.");
        }
        return _mapper.Map<List<RatingDto>>(ratings);
    }

    public async Task<RatingDto> CreateRatingAsync(CreateRatingDTO createRatingDto, int userId)
    {
        var gameExists = await _context.Games.AnyAsync(g => g.Id == createRatingDto.GameId);

        if (!gameExists)
        {
            throw new NotFoundException("Game with id {createRatingDto.GameId} does not exist.");
        }

        var userExists = await _context.Users
           .AnyAsync(u => u.Id == userId);

        if (!userExists)
        {
            throw new NotFoundException($"User with id {userId} does not exist.");
        }

        var rating = _mapper.Map<Rating>(createRatingDto);

        rating.UserId = userId;
        rating.CreatedAt = DateTime.UtcNow;
        rating.UpdatedAt = rating.CreatedAt;

        _context.Ratings.Add(rating);
        await _context.SaveChangesAsync();

        return _mapper.Map<RatingDto>(rating);
    }

    public async Task<RatingDto> UpdateRatingAsync(int id, RatingDto ratingDto)
    {
        var rating = await _context.Ratings.FindAsync(id);

        if (rating is null)
        {
            throw new NotFoundException($"Rating with id {id} was not found.");
        }

        rating.Score = ratingDto.Score;
        rating.Review = ratingDto.Review;
        rating.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return _mapper.Map<RatingDto>(rating);
    }

    public async Task<bool> DeleteRatingAsync(int id)
    {
        var rating = await _context.Ratings.FindAsync(id);

        if (rating is null)
        {
            throw new NotFoundException($"Rating with id {id} was not found.");
        }

        _context.Ratings.Remove(rating);
        await _context.SaveChangesAsync();

        return true;
    }
}