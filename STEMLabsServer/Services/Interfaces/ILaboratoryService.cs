using STEMLabsServer.Models.DTOs;

namespace STEMLabsServer.Services.Interfaces;

public interface ILaboratoryService
{
    public Task CreateLaboratory(LaboratoryDto laboratoryDto, CancellationToken cancellationToken);
    public Task<bool> UpdateLaboratory(int id, LaboratoryDto laboratoryDto, CancellationToken cancellationToken);
    public Task<IEnumerable<LaboratoryFullListItemDto>> GetLaboratories(CancellationToken cancellationToken);
    public Task<IEnumerable<IdNameDto>> GetLaboratoriesSimplified(CancellationToken cancellationToken);
    public Task<LaboratoryDto?> GetLaboratory(int id, CancellationToken cancellationToken);
    public Task<IEnumerable<string>> GetLaboratorySteps(int id, CancellationToken cancellationToken);
    public Task<IEnumerable<IdDateDto>> GetLaboratorySessions(int id, CancellationToken cancellationToken);
}