using System.ComponentModel.DataAnnotations.Schema;

namespace STEMLabsServer.Models.Entities;

public class StudentLaboratoryCompletedStep
{
    public int Id { get; set; }
    
    [ForeignKey(nameof(LaboratorySession))]
    public int LaboratorySessionId { get; set; }
    public required LaboratorySession LaboratorySession { get; set; }
    
    [ForeignKey(nameof(LaboratoryChecklistStep))]
    public int LaboratoryChecklistStepId { get; set; }
    public required LaboratoryChecklistStep LaboratoryChecklistStep { get; set; }
    
    [ForeignKey(nameof(Student))]
    public int StudentId { get; set; }
    public required User Student { get; set; }
}