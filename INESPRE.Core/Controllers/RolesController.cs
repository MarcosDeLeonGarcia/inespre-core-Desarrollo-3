using INESPRE.Core.Models.Roles;
using INESPRE.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace INESPRE.Core.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly IRolesService _svc;
    public RolesController(IRolesService svc) => _svc = svc;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _svc.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
        => (await _svc.GetByIdAsync(id)) is { } r ? Ok(r) : NotFound();

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Role r)
    {
        var id = await _svc.CreateAsync(r);
        return CreatedAtAction(nameof(GetById), new { id }, new { roleId = id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Role r)
    {
        if (id != r.RoleId) return BadRequest("RoleId inconsistente");
        await _svc.UpdateAsync(r);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _svc.DeleteAsync(id);
        return NoContent();
    }
}
