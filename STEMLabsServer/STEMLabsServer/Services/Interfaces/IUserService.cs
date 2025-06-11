using STEMLabsServer.Models.DTOs;
using STEMLabsServer.Shared;

namespace STEMLabsServer.Services.Interfaces;

public interface IUserService
{
    // Registers a new user in the system.
    public Task<ServiceStatus> RegisterUser(UserRegisterDto userRegisterDto, CancellationToken cancellationToken);
    // Retrieves the list of laboratories related to a user.
    public Task<IEnumerable<RelatedLaboratoryDto>?> GetRelatedLaboratories(int userId, CancellationToken cancellationToken);
    // Updates the role of a user.
    public Task<ServiceStatus> UpdateUserRole(int userId, string role, CancellationToken cancellationToken);
    // Retrieves the list of sessions related to a user in a specific laboratory.
    public Task<IEnumerable<IdDateDto>?> GetRelatedSessions(int userId, int labId, CancellationToken cancellationToken);
    // Retrieves the user profile information.
    public Task<UserProfileDto?> GetUserProfile(int userId, CancellationToken cancellationToken);
    // Updates the user profile information.
    public Task<ServiceStatus> UpdateUserProfile(int userId, UserProfileDto userProfileUpdateDto, CancellationToken cancellationToken);
    // Retrieves the email of a user.
    public Task<ServiceStatusWithValue<string>> GetUserEmail(int userId, CancellationToken cancellationToken);
    // Updates the email of a user.
    public Task<ServiceStatus> UpdateUserEmail(int userId, UserEmailUpdateDto userEmailUpdateDto, CancellationToken cancellationToken);
    // Updates the password of a user.
    public Task<ServiceStatus> UpdateUserPassword(int userId, UserPasswordUpdateDto userPasswordUpdateDto, CancellationToken cancellationToken);
    // Retrieves a list of all users in the system.
    public Task<IEnumerable<UserListItemDto>> GetAllUsers(CancellationToken cancellationToken);
    // Deletes a user from the system.
    public Task<ServiceStatus> DeleteUser(int userId, CancellationToken cancellationToken);
}