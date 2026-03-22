using AstralRecordApi.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace AstralRecordApi.Repositories;

public class AccountRepository(IConfiguration configuration) : IAccountRepository
{
    private readonly string _connectionString =
        configuration.GetConnectionString("SqlServer")
        ?? throw new InvalidOperationException("Connection string 'SqlServer' is not configured.");

    private const string SelectColumns = """
        SELECT
            [uuid]         AS Uuid,
            [user_id]      AS UserId,
            [account_name] AS AccountName,
            [slot_index]   AS SlotIndex,
            [is_active]    AS IsActive,
            [mode]         AS Mode,
            [created_at]   AS CreatedAt,
            [updated_at]   AS UpdatedAt,
            [created_by]   AS CreatedBy,
            [updated_by]   AS UpdatedBy,
            [is_deleted]   AS IsDeleted
        FROM [dbo].[account]
        """;

    public async Task<AccountResponse?> GetByUuidAsync(Guid uuid)
    {
        var sql = $"""
            {SelectColumns}
            WHERE [uuid] = @Uuid
              AND [is_deleted] = 0
            """;

        await using var connection = new SqlConnection(_connectionString);
        return await connection.QuerySingleOrDefaultAsync<AccountResponse>(sql, new { Uuid = uuid });
    }

    public async Task<AccountResponse> CreateAsync(AccountCreateRequest request)
    {
        var newUuid = Guid.NewGuid();
        var now = DateTime.UtcNow;
        const string sql = """
            INSERT INTO [dbo].[account] (
                [uuid], [user_id], [account_name], [slot_index], [mode],
                [created_at], [updated_at], [created_by], [updated_by]
            ) VALUES (
                @Uuid, @UserId, @AccountName, @SlotIndex, @Mode,
                @Now, @Now, @CreatedBy, @CreatedBy
            );
            """;

        await using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(sql, new
        {
            Uuid = newUuid,
            request.UserId,
            request.AccountName,
            request.SlotIndex,
            request.Mode,
            Now = now,
            request.CreatedBy,
        });

        return (await GetByUuidAsync(newUuid))!;
    }

    public async Task<AccountResponse?> UpdateAsync(Guid uuid, AccountUpdateRequest request)
    {
        var now = DateTime.UtcNow;
        const string sql = """
            UPDATE [dbo].[account]
            SET
                [account_name] = COALESCE(@AccountName, [account_name]),
                [is_active]    = COALESCE(@IsActive, [is_active]),
                [mode]         = COALESCE(@Mode, [mode]),
                [updated_at]   = @Now,
                [updated_by]   = @UpdatedBy
            WHERE [uuid] = @Uuid
              AND [is_deleted] = 0;
            """;

        await using var connection = new SqlConnection(_connectionString);
        var affected = await connection.ExecuteAsync(sql, new
        {
            Uuid = uuid,
            request.AccountName,
            request.IsActive,
            request.Mode,
            Now = now,
            request.UpdatedBy,
        });

        if (affected == 0)
            return null;

        return await GetByUuidAsync(uuid);
    }
}
