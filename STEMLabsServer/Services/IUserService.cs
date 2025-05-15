using STEMLabsServer.Models.DTOs;

namespace STEMLabsServer.Services;

public interface IUserService
{
    Task<bool> RegisterUser(UserRegisterDto user, CancellationToken cancellationToken);
    Task<bool> AddLaboratorySession(int teacherId, LaboratorySessionDto laboratorySessionDto, CancellationToken cancellationToken);

    Task<bool> AddLaboratoryReport(int studentId, LaboratoryReportDto laboratoryReportDto,
        CancellationToken cancellationToken);
}