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

    public IReadOnlyList<string> Lore { get; init; } = [];

    public bool UnTradeable { get; init; }

    public bool UnSellable { get; init; }

    public ItemConsumableResponse? Consumable { get; init; }
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