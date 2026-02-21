using Microsoft.AspNetCore.Mvc;
using DuelMastersApi.Data.Models;
using DuelMastersApi.Services;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DeckCardsController : ControllerBase
{
    private readonly IDeckCardService _svc;
    public DeckCardsController(IDeckCardService svc) { _svc = svc; }

    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await _svc.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var item = await _svc.GetByIdAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Post(DeckCard dc)
    {
        var created = await _svc.CreateAsync(dc);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _svc.DeleteAsync(id);
        return NoContent();
    }
}
