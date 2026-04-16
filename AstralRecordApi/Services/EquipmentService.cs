using AstralRecordApi.Data.Entities;
using AstralRecordApi.Models;
using AstralRecordApi.Repositories;
using AstralRecordApi.Utilities;

namespace AstralRecordApi.Services;

public class EquipmentService(IItemRepository itemRepository, IEquipmentRepository equipmentRepository) : IEquipmentService
{
    public async Task<EquipmentInstanceResponse?> CreateAsync(EquipmentCreateRequest request)
    {
        // マスタデータ検証
        var item = itemRepository.GetById(request.EquipmentId);
        if (item?.Equipment is null)
            return null;

        var equipment = item.Equipment;
        var now = DateTime.UtcNow;
        var instanceId = Guid.NewGuid();

        // ルーンスロット数の解決（範囲指定の場合はランダムに確定）
        var runeMaxSlots = equipment.Rune is not null
            ? RangeValueResolver.ResolveInt(equipment.Rune.MaxSlots)
            : 0;

        // 装備インスタンス本体の構築
        var instance = new EquipmentInstanceEntity
        {
            EquipmentInstanceId = instanceId,
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

        // ステータス個体差ロールの構築（value.min / value.max に範囲候補を持つ stat のみ）
        var statRolls = BuildStatRolls(instanceId, equipment.Stats, request.CreatedBy, now);

        // エンチャントプールの構築（個体生成時点のプール構成）
        var enchantPools = BuildEnchantPools(instanceId, equipment, request.CreatedBy, now);

        // 永続化
        await equipmentRepository.AddAsync(instance, statRolls, enchantPools);

        return MapToResponse(instance, statRolls, enchantPools);
    }

    public async Task<EquipmentInstanceResponse?> GetByInstanceIdAsync(Guid instanceId)
    {
        var instance = await equipmentRepository.FindInstanceAsync(instanceId);
        if (instance is null)
            return null;

        IReadOnlyList<EquipmentInstanceStatRollEntity>? statRolls = await equipmentRepository.FindStatRollsAsync(instanceId);
        IReadOnlyList<EquipmentInstanceEnchantPoolEntity>? enchantPools = await equipmentRepository.FindEnchantPoolsAsync(instanceId);

        return MapToResponse(instance, statRolls, enchantPools);
    }

    private static IReadOnlyList<EquipmentInstanceStatRollEntity> BuildStatRolls(
        Guid instanceId,
        IReadOnlyList<ItemEquipmentStatResponse> stats,
        Guid createdBy,
        DateTime now)
    {
        var result = new List<EquipmentInstanceStatRollEntity>();
        var sortOrder = 0;

        foreach (var stat in stats)
        {
            if (stat.Status is null || stat.Value is null)
                continue;

            var min = stat.Value.Min.Trim();
            var max = stat.Value.Max.Trim();

            if (string.IsNullOrWhiteSpace(min) || string.IsNullOrWhiteSpace(max))
                continue;

            if (!min.Contains('~') && !max.Contains('~'))
                continue;

            result.Add(new EquipmentInstanceStatRollEntity
            {
                StatRollId = Guid.NewGuid(),
                EquipmentInstanceId = instanceId,
                Status = stat.Status,
                RandomMin = min,
                RandomMax = max,
                SortOrder = sortOrder++,
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = createdBy,
                UpdatedBy = createdBy,
                IsDeleted = false,
            });
        }

        return result;
    }

    private static IReadOnlyList<EquipmentInstanceEnchantPoolEntity> BuildEnchantPools(
        Guid instanceId,
        ItemEquipmentResponse equipment,
        Guid createdBy,
        DateTime now)
    {
        if (equipment.Enchant is null)
            return [];

        var result = new List<EquipmentInstanceEnchantPoolEntity>(equipment.Enchant.Pools.Count);

        for (var i = 0; i < equipment.Enchant.Pools.Count; i++)
        {
            var pool = equipment.Enchant.Pools[i];
            result.Add(new EquipmentInstanceEnchantPoolEntity
            {
                EnchantPoolId = Guid.NewGuid(),
                EquipmentInstanceId = instanceId,
                PoolIndex = i,
                RecipeId = pool.RecipeId,
                RequiredMaterialItemId = pool.RequiredMaterial?.ItemId,
                RequiredMaterialAmount = pool.RequiredMaterial?.Amount ?? 1,
                RequiredCurrency = pool.RequiredCurrency,
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = createdBy,
                UpdatedBy = createdBy,
                IsDeleted = false,
            });
        }

        return result;
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
            Min = r.RandomMin,
            Max = r.RandomMax,
            SortOrder = r.SortOrder,
        }).ToList(),
        EnchantPools = enchantPools.Select(p => new EquipmentInstanceEnchantPoolResponse
        {
            PoolIndex = p.PoolIndex,
            RecipeId = p.RecipeId,
            RequiredMaterialItemId = p.RequiredMaterialItemId,
            RequiredMaterialAmount = p.RequiredMaterialAmount,
            RequiredCurrency = p.RequiredCurrency,
        }).ToList(),
    };
}
