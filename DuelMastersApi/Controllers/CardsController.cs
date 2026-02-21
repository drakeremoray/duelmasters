using Microsoft.AspNetCore.Mvc;
using DuelMastersApi.Data.Models;
using DuelMastersApi.Services;
using DuelMastersApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class CardsController : ControllerBase
{
    private readonly ICardService _svc;
    public CardsController(ICardService svc) { _svc = svc; }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await _svc.GetAllAsync());

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var item = await _svc.GetByIdAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CardDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var card = new Card { Name = dto.Name, CardType = dto.CardType, Cost = dto.Cost, Text = dto.Text };
        var created = await _svc.CreateAsync(card);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] CardDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var existing = await _svc.GetByIdAsync(id);
        if (existing is null) return NotFound();
        existing.Name = dto.Name;
        existing.CardType = dto.CardType;
        existing.Cost = dto.Cost;
        existing.Text = dto.Text;
        await _svc.UpdateAsync(existing);
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _svc.DeleteAsync(id);
        return NoContent();
    }
}
