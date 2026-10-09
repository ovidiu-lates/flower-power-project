using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Validators;
using Xunit;

namespace FlowerPowerGames.Tests.Validators;

public class UserDtoValidatorTests
{
    private readonly UserDtoValidator _validator = new();

    [Fact]
    public void Validate_AcceptsValidUser()
    {
        var result = _validator.Validate(new UserDto
        {
            Email = "user@example.com",
            Username = "player",
            FullName = "Test Player",
            RoleId = 1
        });

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_RejectsInvalidEmailAndMissingNames()
    {
        var result = _validator.Validate(new UserDto
        {
            Email = "invalid-email",
            Username = "",
            FullName = "",
            RoleId = 0
        });

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UserDto.Email));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UserDto.Username));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UserDto.FullName));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UserDto.RoleId));
    }
}

public class RegisterRequestDTOValidatorTests
{
    private readonly RegisterRequestDtoValidator _validator = new();

    [Fact]
    public void Validate_AcceptsValidRegistration()
    {
        var result = _validator.Validate(new RegisterRequestDto
        {
            Email = "user@example.com",
            Username = "player",
            FullName = "Test Player",
            Password = "password",
            ConfirmPassword = "password"
        });

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_RejectsInvalidEmailAndMissingRequiredFields()
    {
        var result = _validator.Validate(new RegisterRequestDto
        {
            Email = "invalid-email",
            Password = "short",
            ConfirmPassword = "different"
        });

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterRequestDto.Email));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterRequestDto.Username));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterRequestDto.FullName));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterRequestDto.Password));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterRequestDto.ConfirmPassword));
    }
}

public class LoginRequestDTOValidatorTests
{
    private readonly LoginRequestDtoValidator _validator = new();

    [Fact]
    public void Validate_AcceptsPasswordAtMinimumLength()
    {
        var result = _validator.Validate(new LoginRequestDto
        {
            EmailOrUsername = "player",
            Password = "password"
        });

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_RejectsMissingIdentityAndShortPassword()
    {
        var result = _validator.Validate(new LoginRequestDto { Password = "short" });

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(LoginRequestDto.EmailOrUsername));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(LoginRequestDto.Password));
    }
}

public class AdminUpdateUserRequestDTOValidatorTests
{
    private readonly AdminUpdateUserRequestDtoValidator _validator = new();

    [Fact]
    public void Validate_AcceptsEitherUpdateField()
    {
        Assert.True(_validator.Validate(new AdminUpdateUserRequestDto { IsActive = false }).IsValid);
        Assert.True(_validator.Validate(new AdminUpdateUserRequestDto { RoleId = 1 }).IsValid);
    }

    [Fact]
    public void Validate_RejectsEmptyUpdate()
    {
        var result = _validator.Validate(new AdminUpdateUserRequestDto());

        Assert.Contains(result.Errors, error => error.ErrorMessage == "At least one field must be provided.");
    }

    [Fact]
    public void Validate_RejectsNonPositiveRoleId()
    {
        var result = _validator.Validate(new AdminUpdateUserRequestDto { RoleId = 0 });

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(AdminUpdateUserRequestDto.RoleId));
    }
}

public class UpdateProfileRequestDTOValidatorTests
{
    private readonly UpdateProfileRequestDtoValidator _validator = new();

    [Fact]
    public void Validate_AcceptsPartialProfileUpdate()
    {
        var result = _validator.Validate(new UpdateProfileRequestDto { Username = "player" });

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_RejectsRequestWithNoFields()
    {
        var result = _validator.Validate(new UpdateProfileRequestDto());

        Assert.Contains(result.Errors, error => error.ErrorMessage == "At least one field must be provided.");
    }

    [Fact]
    public void Validate_RejectsInvalidEmailAndWhitespaceNames()
    {
        var result = _validator.Validate(new UpdateProfileRequestDto
        {
            Email = "invalid-email",
            Username = " ",
            FullName = "\t"
        });

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UpdateProfileRequestDto.Email));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UpdateProfileRequestDto.Username));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UpdateProfileRequestDto.FullName));
    }
}

public class ChangePasswordRequestDTOValidatorTests
{
    private readonly ChangePasswordRequestDtoValidator _validator = new();

    [Fact]
    public void Validate_AcceptsMatchingPasswordsAtMinimumLength()
    {
        var result = _validator.Validate(new ChangePasswordRequestDto
        {
            CurrentPassword = "old-password",
            NewPassword = "new-pass",
            ConfirmNewPassword = "new-pass"
        });

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_RejectsMissingCurrentPasswordAndShortNewPassword()
    {
        var result = _validator.Validate(new ChangePasswordRequestDto
        {
            NewPassword = "short",
            ConfirmNewPassword = "short"
        });

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(ChangePasswordRequestDto.CurrentPassword));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(ChangePasswordRequestDto.NewPassword));
    }

    [Fact]
    public void Validate_RejectsMismatchedConfirmation()
    {
        var result = _validator.Validate(new ChangePasswordRequestDto
        {
            CurrentPassword = "old-password",
            NewPassword = "new-password",
            ConfirmNewPassword = "different-password"
        });

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(ChangePasswordRequestDto.ConfirmNewPassword));
    }
}
