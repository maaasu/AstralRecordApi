using AstralRecordApi.Data;
using AstralRecordApi.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AstralRecordApi.Repositories;

public class EquipmentRepository(AstralRecordDbContext dbContext) : IEquipmentRepository
{
    public async Task AddAsync(
        EquipmentInstanceEntity instance,
        IReadOnlyList<EquipmentInstanceStatRollEntity> statRolls,
        IReadOnlyList<EquipmentInstanceEnchantPoolEntity> enchantPools)
    {
        await dbContext.EquipmentInstances.AddAsync(instance);

        if (statRolls.Count > 0)
            await dbContext.EquipmentInstanceStatRolls.AddRangeAsync(statRolls);

        if (enchantPools.Count > 0)
            await dbContext.EquipmentInstanceEnchantPools.AddRangeAsync(enchantPools);

        await dbContext.SaveChangesAsync();
    }

    public async Task<EquipmentInstanceEntity?> FindInstanceAsync(Guid instanceId)
        => await dbContext.EquipmentInstances
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.EquipmentInstanceId == instanceId && !x.IsDeleted);

    public async Task<IReadOnlyList<EquipmentInstanceStatRollEntity>> FindStatRollsAsync(Guid instanceId)
        => await dbContext.EquipmentInstanceStatRolls
            .AsNoTracking()
            .Where(x => x.EquipmentInstanceId == instanceId && !x.IsDeleted)
            .OrderBy(x => x.SortOrder)
            .ToListAsync();

    public async Task<IReadOnlyList<EquipmentInstanceEnchantPoolEntity>> FindEnchantPoolsAsync(Guid instanceId)
        => await dbContext.EquipmentInstanceEnchantPools
            .AsNoTracking()
            .Where(x => x.EquipmentInstanceId == instanceId && !x.IsDeleted)
            .OrderBy(x => x.PoolIndex)
            .ToListAsync();
}
