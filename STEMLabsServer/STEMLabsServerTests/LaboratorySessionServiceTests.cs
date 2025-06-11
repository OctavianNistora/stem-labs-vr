using Microsoft.EntityFrameworkCore;
using STEMLabsServer.Data;
using STEMLabsServer.Models.DTOs;
using STEMLabsServer.Models.Entities;
using STEMLabsServer.Services;

namespace STEMLabsServerTests;

public class LaboratorySessionServiceTests
{
    private readonly MainDbContext _context;
    private readonly LaboratorySessionService _laboratorySessionService;
    
    public LaboratorySessionServiceTests()
    {
        var dbContextOptions = new DbContextOptionsBuilder<MainDbContext>()
            .UseInMemoryDatabase(databaseName: "LaboratorySessionServiceTestDatabase")
            .Options;
        _context = new MainDbContext(dbContextOptions);
        _context.Database.EnsureDeleted();
        _laboratorySessionService = new LaboratorySessionService(_context);
    }

    [Fact]
    public async Task TestSessionCreation()
    {
        var creator = new User
        {
            Email = "",
            FirstName = "User",
            LastName = "Test",
            PasswordHashed = "",
            PhoneNumber = "1234567890",
            Username = "testuser",
        };
        await _context.Users.AddAsync(creator);
        
        var laboratory = new Laboratory
        {
            Name = "Test Laboratory",
            SceneId = 2,
            CheckListStepCount = 3,
        };
        await _context.Laboratories.AddAsync(laboratory);
        
        await _context.SaveChangesAsync();
        
        var dto = new LaboratorySessionDto
        {
            InviteCode = "INVITE123",
            SceneId = 2,
            creatorId = creator.Id,
        };

        
        var result = await _laboratorySessionService.AddLaboratorySession(dto, CancellationToken.None);
        
        
        Assert.True(result);
    }
    
    [Fact]
    public async Task TestSessionParticipantsRetrieval()
    {
        var users = new List<User>
        {
            new User { Email = "user1@email.com", PasswordHashed = "", Username = "user1", FirstName = "First1", LastName = "Last1", PhoneNumber = "1111111111" },
            new User { Email = "user2@email.com", PasswordHashed = "", Username = "user2", FirstName = "First2", LastName = "Last2", PhoneNumber = "2222222222" },
            new User { Email = "user3@email.com", PasswordHashed = "", Username = "user3", FirstName = "First3", LastName = "Last3", PhoneNumber = "3333333333" },
            new User { Email = "user4@email.com", PasswordHashed = "", Username = "user4", FirstName = "First4", LastName = "Last4", PhoneNumber = "4444444444" },
        };
        await _context.Users.AddRangeAsync(users);
        
        var laboratory = new Laboratory
        {
            Name = "Test Laboratory",
            SceneId = 2,
            CheckListStepCount = 3,
        };
        await _context.Laboratories.AddAsync(laboratory);
        
        var session = new LaboratorySession
        {
            InviteCode = "INVITE123",
            CreatedBy = users[0],
            Laboratory = laboratory,
        };
        await _context.LaboratorySessions.AddAsync(session);
        
        var dateTime = DateTime.UtcNow;
        for (int i=1; i < users.Count; i++)
        {
            var participantReport = new StudentLaboratoryReport()
            {
                LaboratorySession = session,
                Student = users[i],
                CreatedAt = dateTime.AddMinutes(5 * i)
            };
            await _context.StudentLaboratoryReports.AddAsync(participantReport);
        }
        
        await _context.SaveChangesAsync();
        
        
        var result = await _laboratorySessionService.GetLaboratorySessionParticipants(session.Id, CancellationToken.None);
        
        
        Assert.NotEmpty(result);
        Assert.Equal(3, result.Count());
        Assert.Contains(result,
            p => p.Id == users[1].Id && p.Name == users[1].FirstName + " " + users[1].LastName &&
                 p.Date == dateTime.AddMinutes(5));
        Assert.Contains(result,
            p => p.Id == users[2].Id && p.Name == users[2].FirstName + " " + users[2].LastName &&
                 p.Date == dateTime.AddMinutes(10));
        Assert.Contains(result,
            p => p.Id == users[3].Id && p.Name == users[3].FirstName + " " + users[3].LastName &&
                 p.Date == dateTime.AddMinutes(15));
    }
}