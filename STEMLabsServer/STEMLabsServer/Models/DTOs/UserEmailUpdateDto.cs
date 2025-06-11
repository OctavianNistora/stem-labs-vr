namespace STEMLabsServer.Models.DTOs;

public class UserEmailUpdateDto
{
    public required string NewEmail { set; get; }
    public required string Password { set; get; }
}