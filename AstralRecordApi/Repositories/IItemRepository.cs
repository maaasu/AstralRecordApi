using AstralRecordApi.Models;

namespace AstralRecordApi.Repositories;

public interface IItemRepository
{
    bool IsSupportedCategory(string category);

    ItemResponse? GetByCategoryAndId(string category, string itemId);
}