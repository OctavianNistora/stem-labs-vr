using Microsoft.EntityFrameworkCore;
using STEMLabsServer.Models.Entities;

namespace STEMLabsServer.Data;

public class MainDbContext(DbContextOptions<MainDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<PasswordResetRequest> PasswordResetRequests { get; set; }
    
    public DbSet<Laboratory> Laboratories { get; set; }
    public DbSet<LaboratoryChecklistStep> LaboratoryChecklistSteps { get; set; }
    
    public DbSet<LaboratorySession> LaboratorySessions { get; set; }
    
    public DbSet<StudentLaboratoryReport> StudentLaboratoryReports { get; set; }
    public DbSet<StudentLaboratoryReportStep> StudentLaboratoryReportSteps { get; set; }
}