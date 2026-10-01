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
    private readonly IValidator<RegisterRequestDTO> _registerValidator;
    private readonly IValidator<LoginRequestDTO> _loginValidator;
    private readonly IValidator<RefreshTokenRequestDTO> _refreshValidator;
    public AuthController(IAuthService authService, IValidator<RegisterRequestDTO> registerValidator, IValidator<LoginRequestDTO> loginValidator, IValidator<RefreshTokenRequestDTO> refreshValidator)
    {
        _authService = authService;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
        _refreshValidator = refreshValidator;
    }

    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponseDTO>> Register(RegisterRequestDTO request)
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
    public async Task<ActionResult<LoginResponseDTO>> Login(LoginRequestDTO request)
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
    public async Task<ActionResult<LoginResponseDTO>> Refresh(RefreshTokenRequestDTO request)
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
    public async Task<IActionResult> Logout(RefreshTokenRequestDTO request)
    {
        var validationResult = await _refreshValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

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