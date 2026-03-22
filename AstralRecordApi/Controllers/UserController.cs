using AstralRecordApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AstralRecordApi.Controllers;

/// <summary>ユーザー API</summary>
[ApiController]
[Route("api/[controller]")]
public class UserController(IUserRepository userRepository) : ControllerBase
{
    /// <summary>ユーザー情報取得</summary>
    /// <param name="uuid">Minecraft プレイヤーの UUID</param>
    /// <response code="200">ユーザー取得成功</response>
    /// <response code="404">指定 UUID のユーザーが存在しない、または論理削除済み</response>
    [HttpGet("{uuid:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByUuid(Guid uuid)
    {
        var user = await userRepository.GetByUuidAsync(uuid);
        if (user is null)
            return NotFound();

        return Ok(user);
    }
}
