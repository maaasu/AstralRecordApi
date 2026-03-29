namespace AstralRecordApi.Models;

public class ItemResponse
{
    public required int SchemaVersion { get; init; }

    public required string Id { get; init; }

    public required string Category { get; init; }

    public required string Name { get; init; }

    public required string Icon { get; init; }

    public required string Rarity { get; init; }

    public int SaleValue { get; init; }

    public int? CustomModelData { get; init; }

    public int MaxStack { get; init; } = 64;

    public IReadOnlyList<string> Lore { get; init; } = [];

    public bool UnTradeable { get; init; }

    public bool UnSellable { get; init; }

    public ItemConsumableResponse? Consumable { get; init; }

    public ItemEquipmentResponse? Equipment { get; init; }

    public ItemCurrencyResponse? Currency { get; init; }

    public ItemBundleResponse? Bundle { get; init; }
}

public class ItemConsumableResponse
{
    public ItemConsumableOnUseResponse? OnUse { get; init; }

    public IReadOnlyList<ItemConsumableEffectResponse> Effects { get; init; } = [];
}

public class ItemConsumableOnUseResponse
{
    public string? Sound { get; init; }

    public string? Effect { get; init; }

    public int Amount { get; init; } = 1;
}

public class ItemConsumableEffectResponse
{
    public required string Type { get; init; }

    public double Rate { get; init; } = 100;

    public double? Value { get; init; }

    public string? Status { get; init; }

    public bool IsPercent { get; init; }

    public string? BuffId { get; init; }
}

public class ItemEquipmentResponse
{
    public required string Slot { get; init; }

    public string HandType { get; init; } = "ONE";

    public int RequiredLevel { get; init; }

    public IReadOnlyList<string> RequiredClasses { get; init; } = [];

    public required IReadOnlyList<ItemEquipmentStatResponse> Stats { get; init; }

    public ItemEquipmentDurabilityResponse? Durability { get; init; }

    public ItemEquipmentOnUseResponse? OnUse { get; init; }

    public IReadOnlyList<string> Skills { get; init; } = [];
}

public class ItemEquipmentStatResponse
{
    public required string Status { get; init; }

    public required string Type { get; init; }

    public required string Value { get; init; }
}

public class ItemEquipmentDurabilityResponse
{
    public int? Max { get; init; }

    public int Consume { get; init; } = 1;
}

public class ItemEquipmentOnUseResponse
{
    public int? LeftClickCooldownTicks { get; init; }

    public string? LeftClickSkillId { get; init; }

    public int? RightClickCooldownTicks { get; init; }

    public string? RightClickSkillId { get; init; }
}

public class ItemCurrencyResponse
{
    public string? Type { get; init; }

    public string? Group { get; init; }

    public string? ExpiresAt { get; init; }
}

public class ItemBundleResponse
{
    public required string LootTableId { get; init; }

    public ItemBundleOnUseResponse? OnUse { get; init; }
}

public class ItemBundleOnUseResponse
{
    public string? Sound { get; init; }

    public string? Particle { get; init; }
}