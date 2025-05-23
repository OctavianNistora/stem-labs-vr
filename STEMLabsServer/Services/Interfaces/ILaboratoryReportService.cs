using STEMLabsServer.Models.DTOs;

namespace STEMLabsServer.Services.Interfaces;

public interface ILaboratoryReportService
{
    public Task<bool> AddLaboratoryReport(SubmittedLaboratoryReportDto submittedLaboratoryReportDto, CancellationToken cancellationToken);
    public Task<int> GetLaboratorySessionCreatorId(int reportId, CancellationToken cancellationToken);
    public Task<int> GetLaboratoryReportSubmitterId(int reportId, CancellationToken cancellationToken);
    public Task<DetailedLaboratoryReportDto?> GetLaboratoryReport(int reportId, CancellationToken cancellationToken);
}