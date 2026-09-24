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
    public async Task<RatingDto?> GetRatingByIdAsync(int id)
    {
        var rating = await _context.Ratings
            .Include(r => r.Game)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (rating is null)
        {
            return null;
        }
        return _mapper.Map<RatingDto>(rating);
    }

    public async Task<RatingDto> CreateRatingAsync(RatingDto ratingDto)
    {
        var gameExists = await _context.Games.AnyAsync(g => g.Id == ratingDto.GameId);

        if (!gameExists)
        {
            throw new ArgumentException("Game does not exist");
        }

        var rating = _mapper.Map<Rating>(ratingDto);

        rating.CreatedAt = DateTime.UtcNow;
        rating.UpdatedAt = rating.CreatedAt;

        _context.Ratings.Add(rating);
        await _context.SaveChangesAsync();

        return _mapper.Map<RatingDto>(rating);
    }

    public async Task<RatingDto?> UpdateRatingAsync(int id, RatingDto ratingDto)
    {
        var rating = await _context.Ratings.FindAsync(id);

        if (rating is null)
        {
            return null;
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
            return false;
        }

        _context.Ratings.Remove(rating);
        await _context.SaveChangesAsync();

        return true;
    }
}