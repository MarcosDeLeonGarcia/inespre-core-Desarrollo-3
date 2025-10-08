using INESPRE.Core.Models.Cash;
using INESPRE.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace INESPRE.Core.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CashController : ControllerBase
{
    private readonly ICashService _svc;
    public CashController(ICashService svc) => _svc = svc;

    [HttpPost("open")]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    public async Task<IActionResult> Open([FromBody] CashOpenRequest req)
    {
        var id = await _svc.OpenAsync(req);
        return CreatedAtAction(nameof(GetById), new { id }, new { cashSessionId = id });
    }

    [HttpPost("{id:int}/close")]
    public async Task<IActionResult> Close(int id, [FromBody] CashCloseRequest req)
        => Ok(await _svc.CloseAsync(id, req));

    [HttpGet]
    public async Task<IActionResult> Get(DateTime? from, DateTime? to, int? userId)
        => Ok(await _svc.SessionsAsync(from, to, userId));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var (header, totals) = await _svc.SessionDetailAsync(id);
        return header is null ? NotFound() : Ok(new { header, totals });
    }

    [HttpGet("{id:int}/movements")]
    public async Task<IActionResult> Movements(int id)
        => Ok(await _svc.MovementsAsync(id));

    [HttpPost("{id:int}/movement")]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    public async Task<IActionResult> AddMovement(int id, [FromBody] CashMovementRequest req)
    {
        var mvId = await _svc.AddMovementAsync(id, req);
        return CreatedAtAction(nameof(Movements), new { id }, new { movementId = mvId });
    }

    [HttpPost("{id:int}/pay-po")]
    public async Task<IActionResult> PayPO(int id, [FromQuery] int poId, [FromQuery] decimal amount,
                                           [FromQuery] string method = "EFECTIVO", [FromQuery] string? notes = null)
    {
        var paymentId = await _svc.PayPOAsync(id, poId, amount, method, notes);
        return Ok(new { paymentId });
    }
}
