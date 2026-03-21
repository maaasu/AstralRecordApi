using AstralRecordApi.Models;
using AstralRecordApi.Options;
using Microsoft.Extensions.Options;
using System.Text;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace AstralRecordApi.Repositories;

public class BuffRepository : IBuffRepository
{
    private static readonly StringComparer KeyComparer = StringComparer.OrdinalIgnoreCase;

    private readonly IReadOnlyDictionary<string, BuffResponse> _buffs;

    public BuffRepository(IOptions<FileDatabaseOptions> options)
    {
        var rootPath = options.Value.RootPath;
        if (string.IsNullOrWhiteSpace(rootPath))
            throw new InvalidOperationException("FileDatabase:RootPath is not configured.");

        _buffs = LoadBuffs(rootPath);
    }

    public BuffResponse? GetById(string buffId)
        => _buffs.TryGetValue(buffId, out var buff) ? buff : null;

    private static IReadOnlyDictionary<string, BuffResponse> LoadBuffs(string rootPath)
    {
        var buffRootPath = Path.Combine(rootPath, "70.shared.buff");
        if (!Directory.Exists(buffRootPath))
            throw new DirectoryNotFoundException($"Buff directory was not found: {buffRootPath}");

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();

        var buffs = new Dictionary<string, BuffResponse>(KeyComparer);

        foreach (var filePath in Directory.EnumerateFiles(buffRootPath, "*.yml", SearchOption.TopDirectoryOnly))
        {
            var yaml = ReadYamlWithNormalizedAmpersandScalars(filePath);
            var yamlBuff = deserializer.Deserialize<BuffYamlDocument>(yaml)
                ?? throw new InvalidOperationException($"Failed to deserialize buff YAML: {filePath}");

            var buff = yamlBuff.ToResponse(filePath);
            if (!buffs.TryAdd(buff.Id, buff))
                throw new InvalidOperationException($"Duplicate buff id '{buff.Id}'.");
        }

        return buffs;
    }

    private static string ReadYamlWithNormalizedAmpersandScalars(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        var builder = new StringBuilder();

        foreach (var line in lines)
        {
            builder.AppendLine(NormalizeAmpersandScalar(line));
        }

        return builder.ToString();
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

    private sealed class BuffYamlDocument
    {
        public int? SchemaVersion { get; init; }

        public string? Id { get; init; }

        public string? Type { get; init; }

        public string? Name { get; init; }

        public string? Icon { get; init; }

        public List<string>? Lore { get; init; }

        public long? DurationTicks { get; init; }

        public bool? IsDebuff { get; init; }

        public List<BuffModifierYamlDocument>? Modifiers { get; init; }

        public BuffResponse ToResponse(string filePath)
        {
            if (SchemaVersion is null)
                throw new InvalidOperationException($"schemaVersion is required: {filePath}");
            if (string.IsNullOrWhiteSpace(Id))
                throw new InvalidOperationException($"id is required: {filePath}");
            if (string.IsNullOrWhiteSpace(Type))
                throw new InvalidOperationException($"type is required: {filePath}");
            if (string.IsNullOrWhiteSpace(Name))
                throw new InvalidOperationException($"name is required: {filePath}");
            if (DurationTicks is null)
                throw new InvalidOperationException($"durationTicks is required: {filePath}");
            if (Modifiers is null || Modifiers.Count == 0)
                throw new InvalidOperationException($"modifiers is required: {filePath}");

            return new BuffResponse
            {
                SchemaVersion = SchemaVersion.Value,
                Id = Id,
                Type = Type,
                Name = Name,
                Icon = Icon,
                Lore = Lore ?? [],
                DurationTicks = DurationTicks.Value,
                IsDebuff = IsDebuff ?? false,
                Modifiers = Modifiers.Select(modifier => modifier.ToResponse(filePath)).ToArray()
            };
        }
    }

    private sealed class BuffModifierYamlDocument
    {
        public string? Status { get; init; }

        public string? Type { get; init; }

        public double? Value { get; init; }

        public BuffModifierResponse ToResponse(string filePath)
        {
            if (string.IsNullOrWhiteSpace(Status))
                throw new InvalidOperationException($"modifiers[].status is required: {filePath}");
            if (string.IsNullOrWhiteSpace(Type))
                throw new InvalidOperationException($"modifiers[].type is required: {filePath}");
            if (Value is null)
                throw new InvalidOperationException($"modifiers[].value is required: {filePath}");

            return new BuffModifierResponse
            {
                Status = Status,
                Type = Type,
                Value = Value.Value
            };
        }
    }
}