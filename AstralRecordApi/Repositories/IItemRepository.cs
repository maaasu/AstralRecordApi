using AstralRecordApi.Models;

namespace AstralRecordApi.Repositories;

public interface IItemRepository
{
    IReadOnlyList<ItemSummaryResponse> GetAllSummaries();

    bool IsSupportedCategory(string category);

    ItemResponse? GetByCategoryAndId(string category, string itemId);
}