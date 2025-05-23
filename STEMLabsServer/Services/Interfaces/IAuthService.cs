using STEMLabsServer.Models.DTOs;

namespace STEMLabsServer.Services.Interfaces;

public interface IAuthService
{
    public Task<AuthResponseDto?> LoginAsync(UserLoginDto userLoginDto, CancellationToken cancellationToken);
    
    public Task<AuthResponseDto?> GenerateAuthDataFromRefreshToken(string refreshToken, CancellationToken cancellationToken);
}