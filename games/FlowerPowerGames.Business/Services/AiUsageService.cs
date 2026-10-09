using AutoMapper;
using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Interfaces;
using FlowerPowerGames.Data;
using FlowerPowerGames.Data.Models;
using Microsoft.EntityFrameworkCore;
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


    public async Task<AiUsageDto> CreateAiUsageAsync(CreateAiUsageDto createAiUsageDto, int userId)
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

        return _mapper.Map<AiUsageDto>(aiUsage);

    }

    public async Task<AiUsageDto> GetAiUsageByIdAsync(int id)
    {
        var aiUsage = await _context.AiUsages
            .FirstOrDefaultAsync(a => a.Id == id);

        if (aiUsage == null)
        {
            throw new NotFoundException($"AI usage with ID {id} does not exist.");
        }
        return _mapper.Map<AiUsageDto>(aiUsage);
    }

    public async Task<AiUsageDto> GetAiUsageByUserIdAsync(int userId)
    {
        var aiUsage = await _context.AiUsages
            .FirstOrDefaultAsync(a => a.UserId == userId);

        if (aiUsage == null)
        {
            throw new NotFoundException($"AI usage for user with ID {userId} does not exist.");
        }
        return _mapper.Map<AiUsageDto>(aiUsage);
    }

    public async Task<AiUsageDto> UpdateAiUsageAsync(int id, AiUsageDto aiUsageDto)
    {
        var aiUsage = _context.AiUsages.FindAsync(id).Result;
        if (aiUsage == null)
        {
            throw new NotFoundException($"AI usage with ID {id} does not exist.");
        }

        aiUsageDto.UserId = aiUsage.UserId;

        _mapper.Map(aiUsageDto, aiUsage);

        await _context.SaveChangesAsync();

        return _mapper.Map<AiUsageDto>(aiUsage);
    }

    public async Task<AiUsageDto> UpdateAiUsageAdminAsync(int id, UpdateAiUsageDto updateAiUsageDto)
    {
        var aiUsage = await _context.AiUsages.FindAsync(id);
        if (aiUsage == null)
        {
            throw new NotFoundException($"AI usage with ID {id} does not exist.");
        }

        _mapper.Map(updateAiUsageDto, aiUsage);

        await _context.SaveChangesAsync();

        return _mapper.Map<AiUsageDto>(aiUsage);
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

    public async Task EnsureAiUsageAvailableAsync( int userId, CancellationToken cancellationToken = default)
    {
        var userExists = await _context.Users
            .AsNoTracking()
            .AnyAsync(
                user => user.Id == userId,
                cancellationToken);

        if (!userExists)
        {
            throw new NotFoundException($"User with ID {userId} does not exist.");
        }

        var aiUsage = await _context.AiUsages
            .AsNoTracking()
            .FirstOrDefaultAsync(
                usage => usage.UserId == userId,
                cancellationToken);

        if (aiUsage is null)
        {
            return;
        }

        if (aiUsage.TotalPromptUsed >= aiUsage.TotalAvailablePrompt)
        {
            throw new AiUsageLimitExceededException("The AI usage limit has been reached.");
        }
    }

    public async Task RecordAiRequestAsync(int userId, int tokensUsed, CancellationToken cancellationToken = default)
    {
        var aiUsage = await _context.AiUsages
            .FirstOrDefaultAsync(
                usage => usage.UserId == userId,
                cancellationToken);

        if (aiUsage is null)
        {
            var userExists = await _context.Users
                .AsNoTracking()
                .AnyAsync(
                    user => user.Id == userId,
                    cancellationToken);

            if (!userExists)
            {
                throw new NotFoundException($"User with ID {userId} does not exist.");
            }

            aiUsage = new AiUsage
            {
                UserId = userId,
                TotalRequests = 1,
                TotalPromptUsed = tokensUsed
            };

            _context.AiUsages.Add(aiUsage);
        }
        else
        {
            aiUsage.TotalRequests++;
            aiUsage.TotalPromptUsed += tokensUsed;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}