using AstralRecordApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AstralRecordApi.Controllers;

/// <summary>バフ API</summary>
[ApiController]
[Route("api/[controller]")]
public class BuffController(IBuffRepository buffRepository) : ControllerBase
{
    /// <summary>バフ取得</summary>
    /// <param name="buffId">バフ ID</param>
    /// <response code="200">バフ取得成功</response>
    /// <response code="404">指定 buff ID が存在しない</response>
    [HttpGet("{buffId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(string buffId)
    {
        var buff = buffRepository.GetById(buffId);
        if (buff is null)
            return NotFound();

        return Ok(buff);
    }
}