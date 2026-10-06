using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace FlowerPowerGames.Business.AI;

public sealed class OpenAiRecommendationClient : IAiRecommendationClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _model;

    public OpenAiRecommendationClient(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;

        _apiKey = configuration["OpenAI:ApiKey"]
            ?? throw new InvalidOperationException(
                "OpenAI API key is not configured.");

        _model = configuration["OpenAI:Model"]
            ?? throw new InvalidOperationException(
                "OpenAI model is not configured.");
    }

    public async Task<AiRecommendationResult> GetRecommendationsAsync(
        RecommendationContext context,
        int recommendationCount,
        CancellationToken cancellationToken = default)
    {
        var candidateIds = context.CandidateGames
            .Select(game => game.Id)
            .ToArray();

        var requestBody = new
        {
            model = _model,

            store = false,

            instructions = """
                You are a board game recommendation system.

                Recommend games ONLY from CandidateGames.

                Never invent games.
                Never return a game ID that is not present in CandidateGames.

                UserMessage represents what the user is specifically looking for
                right now and should be treated as the strongest recommendation signal.

                UserPreference represents the user's general stored preferences.

                FavoriteGames represents games the user already likes and should
                be used to infer additional taste patterns.

                Favorite games must not be recommended again.

                Balance the recommendation using:
                1. the current UserMessage;
                2. stored user preferences;
                3. patterns from favorite games;
                4. candidate game metadata and descriptions.

                Pay attention to themes and concepts mentioned in UserMessage,
                even when they are not represented directly by genre or type.

                Return concise reasons explaining why each recommendation fits.
                """,

            input = JsonSerializer.Serialize(new
            {
                RequestedRecommendationCount = recommendationCount,
                UserMessage = context.UserMessage,
                UserPreference = context.Preference,
                FavoriteGames = context.FavoriteGames,
                CandidateGames = context.CandidateGames
            }),

            text = new
            {
                format = new
                {
                    type = "json_schema",
                    name = "board_game_recommendations",
                    strict = true,

                    schema = new
                    {
                        type = "object",

                        properties = new
                        {
                            recommendations = new
                            {
                                type = "array",

                                items = new
                                {
                                    type = "object",

                                    properties = new
                                    {
                                        gameId = new
                                        {
                                            type = "integer"
                                        },

                                        reason = new
                                        {
                                            type = "string"
                                        }
                                    },

                                    required = new[]
                                    {
                                        "gameId",
                                        "reason"
                                    },

                                    additionalProperties = false
                                }
                            }
                        },

                        required = new[]
                        {
                            "recommendations"
                        },

                        additionalProperties = false
                    }
                }
            }
        };

        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            "https://api.openai.com/v1/responses");

        request.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", _apiKey);

        request.Content = JsonContent.Create(requestBody);

        using var response = await _httpClient.SendAsync(
            request,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody =
                await response.Content.ReadAsStringAsync(cancellationToken);

            throw new AiRecommendationException($"OpenAI recommendation request failed. " + $"Status code: {(int)response.StatusCode}. " + $"Response: {errorBody}");
        }

        var responseJson = await response.Content
            .ReadAsStringAsync(cancellationToken);

        using var document = JsonDocument.Parse(responseJson);

        var root = document.RootElement;

        var outputText = ExtractOutputText(root);

        var recommendationData =
            JsonSerializer.Deserialize<AiRecommendationResponse>(
                outputText,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

        if (recommendationData is null)
        {
            throw new AiRecommendationException("The AI response could not be deserialized.");
        }

        var usage = ExtractUsage(root);

        var allowedIds = candidateIds.ToHashSet();

        var validRecommendations = recommendationData.Recommendations
            .Where(r => allowedIds.Contains(r.GameId))
            .DistinctBy(r => r.GameId)
            .Take(recommendationCount)
            .ToList();

        return new AiRecommendationResult
        {
            Recommendations = validRecommendations,
            Usage = usage
        };
    }

    private static string ExtractOutputText(JsonElement root)
    {
        if (!root.TryGetProperty("output", out var output))
        {
            throw new AiRecommendationException( "OpenAI response did not contain output.");
        }

        foreach (var outputItem in output.EnumerateArray())
        {
            if (!outputItem.TryGetProperty("content", out var content))
            {
                continue;
            }

            foreach (var contentItem in content.EnumerateArray())
            {
                if (!contentItem.TryGetProperty("type", out var type))
                {
                    continue;
                }

                if (type.GetString() != "output_text")
                {
                    continue;
                }

                if (contentItem.TryGetProperty("text", out var text))
                {
                    return text.GetString() ?? throw new AiRecommendationException( "OpenAI returned an empty response.");
                }
            }
        }

        throw new AiRecommendationException("OpenAI did not return recommendation text.");
    }

    private static AiTokenUsage ExtractUsage(JsonElement root)
    {
        if (!root.TryGetProperty("usage", out var usage))
        {
            return new AiTokenUsage();
        }

        return new AiTokenUsage
        {
            InputTokens =
                GetIntProperty(usage, "input_tokens"),

            OutputTokens =
                GetIntProperty(usage, "output_tokens"),

            TotalTokens =
                GetIntProperty(usage, "total_tokens")
        };
    }

    private static int GetIntProperty(
        JsonElement element,
        string propertyName)
    {
        if (!element.TryGetProperty(
                propertyName,
                out var property))
        {
            return 0;
        }

        return property.GetInt32();
    }

    private sealed class AiRecommendationResponse
    {
        public List<AiRecommendedGame> Recommendations { get; set; } = [];
    }
}