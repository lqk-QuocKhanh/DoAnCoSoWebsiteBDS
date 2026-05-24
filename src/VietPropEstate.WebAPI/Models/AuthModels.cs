using System.ComponentModel.DataAnnotations;

namespace VietPropEstate.WebAPI.Models;

public record RegisterRequest(
    [Required][EmailAddress] string Email,
    [Required][MinLength(8)] string Password);

public record LoginRequest(
    [Required][EmailAddress] string Email,
    [Required] string Password);
