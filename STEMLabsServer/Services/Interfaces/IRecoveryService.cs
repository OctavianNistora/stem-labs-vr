using STEMLabsServer.Models.DTOs;

namespace STEMLabsServer.Services.Interfaces;

public interface IRecoveryService
{
    // Sends a reminder email with the username associated with the provided email address.
    public Task<bool> RemindUsernameAsync(string email, CancellationToken cancellationToken);
    // Sends a password reset email to the user with the provided username.
    public Task<bool> RequestPasswordResetAsync(string username, CancellationToken cancellationToken);
    // Resets the user's password using the provided recovery password reset data.
    public Task<bool> ResetPasswordAsync(RecoveryPasswordResetDto recoveryPasswordResetDto,
        CancellationToken cancellationToken);
}