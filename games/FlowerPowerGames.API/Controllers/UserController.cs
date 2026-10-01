using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;

namespace FlowerPowerGames.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _appUserService;
    private readonly IValidator<UserDto> _userValidator;

    public UsersController(IUserService appUserService, IValidator<UserDto> userValidator)
    {
        _appUserService = appUserService;
        _userValidator = userValidator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
    {
        var users = await _appUserService.GetAllUsersAsync();

        return Ok(users);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetUserById(int id)
    {
        var user = await _appUserService.GetUserByIdAsync(id);

        if (user is null)
        {
            return NotFound();
        }

        return Ok(user);
    }


    [HttpPut("{id:int}")]
    public async Task<ActionResult<UserDto>> UpdateUser(
        int id,
        [FromBody] UserDto userDto)
    {
        var validationResult = await _userValidator.ValidateAsync(userDto);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var updatedUser =
            await _appUserService.UpdateUserAsync(id, userDto);

        return Ok(updatedUser);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var deleted = await _appUserService.DeleteUserAsync(id);

        return NoContent();
    }
}