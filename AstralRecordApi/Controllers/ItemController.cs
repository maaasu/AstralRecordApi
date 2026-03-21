using AstralRecordApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AstralRecordApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemController(IItemRepository itemRepository) : ControllerBase
{
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