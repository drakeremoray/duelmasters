using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DuelMastersApi.Data.Models;
using DuelMastersApi.Services;
using Microsoft.AspNetCore.Authorization;
using DuelMastersApi.Models;
using System.IdentityModel.Tokens.Jwt;
using DuelMastersApi.Services;
using System.Text.Json.Nodes;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MatchesController : ControllerBase
{
    private readonly IMatchService _svc;
    private readonly IGameStateService _gameState;
    public MatchesController(IMatchService svc, IGameStateService gameState) { _svc = svc; _gameState = gameState; }

    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await _svc.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var m = await _svc.GetByIdAsync(id);
        return m is null ? NotFound() : Ok(m);
    }

    [HttpPost]
    public async Task<IActionResult> Post(Match match)
    {
        var created = await _svc.CreateAsync(match);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, Match match)
    {
        if (id != match.Id) return BadRequest();
        await _svc.UpdateAsync(match);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _svc.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{id}/actions")]
    public async Task<IActionResult> PostAction(int id, MatchActionDto action)
    {
        // determine player id from token
        var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (string.IsNullOrEmpty(sub) || !int.TryParse(sub, out var playerId)) return Unauthorized();

        // authorization: ensure player is a participant in the latest match state
        var latest = await _gameState.GetLatestByMatchIdAsync(id);
        if (latest is null) return NotFound(new { message = "Match state not found" });

        try
        {
            var node = JsonNode.Parse(latest.State);
            var players = node?["players"] as JsonArray;
            var isParticipant = players != null && players.Any(p => p != null && p["id"] != null && p["id"]!.GetValue<int>() == playerId);
            if (!isParticipant) return Forbid();
        }
        catch
        {
            // malformed state: deny
            return Forbid();
        }

        var result = await _svc.ApplyActionAsync(id, action, playerId);
        return Ok(result);
    }

    [HttpPost("{id}/join")]
    public async Task<IActionResult> JoinMatch(int id, [FromQuery] bool spectator = false)
    {
        var sub = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
        if (string.IsNullOrEmpty(sub) || !int.TryParse(sub, out var playerId)) return Unauthorized();

        // record participant
        var participant = new DuelMastersApi.Data.Models.MatchParticipant { MatchId = id, PlayerId = playerId, IsSpectator = spectator };
        // use db context via service locator pattern (small controller change to keep scoped dependencies minimal)
        var db = HttpContext.RequestServices.GetService(typeof(DuelMastersApi.Data.DuelMastersContext)) as DuelMastersApi.Data.DuelMastersContext;
        if (db == null) return StatusCode(500);
        db.MatchParticipants.Add(participant);
        await db.SaveChangesAsync();

        return Ok(new { status = "joined", participantId = participant.Id });
    }

    [HttpPost("{id}/leave")]
    public async Task<IActionResult> LeaveMatch(int id)
    {
        var sub = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
        if (string.IsNullOrEmpty(sub) || !int.TryParse(sub, out var playerId)) return Unauthorized();

        var db = HttpContext.RequestServices.GetService(typeof(DuelMastersApi.Data.DuelMastersContext)) as DuelMastersApi.Data.DuelMastersContext;
        if (db == null) return StatusCode(500);

        var part = await db.MatchParticipants.FirstOrDefaultAsync(p => p.MatchId == id && p.PlayerId == playerId);
        if (part != null)
        {
            db.MatchParticipants.Remove(part);
            await db.SaveChangesAsync();
        }

        return Ok(new { status = "left" });
    }
}
