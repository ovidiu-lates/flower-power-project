using FlowerPowerGames.Business.Authentication;
using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Exceptions;
using FlowerPowerGames.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowerPowerGames.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ICurrentUserService _currentUserService;
    public UsersController(IUserService userService, ICurrentUserService currentUserService)
    {
        _userService = userService;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();

        return Ok(users);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<ActionResult<UserDto>> GetUserById(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);

        return Ok(user);
    }

    [HttpPatch("{id:int}")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<ActionResult<UserDto>> UpdateUser(int id, AdminUpdateUserRequestDto request)
    {
        var updatedUser = await _userService.UpdateAdminUserAsync(id, request);

        return Ok(updatedUser);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> DeleteUser(int id)
    {
        await _userService.DeleteUserAsync(id);

        return NoContent();
    }

    [HttpPut("me")]
    [Authorize]
    public async Task<ActionResult<UserDto>> UpdateMyProfile(UpdateProfileRequestDto request)
    {
        var userId = _currentUserService.UserId ?? throw new UnauthorizedException("User is not authenticated.");

        var updatedUser = await _userService.UpdateMyProfileAsync(userId, request);

        return Ok(updatedUser);
    }

    [HttpPut("me/password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
    {
        var userId = _currentUserService.UserId ?? throw new UnauthorizedException("User is not authenticated.");

        await _userService.ChangePasswordAsync(userId, request);

        return NoContent();
    }
}