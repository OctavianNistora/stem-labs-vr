namespace STEMLabsServer.Models.DTOs;

public class LaboratoryDto
{
    public required string Name { get; set; }
    public required int SceneId { get; set; }
    public required List<string> Steps { get; set; }
}