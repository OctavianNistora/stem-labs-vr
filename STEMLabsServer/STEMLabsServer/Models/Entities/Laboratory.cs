using System.ComponentModel.DataAnnotations.Schema;

namespace STEMLabsServer.Models.Entities;

public class Laboratory
{
    public int Id { get; set; }
    
    public required string Name { get; set; }
    
    public required int SceneId { get; set; }
    
    public required int CheckListStepCount { get; set; }
}
