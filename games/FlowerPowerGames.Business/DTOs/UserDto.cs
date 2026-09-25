using System;
using System.ComponentModel.DataAnnotations;

namespace FlowerPowerGames.Business.DTOs;

public class UserDto
{
    public int UserId { get; set; }

    [Range(1, int.MaxValue)]
    public int RoleId { get; set; }

    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    public required string PasswordHash { get; set; }

    [Required]
    public required string FullName { get; set; }

    [Required]
    public required string Username { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}