using STEMLabsServer.Models.DTOs;

namespace STEMLabsServer.Services.Interfaces;

public interface ILaboratoryService
{
    // Creates a new laboratory with the provided details.
    public Task CreateLaboratory(LaboratoryDto laboratoryDto, CancellationToken cancellationToken);
    // Updates an existing laboratory by its ID with the provided details.
    public Task<bool> UpdateLaboratory(int id, LaboratoryDto laboratoryDto, CancellationToken cancellationToken);
    // Retrieves a list of all laboratories with detailed information.
    public Task<IEnumerable<LaboratoryFullListItemDto>> GetLaboratories(CancellationToken cancellationToken);
    // Retrieves a simplified list of laboratories.
    public Task<IEnumerable<IdNameDto>> GetLaboratoriesSimplified(CancellationToken cancellationToken);
    // Retrieves a laboratory by its ID, returning detailed information.
    public Task<LaboratoryDto?> GetLaboratory(int id, CancellationToken cancellationToken);
    // Retrieves a list of laboratory steps for a specific scene by its ID.
    public Task<IEnumerable<string>> GetLaboratorySteps(int sceneId, CancellationToken cancellationToken);
    // Retrieves a list of laboratory sessions for a specific laboratory by its ID.
    public Task<IEnumerable<IdDateDto>> GetLaboratorySessions(int id, CancellationToken cancellationToken);
}