namespace AstralRecordApi.Utilities;

/// <summary>
/// "min~max" 形式の範囲文字列を解決するユーティリティ。
/// </summary>
public static class RangeValueResolver
{
    /// <summary>
    /// "min~max" 形式の文字列をランダムに解決して整数値を返す。
    /// 固定値の場合はそのまま返す。
    /// </summary>
    public static int ResolveInt(string value)
    {
        if (TryParseIntRange(value, out var min, out var max))
            return Random.Shared.Next(min, max + 1);

        return int.Parse(value);
    }

    /// <summary>
    /// "min~max" 形式かどうかを判定し、min / max を返す。
    /// </summary>
    public static bool TryParseIntRange(string value, out int min, out int max)
    {
        var tildeIndex = value.IndexOf('~');
        if (tildeIndex >= 0)
        {
            min = int.Parse(value[..tildeIndex]);
            max = int.Parse(value[(tildeIndex + 1)..]);
            return true;
        }

        min = max = 0;
        return false;
    }

    /// <summary>
    /// "min~max" 形式の文字列を (min, max) タプルに分解して返す。
    /// 範囲でない場合は (value, value) を返す。
    /// </summary>
    public static (string Min, string Max) SplitRange(string value)
    {
        var tildeIndex = value.IndexOf('~');
        return tildeIndex >= 0
            ? (value[..tildeIndex], value[(tildeIndex + 1)..])
            : (value, value);
    }
}
