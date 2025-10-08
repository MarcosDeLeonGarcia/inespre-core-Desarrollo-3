using INESPRE.Core.Models.Purchasing;
using INESPRE.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace INESPRE.Core.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PurchaseOrderItemsController : ControllerBase
{
    private readonly IPurchaseOrderItemsService _svc;
    public PurchaseOrderItemsController(IPurchaseOrderItemsService svc) => _svc = svc;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _svc.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
        => (await _svc.GetByIdAsync(id)) is { } item ? Ok(item) : NotFound();

    [HttpGet("po/{poId:int}")]
    public async Task<IActionResult> GetByPO(int poId)
        => Ok(await _svc.GetByPOAsync(poId));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PurchaseOrderItem item)
    {
        var id = await _svc.CreateAsync(item);
        return CreatedAtAction(nameof(GetById), new { id }, new { poItemId = id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] PurchaseOrderItem item)
    {
        if (id != item.POItemId) return BadRequest("POItemId inconsistente");
        await _svc.UpdateAsync(item);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _svc.DeleteAsync(id);
        return NoContent();
    }
}
