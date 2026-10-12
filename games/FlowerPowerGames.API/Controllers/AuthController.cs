using FlowerPowerGames.Business.DTOs;
using FlowerPowerGames.Business.Interfaces;
using FluentValidation;
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
    private readonly IValidator<RegisterRequestDto> _registerValidator;
    private readonly IValidator<LoginRequestDto> _loginValidator;
    private readonly IValidator<RefreshTokenRequestDto> _refreshValidator;
    public AuthController(IAuthService authService, IValidator<RegisterRequestDto> registerValidator, IValidator<LoginRequestDto> loginValidator, IValidator<RefreshTokenRequestDto> refreshValidator)
    {
        _authService = authService;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
        _refreshValidator = refreshValidator;
    }

    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponseDto>> Register(RegisterRequestDto request)
    {
        var validationResult = await _registerValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var result = await _authService.RegisterAsync(request);

        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto request)
    {
        var validationResult = await _loginValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var result = await _authService.LoginAsync(request);

        return Ok(result);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<LoginResponseDto>> Refresh(RefreshTokenRequestDto request)
    {
        var validationResult = await _refreshValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var result = await _authService.RefreshAsync(request.RefreshToken);

        return Ok(result);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(RefreshTokenRequestDto request)
    {
        var validationResult = await _refreshValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        await _authService.LogoutAsync(request.RefreshToken);

        return NoContent();
    }
}