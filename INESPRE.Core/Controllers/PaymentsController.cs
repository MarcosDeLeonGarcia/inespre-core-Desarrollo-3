using INESPRE.Core.Models.Payments;
using INESPRE.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace INESPRE.Core.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentsService _svc;
    public PaymentsController(IPaymentsService svc) => _svc = svc;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _svc.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
        => (await _svc.GetByIdAsync(id)) is { } p ? Ok(p) : NotFound();

    [HttpGet("po/{poId:int}")]
    public async Task<IActionResult> GetByPO(int poId)
        => Ok(await _svc.GetByPOAsync(poId));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Payment p)
    {
        var id = await _svc.CreateAsync(p);
        return CreatedAtAction(nameof(GetById), new { id }, new { paymentId = id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Payment p)
    {
        if (id != p.PaymentId) return BadRequest("PaymentId inconsistente");
        await _svc.UpdateAsync(p);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _svc.DeleteAsync(id);
        return NoContent();
    }
}
