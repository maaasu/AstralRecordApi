namespace AstralRecordApi.Models;

public class EquipmentInstanceResponse
{
    public Guid EquipmentInstanceId { get; init; }
    public Guid AccountId { get; init; }
    public string ItemId { get; init; } = string.Empty;
    public int EnhanceLevel { get; init; }
    public int RuneMaxSlots { get; init; }
    public int TranscendenceRank { get; init; }
    public int? DurabilityMax { get; init; }
    public int? DurabilityValue { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public IReadOnlyList<EquipmentInstanceStatRollResponse> StatRolls { get; init; } = [];
    public IReadOnlyList<EquipmentInstanceEnchantPoolResponse> EnchantPools { get; init; } = [];
}

public class EquipmentInstanceStatRollResponse
{
    public Guid StatRollId { get; init; }
    public string Status { get; init; } = string.Empty;
    public string Min { get; init; } = string.Empty;
    public string Max { get; init; } = string.Empty;
    public int SortOrder { get; init; }
}

public class EquipmentInstanceEnchantPoolResponse
{
    public int PoolIndex { get; init; }
    public string? RecipeId { get; init; }
    public string? RequiredMaterialItemId { get; init; }
    public int RequiredMaterialAmount { get; init; }
    public int RequiredCurrency { get; init; }
}
