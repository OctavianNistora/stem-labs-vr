using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Humanizer.DateTimeHumanizeStrategy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using STEMLabsServer.Data;
using STEMLabsServer.Models.DTOs;
using STEMLabsServer.Models.Entities;
using STEMLabsServer.Services;
using STEMLabsServer.Shared;

namespace STEMLabsServerTests;

public class AuthServiceTests
{
    private readonly MainDbContext _context;
    private readonly AuthService _authService;
    
    public AuthServiceTests()
    {
        var dbContextOptions = new DbContextOptionsBuilder<MainDbContext>()
            .UseInMemoryDatabase(databaseName: "AuthServiceTestDatabase")
            .Options;
        _context = new MainDbContext(dbContextOptions);
        _context.Database.EnsureDeleted();
        _authService = new AuthService(_context);
        
        Environment.SetEnvironmentVariable("JWT_TOKEN_KEY", "test_super_secret_key_01_02_03_04_05_06_07_08_09_10_11_12_13_14_15_16");
        Environment.SetEnvironmentVariable("JWT_ISSUER", "test_issuer");
        Environment.SetEnvironmentVariable("JWT_AUDIENCE", "test_audience");
    }
    
    [Fact]
    public async Task TestAuthentication()
    {
        var testUser = new User()
        {
            Email = "example@email.com",
            Username = "testuser",
            PasswordHashed = new PasswordHasher<User>().HashPassword(null!, "testpassword"),
            FirstName = "User",
            LastName = "Test",
            PhoneNumber = "1234567890",
            UserRole = UserRole.Professor
        };
        await _context.Users.AddAsync(testUser);
        await _context.SaveChangesAsync();
        var dto = new UserLoginDto()
        {
            Username = "testuser",
            Password = "testpassword",
            RespondWithRefreshToken = true
        };

        
        var authResponse = await _authService.LoginAsync(dto, CancellationToken.None);
        
        
        Assert.NotNull(authResponse);
        Assert.Equal(authResponse.Uid, testUser.Id);
        Assert.Equal(authResponse.Role, testUser.UserRole.ToString());
        
        var refreshTokenDatabase = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.UserId == testUser.Id, CancellationToken.None);
        Assert.NotNull(refreshTokenDatabase);
        Assert.Equal(authResponse.RefreshToken, refreshTokenDatabase.Token);
        
        SecurityToken securityToken;
        var claims = new JwtSecurityTokenHandler().ValidateToken(authResponse.AccessToken, 
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ??
                              throw new InvalidOperationException("JWT_ISSUER is not set."),
                ValidateAudience = true,
                ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? 
                                throw new InvalidOperationException("JWT_AUDIENCE is not set."),
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_TOKEN_KEY") ??
                                           throw new InvalidOperationException())),
                ValidateIssuerSigningKey = true
            }, out securityToken);
        Assert.NotNull(securityToken);
    }
    
    [Fact]
    public async Task TestRefreshTokenAuthentication()
    {
        var testUser = new User()
        {
            Email = "example@email.com",
            Username = "testuser",
            PasswordHashed = new PasswordHasher<User>().HashPassword(null!, "testpassword"),
            FirstName = "User",
            LastName = "Test",
            PhoneNumber = "1234567890",
            UserRole = UserRole.Admin
        };
        await _context.Users.AddAsync(testUser);
        var refreshToken = new RefreshToken()
        {
            User = testUser,
            Token = "test_refresh_token",
            ExpiryDate = DateTime.UtcNow.AddDays(1)
        };
        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();
        
        var authResponse = await _authService.GenerateAuthDataFromRefreshToken("test_refresh_token", CancellationToken.None);
        
        Assert.NotNull(authResponse);
        Assert.Equal(authResponse.Uid, testUser.Id);
        Assert.Equal(authResponse.Role, testUser.UserRole.ToString());
        
        var refreshTokenDatabase = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.UserId == testUser.Id, CancellationToken.None);
        Assert.NotNull(refreshTokenDatabase);
        Assert.Equal(authResponse.RefreshToken, refreshTokenDatabase.Token);
        
        SecurityToken securityToken;
        var claims = new JwtSecurityTokenHandler().ValidateToken(authResponse.AccessToken, 
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ??
                              throw new InvalidOperationException("JWT_ISSUER is not set."),
                ValidateAudience = true,
                ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? 
                                throw new InvalidOperationException("JWT_AUDIENCE is not set."),
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_TOKEN_KEY") ??
                                           throw new InvalidOperationException())),
                ValidateIssuerSigningKey = true
            }, out securityToken);
        Assert.NotNull(securityToken);
    }
}