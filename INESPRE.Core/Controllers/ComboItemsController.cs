using INESPRE.Core.Models.Products;
using INESPRE.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace INESPRE.Core.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ComboItemsController : ControllerBase
{
    private readonly IComboItemsService _svc;
    public ComboItemsController(IComboItemsService svc) => _svc = svc;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _svc.GetAllAsync());

    [HttpGet("combo/{comboProductId:int}")]
    public async Task<IActionResult> GetByCombo(int comboProductId)
        => Ok(await _svc.GetByComboAsync(comboProductId));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ComboItem c)
    {
        await _svc.CreateAsync(c);
        return Created("api/comboitems", c);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] ComboItem c)
    {
        await _svc.UpdateAsync(c);
        return NoContent();
    }

    [HttpDelete("{comboProductId:int}/{componentProductId:int}")]
    public async Task<IActionResult> Delete(int comboProductId, int componentProductId)
    {
        await _svc.DeleteAsync(comboProductId, componentProductId);
        return NoContent();
    }
}
