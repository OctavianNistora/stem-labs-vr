using STEMLabsServer.Models.DTOs;

namespace STEMLabsServer.Services.Interfaces;

public interface ILaboratoryReportService
{
    // Adds a new laboratory report
    public Task<bool> AddLaboratoryReport(SubmittedLaboratoryReportDto submittedLaboratoryReportDto, CancellationToken cancellationToken);
    // Retrieves the creator ID of the laboratory session associated with a report
    public Task<int> GetLaboratorySessionCreatorId(int reportId, CancellationToken cancellationToken);
    // Retrieves the submitter ID of a laboratory report
    public Task<int> GetLaboratoryReportSubmitterId(int reportId, CancellationToken cancellationToken);
    // Retrieves detailed information about a laboratory report
    public Task<DetailedLaboratoryReportDto?> GetLaboratoryReport(int reportId, CancellationToken cancellationToken);
}