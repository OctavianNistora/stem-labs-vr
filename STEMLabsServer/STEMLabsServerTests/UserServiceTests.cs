using Microsoft.EntityFrameworkCore;
using STEMLabsServer.Data;
using STEMLabsServer.Models.Entities;
using STEMLabsServer.Services;

namespace STEMLabsServerTests;

public class UserServiceTests
{
    private readonly MainDbContext _context;
    private readonly UserService _userService;

    private DateTime _dateTime;
    
    public UserServiceTests()
    {
        var dbContextOptions = new DbContextOptionsBuilder<MainDbContext>()
            .UseInMemoryDatabase(databaseName: "UserServiceTestDatabase")
            .Options;
        _context = new MainDbContext(dbContextOptions);
        _context.Database.EnsureDeleted();
        _userService = new UserService(_context);

        var creator = new User
        {
            Id = 1,
            Email = "creator@mail.com",
            Username = "testcreator",
            PasswordHashed = "",
            FirstName = "Creator",
            LastName = "Test",
            PhoneNumber = "0987654321",
        };
        
        var user = new User
        {
            Id = 2,
            Email = "user@mail.com",
            Username = "testuser",
            PasswordHashed = "",
            FirstName = "User",
            LastName = "Test",
            PhoneNumber = "1234567890",
        };
        _context.Users.Add(user);
        
        var laboratories = new List<Laboratory>
        {
            new Laboratory { Id = 1, Name = "Lab1", SceneId = 1, CheckListStepCount = 3 },
            new Laboratory { Id = 2, Name = "Lab2", SceneId = 2, CheckListStepCount = 4 },
            new Laboratory { Id = 3, Name = "Lab3", SceneId = 3, CheckListStepCount = 3 }
        };
        _context.Laboratories.AddRange(laboratories);
        
        _dateTime = DateTime.UtcNow;
        var sessions = new List<LaboratorySession>
        {
            new LaboratorySession { Id = 1, InviteCode = "INVITE1", Laboratory = laboratories[0], CreatedBy = creator, CreatedAt = _dateTime },
            new LaboratorySession { Id = 2, InviteCode = "INVITE2", Laboratory = laboratories[0], CreatedBy = creator, CreatedAt = _dateTime.AddMinutes(5) },
            new LaboratorySession { Id = 3, InviteCode = "INVITE3", Laboratory = laboratories[1], CreatedBy = creator, CreatedAt = _dateTime.AddMinutes(10) },
            new LaboratorySession { Id = 4, InviteCode = "INVITE4", Laboratory = laboratories[2], CreatedBy = creator, CreatedAt = _dateTime.AddMinutes(15) },
            new LaboratorySession { Id = 5, InviteCode = "INVITE5", Laboratory = laboratories[2], CreatedBy = creator, CreatedAt = _dateTime.AddMinutes(20) }
        };
        _context.LaboratorySessions.AddRange(sessions);
        
        var reports = new List<StudentLaboratoryReport>
        {
            new StudentLaboratoryReport { LaboratorySession = sessions[0], Student = user },
            new StudentLaboratoryReport { LaboratorySession = sessions[0], Student = user },
            new StudentLaboratoryReport { LaboratorySession = sessions[1], Student = user },
            new StudentLaboratoryReport { LaboratorySession = sessions[2], Student = user },
        };
        _context.StudentLaboratoryReports.AddRange(reports);
        
        _context.SaveChanges();
    }

    [Fact]
    public async Task TestRelatedLaboratoriesRetrieval()
    {
        var relatedLabs = await _userService.GetRelatedLaboratories(2, CancellationToken.None);
        
        
        Assert.NotEmpty(relatedLabs);
        Assert.Equal(2, relatedLabs.Count());
        Assert.Contains(relatedLabs, lab => lab.Id == 1 && lab.Name == "Lab1");
        Assert.Contains(relatedLabs, lab => lab.Id == 2 && lab.Name == "Lab2");
    }

    [Fact]
    public async Task TestRelatedSessionsRetrieval()
    {
        var relatedSessions = await _userService.GetRelatedSessions(2, 1, CancellationToken.None);
        
        Assert.Equal(2, relatedSessions.Count());
        Assert.Contains(relatedSessions, session => session.Id == 1 && session.Date == _dateTime);
        Assert.Contains(relatedSessions, session => session.Id == 2 && session.Date == _dateTime.AddMinutes(5));
    }
}