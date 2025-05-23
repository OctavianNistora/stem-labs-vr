namespace STEMLabsServer.Models.DTOs;

public class LaboratoryFullListItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int SceneId { get; set; }
    public int CheckListStepCount { get; set; }
}