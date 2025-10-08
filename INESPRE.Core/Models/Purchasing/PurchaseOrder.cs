using INESPRE.Core.Models.Common;

namespace INESPRE.Core.Models.Purchasing
{
    public sealed class PurchaseOrder
    {
        public int POId { get; set; }
        public int ProducerId { get; set; }
        public int CreatedBy { get; set; }         // o string si así lo usas
        public int? EventId { get; set; }
        public DateTime? ExpectedDate { get; set; }
        public string? Notes { get; set; }
        public decimal Total { get; set; }         // lo vi en tu Update SP
        public decimal AmountPaid { get; set; }    // útil para validaciones en UI/servicio pagos

        // ¡clave!: enum en código, texto en BD
        public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.PENDIENTE;
    }
}
