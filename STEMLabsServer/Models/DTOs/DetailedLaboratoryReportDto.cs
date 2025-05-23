namespace STEMLabsServer.Models.DTOs;

public class DetailedLaboratoryReportDto
{
    public int Id { get; set; }
    public string SubmitterFullName { get; set; } = string.Empty;
    public List<ChecklistStepDto> ChecklistSteps { get; set; } = [];
    public string? ObservationsImageLink { get; set; }
}

public class ChecklistStepDto
{
    public string Statement { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
}