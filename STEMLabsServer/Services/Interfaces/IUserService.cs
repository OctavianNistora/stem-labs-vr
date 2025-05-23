using STEMLabsServer.Models.DTOs;
using STEMLabsServer.Shared;

namespace STEMLabsServer.Services.Interfaces;

public interface IUserService
{
    public Task<ServiceStatus> RegisterUser(UserRegisterDto userRegisterDto, CancellationToken cancellationToken);
    public Task<IEnumerable<RelatedLaboratoryDto>?> GetRelatedLaboratories(int userId, CancellationToken cancellationToken);
    public Task<ServiceStatus> UpdateUserRole(int userId, string role, CancellationToken cancellationToken);
    public Task<IEnumerable<RelatedSessionDto>?> GetRelatedSessions(int userId, int labId, CancellationToken cancellationToken);
    public Task<UserProfileDto?> GetUserProfile(int userId, CancellationToken cancellationToken);
    public Task<ServiceStatus> UpdateUserProfile(int userId, UserProfileDto userProfileUpdateDto, CancellationToken cancellationToken);
    public Task<ServiceStatusWithValue<string>> GetUserEmail(int userId, CancellationToken cancellationToken);
    public Task<ServiceStatus> UpdateUserEmail(int userId, UserEmailUpdateDto userEmailUpdateDto, CancellationToken cancellationToken);
    public Task<ServiceStatus> UpdateUserPassword(int userId, UserPasswordUpdateDto userPasswordUpdateDto, CancellationToken cancellationToken);
    public Task<IEnumerable<UserListItemDto>> GetAllUsers(CancellationToken cancellationToken);
    public Task<ServiceStatus> DeleteUser(int userId, CancellationToken cancellationToken);
}