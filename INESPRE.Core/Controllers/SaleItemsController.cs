using INESPRE.Core.Models.Sales;
using INESPRE.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace INESPRE.Core.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SaleItemsController : ControllerBase
{
    private readonly ISaleItemsService _svc;
    public SaleItemsController(ISaleItemsService svc) => _svc = svc;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _svc.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
        => (await _svc.GetByIdAsync(id)) is { } si ? Ok(si) : NotFound();

    [HttpGet("sale/{saleId:int}")]
    public async Task<IActionResult> GetBySale(int saleId)
        => Ok(await _svc.GetBySaleAsync(saleId));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SaleItem si)
    {
        var id = await _svc.CreateAsync(si);
        return CreatedAtAction(nameof(GetById), new { id }, new { saleItemId = id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] SaleItem si)
    {
        if (id != si.SaleItemId) return BadRequest("SaleItemId inconsistente");
        await _svc.UpdateAsync(si);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _svc.DeleteAsync(id);
        return NoContent();
    }
}
