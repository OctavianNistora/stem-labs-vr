namespace STEMLabsServer.Models.DTOs;

public class RecoveryPasswordResetDto
{
    public required string Username { get; set; }
    public required string NewPassword { get; set; }
    public required string Token { get; set; }
}