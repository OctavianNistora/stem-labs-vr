using STEMLabsServer.Models.DTOs;

namespace STEMLabsServer.Services.Interfaces;

public interface ILaboratorySessionService
{
    public Task<bool> AddLaboratorySession(LaboratorySessionDto laboratorySessionDto, CancellationToken cancellationToken);
    public Task<int> GetLaboratorySessionCreator(int sessionId, CancellationToken cancellationToken);
    public Task<IEnumerable<LaboratorySessionParticipantDto>> GetLaboratorySessionParticipants(int sessionId, CancellationToken cancellationToken);
    public Task<IEnumerable<LaboratoryReportListItemDto>> GetParticipantReports(int sessionId, int participantId, CancellationToken cancellationToken);
}