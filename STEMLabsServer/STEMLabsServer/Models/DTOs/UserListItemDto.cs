using STEMLabsServer.Shared;

namespace STEMLabsServer.Models.DTOs;

public class UserListItemDto
{
    public int Uid { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string Role { get; set; } = string.Empty;
}