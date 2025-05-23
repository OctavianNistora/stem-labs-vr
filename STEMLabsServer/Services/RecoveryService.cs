using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using STEMLabsServer.Data;
using STEMLabsServer.Models.DTOs;
using STEMLabsServer.Models.Entities;
using STEMLabsServer.Services.Interfaces;
using STEMLabsServer.Shared;

namespace STEMLabsServer.Services;

public class RecoveryService(MainDbContext context) : IRecoveryService
{
    
    public async Task<bool> RemindUsernameAsync(string email, CancellationToken cancellationToken)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        if (user == null)
        {
            return false;
        }
        
        var subject = "STEMLabsVR Username Reminder";
        var body = $"Your username is: {user.Username}";
        await MailProvider.SendEmailAsync(email, subject, body, cancellationToken);
        return true;
    }
    
    public async Task<bool> RequestPasswordResetAsync(string username, CancellationToken cancellationToken)
    {
        var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        var user = await context.Users.FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
        if (user == null)
        {
            return false;
        }

        var token = RandomStringGenerator.Generate(6);
        var resetRequest = new PasswordResetRequest
        {
            User = user,
            Token = token
        };
        
        await context.PasswordResetRequests.AddAsync(resetRequest, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        
        var subject = "STEMLabsVR Password Recovery";
        var body = $"Your password reset token is: {token}\n" +
                   "Please use this token to reset your password within the next 15 minutes.";
        await MailProvider.SendEmailAsync(user.Email, subject, body, cancellationToken);
        return true;
    }
    
    public async Task<bool> ResetPasswordAsync(RecoveryPasswordResetDto recoveryPasswordResetDto, CancellationToken cancellationToken)
    {
        var resetRequest = await context.PasswordResetRequests.Include(r => r.User)
            .FirstOrDefaultAsync(
                r => r.User.Username == recoveryPasswordResetDto.Username && r.Token == recoveryPasswordResetDto.Token,
                cancellationToken);
        
        if (resetRequest == null || resetRequest.User.Username != recoveryPasswordResetDto.Username)
        {
            return false;
        }
        if (resetRequest.CreatedAt < DateTime.UtcNow.AddMinutes(-15))
        {
            context.PasswordResetRequests.Remove(resetRequest);
            await context.SaveChangesAsync(cancellationToken);
            return false;
        }

        var passwordHashed = new PasswordHasher<User>()
            .HashPassword(resetRequest.User, recoveryPasswordResetDto.NewPassword);
        resetRequest.User.PasswordHashed = passwordHashed;
        
        context.Users.Update(resetRequest.User);
        context.PasswordResetRequests.Remove(resetRequest);
        await context.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}