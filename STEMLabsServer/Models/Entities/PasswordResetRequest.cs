using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace STEMLabsServer.Models.Entities;

public class PasswordResetRequest
{
    [Key]
    public int Id { get; set; }
    
    [ForeignKey(nameof(User))]
    public int UserId { get; set; }
    public required User User { get; set; }
    
    public required string Token { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}