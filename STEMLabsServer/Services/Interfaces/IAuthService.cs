using STEMLabsServer.Models.DTOs;

namespace STEMLabsServer.Services.Interfaces;

public interface IAuthService
{
    // Retrieves the authentication data using a username and password
    public Task<AuthResponseDto?> LoginAsync(UserLoginDto userLoginDto, CancellationToken cancellationToken);
    // Retrieves the authentication data using a refresh token
    public Task<AuthResponseDto?> GenerateAuthDataFromRefreshToken(string refreshToken, CancellationToken cancellationToken);
}