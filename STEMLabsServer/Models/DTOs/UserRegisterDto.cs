using System.ComponentModel.DataAnnotations;

namespace STEMLabsServer.Models.DTOs;

public class UserRegisterDto
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
}