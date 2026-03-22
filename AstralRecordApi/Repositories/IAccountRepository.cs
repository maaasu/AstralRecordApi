using AstralRecordApi.Models;

namespace AstralRecordApi.Repositories;

public interface IAccountRepository
{
    Task<AccountResponse?> GetByUuidAsync(Guid uuid);
    Task<AccountResponse> CreateAsync(AccountCreateRequest request);
    Task<AccountResponse?> UpdateAsync(Guid uuid, AccountUpdateRequest request);
}
