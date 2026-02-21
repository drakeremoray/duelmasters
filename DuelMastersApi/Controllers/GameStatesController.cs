using Microsoft.AspNetCore.Mvc;
using DuelMastersApi.Data.Models;
using DuelMastersApi.Services;
using DuelMastersApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GameStatesController : ControllerBase
{
    private readonly IGameStateService _svc;
    public GameStatesController(IGameStateService svc) { _svc = svc; }

    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await _svc.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var gs = await _svc.GetByIdAsync(id);
        return gs is null ? NotFound() : Ok(gs);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] GameStateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var gs = new GameState { MatchId = dto.MatchId, Turn = dto.Turn, State = dto.State };
        var created = await _svc.CreateAsync(gs);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }
}
