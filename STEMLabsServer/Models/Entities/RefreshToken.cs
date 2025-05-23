using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace STEMLabsServer.Models.Entities;

public class RefreshToken
{
    [Key]
    public int Id { get; set; }
    
    public required string Token { get; set; }
    
    public required DateTime ExpiryDate { get; set; }
    
    [ForeignKey(nameof(User))]
    public int UserId { get; set; }
    public required User User { get; set; }
}