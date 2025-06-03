using STEMLabsServer.Models.DTOs;

namespace STEMLabsServer.Services.Interfaces;

public interface ILaboratorySessionService
{
    // Add a new laboratory session
    public Task<bool> AddLaboratorySession(LaboratorySessionDto laboratorySessionDto, CancellationToken cancellationToken);
    // Retrieve the identifier of the creator of a laboratory session
    public Task<int> GetLaboratorySessionCreator(int sessionId, CancellationToken cancellationToken);
    // Retrieve the list of participants in a laboratory session along with the last submission date
    public Task<IEnumerable<LaboratorySessionParticipantDto>> GetLaboratorySessionParticipants(int sessionId, CancellationToken cancellationToken);
    // Retrieve the list of reports for a specific laboratory session from a specific participant
    public Task<IEnumerable<LaboratoryReportListItemDto>> GetParticipantReports(int sessionId, int participantId, CancellationToken cancellationToken);
}