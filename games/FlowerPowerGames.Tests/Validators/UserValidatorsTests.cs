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
    private readonly RegisterRequestDTOValidator _validator = new();

    [Fact]
    public void Validate_AcceptsValidRegistration()
    {
        var result = _validator.Validate(new RegisterRequestDTO
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
        var result = _validator.Validate(new RegisterRequestDTO
        {
            Email = "invalid-email",
            Password = "short",
            ConfirmPassword = "different"
        });

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterRequestDTO.Email));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterRequestDTO.Username));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterRequestDTO.FullName));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterRequestDTO.Password));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(RegisterRequestDTO.ConfirmPassword));
    }
}

public class LoginRequestDTOValidatorTests
{
    private readonly LoginRequestDTOValidator _validator = new();

    [Fact]
    public void Validate_AcceptsPasswordAtMinimumLength()
    {
        var result = _validator.Validate(new LoginRequestDTO
        {
            EmailOrUsername = "player",
            Password = "password"
        });

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_RejectsMissingIdentityAndShortPassword()
    {
        var result = _validator.Validate(new LoginRequestDTO { Password = "short" });

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(LoginRequestDTO.EmailOrUsername));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(LoginRequestDTO.Password));
    }
}

public class AdminUpdateUserRequestDTOValidatorTests
{
    private readonly AdminUpdateUserRequestDTOValidator _validator = new();

    [Fact]
    public void Validate_AcceptsEitherUpdateField()
    {
        Assert.True(_validator.Validate(new AdminUpdateUserRequestDTO { IsActive = false }).IsValid);
        Assert.True(_validator.Validate(new AdminUpdateUserRequestDTO { RoleId = 1 }).IsValid);
    }

    [Fact]
    public void Validate_RejectsEmptyUpdate()
    {
        var result = _validator.Validate(new AdminUpdateUserRequestDTO());

        Assert.Contains(result.Errors, error => error.ErrorMessage == "At least one field must be provided.");
    }

    [Fact]
    public void Validate_RejectsNonPositiveRoleId()
    {
        var result = _validator.Validate(new AdminUpdateUserRequestDTO { RoleId = 0 });

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(AdminUpdateUserRequestDTO.RoleId));
    }
}

public class UpdateProfileRequestDTOValidatorTests
{
    private readonly UpdateProfileRequestDTOValidator _validator = new();

    [Fact]
    public void Validate_AcceptsPartialProfileUpdate()
    {
        var result = _validator.Validate(new UpdateProfileRequestDTO { Username = "player" });

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_RejectsRequestWithNoFields()
    {
        var result = _validator.Validate(new UpdateProfileRequestDTO());

        Assert.Contains(result.Errors, error => error.ErrorMessage == "At least one field must be provided.");
    }

    [Fact]
    public void Validate_RejectsInvalidEmailAndWhitespaceNames()
    {
        var result = _validator.Validate(new UpdateProfileRequestDTO
        {
            Email = "invalid-email",
            Username = " ",
            FullName = "\t"
        });

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UpdateProfileRequestDTO.Email));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UpdateProfileRequestDTO.Username));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(UpdateProfileRequestDTO.FullName));
    }
}

public class ChangePasswordRequestDTOValidatorTests
{
    private readonly ChangePasswordRequestDTOValidator _validator = new();

    [Fact]
    public void Validate_AcceptsMatchingPasswordsAtMinimumLength()
    {
        var result = _validator.Validate(new ChangePasswordRequestDTO
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
        var result = _validator.Validate(new ChangePasswordRequestDTO
        {
            NewPassword = "short",
            ConfirmNewPassword = "short"
        });

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(ChangePasswordRequestDTO.CurrentPassword));
        Assert.Contains(result.Errors, error => error.PropertyName == nameof(ChangePasswordRequestDTO.NewPassword));
    }

    [Fact]
    public void Validate_RejectsMismatchedConfirmation()
    {
        var result = _validator.Validate(new ChangePasswordRequestDTO
        {
            CurrentPassword = "old-password",
            NewPassword = "new-password",
            ConfirmNewPassword = "different-password"
        });

        Assert.Contains(result.Errors, error => error.PropertyName == nameof(ChangePasswordRequestDTO.ConfirmNewPassword));
    }
}
