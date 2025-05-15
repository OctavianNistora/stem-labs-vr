using STEMLabsServer.Models.DTOs;

namespace STEMLabsServer.Services;

public interface IAuthService
{
    Task<string?> LoginAsync(UserLoginDto userLoginDto, CancellationToken cancellationToken);
}