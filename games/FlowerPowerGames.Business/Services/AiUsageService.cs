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

public class AiUsageService : IAiUsageService
{

    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public AiUsageService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }


    public async Task<AiUsageDTO> CreateAiUsageAsync(CreateAiUsageDTO createAiUsageDto, int userId)
    {
        await CheckExistenceOfUserAsync(userId);

        var aiUsageExists = await _context.AiUsages.AnyAsync(a => a.UserId == userId);

        if (aiUsageExists)
        {
            throw new ConflictException($"AI usage for user with ID {userId} already exists.");
        }

        var aiUsage = _mapper.Map<AiUsage>(createAiUsageDto);
        aiUsage.UserId = userId;
        aiUsage.TotalRequests = 0;
        aiUsage.TotalPromptUsed = 0;

        _context.AiUsages.Add(aiUsage);
        await _context.SaveChangesAsync();

        return _mapper.Map<AiUsageDTO>(aiUsage);

    }

    public async Task<AiUsageDTO> GetAiUsageByIdAsync(int id)
    {
        var aiUsage = await _context.AiUsages
            .FirstOrDefaultAsync(a => a.Id == id);

        if (aiUsage == null)
        {
            throw new NotFoundException($"AI usage with ID {id} does not exist.");
        }
        return _mapper.Map<AiUsageDTO>(aiUsage);
    }

    public async Task<AiUsageDTO> GetAiUsageByUserIdAsync(int userId)
    {
        var aiUsage = await _context.AiUsages
            .FirstOrDefaultAsync(a => a.UserId == userId);

        if (aiUsage == null)
        {
            throw new NotFoundException($"AI usage for user with ID {userId} does not exist.");
        }
        return _mapper.Map<AiUsageDTO>(aiUsage);
    }

    public async Task<AiUsageDTO> UpdateAiUsageAsync(int id, AiUsageDTO aiUsageDto)
    {
        var aiUsage = _context.AiUsages.FindAsync(id).Result;
        if (aiUsage == null)
        {
            throw new NotFoundException($"AI usage with ID {id} does not exist.");
        }

        aiUsageDto.UserId = aiUsage.UserId;

        _mapper.Map(aiUsageDto, aiUsage);

        await _context.SaveChangesAsync();

        return _mapper.Map<AiUsageDTO>(aiUsage);
    }

    public async Task<AiUsageDTO> UpdateAiUsageAdminAsync(int id, UpdateAiUsageDTO updateAiUsageDto)
    {
        var aiUsage = await _context.AiUsages.FindAsync(id);
        if (aiUsage == null)
        {
            throw new NotFoundException($"AI usage with ID {id} does not exist.");
        }

        aiUsage.UserId = aiUsage.UserId;
        aiUsage.TotalRequests = aiUsage.TotalRequests;
        aiUsage.TotalPromptUsed = aiUsage.TotalPromptUsed;

        _mapper.Map(updateAiUsageDto, aiUsage);

        await _context.SaveChangesAsync();

        return _mapper.Map<AiUsageDTO>(aiUsage);
    }

    private async Task CheckExistenceOfUserAsync(int userId)
    {
        var userExists = await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.Id == userId);

        if (!userExists)
        {
            throw new NotFoundException($"User with id '{userId}' does not exist.");
        }
    }
}