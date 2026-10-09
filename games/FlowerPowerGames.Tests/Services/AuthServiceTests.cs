using FlowerPowerGames.Business.Authentication;
using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Exceptions;
using FlowerPowerGames.Business.Interfaces;
using FlowerPowerGames.Business.Services;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace FlowerPowerGames.Tests.Services;

public class AuthServiceTests
{
    [Fact]
    public async Task RegisterAsync_CreatesUserWithHashedPassword()
    {
        var request = CreateRegistrationRequest();
        var userStore = new Mock<IUserService>();
        userStore.Setup(item => item.FindByEmailOrUsernameAsync(It.IsAny<string>())).ReturnsAsync((AuthUser?)null);
        AuthUser? createdUser = null;
        userStore.Setup(item => item.CreateAsync(It.IsAny<AuthUser>()))
            .Callback<AuthUser>(user => createdUser = user)
            .ReturnsAsync((AuthUser user) => new AuthUser
            {
                Id = 17,
                Email = user.Email,
                Username = user.Username,
                FullName = user.FullName,
                Role = "User",
                PasswordHash = user.PasswordHash
            });
        var passwordHasher = new Mock<IPasswordHasher<AuthUser>>();
        passwordHasher.Setup(item => item.HashPassword(It.IsAny<AuthUser>(), request.Password)).Returns("hashed-password");
        var service = CreateService(userStore.Object, passwordHasher.Object, new Mock<ITokenService>().Object);

        var result = await service.RegisterAsync(request);

        Assert.Equal(17, result.UserId);
        Assert.Equal(request.Email, result.Email);
        Assert.Equal(request.Username, result.Username);
        Assert.Equal("User", result.Role);
        Assert.NotNull(createdUser);
        Assert.Equal("hashed-password", createdUser.PasswordHash);
        userStore.Verify(item => item.FindByEmailOrUsernameAsync(request.Email), Times.Once);
        userStore.Verify(item => item.FindByEmailOrUsernameAsync(request.Username), Times.Once);
        passwordHasher.Verify(item => item.HashPassword(It.IsAny<AuthUser>(), request.Password), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_RejectsDuplicateEmailOrUsername()
    {
        var request = CreateRegistrationRequest();
        var userStore = new Mock<IUserService>();
        userStore.Setup(item => item.FindByEmailOrUsernameAsync(request.Email))
            .ReturnsAsync(CreateUser());
        var service = CreateService(userStore.Object, Mock.Of<IPasswordHasher<AuthUser>>(), Mock.Of<ITokenService>());

        var exception = await Assert.ThrowsAsync<ConflictException>(() => service.RegisterAsync(request));

        Assert.Equal("Email or username already exists.", exception.Message);
        userStore.Verify(item => item.CreateAsync(It.IsAny<AuthUser>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_ReturnsTokensForValidCredentials()
    {
        var request = new LoginRequestDTO { EmailOrUsername = "player", Password = "password123" };
        var user = CreateUser();
        var userStore = new Mock<IUserService>();
        userStore.Setup(item => item.FindByEmailOrUsernameAsync(request.EmailOrUsername)).ReturnsAsync(user);
        var passwordHasher = new Mock<IPasswordHasher<AuthUser>>();
        passwordHasher.Setup(item => item.VerifyHashedPassword(user, user.PasswordHash, request.Password))
            .Returns(PasswordVerificationResult.Success);
        var tokenService = CreateTokenService(user);
        var service = CreateService(userStore.Object, passwordHasher.Object, tokenService.Object);

        var response = await service.LoginAsync(request);

        Assert.Equal(user.Id, response.UserId);
        Assert.Equal(user.Email, response.Email);
        Assert.Equal(user.Username, response.Username);
        Assert.Equal(user.Role, response.Role);
        Assert.Equal("access-token", response.AccessToken);
        Assert.Equal("refresh-token", response.RefreshToken);
        tokenService.Verify(item => item.SaveAsync(It.Is<RefreshToken>(token =>
            token.UserId == user.Id && token.TokenHash == "hash:refresh-token")), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_RejectsUnknownUser()
    {
        var request = new LoginRequestDTO { EmailOrUsername = "missing", Password = "password123" };
        var userStore = new Mock<IUserService>();
        userStore.Setup(item => item.FindByEmailOrUsernameAsync(request.EmailOrUsername)).ReturnsAsync((AuthUser?)null);
        var passwordHasher = new Mock<IPasswordHasher<AuthUser>>();
        var tokenService = new Mock<ITokenService>();
        var service = CreateService(userStore.Object, passwordHasher.Object, tokenService.Object);

        await Assert.ThrowsAsync<UnauthorizedException>(() => service.LoginAsync(request));

        passwordHasher.Verify(item => item.VerifyHashedPassword(It.IsAny<AuthUser>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        tokenService.Verify(item => item.CreateTokenPair(It.IsAny<AuthUser>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_RejectsInactiveUser()
    {
        var request = new LoginRequestDTO { EmailOrUsername = "player", Password = "password123" };
        var user = CreateUser(isActive: false);
        var userStore = new Mock<IUserService>();
        userStore.Setup(item => item.FindByEmailOrUsernameAsync(request.EmailOrUsername)).ReturnsAsync(user);
        var passwordHasher = new Mock<IPasswordHasher<AuthUser>>();
        var tokenService = new Mock<ITokenService>();
        var service = CreateService(userStore.Object, passwordHasher.Object, tokenService.Object);

        await Assert.ThrowsAsync<UnauthorizedException>(() => service.LoginAsync(request));

        passwordHasher.Verify(item => item.VerifyHashedPassword(It.IsAny<AuthUser>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_RejectsIncorrectPassword()
    {
        var request = new LoginRequestDTO { EmailOrUsername = "player", Password = "wrong-password" };
        var user = CreateUser();
        var userStore = new Mock<IUserService>();
        userStore.Setup(item => item.FindByEmailOrUsernameAsync(request.EmailOrUsername)).ReturnsAsync(user);
        var passwordHasher = new Mock<IPasswordHasher<AuthUser>>();
        passwordHasher.Setup(item => item.VerifyHashedPassword(user, user.PasswordHash, request.Password))
            .Returns(PasswordVerificationResult.Failed);
        var tokenService = new Mock<ITokenService>();
        var service = CreateService(userStore.Object, passwordHasher.Object, tokenService.Object);

        await Assert.ThrowsAsync<UnauthorizedException>(() => service.LoginAsync(request));

        tokenService.Verify(item => item.CreateTokenPair(It.IsAny<AuthUser>()), Times.Never);
    }

    [Fact]
    public async Task RefreshAsync_RevokesOldTokenAndCreatesNewSession()
    {
        var user = CreateUser();
        const string refreshToken = "old-refresh-token";
        var storedToken = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = "hash:old-refresh-token",
            ExpiresAtUtc = DateTime.UtcNow.AddDays(1)
        };
        var userStore = new Mock<IUserService>();
        userStore.Setup(item => item.FindByIdAsync(user.Id)).ReturnsAsync(user);
        var tokenService = CreateTokenService(user);
        tokenService.Setup(item => item.FindAsync("hash:" + refreshToken)).ReturnsAsync(storedToken);
        var service = CreateService(userStore.Object, Mock.Of<IPasswordHasher<AuthUser>>(), tokenService.Object);

        var response = await service.RefreshAsync(refreshToken);

        Assert.Equal(user.Id, response.UserId);
        Assert.Equal("refresh-token", response.RefreshToken);
        tokenService.Verify(item => item.RevokeAsync("hash:" + refreshToken), Times.Once);
        tokenService.Verify(item => item.SaveAsync(It.Is<RefreshToken>(token =>
            token.UserId == user.Id && token.TokenHash == "hash:refresh-token")), Times.Once);
    }

    [Fact]
    public async Task RefreshAsync_RejectsMissingRevokedOrExpiredToken()
    {
        var tokenService = new Mock<ITokenService>();
        tokenService.Setup(item => item.HashRefreshToken("invalid-token")).Returns("hash:invalid-token");
        tokenService.Setup(item => item.FindAsync("hash:invalid-token")).ReturnsAsync((RefreshToken?)null);
        var service = CreateService(Mock.Of<IUserService>(), Mock.Of<IPasswordHasher<AuthUser>>(), tokenService.Object);

        await Assert.ThrowsAsync<UnauthorizedException>(() => service.RefreshAsync("invalid-token"));

        tokenService.Verify(item => item.RevokeAsync(It.IsAny<string>()), Times.Never);
        tokenService.Verify(item => item.CreateTokenPair(It.IsAny<AuthUser>()), Times.Never);
    }

    [Fact]
    public async Task RefreshAsync_RejectsInactiveUser()
    {
        var user = CreateUser(isActive: false);
        var tokenService = new Mock<ITokenService>();
        tokenService.Setup(item => item.HashRefreshToken("valid-token")).Returns("hash:valid-token");
        tokenService.Setup(item => item.FindAsync("hash:valid-token")).ReturnsAsync(new RefreshToken
        {
            UserId = user.Id,
            TokenHash = "hash:valid-token",
            ExpiresAtUtc = DateTime.UtcNow.AddDays(1)
        });
        var userStore = new Mock<IUserService>();
        userStore.Setup(item => item.FindByIdAsync(user.Id)).ReturnsAsync(user);
        var service = CreateService(userStore.Object, Mock.Of<IPasswordHasher<AuthUser>>(), tokenService.Object);

        await Assert.ThrowsAsync<UnauthorizedException>(() => service.RefreshAsync("valid-token"));

        tokenService.Verify(item => item.RevokeAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task LogoutAsync_RevokesKnownToken()
    {
        var tokenService = new Mock<ITokenService>();
        tokenService.Setup(item => item.HashRefreshToken("valid-token")).Returns("hash:valid-token");
        tokenService.Setup(item => item.FindAsync("hash:valid-token")).ReturnsAsync(new RefreshToken
        {
            UserId = 17,
            TokenHash = "hash:valid-token",
            ExpiresAtUtc = DateTime.UtcNow.AddDays(1)
        });
        var service = CreateService(Mock.Of<IUserService>(), Mock.Of<IPasswordHasher<AuthUser>>(), tokenService.Object);

        var result = await service.LogoutAsync("valid-token");

        Assert.True(result);
        tokenService.Verify(item => item.RevokeAsync("hash:valid-token"), Times.Once);
    }

    [Fact]
    public async Task LogoutAsync_ReturnsFalseForUnknownToken()
    {
        var tokenService = new Mock<ITokenService>();
        tokenService.Setup(item => item.HashRefreshToken("unknown-token")).Returns("hash:unknown-token");
        tokenService.Setup(item => item.FindAsync("hash:unknown-token")).ReturnsAsync((RefreshToken?)null);
        var service = CreateService(Mock.Of<IUserService>(), Mock.Of<IPasswordHasher<AuthUser>>(), tokenService.Object);

        var result = await service.LogoutAsync("unknown-token");

        Assert.False(result);
        tokenService.Verify(item => item.RevokeAsync(It.IsAny<string>()), Times.Never);
    }

    private static AuthService CreateService(
        IUserService userStore,
        IPasswordHasher<AuthUser> passwordHasher,
        ITokenService tokenService)
    {
        return new AuthService(userStore, passwordHasher, tokenService);
    }

    private static Mock<ITokenService> CreateTokenService(AuthUser user)
    {
        var tokenService = new Mock<ITokenService>();
        tokenService.Setup(item => item.CreateTokenPair(user)).Returns(new TokenPair
        {
            AccessToken = "access-token",
            RefreshToken = "refresh-token",
            AccessTokenExpiresAtUtc = DateTime.UtcNow.AddMinutes(10),
            RefreshTokenExpiresAtUtc = DateTime.UtcNow.AddDays(7)
        });
        tokenService.Setup(item => item.HashRefreshToken(It.IsAny<string>()))
            .Returns<string>(token => "hash:" + token);
        tokenService.Setup(item => item.SaveAsync(It.IsAny<RefreshToken>())).Returns(Task.CompletedTask);
        return tokenService;
    }

    private static AuthUser CreateUser(bool isActive = true)
    {
        return new AuthUser
        {
            Id = 17,
            Email = "player@example.com",
            Username = "player",
            FullName = "Test Player",
            Role = "User",
            PasswordHash = "password-hash",
            IsActive = isActive
        };
    }

    private static RegisterRequestDTO CreateRegistrationRequest()
    {
        return new RegisterRequestDTO
        {
            Email = "player@example.com",
            Username = "player",
            FullName = "Test Player",
            Password = "password123",
            ConfirmPassword = "password123"
        };
    }
}
