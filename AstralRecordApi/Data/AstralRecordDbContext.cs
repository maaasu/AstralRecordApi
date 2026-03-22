using AstralRecordApi.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AstralRecordApi.Data;

public class AstralRecordDbContext(DbContextOptions<AstralRecordDbContext> options) : DbContext(options)
{
    public DbSet<UserEntity> Users => Set<UserEntity>();
    public DbSet<AccountEntity> Accounts => Set<AccountEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserEntity>(entity =>
        {
            entity.ToTable("user", "dbo");
            entity.HasKey(user => user.Uuid);

            entity.Property(user => user.Uuid).HasColumnName("uuid");
            entity.Property(user => user.Mcid).HasColumnName("mcid");
            entity.Property(user => user.JoinDate).HasColumnName("join_date");
            entity.Property(user => user.LastJoinDate).HasColumnName("last_join_date");
            entity.Property(user => user.GlobalIp).HasColumnName("global_ip");
            entity.Property(user => user.AccountId).HasColumnName("account_id");
            entity.Property(user => user.BanIndefinite).HasColumnName("ban_indefinite");
            entity.Property(user => user.BanDate).HasColumnName("ban_date");
            entity.Property(user => user.KickIp).HasColumnName("kick_ip");
            entity.Property(user => user.Permission).HasColumnName("permission");
            entity.Property(user => user.CreatedAt).HasColumnName("created_at");
            entity.Property(user => user.UpdatedAt).HasColumnName("updated_at");
            entity.Property(user => user.CreatedBy).HasColumnName("created_by");
            entity.Property(user => user.UpdatedBy).HasColumnName("updated_by");
            entity.Property(user => user.IsDeleted).HasColumnName("is_deleted");
        });

        modelBuilder.Entity<AccountEntity>(entity =>
        {
            entity.ToTable("account", "dbo");
            entity.HasKey(account => account.Uuid);

            entity.Property(account => account.Uuid).HasColumnName("uuid");
            entity.Property(account => account.UserId).HasColumnName("user_id");
            entity.Property(account => account.AccountName).HasColumnName("account_name");
            entity.Property(account => account.SlotIndex).HasColumnName("slot_index");
            entity.Property(account => account.IsActive).HasColumnName("is_active");
            entity.Property(account => account.Mode).HasColumnName("mode");
            entity.Property(account => account.CreatedAt).HasColumnName("created_at");
            entity.Property(account => account.UpdatedAt).HasColumnName("updated_at");
            entity.Property(account => account.CreatedBy).HasColumnName("created_by");
            entity.Property(account => account.UpdatedBy).HasColumnName("updated_by");
            entity.Property(account => account.IsDeleted).HasColumnName("is_deleted");
        });
    }
}
