using AstralRecordApi.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace AstralRecordApi.Repositories;

public class UserRepository(IConfiguration configuration) : IUserRepository
{
    private readonly string _connectionString =
        configuration.GetConnectionString("SqlServer")
        ?? throw new InvalidOperationException("Connection string 'SqlServer' is not configured.");

    private const string SelectColumns = """
        SELECT
            [uuid]           AS Uuid,
            [mcid]           AS Mcid,
            [join_date]      AS JoinDate,
            [last_join_date] AS LastJoinDate,
            [global_ip]      AS GlobalIp,
            [account_id]     AS AccountId,
            [ban_indefinite] AS BanIndefinite,
            [ban_date]       AS BanDate,
            [kick_ip]        AS KickIp,
            [permission]     AS Permission,
            [created_at]     AS CreatedAt,
            [updated_at]     AS UpdatedAt,
            [created_by]     AS CreatedBy,
            [updated_by]     AS UpdatedBy,
            [is_deleted]     AS IsDeleted
        FROM [dbo].[user]
        """;

    public async Task<UserResponse?> GetByUuidAsync(Guid uuid)
    {
        var sql = $"""
            {SelectColumns}
            WHERE [uuid] = @Uuid
              AND [is_deleted] = 0
            """;

        await using var connection = new SqlConnection(_connectionString);
        return await connection.QuerySingleOrDefaultAsync<UserResponse>(sql, new { Uuid = uuid });
    }

    public async Task<UserResponse> CreateAsync(UserCreateRequest request)
    {
        var now = DateTime.UtcNow;
        const string sql = """
            INSERT INTO [dbo].[user] (
                [uuid], [mcid], [join_date], [last_join_date], [global_ip],
                [created_at], [updated_at], [created_by], [updated_by]
            ) VALUES (
                @Uuid, @Mcid, @JoinDate, @LastJoinDate, @GlobalIp,
                @Now, @Now, @CreatedBy, @CreatedBy
            );
            """;

        await using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(sql, new
        {
            request.Uuid,
            request.Mcid,
            request.JoinDate,
            request.LastJoinDate,
            request.GlobalIp,
            Now = now,
            request.CreatedBy,
        });

        return (await GetByUuidAsync(request.Uuid))!;
    }

    public async Task<UserResponse?> UpdateAsync(Guid uuid, UserUpdateRequest request)
    {
        var now = DateTime.UtcNow;
        const string sql = """
            UPDATE [dbo].[user]
            SET
                [mcid]           = COALESCE(@Mcid, [mcid]),
                [last_join_date] = COALESCE(@LastJoinDate, [last_join_date]),
                [global_ip]      = COALESCE(@GlobalIp, [global_ip]),
                [account_id]     = CASE WHEN @AccountIdSet = 1 THEN @AccountId ELSE [account_id] END,
                [ban_indefinite] = COALESCE(@BanIndefinite, [ban_indefinite]),
                [ban_date]       = CASE WHEN @BanDateSet = 1 THEN @BanDate ELSE [ban_date] END,
                [kick_ip]        = COALESCE(@KickIp, [kick_ip]),
                [permission]     = COALESCE(@Permission, [permission]),
                [updated_at]     = @Now,
                [updated_by]     = @UpdatedBy
            WHERE [uuid] = @Uuid
              AND [is_deleted] = 0;
            """;

        await using var connection = new SqlConnection(_connectionString);
        var affected = await connection.ExecuteAsync(sql, new
        {
            Uuid = uuid,
            request.Mcid,
            request.LastJoinDate,
            request.GlobalIp,
            AccountIdSet = request.AccountId.HasValue ? 1 : 0,
            request.AccountId,
            request.BanIndefinite,
            BanDateSet = request.BanDate.HasValue ? 1 : 0,
            request.BanDate,
            request.KickIp,
            request.Permission,
            Now = now,
            request.UpdatedBy,
        });

        if (affected == 0)
            return null;

        return await GetByUuidAsync(uuid);
    }
}
