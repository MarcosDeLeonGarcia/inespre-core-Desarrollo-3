using INESPRE.Core.Models.Producers;
using INESPRE.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace INESPRE.Core.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProducersController : ControllerBase
{
    private readonly IProducersService _svc;
    public ProducersController(IProducersService svc) => _svc = svc;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _svc.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
        => (await _svc.GetByIdAsync(id)) is { } p ? Ok(p) : NotFound();

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Producer p)
    {
        var id = await _svc.CreateAsync(p);
        return CreatedAtAction(nameof(GetById), new { id }, new { producerId = id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Producer p)
    {
        if (id != p.ProducerId) return BadRequest("ProducerId inconsistente");
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
