namespace STEMLabsServer.Models.DTOs;

public class AuthResponseDto
{
    public int Uid { get; set; }
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
}