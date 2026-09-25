using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FlowerPowerGames.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponseDTO>> Register(RegisterRequestDTO request)
    {
        var result = await _authService.RegisterAsync(request);

        if (result is null)
        {
            return Conflict(new
            {
                message ="Email or username already exists."
            });
        }

        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDTO>> Login(LoginRequestDTO request)
    {
        var result = await _authService.LoginAsync(request);

        if (result is null)
        {
            return Unauthorized(new
            {
                message = "Invalid username/email or password."
            });
        }

        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<LoginResponseDTO>> Refresh(RefreshTokenRequestDTO request)
    {
        var result = await _authService.RefreshAsync(request.RefreshToken);

        if (result is null)
        {
            return Unauthorized(new
            {
                message = "Invalid or expired refresh token."
            });
        }

        return Ok(result);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(RefreshTokenRequestDTO request)
    {
        await _authService.LogoutAsync(request.RefreshToken);

        return NoContent();
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult CurrentUser()
    {
        return Ok(new
        {
            userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,

            username = User.Identity?.Name,

            email = User.FindFirst(ClaimTypes.Email)?.Value,

            role = User.FindFirst(ClaimTypes.Role)?.Value
        });
    }
}