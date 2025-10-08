using INESPRE.Core.Models.Products;
using INESPRE.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace INESPRE.Core.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductsService _svc;
    public ProductsController(IProductsService svc) => _svc = svc;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _svc.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
        => (await _svc.GetByIdAsync(id)) is { } p ? Ok(p) : NotFound();

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Product p)
    {
        var id = await _svc.CreateAsync(p);
        return CreatedAtAction(nameof(GetById), new { id }, new { productId = id });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Product p)
    {
        if (id != p.ProductId) return BadRequest("ProductId inconsistente");
        await _svc.UpdateAsync(p);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Disable(int id)
    {
        await _svc.DisableAsync(id);
        return NoContent();
    }
}
