using AstralRecordApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AstralRecordApi.Controllers;

/// <summary>アイテム API</summary>
[ApiController]
[Route("api/[controller]")]
public class ItemController(IItemRepository itemRepository) : ControllerBase
{
    /// <summary>アイテム一覧取得（最小項目）</summary>
    /// <response code="200">アイテム一覧取得成功</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        var items = itemRepository.GetAllSummaries();
        return Ok(items);
    }

    /// <summary>アイテム取得</summary>
    /// <param name="category">アイテムカテゴリ（material, consumable, equipment, currency, bundle）</param>
    /// <param name="itemId">アイテム ID</param>
    /// <response code="200">アイテム取得成功</response>
    /// <response code="400">未対応カテゴリを指定</response>
    /// <response code="404">指定カテゴリ内に対象アイテムが存在しない</response>
    [HttpGet("{category}/{itemId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetByCategoryAndId(string category, string itemId)
    {
        if (!itemRepository.IsSupportedCategory(category))
            return BadRequest(new { message = $"Unsupported category: {category}" });

        var item = itemRepository.GetByCategoryAndId(category, itemId);
        if (item is null)
            return NotFound();

        return Ok(item);
    }
}