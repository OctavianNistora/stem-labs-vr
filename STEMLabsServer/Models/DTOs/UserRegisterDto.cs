using System.ComponentModel.DataAnnotations;

namespace STEMLabsServer.Models.DTOs;

public class UserRegisterDto
{
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Role { get; set; }
}