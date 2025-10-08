using INESPRE.Core.Models.Sales;
using INESPRE.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace INESPRE.Core.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SalesController : ControllerBase
{
    private readonly ISalesService _svc;
    public SalesController(ISalesService svc) => _svc = svc;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _svc.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
        => (await _svc.GetByIdAsync(id)) is { } s ? Ok(s) : NotFound();

    [HttpGet("event/{eventId:int}")]
    public async Task<IActionResult> GetByEvent(int eventId)
        => Ok(await _svc.GetByEventAsync(eventId));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Sale s)
    {
        var id = await _svc.CreateAsync(s);
        return CreatedAtAction(nameof(GetById), new { id }, new { saleId = id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Sale s)
    {
        if (id != s.SaleId) return BadRequest("SaleId inconsistente");
        await _svc.UpdateAsync(s);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _svc.DeleteAsync(id);
        return NoContent();
    }
}
