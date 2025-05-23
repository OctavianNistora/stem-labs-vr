namespace STEMLabsServer.Models.DTOs;

public class SubmittedLaboratoryReportDto
{
    public required int submitterId { get; set; }
    public required string InvitedCode { get; set; }
    public List<int>? StepsCompleted { get; set; }
    public IFormFile? ObservationsImage { get; set; }
}