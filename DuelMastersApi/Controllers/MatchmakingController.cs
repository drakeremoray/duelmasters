using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using DuelMastersApi.Services;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MatchmakingController : ControllerBase
{
    private readonly IMatchmakingService _svc;
    public MatchmakingController(IMatchmakingService svc) { _svc = svc; }

    [HttpPost("join")]
    public async Task<IActionResult> Join()
    {
        var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (string.IsNullOrEmpty(sub) || !int.TryParse(sub, out var playerId)) return Unauthorized();

        var matchId = await _svc.JoinQueueAsync(playerId);
        if (matchId is null) return Ok(new { status = "queued" });
        return Ok(new { status = "matched", matchId });
    }
}
