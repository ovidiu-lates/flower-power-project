using FlowerPowerGames.Business.AI;
using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Interfaces;
using FlowerPowerGames.Data;
using Microsoft.EntityFrameworkCore;
using FlowerPowerGames.Data.Models;
using Azure.Core;

namespace FlowerPowerGames.Business.Services;

public sealed class RecommendationService : IRecommendationService
{
    private readonly AppDbContext _context;
    private readonly IAiRecommendationClient _aiClient;
    private readonly IAiUsageService _aiUsageService;

    public RecommendationService(AppDbContext context, IAiRecommendationClient aiClient, IAiUsageService aiUsageService)
    {
        _context = context;
        _aiClient = aiClient;
        _aiUsageService = aiUsageService;
    }

    public async Task<RecommendationResponseDTO> GetRecommendationsAsync(int userId,
            RecommendationRequestDTO request, int recommendationCount = 5, CancellationToken cancellationToken = default)
    {
        if (recommendationCount <= 0)
        {
            throw new ArgumentException("Recommendation count must be greater than zero.");
        }

        await _aiUsageService.EnsureAiUsageAvailableAsync(userId,cancellationToken);

        var preference = await GetPreferenceAsync(userId, cancellationToken);

        var favoriteGames = await GetFavoriteGamesAsync(userId, cancellationToken);

        var favoriteGameIds = favoriteGames
            .Select(game => game.Id)
            .ToHashSet();

        var candidateGames = await GetCandidateGamesAsync(favoriteGameIds, preference, cancellationToken);

        if (candidateGames.Count == 0)
        {
            return new RecommendationResponseDTO();
        }

        var actualRecommendationCount = Math.Min(recommendationCount, candidateGames.Count);

        var recommendationContext = new RecommendationContext
        {
            UserMessage = request.Message,
            Preference = preference,
            FavoriteGames = favoriteGames,
            CandidateGames = candidateGames
        };

        var aiResult = await _aiClient.GetRecommendationsAsync(
            recommendationContext,
            actualRecommendationCount,
            cancellationToken);

        await _aiUsageService.RecordAiRequestAsync(
            userId,
            cancellationToken);

        return BuildResponse(
            aiResult,
            candidateGames);
    }

    private async Task<AiUserPreferenceContext?> GetPreferenceAsync( int userId, CancellationToken cancellationToken)
    {
        return await _context.UserPreferences
            .AsNoTracking()
            .Where(preference => preference.UserId == userId)
            .Select(preference => new AiUserPreferenceContext
            {
                MinBudget = preference.minBudget,
                MaxBudget = preference.maxBudget,

                MinPlayTime = preference.minPlayTime,
                MaxPlayTime = preference.maxPlayTime,

                MinPlayers = preference.minPlayers,
                MaxPlayers = preference.maxPlayers,

                Genres = preference.Genres
                    .Select(genre => genre.Name)
                    .ToList(),

                Types = preference.Types
                    .Select(type => type.Name)
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    private async Task<List<AiGameContext>> GetFavoriteGamesAsync( int userId, CancellationToken cancellationToken)
    {
        return await _context.Favorites
            .AsNoTracking()
            .Where(favorite => favorite.UserId == userId)
            .Select(favorite => new AiGameContext
            {
                Id = favorite.Game.Id,
                Name = favorite.Game.Name,
                Description = favorite.Game.Description,

                Price = favorite.Game.Price,

                MinPlayers = favorite.Game.MinPlayers,
                MaxPlayers = favorite.Game.MaxPlayers,

                PlayTimeMinutes = favorite.Game.PlayTimeMinutes,
                LearningTimeMinutes = favorite.Game.LearningTimeMinutes,

                MinimumAge = favorite.Game.MinimumAge,

                Rating = favorite.Game.Rating,

                Genres = favorite.Game.Genres
                    .Select(genre => genre.Name)
                    .ToList(),

                Types = favorite.Game.Types
                    .Select(type => type.Name)
                    .ToList()
            })
            .ToListAsync(cancellationToken);
    }

    private async Task<List<AiGameContext>> GetCandidateGamesAsync(HashSet<int> favoriteGameIds, AiUserPreferenceContext? preference, CancellationToken cancellationToken)
    {
        var query = _context.Games
            .AsNoTracking()
            .Where(game => !favoriteGameIds.Contains(game.Id));

        if (preference is not null)
        {
            query = ApplyPreferenceFilters(query,preference);
        }

        return await query
            .Select(game => new AiGameContext
            {
                Id = game.Id,
                Name = game.Name,
                Description = game.Description,

                Price = game.Price,

                MinPlayers = game.MinPlayers,
                MaxPlayers = game.MaxPlayers,

                PlayTimeMinutes = game.PlayTimeMinutes,
                LearningTimeMinutes = game.LearningTimeMinutes,

                MinimumAge = game.MinimumAge,

                Rating = game.Rating,

                Genres = game.Genres
                    .Select(genre => genre.Name)
                    .ToList(),

                Types = game.Types
                    .Select(type => type.Name)
                    .ToList()
            })
            .ToListAsync(cancellationToken);
    }

    private static IQueryable<Game> ApplyPreferenceFilters(IQueryable<Data.Models.Game> query, AiUserPreferenceContext preference)
    {
        query = query.Where(game =>
            game.Price >= preference.MinBudget &&
            game.Price <= preference.MaxBudget);

        query = query.Where(game =>
            game.PlayTimeMinutes >= preference.MinPlayTime &&
            game.PlayTimeMinutes <= preference.MaxPlayTime);

        query = query.Where(game =>
            game.MaxPlayers >= preference.MinPlayers &&
            game.MinPlayers <= preference.MaxPlayers);

        return query;
    }

    private static RecommendationResponseDTO BuildResponse( AiRecommendationResult aiResult, List<AiGameContext> candidateGames)
    {
        var candidateDictionary = candidateGames
            .ToDictionary(game => game.Id);

        var recommendations = aiResult.Recommendations
            .Where(recommendation =>
                candidateDictionary.ContainsKey(
                    recommendation.GameId))
            .Select(recommendation =>
            {
                var game = candidateDictionary[recommendation.GameId];

                return new RecommendedGameDTO
                {
                    GameId = game.Id,
                    Name = game.Name,
                    Reason = recommendation.Reason
                };
            })
            .ToList();

        return new RecommendationResponseDTO
        {
            Recommendations = recommendations
        };
    }
}
