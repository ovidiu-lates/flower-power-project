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


    public async Task<AiUsageDTO> CreateAiUsageAsync(AiUsageDTO aiUsageDto)
    {
        //need to verify that the user for which i create the ai usage exists in the database

        /*var userExists = await _context.Users.AnyAsync(u => u.Id == aiUsageDto.UserId);

        if (!userExists)
        {
            throw new ArgumentException($"User with ID {aiUsageDto.UserId} does not exist.");
        }*/

        var aiUsageExists = await _context.AiUsages.AnyAsync(a => a.UserId == aiUsageDto.UserId);

        if (aiUsageExists)
        {
            throw new ArgumentException($"AI usage for user with ID {aiUsageDto.UserId} already exists.");
        }

        var aiUsage = _mapper.Map<AiUsage>(aiUsageDto);

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
            throw new ArgumentException($"AI usage with ID {id} does not exist.");
        }
        return _mapper.Map<AiUsageDTO>(aiUsage);
    }

    public async Task<AiUsageDTO> GetAiUsageByUserIdAsync(int userId)
    {
        var aiUsage = await _context.AiUsages
            .FirstOrDefaultAsync(a => a.UserId == userId);

        if (aiUsage == null)
        {
            throw new ArgumentException($"AI usage for user with ID {userId} does not exist.");
        }
        return _mapper.Map<AiUsageDTO>(aiUsage);
    }

    public async Task<AiUsageDTO> UpdateAiUsageAsync(int id, AiUsageDTO aiUsageDto)
    {
        var aiUsage = _context.AiUsages.FindAsync(id).Result;
        if (aiUsage == null)
        {
            throw new ArgumentException($"AI usage with ID {id} does not exist.");
        }

        _mapper.Map(aiUsageDto, aiUsage);

        await _context.SaveChangesAsync();

        return _mapper.Map<AiUsageDTO>(aiUsage);
    }
}