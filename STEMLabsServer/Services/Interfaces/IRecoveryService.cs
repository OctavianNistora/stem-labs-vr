using STEMLabsServer.Models.DTOs;

namespace STEMLabsServer.Services.Interfaces;

public interface IRecoveryService
{
    public Task<bool> RemindUsernameAsync(string email, CancellationToken cancellationToken);
    public Task<bool> RequestPasswordResetAsync(string username, CancellationToken cancellationToken);

    public Task<bool> ResetPasswordAsync(RecoveryPasswordResetDto recoveryPasswordResetDto,
        CancellationToken cancellationToken);
}