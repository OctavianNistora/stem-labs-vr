using System.ComponentModel.DataAnnotations;
using STEMLabsServer.Shared;

namespace STEMLabsServer.Models.Entities;

public class User
{
    [Key]
    public int Id { get; set; }
    
    public required string Username { get; set; }
    
    public required string PasswordHashed { get; set; }
    
    public required string Email { get; set; }
    
    public required string FirstName { get; set; }
    
    public required string LastName { get; set; }
    
    public required string PhoneNumber { get; set; }
    
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    
    public bool IsVerified { get; set; } = false;
    
    public UserRole UserRole { get; set; } = UserRole.Student;
}