using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using STEMLabsServer.Data;
using STEMLabsServer.Models.DTOs;
using STEMLabsServer.Models.Entities;
using STEMLabsServer.Services.Interfaces;

namespace STEMLabsServer.Services;

public class AuthService(MainDbContext context) : IAuthService
{
    private readonly int _tokenExpiryMinutesWithoutRefreshToken = 135;
    private readonly int _tokenExpiryMinutesWithRefreshToken = 5;
    
    public async Task<AuthResponseDto?> LoginAsync(UserLoginDto userLoginDto, CancellationToken cancellationToken)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Username == userLoginDto.Username, cancellationToken);
        if (user is null)
        {
            return null;
        }

        if (!user.IsVerified)
        {
            user.IsVerified = true;
            context.Users.Update(user);
            await context.SaveChangesAsync(cancellationToken);
        }
        
        if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHashed, userLoginDto.Password)
            == PasswordVerificationResult.Failed)
        {
            return null;
        }

        string refreshTokenString = "";
        int expiryMinutes = _tokenExpiryMinutesWithoutRefreshToken;
        if (userLoginDto.RespondWithRefreshToken)
        {
            refreshTokenString = await CreateAndStoreNewRefreshTokenAsync(user, cancellationToken);
            expiryMinutes = _tokenExpiryMinutesWithRefreshToken;
        }
        
        var accessToken = CreateAccessToken(user, expiryMinutes);
        
        return new AuthResponseDto
        {
            Uid = user.Id,
            AccessToken = accessToken,
            RefreshToken = refreshTokenString,
            Role = user.UserRole.ToString(),
            FullName = $"{user.FirstName} {user.LastName}"
        };
    }
    
    public async Task<AuthResponseDto?> GenerateAuthDataFromRefreshToken(string refreshToken, CancellationToken cancellationToken)
    {
        var token = await context.RefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == refreshToken, cancellationToken);
        
        if (token is null || token.ExpiryDate < DateTime.UtcNow)
        {
            return null;
        }

        string newRefreshToken = await ModifyExistingRefreshTokenAsync(token, cancellationToken);
        
        var user = token.User;
        var accessToken = CreateAccessToken(user, _tokenExpiryMinutesWithRefreshToken);
        
        return new AuthResponseDto
        {
            Uid = user.Id,
            AccessToken = accessToken,
            RefreshToken = newRefreshToken,
            Role = user.UserRole.ToString(),
            FullName = $"{user.FirstName} {user.LastName}"
        };
    }
    
    private string CreateAccessToken(User user, int expiryMinutes)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.UserRole.ToString())
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_TOKEN_KEY")!));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var tokenDescriptor = new JwtSecurityToken(
            issuer: Environment.GetEnvironmentVariable("JWT_ISSUER"),
            audience: Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(135),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }
    
    private async Task<string> CreateAndStoreNewRefreshTokenAsync(User user, CancellationToken cancellationToken)
    {
        var randomNumber = new byte[32];
        var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        var refreshTokenString = Convert.ToBase64String(randomNumber);
        
        var refreshToken = new RefreshToken
        {
            Token = refreshTokenString,
            ExpiryDate = DateTime.UtcNow.AddHours(1),
            User = user
        };
        
        await context.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        return refreshTokenString;
    }
    
    private async Task<string> ModifyExistingRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
    {
        var randomNumber = new byte[32];
        var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        var refreshTokenString = Convert.ToBase64String(randomNumber);
        
        refreshToken.Token = refreshTokenString;
        refreshToken.ExpiryDate = DateTime.UtcNow.AddHours(1);
        
        context.RefreshTokens.Update(refreshToken);
        await context.SaveChangesAsync(cancellationToken);
        
        return refreshTokenString;
    }
}