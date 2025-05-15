using STEMLabsServer.Models.DTOs;

namespace STEMLabsServer.Services;

public interface ILabService
{
    Task CreateLaboratory(LaboratoryDto laboratoryDto, CancellationToken cancellationToken);
    Task<bool> UpdateLaboratory(int id, LaboratoryDto laboratoryDto, CancellationToken cancellationToken);
}