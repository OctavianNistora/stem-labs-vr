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
    private const int TokenExpiryMinutesWithoutRefreshToken = 135;
    private const int TokenExpiryMinutesWithRefreshToken = 1;

    private static readonly SymmetricSecurityKey JwtSecurityKey = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_TOKEN_KEY") ??
                               throw new InvalidOperationException("JWT_TOKEN_KEY is not set.")));
    private static readonly string JwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ??
                                                throw new InvalidOperationException("JWT_ISSUER is not set.");
    private static readonly string JwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ??
                                                  throw new InvalidOperationException("JWT_AUDIENCE is not set.");
    
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
        int expiryMinutes = TokenExpiryMinutesWithoutRefreshToken;
        if (userLoginDto.RespondWithRefreshToken)
        {
            refreshTokenString = await CreateAndStoreNewRefreshTokenAsync(user, cancellationToken);
            expiryMinutes = TokenExpiryMinutesWithRefreshToken;
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
        var accessToken = CreateAccessToken(user, TokenExpiryMinutesWithRefreshToken);
        
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

        var credentials = new SigningCredentials(JwtSecurityKey, SecurityAlgorithms.HmacSha512);

        var tokenDescriptor = new JwtSecurityToken(
            issuer: JwtIssuer,
            audience: JwtAudience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials
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