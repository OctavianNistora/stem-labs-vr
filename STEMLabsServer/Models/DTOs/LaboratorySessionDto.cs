namespace STEMLabsServer.Models.DTOs;

public class LaboratorySessionDto
{
    public required int creatorId { get; set; }
    public required int SceneId { get; set; }
    public required string InviteCode { get; set; }
}