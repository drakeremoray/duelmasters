using Microsoft.AspNetCore.Mvc;
using DuelMastersApi.Services;
using DuelMastersApi.Models;
using DuelMastersApi.Data.Models;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IPlayerService _players;
    private readonly ITokenService _tokenService;
    public AuthController(IPlayerService players, ITokenService tokenService)
    {
        _players = players;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        if (await _players.GetByUsernameAsync(dto.Username) is not null)
            return Conflict(new { message = "Username already taken" });

        var player = new Player
        {
            Username = dto.Username,
            DisplayName = dto.DisplayName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        var created = await _players.CreateAsync(player);
        var token = _tokenService.CreateToken(created);
        return Ok(new { token, player = new { created.Id, created.Username, created.DisplayName } });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var player = await _players.GetByUsernameAsync(dto.Username);
        if (player is null) return Unauthorized();
        if (player.PasswordHash is null || !BCrypt.Net.BCrypt.Verify(dto.Password, player.PasswordHash)) return Unauthorized();

        var token = _tokenService.CreateToken(player);
        return Ok(new { token });
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var username = User.FindFirst(JwtRegisteredClaimNames.UniqueName)?.Value;
        if (string.IsNullOrEmpty(username)) return Unauthorized();
        var player = await _players.GetByUsernameAsync(username);
        if (player is null) return Unauthorized();
        return Ok(new { player = new { player.Id, player.Username, player.DisplayName } });
    }
}
