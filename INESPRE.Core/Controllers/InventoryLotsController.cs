using INESPRE.Core.Models.Inventory;
using INESPRE.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace INESPRE.Core.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryLotsController : ControllerBase
{
    private readonly IInventoryLotsService _svc;
    public InventoryLotsController(IInventoryLotsService svc) => _svc = svc;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _svc.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
        => (await _svc.GetByIdAsync(id)) is { } l ? Ok(l) : NotFound();

    [HttpGet("product/{productId:int}")]
    public async Task<IActionResult> GetByProduct(int productId)
        => Ok(await _svc.GetByProductAsync(productId));

    [HttpGet("event/{eventId:int}")]
    public async Task<IActionResult> GetByEvent(int eventId)
        => Ok(await _svc.GetByEventAsync(eventId));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] InventoryLot l)
    {
        var id = await _svc.CreateAsync(l);
        return CreatedAtAction(nameof(GetById), new { id }, new { lotId = id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] InventoryLot l)
    {
        if (id != l.LotId) return BadRequest("LotId inconsistente");
        await _svc.UpdateAsync(l);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _svc.DeleteAsync(id);
        return NoContent();
    }
}
