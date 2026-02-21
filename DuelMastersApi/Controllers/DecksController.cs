using Microsoft.AspNetCore.Mvc;
using DuelMastersApi.Data.Models;
using DuelMastersApi.Services;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DecksController : ControllerBase
{
    private readonly IDeckService _svc;
    public DecksController(IDeckService svc) { _svc = svc; }

    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await _svc.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var deck = await _svc.GetByIdAsync(id);
        return deck is null ? NotFound() : Ok(deck);
    }

    [HttpPost]
    public async Task<IActionResult> Post(Deck deck)
    {
        var created = await _svc.CreateAsync(deck);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, Deck deck)
    {
        if (id != deck.Id) return BadRequest();
        await _svc.UpdateAsync(deck);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _svc.DeleteAsync(id);
        return NoContent();
    }
}
