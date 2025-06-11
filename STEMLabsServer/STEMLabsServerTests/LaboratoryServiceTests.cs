using Microsoft.EntityFrameworkCore;
using STEMLabsServer.Data;
using STEMLabsServer.Models.DTOs;
using STEMLabsServer.Models.Entities;
using STEMLabsServer.Services;

namespace STEMLabsServerTests;

public class LaboratoryServiceTests
{
    private readonly MainDbContext _context;
    private readonly LaboratoryService _laboratoryService;
    
    public LaboratoryServiceTests()
    {
        var dbContextOptions = new DbContextOptionsBuilder<MainDbContext>()
            .UseInMemoryDatabase(databaseName: "LaboratoryServiceTestDatabase")
            .Options;
        _context = new MainDbContext(dbContextOptions);
        _context.Database.EnsureDeleted();
        _laboratoryService = new LaboratoryService(_context);
    }
    
    [Fact]
    public async Task TestLaboratorySimplifiedListRetrieval()
    {
        var laboratoryList = new List<Laboratory>()
        {
            new Laboratory { Id = 1, Name = "Lab 1", SceneId = 3, CheckListStepCount = 3 },
            new Laboratory { Id = 2, Name = "Lab 2", SceneId = 2, CheckListStepCount = 5 },
            new Laboratory { Id = 3, Name = "Lab 3", SceneId = 1, CheckListStepCount = 2 }
        };
        await _context.Laboratories.AddRangeAsync(laboratoryList);
        await _context.SaveChangesAsync();
        
        
        var response = await _laboratoryService.GetLaboratoriesSimplified(CancellationToken.None);
        
        
        Assert.NotEmpty(response);
        Assert.Equal(3, response.Count());
        Assert.Contains(response, l => l.Id == 1 && l.Name == "Lab 1");
        Assert.Contains(response, l => l.Id == 2 && l.Name == "Lab 2");
        Assert.Contains(response, l => l.Id == 3 && l.Name == "Lab 3");
    }

    [Fact]
    public async Task TestLaboratoryStepsRetrieval()
    {
        var laboratory = new Laboratory
        {
            Id = 1,
            Name = "Test Lab",
            SceneId = 1,
            CheckListStepCount = 3,
        };
        await _context.Laboratories.AddAsync(laboratory);
        
        var steps = new List<LaboratoryChecklistStep>
        {
            new LaboratoryChecklistStep { Id = 1, Laboratory = laboratory, StepNumber = 1, Statement = "Step 1", Version = 1 },
            new LaboratoryChecklistStep { Id = 2, Laboratory = laboratory, StepNumber = 2, Statement = "Step 2", Version = 1 },
            new LaboratoryChecklistStep { Id = 3, Laboratory = laboratory, StepNumber = 3, Statement = "Step 3", Version = 1 }
        };
        await _context.LaboratoryChecklistSteps.AddRangeAsync(steps);
        
        await _context.SaveChangesAsync();
        
        
        var response = await _laboratoryService.GetLaboratorySteps(1, CancellationToken.None);
        
        
        Assert.NotEmpty(response);
        Assert.Equal(3, response.Count());
        Assert.Contains(response, step => step == "Step 1");
        Assert.Contains(response, step => step == "Step 2");
        Assert.Contains(response, step => step == "Step 3");
    }
    
    [Fact]
    public async Task TestLaboratorySessionsRetrieval()
    {
        var createdByList = new List<User>()
        {
            new User { Email = "user1@email.com", FirstName = "User1", LastName = "Test", PasswordHashed = "", PhoneNumber = "1234567890", Username = "user_test1", },
            new User { Email = "uwer2@email.com", FirstName = "User2", LastName = "Test", PasswordHashed = "", PhoneNumber = "0987654321", Username = "user_test2", }
        };
        await _context.Users.AddRangeAsync(createdByList);
        
        var laboratory = new Laboratory
        {
            Name = "Test Lab",
            SceneId = 1,
            CheckListStepCount = 3,
        };
        await _context.Laboratories.AddAsync(laboratory);
        
        var dateTimeNow = DateTime.UtcNow;
        var sessions = new List<LaboratorySession>
        {
            new LaboratorySession { Id = 1, CreatedBy = createdByList[0], InviteCode = "INVITE123", Laboratory = laboratory, CreatedAt = dateTimeNow },
            new LaboratorySession { Id = 2, CreatedBy = createdByList[0], InviteCode = "INVITE456", Laboratory = laboratory, CreatedAt = dateTimeNow.AddMinutes(5) },
            new LaboratorySession { Id = 3, CreatedBy = createdByList[0], InviteCode = "INVITE789", Laboratory = laboratory, CreatedAt = dateTimeNow.AddMinutes(10) },
            new LaboratorySession { Id = 4, CreatedBy = createdByList[1], InviteCode = "INVITE000", Laboratory = laboratory, CreatedAt = dateTimeNow.AddMinutes(15) }
        };
        await _context.LaboratorySessions.AddRangeAsync(sessions);
        
        await _context.SaveChangesAsync();
        
        
        var response = await _laboratoryService.GetLaboratorySessions(laboratory.Id, CancellationToken.None);
        
        
        Assert.NotEmpty(response);
        Assert.Equal(4, response.Count());
        Assert.Contains(response, session => session.Id == 1 && session.Date == dateTimeNow);
        Assert.Contains(response, session => session.Id == 2 && session.Date == dateTimeNow.AddMinutes(5));
        Assert.Contains(response, session => session.Id == 3 && session.Date == dateTimeNow.AddMinutes(10));
        Assert.Contains(response, session => session.Id == 4 && session.Date == dateTimeNow.AddMinutes(15));
    }
}