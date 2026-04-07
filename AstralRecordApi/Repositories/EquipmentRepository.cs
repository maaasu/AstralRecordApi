using AstralRecordApi.Data;
using AstralRecordApi.Data.Entities;
using AstralRecordApi.Models;
using AstralRecordApi.Utilities;
using Microsoft.EntityFrameworkCore;

namespace AstralRecordApi.Repositories;

public class EquipmentRepository(AstralRecordDbContext dbContext, IItemRepository itemRepository) : IEquipmentRepository
{
    public async Task<EquipmentInstanceResponse?> CreateAsync(EquipmentCreateRequest request)
    {
        var item = itemRepository.GetById(request.EquipmentId);
        if (item?.Equipment is null)
            return null;

        var equipment = item.Equipment;
        var now = DateTime.UtcNow;

        // ルーンスロット数の解決（範囲指定の場合はランダムに確定）
        var runeMaxSlots = 0;
        if (equipment.Rune is not null)
            runeMaxSlots = RangeValueResolver.ResolveInt(equipment.Rune.MaxSlots);

        // 装備インスタンス本体
        var instance = new EquipmentInstanceEntity
        {
            EquipmentInstanceId = Guid.NewGuid(),
            AccountId = request.AccountId,
            ItemId = request.EquipmentId,
            EnhanceLevel = 0,
            RuneMaxSlots = runeMaxSlots,
            TranscendenceRank = 0,
            DurabilityMax = equipment.Durability?.Max,
            DurabilityValue = equipment.Durability?.Max,
            CreatedAt = now,
            UpdatedAt = now,
            CreatedBy = request.CreatedBy,
            UpdatedBy = request.CreatedBy,
            IsDeleted = false,
        };

        await dbContext.EquipmentInstances.AddAsync(instance);

        // ステータス乱数ロール（random フィールドを持つ stat のみ保存）
        var statRollEntities = new List<EquipmentInstanceStatRollEntity>();
        var sortOrder = 0;
        foreach (var stat in equipment.Stats)
        {
            if (stat.Status is null || stat.Random is null)
                continue;

            var tildeIndex = stat.Random.IndexOf('~');
            if (tildeIndex < 0)
                continue;

            statRollEntities.Add(new EquipmentInstanceStatRollEntity
            {
                StatRollId = Guid.NewGuid(),
                EquipmentInstanceId = instance.EquipmentInstanceId,
                Status = stat.Status,
                RandomMin = stat.Random[..tildeIndex],
                RandomMax = stat.Random[(tildeIndex + 1)..],
                SortOrder = sortOrder++,
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = request.CreatedBy,
                UpdatedBy = request.CreatedBy,
                IsDeleted = false,
            });
        }

        if (statRollEntities.Count > 0)
            await dbContext.EquipmentInstanceStatRolls.AddRangeAsync(statRollEntities);

        // エンチャントプール（個体生成時点のプール構成を保存）
        var enchantPoolEntities = new List<EquipmentInstanceEnchantPoolEntity>();
        if (equipment.Enchant is not null)
        {
            for (var i = 0; i < equipment.Enchant.Pools.Count; i++)
            {
                var pool = equipment.Enchant.Pools[i];
                enchantPoolEntities.Add(new EquipmentInstanceEnchantPoolEntity
                {
                    EnchantPoolId = Guid.NewGuid(),
                    EquipmentInstanceId = instance.EquipmentInstanceId,
                    PoolIndex = i,
                    RecipeId = pool.RecipeId,
                    RequiredMaterialItemId = pool.RequiredMaterial?.ItemId,
                    RequiredMaterialAmount = pool.RequiredMaterial?.Amount ?? 1,
                    RequiredCurrency = pool.RequiredCurrency,
                    CreatedAt = now,
                    UpdatedAt = now,
                    CreatedBy = request.CreatedBy,
                    UpdatedBy = request.CreatedBy,
                    IsDeleted = false,
                });
            }

            if (enchantPoolEntities.Count > 0)
                await dbContext.EquipmentInstanceEnchantPools.AddRangeAsync(enchantPoolEntities);
        }

        await dbContext.SaveChangesAsync();

        return MapToResponse(instance, statRollEntities, enchantPoolEntities);
    }

    public async Task<EquipmentInstanceResponse?> GetByInstanceIdAsync(Guid instanceId)
    {
        var instance = await dbContext.EquipmentInstances
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.EquipmentInstanceId == instanceId && !x.IsDeleted);

        if (instance is null)
            return null;

        var statRolls = await dbContext.EquipmentInstanceStatRolls
            .AsNoTracking()
            .Where(x => x.EquipmentInstanceId == instanceId && !x.IsDeleted)
            .OrderBy(x => x.SortOrder)
            .ToListAsync();

        var enchantPools = await dbContext.EquipmentInstanceEnchantPools
            .AsNoTracking()
            .Where(x => x.EquipmentInstanceId == instanceId && !x.IsDeleted)
            .OrderBy(x => x.PoolIndex)
            .ToListAsync();

        return MapToResponse(instance, statRolls, enchantPools);
    }

    private static EquipmentInstanceResponse MapToResponse(
        EquipmentInstanceEntity instance,
        IEnumerable<EquipmentInstanceStatRollEntity> statRolls,
        IEnumerable<EquipmentInstanceEnchantPoolEntity> enchantPools) => new()
    {
        EquipmentInstanceId = instance.EquipmentInstanceId,
        AccountId = instance.AccountId,
        ItemId = instance.ItemId,
        EnhanceLevel = instance.EnhanceLevel,
        RuneMaxSlots = instance.RuneMaxSlots,
        TranscendenceRank = instance.TranscendenceRank,
        DurabilityMax = instance.DurabilityMax,
        DurabilityValue = instance.DurabilityValue,
        CreatedAt = instance.CreatedAt,
        UpdatedAt = instance.UpdatedAt,
        StatRolls = statRolls.Select(r => new EquipmentInstanceStatRollResponse
        {
            StatRollId = r.StatRollId,
            Status = r.Status,
            RandomMin = r.RandomMin,
            RandomMax = r.RandomMax,
            SortOrder = r.SortOrder,
        }).ToList(),
        EnchantPools = enchantPools.Select(p => new EquipmentInstanceEnchantPoolResponse
        {
            EnchantPoolId = p.EnchantPoolId,
            PoolIndex = p.PoolIndex,
            RecipeId = p.RecipeId,
            RequiredMaterialItemId = p.RequiredMaterialItemId,
            RequiredMaterialAmount = p.RequiredMaterialAmount,
            RequiredCurrency = p.RequiredCurrency,
        }).ToList(),
    };
}
