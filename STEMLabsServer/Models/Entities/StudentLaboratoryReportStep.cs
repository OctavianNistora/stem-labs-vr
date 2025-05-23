using System.ComponentModel.DataAnnotations.Schema;

namespace STEMLabsServer.Models.Entities;

public class StudentLaboratoryReportStep
{
    public int Id { get; set; }
    
    [ForeignKey(nameof(StudentLaboratoryReport))]
    public int StudentLaboratoryReportId { get; set; }
    public required StudentLaboratoryReport StudentLaboratoryReport { get; set; }
    
    [ForeignKey(nameof(LaboratoryChecklistStep))]
    public int LaboratoryChecklistStepId { get; set; }
    public required LaboratoryChecklistStep LaboratoryChecklistStep { get; set; }
    
    public bool IsCompleted { get; set; }
}