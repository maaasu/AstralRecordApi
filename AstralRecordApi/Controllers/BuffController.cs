using AstralRecordApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AstralRecordApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BuffController(IBuffRepository buffRepository) : ControllerBase
{
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