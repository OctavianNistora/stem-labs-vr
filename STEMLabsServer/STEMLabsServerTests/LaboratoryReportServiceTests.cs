using Microsoft.EntityFrameworkCore;
using STEMLabsServer.Data;
using STEMLabsServer.Models.DTOs;
using STEMLabsServer.Models.Entities;
using STEMLabsServer.Services;

namespace STEMLabsServerTests;

public class LaboratoryReportServiceTests
{
    private readonly MainDbContext _context;
    private readonly LaboratoryReportService _laboratoryReportService;
    private LaboratorySession _laboratorySession;
    
    public LaboratoryReportServiceTests()
    {
        var dbContextOptions = new DbContextOptionsBuilder<MainDbContext>()
            .UseInMemoryDatabase(databaseName: "LaboratoryReportServiceTestDatabase")
            .Options;
        _context = new MainDbContext(dbContextOptions);
        _context.Database.EnsureDeleted();
        _laboratoryReportService = new LaboratoryReportService(_context);
    }
    
    private async Task SeedDatabase()
    {
        var creator = new User()
        {
            Email = "creator@email.com",
            FirstName = "",
            LastName = "",
            Username = "creator",
            PasswordHashed = "",
            PhoneNumber = "",
        };
        _context.Users.Add(creator);
        
        var laboratory = new Laboratory()
        {
            SceneId = 1,
            Name = "Test Laboratory",
            CheckListStepCount = 4,
        };
        _context.Laboratories.Add(laboratory);
        
        var laboratorySteps = new List<LaboratoryChecklistStep>()
        {
            new LaboratoryChecklistStep() { Laboratory = laboratory, Statement = "Step 1", StepNumber = 1, Version = 1 },
            new LaboratoryChecklistStep() { Laboratory = laboratory, Statement = "Step 2", StepNumber = 2, Version = 1 },
            new LaboratoryChecklistStep() { Laboratory = laboratory, Statement = "Step 3", StepNumber = 3, Version = 1 },
            new LaboratoryChecklistStep() { Laboratory = laboratory, Statement = "Step 4", StepNumber = 4, Version = 1 },
        };
        _context.LaboratoryChecklistSteps.AddRange(laboratorySteps);
        
        _laboratorySession = new LaboratorySession()
        {
            Id = 1,
            LaboratoryId = laboratory.Id,
            CreatedBy = creator,
            InviteCode = "INVITE123",
            Laboratory = laboratory,
        };
        _context.LaboratorySessions.Add(_laboratorySession);
        
        await _context.SaveChangesAsync();
    }

    [Fact]
    public async Task LaboratoryReportCreation()
    {
        await SeedDatabase();

        var student = new User()
        {
            Email = "",
            FirstName = "John",
            PasswordHashed = "",
            PhoneNumber = "",
            Username = "john_doe",
            LastName = "Doe",
        };
        _context.Users.Add(student);
        await _context.SaveChangesAsync();

        var dto = new SubmittedLaboratoryReportDto()
        {
            InvitedCode = "INVITE123",
            submitterId = student.Id,
            ObservationsImage = null,
            StepsCompleted = [1, 2, 4]
        };
        
        
        var isAdded = await _laboratoryReportService.AddLaboratoryReport(dto, CancellationToken.None);
        
        
        Assert.True(isAdded);

        var report = await _context.StudentLaboratoryReports
            .FirstOrDefaultAsync(r => r.LaboratorySession == _laboratorySession && r.Student == student);
        Assert.NotNull(report);
        
        var reportSteps = await _context.StudentLaboratoryReportSteps
            .Where(rs => rs.StudentLaboratoryReport == report)
            .ToListAsync();
        Assert.Equal(4, reportSteps.Count);
        Assert.Equal(3, reportSteps.Count(rs => rs.IsCompleted));
    }
    
    [Fact]
    public async Task LaboratoryReportRetrieval()
    {
        await SeedDatabase();
        
        var student = new User()
        {
            Email = "",
            FirstName = "Jane",
            PasswordHashed = "",
            PhoneNumber = "",
            Username = "jane_doe",
            LastName = "Doe",
        };
        _context.Users.Add(student);

        var report = new StudentLaboratoryReport()
        {
            LaboratorySession = _laboratorySession,
            Student = student,
        };
        await _context.StudentLaboratoryReports.AddAsync(report);
        var laboratorySteps = _context.LaboratoryChecklistSteps
            .Where(step => step.LaboratoryId == _laboratorySession.LaboratoryId)
            .OrderBy(step => step.StepNumber)
            .ToList();
        var reportSteps = new List<StudentLaboratoryReportStep>()
        {
            new StudentLaboratoryReportStep()
                { LaboratoryChecklistStep = laboratorySteps[0], StudentLaboratoryReport = report, IsCompleted = true },
            new StudentLaboratoryReportStep()
                { LaboratoryChecklistStep = laboratorySteps[1], StudentLaboratoryReport = report, IsCompleted = false },
            new StudentLaboratoryReportStep()
                { LaboratoryChecklistStep = laboratorySteps[2], StudentLaboratoryReport = report, IsCompleted = true },
            new StudentLaboratoryReportStep()
                { LaboratoryChecklistStep = laboratorySteps[3], StudentLaboratoryReport = report, IsCompleted = false },
        };
        await _context.StudentLaboratoryReportSteps.AddRangeAsync(reportSteps);
        
        await _context.SaveChangesAsync();
        
        
        var retrievedReport = await _laboratoryReportService.GetLaboratoryReport(report.Id, CancellationToken.None);
        
        
        Assert.NotNull(retrievedReport);
        Assert.Equal(retrievedReport.Id, report.Id);
        Assert.Equal(retrievedReport.ChecklistSteps.Count, 4);
        Assert.Equal(retrievedReport.ChecklistSteps[0].Statement, laboratorySteps[0].Statement);
        Assert.Equal(retrievedReport.ChecklistSteps[0].IsCompleted, true);
        Assert.Equal(retrievedReport.ChecklistSteps[1].Statement, laboratorySteps[1].Statement);
        Assert.Equal(retrievedReport.ChecklistSteps[1].IsCompleted, false);
        Assert.Equal(retrievedReport.ChecklistSteps[2].Statement, laboratorySteps[2].Statement);
        Assert.Equal(retrievedReport.ChecklistSteps[2].IsCompleted, true);
        Assert.Equal(retrievedReport.ChecklistSteps[3].Statement, laboratorySteps[3].Statement);
        Assert.Equal(retrievedReport.ChecklistSteps[3].IsCompleted, false);
        Assert.Equal(retrievedReport.ObservationsImageLink, report.ObservationsImageLink);
        Assert.Equal(retrievedReport.SubmitterFullName, $"{student.FirstName} {student.LastName}");
    }
}