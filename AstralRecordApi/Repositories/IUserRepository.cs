using AstralRecordApi.Models;

namespace AstralRecordApi.Repositories;

public interface IUserRepository
{
    Task<UserResponse?> GetByUuidAsync(Guid uuid);
}
