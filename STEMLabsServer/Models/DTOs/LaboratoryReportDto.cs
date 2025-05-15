namespace STEMLabsServer.Models.DTOs;

public class LaboratoryReportDto
{
    public required string InvitedCode { get; set; }
    public required List<int> StepsCompleted { get; set; }
    public IFormFile? ObservationsImage { get; set; }
}