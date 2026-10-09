using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Validators;
using Xunit;

namespace FlowerPowerGames.Tests.Validators;

public class RoleDtoValidatorTests
{
    private readonly RoleDtoValidator _validator = new();

    [Fact]
    public void Validate_AcceptsNonEmptyRoleName()
    {
        var result = _validator.Validate(new RoleDto { Name = "Admin" });

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_RejectsEmptyRoleName(string name)
    {
        var result = _validator.Validate(new RoleDto { Name = name });

        Assert.Contains(result.Errors, error =>
            error.PropertyName == nameof(RoleDto.Name) &&
            error.ErrorMessage == "Role name cannot be empty.");
    }
}
