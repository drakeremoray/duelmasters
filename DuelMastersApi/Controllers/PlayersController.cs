using Microsoft.AspNetCore.Mvc;
using DuelMastersApi.Data.Models;
using Microsoft.AspNetCore.Authorization;
using DuelMastersApi.Services;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PlayersController : ControllerBase
{
    private readonly IPlayerService _svc;
    public PlayersController(IPlayerService svc) { _svc = svc; }

    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await _svc.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var p = await _svc.GetByIdAsync(id);
        return p is null ? NotFound() : Ok(p);
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Player player)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        player.PasswordHash = null;
        var created = await _svc.CreateAsync(player);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, new { created.Id, created.Username, created.DisplayName });
    }
}
