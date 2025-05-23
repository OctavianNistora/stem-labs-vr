using Microsoft.EntityFrameworkCore;
using STEMLabsServer.Data;
using STEMLabsServer.Models.DTOs;
using STEMLabsServer.Models.Entities;
using STEMLabsServer.Services.Interfaces;

namespace STEMLabsServer.Services;

public class LaboratorySessionService(MainDbContext context) : ILaboratorySessionService
{
    public async Task<bool> AddLaboratorySession(LaboratorySessionDto laboratorySessionDto,
        CancellationToken cancellationToken)
    {
        var teacher =
            await context.Users.FirstOrDefaultAsync(user => user.Id == laboratorySessionDto.creatorId,
                cancellationToken);
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

    public async Task<int> GetLaboratorySessionCreator(int sessionId, CancellationToken cancellationToken)
    {
        return await context.LaboratorySessions
            .Where(session => session.Id == sessionId)
            .Select(session => session.CreatedBy.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<LaboratorySessionParticipantDto>> GetLaboratorySessionParticipants(int sessionId, CancellationToken cancellationToken)
    {
        var reports = await context.StudentLaboratoryReports.AsNoTracking()
            .Where(report => report.LaboratorySessionId == sessionId)
            .GroupBy(report => report.StudentId,
                (studentId, group) => group.OrderByDescending(report => report.CreatedAt).First())
            .ToArrayAsync(cancellationToken);
        
        var partialLaboratorySession = reports.Select(report => context.Users.Find(report.StudentId)).Select(student => new LaboratorySessionParticipantDto
        {
            Id = student!.Id,
            Name = student.FirstName + " " + student.LastName,
        }).ToArray();
        
        for (var i = 0; i < partialLaboratorySession.Count(); i++)
        {
            partialLaboratorySession[i].Date = reports[i].CreatedAt;
        }
        
        return partialLaboratorySession;
    }

    public async Task<IEnumerable<LaboratoryReportListItemDto>> GetParticipantReports(int sessionId, int participantId, CancellationToken cancellationToken)
    {
        return await context.StudentLaboratoryReports
            .Where(report => report.LaboratorySessionId == sessionId && report.StudentId == participantId)
            .OrderBy(report => report.CreatedAt)
            .Select(report => new LaboratoryReportListItemDto
            {
                Id = report.Id,
                SubmittedAt = report.CreatedAt,
            })
            .ToListAsync(cancellationToken);
    }
}