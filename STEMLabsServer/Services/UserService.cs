using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using STEMLabsServer.Data;
using STEMLabsServer.Models.DTOs;
using STEMLabsServer.Models.Entities;

namespace STEMLabsServer.Services;

public class UserService(MainDbContext context) : IUserService
{
    public async Task<bool> RegisterUser(UserRegisterDto user, CancellationToken cancellationToken)
    {
        if (await context.Users.AnyAsync(u => u.Username == user.Username, cancellationToken))
        {
            return false;
        }
        
        var newUser = new User
        {
            Username = user.Username,
            PasswordHashed = user.Password,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            PhoneNumber = user.PhoneNumber,
        };
        var passwordHashed = new PasswordHasher<User>()
            .HashPassword(newUser, user.Password);
        newUser.PasswordHashed = passwordHashed;
        
        await context.Users.AddAsync(newUser, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        return true;
    }

    public async Task<bool> AddLaboratorySession(int teacherId, LaboratorySessionDto laboratorySessionDto,
        CancellationToken cancellationToken)
    {
        var teacher =
            await context.Users.FirstOrDefaultAsync(user => user.Id == teacherId, cancellationToken);
        if (teacher == null)
        {
            return false;
        }
        
        var laboratory =
            await context.Laboratories.FirstOrDefaultAsync(lab => lab.SceneId == laboratorySessionDto.SceneId,
                cancellationToken);
        if (laboratory == null)
        {
            return false;
        }
        
        var laboratorySession = new LaboratorySession
        {
            Laboratory = laboratory,
            CreatedBy = teacher,
            InviteCode = laboratorySessionDto.InviteCode
        };
        
        await context.LaboratorySessions.AddAsync(laboratorySession, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        return true;
    }
    
    public async Task<bool> AddLaboratoryReport(int studentId, LaboratoryReportDto laboratoryReportDto,
        CancellationToken cancellationToken)
    {
        var student =
            await context.Users.FirstOrDefaultAsync(user => user.Id == studentId, cancellationToken);
        if (student == null)
        {
            return false;
        }
        
        var laboratorySession =
            await context.LaboratorySessions.FirstOrDefaultAsync(lab => lab.InviteCode == laboratoryReportDto.InvitedCode,
                cancellationToken);
        if (laboratorySession == null)
        {
            return false;
        }
        
        var laboratoryReport = new StudentLaboratoryReport()
        {
            LaboratorySession = laboratorySession,
            Student = student
        };
        
        await context.StudentLaboratoryReports.AddAsync(laboratoryReport, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        
        List<StudentLaboratoryCompletedStep> completedSteps = [];
        if (laboratoryReportDto.StepsCompleted.Count > 0)
        {
            foreach (var step in laboratoryReportDto.StepsCompleted)
            {
                var laboratoryStep = context.LaboratoryChecklistSteps
                    .Where(lab => lab.LaboratoryId == laboratorySession.LaboratoryId && lab.StepNumber == step)
                    .MaxBy(lab => lab.Version);

                if (laboratoryStep == null)
                {
                    return false;
                }
                
                var completedStep = new StudentLaboratoryCompletedStep
                {
                    LaboratorySession = laboratorySession,
                    LaboratoryChecklistStep = laboratoryStep,
                    Student = student,
                };
                completedSteps.Add(completedStep);
            }
            await context.StudentLaboratoryCompletedSteps.AddRangeAsync(completedSteps, cancellationToken);
        }
        
        
        return true;
    }
}