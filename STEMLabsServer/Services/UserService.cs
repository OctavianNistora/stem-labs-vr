using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using STEMLabsServer.Data;
using STEMLabsServer.Models.DTOs;
using STEMLabsServer.Models.Entities;
using STEMLabsServer.Services.Interfaces;
using STEMLabsServer.Shared;

namespace STEMLabsServer.Services;

public class UserService(MainDbContext context) : IUserService
{
    public async Task<ServiceStatus> RegisterUser(UserRegisterDto userRegisterDto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(userRegisterDto.Email) ||
            string.IsNullOrWhiteSpace(userRegisterDto.FirstName) ||
            string.IsNullOrWhiteSpace(userRegisterDto.LastName) ||
            string.IsNullOrWhiteSpace(userRegisterDto.PhoneNumber))
        {
            return new ServiceStatus(false, "All fields are required.");
        }
        
        if (await context.Users.AnyAsync(u => u.Email == userRegisterDto.Email, cancellationToken))
        {
            return new ServiceStatus(false, "Email already registered.");
        }

        string username;
        do
        {
            username = "username-" + RandomStringGenerator.Generate(8);
        } while (await context.Users.AnyAsync(u => u.Username == username, cancellationToken));
        
        var passwordUnhashed = "password-" + RandomStringGenerator.Generate(16);
        var passwordHashed = new PasswordHasher<User>()
            .HashPassword(null!, passwordUnhashed);
        
        var newUser = new User
        {
            Username = username,
            PasswordHashed = passwordHashed,
            Email = userRegisterDto.Email,
            FirstName = userRegisterDto.FirstName,
            LastName = userRegisterDto.LastName,
            PhoneNumber = userRegisterDto.PhoneNumber,
        };
        
        await context.Users.AddAsync(newUser, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        var subject = "STEMLabsVR Account Created";
        var body = $"Your account has been created successfully.\n" +
                   $"Username: {username}\n" +
                   $"Password: {passwordUnhashed}\n" +
                   "You can change your username and password later in your profile settings after logging in.";
        try
        {
            await MailProvider.SendEmailAsync(userRegisterDto.Email, subject, body, cancellationToken);
        }
        catch (Exception e)
        {
            context.Users.Remove(newUser);
            await context.SaveChangesAsync(cancellationToken);
            Console.WriteLine($"Failed to send email: {e.Message}");
            return new ServiceStatus(false, "Failed to send email. Please try again later.");
        }
        
        return new ServiceStatus(true);
    }
    
    public async Task<IEnumerable<RelatedLaboratoryDto>?> GetRelatedLaboratories(int userId,
        CancellationToken cancellationToken)
    {
        var user =
            await context.Users.FirstOrDefaultAsync(user => user.Id == userId, cancellationToken);
        if (user == null)
        {
            return null;
        }
        
        var relatedLaboratories = await context.StudentLaboratoryReports
            .Where(report => report.StudentId == userId)
            .Include(laborator => laborator.LaboratorySession)
            .ThenInclude(session => session.Laboratory)
            .Select(report => new RelatedLaboratoryDto
            {
                Id = report.LaboratorySession.LaboratoryId,
                Name = report.LaboratorySession.Laboratory.Name,
            })
            .DistinctBy(laboratory => laboratory.Id)
            .ToListAsync(cancellationToken);
        
        return relatedLaboratories;
    }
    
    public async Task<ServiceStatus> UpdateUserRole(int userId, string newRole, CancellationToken cancellationToken)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user == null)
        {
            return new ServiceStatus(false, "User not found.");
        }

        user.UserRole = Enum.Parse<UserRole>(newRole, true);
        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);
        
        return new ServiceStatus(true);
    }

    public async Task<IEnumerable<RelatedSessionDto>?> GetRelatedSessions(int userId, int labId, CancellationToken cancellationToken)
    {
        var laboratorySessionsCreated = context.LaboratorySessions
            .Where(session => session.LaboratoryId == labId && session.CreatedById == userId)
            .Select(session => new RelatedSessionDto
            {
                Id = session.Id,
                DateCreated = session.CreatedAt,
            });
        
        var laboratorySessionsParticipated = context.StudentLaboratoryReports
            .Where(report => report.StudentId == userId && report.LaboratorySession.LaboratoryId == labId)
            .Include(report => report.LaboratorySession)
            .Select(report => new RelatedSessionDto
            {
                Id = report.LaboratorySession.Id,
                DateCreated = report.LaboratorySession.CreatedAt,
            });
        
        var relatedSessions = await laboratorySessionsCreated 
            .Union(laboratorySessionsParticipated)
            .Distinct()
            .ToListAsync(cancellationToken);
        
        return relatedSessions;
    }

    public async Task<UserProfileDto?> GetUserProfile(int userId, CancellationToken cancellationToken)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user == null)
        {
            return null;
        }
        
        return new UserProfileDto
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,
        };
    }

    public async Task<ServiceStatus> UpdateUserProfile(int userId, UserProfileDto userProfileUpdateDto, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(userProfileUpdateDto.FirstName) ||
            string.IsNullOrWhiteSpace(userProfileUpdateDto.LastName) ||
            string.IsNullOrWhiteSpace(userProfileUpdateDto.PhoneNumber))
        {
            return new ServiceStatus(false, "All fields are required.");
        }
        
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user == null)
        {
            return new ServiceStatus(false, "User not found.");
        }
        
        user.FirstName = userProfileUpdateDto.FirstName;
        user.LastName = userProfileUpdateDto.LastName;
        user.PhoneNumber = userProfileUpdateDto.PhoneNumber;
        
        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);
        
        return new ServiceStatus(true);
    }

    public async Task<ServiceStatusWithValue<string>> GetUserEmail(int userId, CancellationToken cancellationToken)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        
        return user == null
            ? new ServiceStatusWithValue<string>(false, "User not found.")
            : new ServiceStatusWithValue<string>(true, value: user.Email);
    }

    public async Task<ServiceStatus> UpdateUserEmail(int userId, UserEmailUpdateDto userEmailUpdateDto, CancellationToken cancellationToken)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user == null)
        {
            return new ServiceStatus(false, "User not found.");
        }
        
        var hashedPassword = new PasswordHasher<User>().HashPassword(user, userEmailUpdateDto.Password);
        if (user.PasswordHashed != hashedPassword)
        {
            return new ServiceStatus(false, "Incorrect password.");
        }

        if (await context.Users.AnyAsync(u => u.Email == userEmailUpdateDto.NewEmail && u.Id != userId, cancellationToken))
        {
            return new ServiceStatus(false, "Email already registered.");
        }

        user.Email = userEmailUpdateDto.NewEmail;
        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);
        
        return new ServiceStatus(true);
    }

    public async Task<ServiceStatus> UpdateUserPassword(int userId, UserPasswordUpdateDto userPasswordUpdateDto, CancellationToken cancellationToken)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user == null)
        {
            return new ServiceStatus(false, "User not found.");
        }
        
        var hashedPassword = new PasswordHasher<User>().HashPassword(user, userPasswordUpdateDto.CurrentPassword);
        if (user.PasswordHashed != hashedPassword)
        {
            return new ServiceStatus(false, "Incorrect current password.");
        }

        user.PasswordHashed = new PasswordHasher<User>().HashPassword(user, userPasswordUpdateDto.NewPassword);
        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);
        
        return new ServiceStatus(true);
    }

    public async Task<IEnumerable<UserListItemDto>> GetAllUsers(CancellationToken cancellationToken)
    {
        return await context.Users.OrderByDescending(user => user.Id)
            .Select(user => new UserListItemDto
            {
                Uid = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                CreatedAt = user.DateCreated,
                Role = user.UserRole.ToString()
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<ServiceStatus> DeleteUser(int userId, CancellationToken cancellationToken)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user == null)
        {
            return new ServiceStatus(false, "User not found.");
        }

        context.Users.Remove(user);
        await context.SaveChangesAsync(cancellationToken);
        
        return new ServiceStatus(true);
    }
}