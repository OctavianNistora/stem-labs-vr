using System.ComponentModel.DataAnnotations.Schema;

namespace STEMLabsServer.Models.Entities;

public class LaboratorySession
{
    public int Id { get; set; }
    
    [ForeignKey(nameof(Laboratory))]
    public int LaboratoryId { get; set; }
    public required Laboratory Laboratory { get; set; }
    
    [ForeignKey(nameof(User))]
    public int CreatedById { get; set; }
    public required User CreatedBy { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public required string InviteCode { get; set; }
}