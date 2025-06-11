using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.EntityFrameworkCore;
using STEMLabsServer.Data;
using STEMLabsServer.Models.DTOs;
using STEMLabsServer.Models.Entities;
using STEMLabsServer.Services.Interfaces;

namespace STEMLabsServer.Services;

public class LaboratoryReportService(MainDbContext context): ILaboratoryReportService
{
    private static readonly AmazonS3Client S3Client = new AmazonS3Client(
        new BasicAWSCredentials(
            Environment.GetEnvironmentVariable("R2_ACCESS_KEY") ??
            throw new InvalidOperationException("R2_ACCESS_KEY is not set."),
            Environment.GetEnvironmentVariable("R2_SECRET_KEY") ??
            throw new InvalidOperationException("R2_SECRET_KEY is not set.")),
        new AmazonS3Config
        {
            ServiceURL = Environment.GetEnvironmentVariable("R2_SERVICE_URL") ??
                         throw new InvalidOperationException("R2_SERVICE_URL is not set."),
            RequestChecksumCalculation = RequestChecksumCalculation.WHEN_REQUIRED,
            ResponseChecksumValidation = ResponseChecksumValidation.WHEN_REQUIRED,
        });
    private static readonly string R2BucketName = Environment.GetEnvironmentVariable("R2_BUCKET_NAME") ??
        throw new InvalidOperationException("R2_BUCKET_NAME is not set.");
    private static readonly string R2PublicUrl = Environment.GetEnvironmentVariable("R2_PUBLIC_URL") ??
        throw new InvalidOperationException("R2_PUBLIC_URL is not set.");
    
    public async Task<bool> AddLaboratoryReport(SubmittedLaboratoryReportDto submittedLaboratoryReportDto,
        CancellationToken cancellationToken)
    {
        var student =
            await context.Users.FirstOrDefaultAsync(user => user.Id == submittedLaboratoryReportDto.submitterId,
                cancellationToken);
        if (student == null)
        {
            return false;
        }

        var laboratorySession =
            await context.LaboratorySessions.FirstOrDefaultAsync(
                lab => lab.InviteCode == submittedLaboratoryReportDto.InvitedCode && lab.CreatedAt > DateTime.UtcNow.AddHours(-12),
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
        
        if (submittedLaboratoryReportDto.ObservationsImage != null)
        {
            var imageKey = $"{laboratoryReport.CreatedAt:yyyy-MM-dd-HH-mm-ss-fffffff}-{student.Id}-{laboratorySession.Id}.png";
            await using (var stream = submittedLaboratoryReportDto.ObservationsImage.OpenReadStream())
            {
                var putRequest = new PutObjectRequest
                {
                    BucketName = R2BucketName,
                    Key = imageKey,
                    InputStream = stream,
                    DisablePayloadSigning = true
                };
                await S3Client.PutObjectAsync(putRequest, cancellationToken);
            }
            laboratoryReport.ObservationsImageLink = $"{R2PublicUrl}/{imageKey}";
        }

        await context.StudentLaboratoryReports.AddAsync(laboratoryReport, cancellationToken);

        if (submittedLaboratoryReportDto.StepsCompleted != null)
        {
            var laboratorySteps = context.LaboratoryChecklistSteps
                .Where(lab => lab.LaboratoryId == laboratorySession.LaboratoryId)
                .GroupBy(step => step.StepNumber, (key, group) => group.OrderByDescending(g => g.Version).First());
            List<StudentLaboratoryReportStep> completedSteps = [];
            foreach (var step in laboratorySteps)
            {
                var studentStep = new StudentLaboratoryReportStep
                {
                    StudentLaboratoryReport = laboratoryReport,
                    LaboratoryChecklistStep = step,
                    IsCompleted = submittedLaboratoryReportDto.StepsCompleted.Contains(step.StepNumber)
                };
                completedSteps.Add(studentStep);
            }

            await context.StudentLaboratoryReportSteps.AddRangeAsync(completedSteps, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        return true;
    }

    public async Task<int> GetLaboratorySessionCreatorId(int reportId, CancellationToken cancellationToken)
    {
        var report = await context.StudentLaboratoryReports
            .Include(r => r.LaboratorySession)
            .FirstOrDefaultAsync(r => r.Id == reportId, cancellationToken);
        if (report == null)
        {
            throw new KeyNotFoundException("Laboratory report not found.");
        }

        return report.LaboratorySession.CreatedById;
    }

    public async Task<int> GetLaboratoryReportSubmitterId(int reportId, CancellationToken cancellationToken)
    {
        var report = await context.StudentLaboratoryReports
            .FirstOrDefaultAsync(r => r.Id == reportId, cancellationToken);
        if (report == null)
        {
            throw new KeyNotFoundException("Laboratory report not found.");
        }

        return report.StudentId;
    }

    public async Task<DetailedLaboratoryReportDto?> GetLaboratoryReport(int reportId, CancellationToken cancellationToken)
    {
        var report = await context.StudentLaboratoryReports
            .FirstOrDefaultAsync(r => r.Id == reportId, cancellationToken);
        
        if (report == null)
        {
            return null;
        }

        var submitter = await context.Users.FindAsync([report.StudentId], cancellationToken);
        
        if (submitter == null)
        {
            return null;
        }

        var steps = await context.StudentLaboratoryReportSteps
            .Where(s => s.StudentLaboratoryReportId == reportId)
            .Include(s => s.LaboratoryChecklistStep)
            .OrderBy(s => s.LaboratoryChecklistStep.StepNumber)
            .ToListAsync(cancellationToken);
        
        var detailedReport = new DetailedLaboratoryReportDto
        {
            Id = report.Id,
            SubmitterFullName = submitter.FirstName + " " + submitter.LastName,
            ChecklistSteps = steps.Select<StudentLaboratoryReportStep, ChecklistStepDto>(step => new ChecklistStepDto
            {
                Statement = step.LaboratoryChecklistStep.Statement,
                IsCompleted = step.IsCompleted,
            }).ToList(),
            ObservationsImageLink = report.ObservationsImageLink,
        };
        
        return detailedReport;
    }
}