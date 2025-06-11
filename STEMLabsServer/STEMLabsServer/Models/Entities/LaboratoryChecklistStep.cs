using System.ComponentModel.DataAnnotations.Schema;

namespace STEMLabsServer.Models.Entities;

public class LaboratoryChecklistStep
{
    public int Id { get; set; }
    
    [ForeignKey(nameof(Laboratory))]
    public int LaboratoryId { get; set; }
    public required Laboratory Laboratory { get; set; }
    
    public required int Version { get; set; }
    
    public required int StepNumber { get; set; }
    
    public required string Statement { get; set; }
}