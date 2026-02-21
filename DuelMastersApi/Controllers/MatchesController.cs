using Microsoft.AspNetCore.Mvc;
using DuelMastersApi.Data.Models;
using DuelMastersApi.Services;
using Microsoft.AspNetCore.Authorization;
using DuelMastersApi.Models;
using System.IdentityModel.Tokens.Jwt;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MatchesController : ControllerBase
{
    private readonly IMatchService _svc;
    public MatchesController(IMatchService svc) { _svc = svc; }

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

        var result = await _svc.ApplyActionAsync(id, action, playerId);
        return Ok(result);
    }
}
