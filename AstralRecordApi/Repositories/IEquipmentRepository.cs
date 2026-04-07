using AstralRecordApi.Models;

namespace AstralRecordApi.Repositories;

public interface IEquipmentRepository
{
    /// <summary>
    /// マスタデータをもとに装備インスタンスを生成して DB に保存する。
    /// <para><paramref name="request"/> の <c>EquipmentId</c> が存在しない、または equipment カテゴリでない場合は <c>null</c> を返す。</para>
    /// </summary>
    Task<EquipmentInstanceResponse?> CreateAsync(EquipmentCreateRequest request);

    /// <summary>
    /// 指定した装備インスタンス ID のデータを取得する。論理削除済みは返さない。
    /// </summary>
    Task<EquipmentInstanceResponse?> GetByInstanceIdAsync(Guid instanceId);
}
