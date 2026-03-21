using AstralRecordApi.Models;
using Dapper;
using Microsoft.Data.SqlClient;

namespace AstralRecordApi.Repositories;

public class UserRepository(IConfiguration configuration) : IUserRepository
{
    private readonly string _connectionString =
        configuration.GetConnectionString("SqlServer")
        ?? throw new InvalidOperationException("Connection string 'SqlServer' is not configured.");

    public async Task<UserResponse?> GetByUuidAsync(Guid uuid)
    {
        const string sql = """
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
            WHERE [uuid] = @Uuid
              AND [is_deleted] = 0
            """;

        await using var connection = new SqlConnection(_connectionString);
        return await connection.QuerySingleOrDefaultAsync<UserResponse>(sql, new { Uuid = uuid });
    }
}
