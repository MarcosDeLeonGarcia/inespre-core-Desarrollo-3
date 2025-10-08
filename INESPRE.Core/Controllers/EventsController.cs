using INESPRE.Core.Models.Events;
using INESPRE.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace INESPRE.Core.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly IEventsService _svc;
    public EventsController(IEventsService svc) => _svc = svc;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _svc.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
        => (await _svc.GetByIdAsync(id)) is { } e ? Ok(e) : NotFound();

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EventCreateRequest request)
    {
        var id = await _svc.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id }, new { eventId = id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] EventUpdateRequest request)
    {
        if (id != request.EventId) return BadRequest("EventId inconsistente");
        await _svc.UpdateAsync(request);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _svc.DeleteAsync(id);
        return NoContent();
    }
}
