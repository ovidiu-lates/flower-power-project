using AutoMapper;
using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Exceptions;
using FlowerPowerGames.Business.Mappers;
using FlowerPowerGames.Business.Services;
using FlowerPowerGames.Data;
using FlowerPowerGames.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace FlowerPowerGames.UnitTests.Services;

public class UserPreferenceServiceTests
{
    [Fact]
    public async Task GetUserPreferenceByUserIdAsync_MappedPreference()
    {
        using var context = CreateContext();
        AddUser(context);
        context.Genres.Add(new Genre { Id = 1, Name = "Strategy" });
        context.GameTypes.Add(new GameType { Id = 1, Name = "Board" });
        await context.SaveChangesAsync();
        context.UserPreferences.Add(CreatePreference(1, [context.Genres.Local.Single()], [context.GameTypes.Local.Single()]));
        await context.SaveChangesAsync();
        var service = CreateService(context);

        var preference = await service.GetUserPreferenceByUserIdAsync(1);

        Assert.Equal(1, preference.UserId);
        Assert.Equal(10m, preference.MinBudget);
        Assert.Equal(100m, preference.MaxBudget);
        Assert.Equal("Family", preference.AgeGroup);
        Assert.Equal([1], preference.GenreIds);
        Assert.Equal([1], preference.TypeIds);
    }

    [Fact]
    public async Task GetUserPreferenceByUserIdAsync_ThrowsNotFoundException_PreferenceDoesNotExist()
    {
        using var context = CreateContext();
        var service = CreateService(context);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            service.GetUserPreferenceByUserIdAsync(42));

        Assert.Equal("User preference for user with id 42 was not found.", exception.Message);
    }

    [Fact]
    public async Task CreateUserPreferenceAsync_PersistsPreferenceAndRelatedGenresAndTypes()
    {
        using var context = CreateContext();
        AddUser(context);
        context.Genres.AddRange(
            new Genre { Id = 1, Name = "Strategy" },
            new Genre { Id = 2, Name = "Family" });
        context.GameTypes.AddRange(
            new GameType { Id = 1, Name = "Board" },
            new GameType { Id = 2, Name = "Card" });
        await context.SaveChangesAsync();
        var service = CreateService(context);
        var request = CreateDto(1, [1, 2, 1], [1, 2, 1]);

        var createdPreference = await service.CreateUserPreferenceAsync(request);

        Assert.True(createdPreference.Id > 0);
        Assert.Equal(1, createdPreference.UserId);
        Assert.Equal([1, 2], createdPreference.GenreIds.OrderBy(id => id));
        Assert.Equal([1, 2], createdPreference.TypeIds.OrderBy(id => id));
        var storedPreference = await context.UserPreferences
            .Include(preference => preference.Genres)
            .Include(preference => preference.Types)
            .SingleAsync();
        Assert.Equal(2, storedPreference.Genres.Count);
        Assert.Equal(2, storedPreference.Types.Count);
    }

    [Fact]
    public async Task CreateUserPreferenceAsync_ThrowsNotFoundException_UserDoesNotExist()
    {
        using var context = CreateContext();
        var service = CreateService(context);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            service.CreateUserPreferenceAsync(CreateDto(42)));

        Assert.Equal("User with id '42' does not exist.", exception.Message);
        Assert.Empty(context.UserPreferences);
    }

    [Fact]
    public async Task CreateUserPreferenceAsync_ThrowsConflictException_PreferenceAlreadyExists()
    {
        using var context = CreateContext();
        AddUser(context);
        context.UserPreferences.Add(CreatePreference(1));
        await context.SaveChangesAsync();
        var service = CreateService(context);

        var exception = await Assert.ThrowsAsync<ConflictException>(() =>
            service.CreateUserPreferenceAsync(CreateDto(1)));

        Assert.Equal("User preference for user ID 1 already exist.", exception.Message);
        Assert.Single(context.UserPreferences);
    }

    [Fact]
    public async Task CreateUserPreferenceAsync_ThrowsArgumentException_GenreIdDoesNotExist()
    {
        using var context = CreateContext();
        AddUser(context);
        await context.SaveChangesAsync();
        var service = CreateService(context);

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CreateUserPreferenceAsync(CreateDto(1, [99])));

        Assert.Equal("The following genre Ids do not exist: 99.", exception.Message);
    }

    [Fact]
    public async Task CreateUserPreferenceAsync_ThrowsArgumentException_TypeIdDoesNotExist()
    {
        using var context = CreateContext();
        AddUser(context);
        await context.SaveChangesAsync();
        var service = CreateService(context);

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CreateUserPreferenceAsync(CreateDto(1, typeIds: [99])));

        Assert.Equal("The following type Ids do not exist: 99.", exception.Message);
    }

    [Fact]
    public async Task UpdateUserPreferenceAsync_UpdatesFieldsAndRelatedGenresAndTypes()
    {
        using var context = CreateContext();
        AddUser(context);
        context.Genres.AddRange(
            new Genre { Id = 1, Name = "Strategy" },
            new Genre { Id = 2, Name = "Family" });
        context.GameTypes.AddRange(
            new GameType { Id = 1, Name = "Board" },
            new GameType { Id = 2, Name = "Card" });
        await context.SaveChangesAsync();
        context.UserPreferences.Add(CreatePreference(
            1,
            [context.Genres.Local.Single(genre => genre.Id == 1)],
            [context.GameTypes.Local.Single(type => type.Id == 1)]));
        await context.SaveChangesAsync();
        var service = CreateService(context);
        var request = CreateDto(1, [2], [2]);
        request.MaxBudget = 150m;
        request.AgeGroup = "Adults";

        var updatedPreference = await service.UpdateUserPreferenceAsync(1, request);

        Assert.Equal(150m, updatedPreference.MaxBudget);
        Assert.Equal("Adults", updatedPreference.AgeGroup);
        Assert.Equal([2], updatedPreference.GenreIds);
        Assert.Equal([2], updatedPreference.TypeIds);
        var storedPreference = await context.UserPreferences
            .Include(preference => preference.Genres)
            .Include(preference => preference.Types)
            .SingleAsync();
        Assert.Equal([2], storedPreference.Genres.Select(genre => genre.Id));
        Assert.Equal([2], storedPreference.Types.Select(type => type.Id));
    }

    [Fact]
    public async Task UpdateUserPreferenceAsync_ThrowsNotFoundException_PreferenceDoesNotExist()
    {
        using var context = CreateContext();
        var service = CreateService(context);

        var exception = await Assert.ThrowsAsync<NotFoundException>(() =>
            service.UpdateUserPreferenceAsync(42, CreateDto(42)));

        Assert.Equal("User preference for user with id 42 was not found.", exception.Message);
    }

    [Fact]
    public async Task UpdateUserPreferenceAsync_ThrowsArgumentException_GenreIdDoesNotExist()
    {
        using var context = CreateContext();
        AddUser(context);
        context.UserPreferences.Add(CreatePreference(1));
        await context.SaveChangesAsync();
        var service = CreateService(context);

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            service.UpdateUserPreferenceAsync(1, CreateDto(1, [99])));

        Assert.Equal("The following genre Ids do not exist: 99.", exception.Message);
    }

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var context = new AppDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    private static void AddUser(AppDbContext context)
    {
        context.Users.Add(new User
        {
            Id = 1,
            Email = "user@example.com",
            Username = "user",
            FullName = "Test User",
            PasswordHash = "password-hash",
            RoleId = 1
        });
    }

    private static UserPreferenceService CreateService(AppDbContext context)
    {
        var configuration = new MapperConfiguration(
            config => config.AddProfile<UserPreferenceProfile>(),
            NullLoggerFactory.Instance);
        return new UserPreferenceService(context, configuration.CreateMapper());
    }

    private static UserPreference CreatePreference(
        int userId,
        ICollection<Genre>? genres = null,
        ICollection<GameType>? types = null)
    {
        return new UserPreference
        {
            UserId = userId,
            minBudget = 10m,
            maxBudget = 100m,
            minPlayTime = 15,
            maxPlayTime = 120,
            minPlayers = 1,
            maxPlayers = 4,
            ageGroup = "Family",
            Genres = genres ?? [],
            Types = types ?? []
        };
    }

    private static UserPreferenceDTO CreateDto(
        int userId,
        ICollection<int>? genreIds = null,
        ICollection<int>? typeIds = null)
    {
        return new UserPreferenceDTO
        {
            UserId = userId,
            MinBudget = 10m,
            MaxBudget = 100m,
            MinPlayTime = 15,
            MaxPlayTime = 120,
            MinPlayers = 1,
            MaxPlayers = 4,
            AgeGroup = "Family",
            GenreIds = genreIds ?? [],
            TypeIds = typeIds ?? []
        };
    }
}
