using AstralRecordApi.Models;
using AstralRecordApi.Options;
using Microsoft.Extensions.Options;
using System.Text;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace AstralRecordApi.Repositories;

public class ItemRepository : IItemRepository
{
    private static readonly StringComparer KeyComparer = StringComparer.OrdinalIgnoreCase;
    private static readonly HashSet<string> SupportedCategories = new(["material", "consumable"], KeyComparer);

    private readonly IReadOnlyDictionary<string, IReadOnlyDictionary<string, ItemResponse>> _itemsByCategory;

    public ItemRepository(IOptions<FileDatabaseOptions> options)
    {
        var rootPath = options.Value.RootPath;
        if (string.IsNullOrWhiteSpace(rootPath))
            throw new InvalidOperationException("FileDatabase:RootPath is not configured.");

        _itemsByCategory = LoadItems(rootPath);
    }

    public bool IsSupportedCategory(string category)
        => SupportedCategories.Contains(category);

    public ItemResponse? GetByCategoryAndId(string category, string itemId)
    {
        if (!_itemsByCategory.TryGetValue(category, out var items))
            return null;

        return items.TryGetValue(itemId, out var item) ? item : null;
    }

    private static IReadOnlyDictionary<string, IReadOnlyDictionary<string, ItemResponse>> LoadItems(string rootPath)
    {
        var itemRootPath = Path.Combine(rootPath, "10.features.item");
        if (!Directory.Exists(itemRootPath))
            throw new DirectoryNotFoundException($"Item directory was not found: {itemRootPath}");

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();

        var itemsByCategory = new Dictionary<string, IReadOnlyDictionary<string, ItemResponse>>(KeyComparer);

        foreach (var category in SupportedCategories)
        {
            var categoryPath = Path.Combine(itemRootPath, category);
            if (!Directory.Exists(categoryPath))
                throw new DirectoryNotFoundException($"Category directory was not found: {categoryPath}");

            var items = new Dictionary<string, ItemResponse>(KeyComparer);

            foreach (var filePath in Directory.EnumerateFiles(categoryPath, "*.yml", SearchOption.TopDirectoryOnly))
            {
                var yaml = ReadYamlWithNormalizedAmpersandScalars(filePath);
                ItemYamlDocument yamlItem;
                try
                {
                    yamlItem = deserializer.Deserialize<ItemYamlDocument>(yaml)
                        ?? throw new InvalidOperationException($"Failed to deserialize item YAML (null result): {filePath}");
                }
                catch (Exception ex) when (ex is not InvalidOperationException)
                {
                    throw new InvalidOperationException($"Failed to deserialize item YAML: {filePath}", ex);
                }

                var item = yamlItem.ToResponse(filePath, category);
                if (!items.TryAdd(item.Id, item))
                    throw new InvalidOperationException($"Duplicate item id '{item.Id}' in category '{category}'.");
            }

            itemsByCategory[category] = items;
        }

        return itemsByCategory;
    }

    private static string ReadYamlWithNormalizedAmpersandScalars(string filePath)
    {
        var rawLines = File.ReadAllLines(filePath);
        var lines = NormalizeRefObjects(rawLines);
        var builder = new StringBuilder();

        foreach (var line in lines)
        {
            builder.AppendLine(NormalizeAmpersandScalar(line));
        }

        return builder.ToString();
    }

    /// <summary>
    /// YAML の参照オブジェクト形式（ref: type:id）をフラットなスカラーに正規化する。
    /// 例:
    ///   buffId:          buffId: cure_poison
    ///     ref: buff:cure_poison
    /// </summary>
    private static string[] NormalizeRefObjects(string[] lines)
    {
        var result = new List<string>(lines.Length);
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            var trimmed = line.TrimStart();

            // 値なしのキー行（例: "      buffId:"）を検出
            if (trimmed.EndsWith(':') && !trimmed.StartsWith('-') && !trimmed.StartsWith('#'))
            {
                // 次の行が ref パターン（例: "        ref: buff:cure_poison"）か確認
                if (i + 1 < lines.Length)
                {
                    var nextTrimmed = lines[i + 1].TrimStart();
                    if (nextTrimmed.StartsWith("ref: ", StringComparison.OrdinalIgnoreCase))
                    {
                        var refValue = nextTrimmed["ref: ".Length..].Trim();
                        // "type:id" 形式から id 部分を抽出（例: buff:cure_poison → cure_poison）
                        var colonIdx = refValue.LastIndexOf(':');
                        var id = colonIdx >= 0 ? refValue[(colonIdx + 1)..].Trim() : refValue;

                        var indent = line[..(line.Length - trimmed.Length)];
                        var key = trimmed.TrimEnd(':');
                        result.Add($"{indent}{key}: {id}");
                        i++; // ref 行をスキップ
                        continue;
                    }
                }
            }

            result.Add(line);
        }
        return result.ToArray();
    }

    private static string NormalizeAmpersandScalar(string line)
    {
        var commentIndex = line.IndexOf(" #", StringComparison.Ordinal);
        var content = commentIndex >= 0 ? line[..commentIndex] : line;
        var comment = commentIndex >= 0 ? line[commentIndex..] : string.Empty;

        var colonIndex = content.IndexOf(':');
        if (colonIndex >= 0)
        {
            var valuePart = content[(colonIndex + 1)..];
            var trimmedValue = valuePart.TrimStart();
            if (trimmedValue.StartsWith('&') && !trimmedValue.StartsWith("\"") && !trimmedValue.StartsWith("'"))
            {
                var leadingWhitespaceLength = valuePart.Length - trimmedValue.Length;
                var leadingWhitespace = valuePart[..leadingWhitespaceLength];
                var escaped = trimmedValue.Replace("\\", "\\\\").Replace("\"", "\\\"");
                return $"{content[..(colonIndex + 1)]}{leadingWhitespace}\"{escaped}\"{comment}";
            }
        }

        var trimmedContent = content.TrimStart();
        if (trimmedContent.StartsWith("- "))
        {
            var itemValue = trimmedContent[2..].TrimStart();
            if (itemValue.StartsWith('&') && !itemValue.StartsWith("\"") && !itemValue.StartsWith("'"))
            {
                var indentLength = content.Length - trimmedContent.Length;
                var indent = content[..indentLength];
                var afterDash = trimmedContent[2..];
                var leadingWhitespaceLength = afterDash.Length - itemValue.Length;
                var leadingWhitespace = afterDash[..leadingWhitespaceLength];
                var escaped = itemValue.Replace("\\", "\\\\").Replace("\"", "\\\"");
                return $"{indent}- {leadingWhitespace}\"{escaped}\"{comment}";
            }
        }

        return line;
    }

    private sealed class ItemYamlDocument
    {
        public int? SchemaVersion { get; init; }

        public string? Id { get; init; }

        public string? Category { get; init; }

        public string? Name { get; init; }

        public string? Icon { get; init; }

        public string? Rarity { get; init; }

        public int? SaleValue { get; init; }

        public int? CustomModelData { get; init; }

        public List<string>? Lore { get; init; }

        public bool? UnTradeable { get; init; }

        public bool? UnSellable { get; init; }

        public ConsumableYamlDocument? Consumable { get; init; }

        public ItemResponse ToResponse(string filePath, string expectedCategory)
        {
            if (SchemaVersion is null)
                throw new InvalidOperationException($"schemaVersion is required: {filePath}");
            if (string.IsNullOrWhiteSpace(Id))
                throw new InvalidOperationException($"id is required: {filePath}");
            if (!string.IsNullOrWhiteSpace(Category)
                && !string.Equals(Category, expectedCategory, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException($"category mismatch in {filePath}. expected '{expectedCategory}', actual '{Category}'.");
            if (string.IsNullOrWhiteSpace(Name))
                throw new InvalidOperationException($"name is required: {filePath}");
            if (string.IsNullOrWhiteSpace(Icon))
                throw new InvalidOperationException($"icon is required: {filePath}");
            if (string.IsNullOrWhiteSpace(Rarity))
                throw new InvalidOperationException($"rarity is required: {filePath}");

            return new ItemResponse
            {
                SchemaVersion = SchemaVersion.Value,
                Id = Id,
                Category = Category ?? expectedCategory,
                Name = Name,
                Icon = Icon,
                Rarity = Rarity,
                SaleValue = SaleValue ?? 0,
                CustomModelData = CustomModelData,
                Lore = Lore ?? [],
                UnTradeable = UnTradeable ?? false,
                UnSellable = UnSellable ?? false,
                Consumable = Consumable?.ToResponse(filePath, expectedCategory)
            };
        }
    }

    private sealed class ConsumableYamlDocument
    {
        public ConsumableOnUseYamlDocument? OnUse { get; init; }

        public List<ConsumableEffectYamlDocument>? Effects { get; init; }

        public ItemConsumableResponse ToResponse(string filePath, string expectedCategory)
        {
            if (!string.Equals(expectedCategory, "consumable", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException($"consumable section is only valid for category 'consumable': {filePath}");

            if (Effects is null || Effects.Count == 0)
                throw new InvalidOperationException($"consumable.effects is required: {filePath}");

            return new ItemConsumableResponse
            {
                OnUse = OnUse?.ToResponse(),
                Effects = Effects.Select(effect => effect.ToResponse(filePath)).ToArray()
            };
        }
    }

    private sealed class ConsumableOnUseYamlDocument
    {
        public string? Sound { get; init; }

        public string? Effect { get; init; }

        public int? Amount { get; init; }

        public ItemConsumableOnUseResponse ToResponse()
        {
            return new ItemConsumableOnUseResponse
            {
                Sound = Sound,
                Effect = Effect,
                Amount = Amount ?? 1
            };
        }
    }

    private sealed class ConsumableEffectYamlDocument
    {
        public string? Type { get; init; }

        public double? Rate { get; init; }

        public double? Value { get; init; }

        public string? Status { get; init; }

        public bool? IsPercent { get; init; }

        public string? BuffId { get; init; }

        public ItemConsumableEffectResponse ToResponse(string filePath)
        {
            if (string.IsNullOrWhiteSpace(Type))
                throw new InvalidOperationException($"consumable.effects[].type is required: {filePath}");

            return new ItemConsumableEffectResponse
            {
                Type = Type,
                Rate = Rate ?? 100,
                Value = Value,
                Status = Status,
                IsPercent = IsPercent ?? false,
                BuffId = BuffId
            };
        }
    }
}