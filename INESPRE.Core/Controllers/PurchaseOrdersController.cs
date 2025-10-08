using Microsoft.AspNetCore.Mvc;
using INESPRE.Core.Models.Purchasing;
using INESPRE.Core.Models.Common; // Enum PurchaseOrderStatus
using INESPRE.Core.Services;

namespace INESPRE.Core.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class PurchaseOrdersController : ControllerBase
    {
        private readonly IPurchaseOrdersService _service;
        public PurchaseOrdersController(IPurchaseOrdersService service) => _service = service;

        // ======== DTOs de Request ========
        public sealed class CreatePORequest
        {
            public int ProducerId { get; set; }
            public int CreatedBy { get; set; }
            public int? EventId { get; set; }
            public DateTime? ExpectedDate { get; set; }
            public string? Notes { get; set; }
            public decimal? Total { get; set; }
            public string? Status { get; set; } // "PENDIENTE" | "PAGADA" | "CERRADA" (opcional)
        }

        public sealed class UpdatePORequest
        {
            public int ProducerId { get; set; }
            public int? EventId { get; set; }
            public DateTime? ExpectedDate { get; set; }
            public string? Notes { get; set; }
            public string Status { get; set; } = "PENDIENTE"; // string en el request
            public decimal Total { get; set; }
        }

        // ======== Helper: string -> enum ========
        private static PurchaseOrderStatus ParseStatus(string? s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return PurchaseOrderStatus.PENDIENTE; // default
            if (Enum.TryParse<PurchaseOrderStatus>(s.Trim(), ignoreCase: true, out var st))
                return st;
            throw new ArgumentException($"Status inválido: {s}. Valores válidos: PENDIENTE, PAGADA, CERRADA.");
        }

        // ======== Endpoints ========

        // GET api/purchaseorders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PurchaseOrder>>> GetAll()
            => Ok(await _service.GetAllAsync());

        // GET api/purchaseorders/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<PurchaseOrder>> GetById(int id)
        {
            var po = await _service.GetByIdAsync(id);
            return po is null ? NotFound() : Ok(po);
        }

        // POST api/purchaseorders
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreatePORequest req)
        {
            if (req.ProducerId <= 0) return BadRequest("ProducerId inválido.");

            var po = new PurchaseOrder
            {
                ProducerId   = req.ProducerId,
                CreatedBy    = req.CreatedBy,
                EventId      = req.EventId,
                ExpectedDate = req.ExpectedDate,
                Notes        = req.Notes,
                Total        = req.Total ?? 0m,
                Status       = ParseStatus(req.Status)   // <- convierte a enum
            };

            var id = await _service.CreateAsync(po);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        // PUT api/purchaseorders/{id}
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdatePORequest req)
        {
            var current = await _service.GetByIdAsync(id);
            if (current is null) return NotFound();

            current.ProducerId   = req.ProducerId;
            current.EventId      = req.EventId;
            current.ExpectedDate = req.ExpectedDate;
            current.Notes        = req.Notes;
            current.Status       = ParseStatus(req.Status); // <- convierte a enum
            current.Total        = req.Total;

            await _service.UpdateAsync(current);
            return NoContent();
        }

        // DELETE api/purchaseorders/{id}
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var current = await _service.GetByIdAsync(id);
            if (current is null) return NotFound();

            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
