using System;
using INESPRE.Core.Models.Common;

namespace INESPRE.Core.Models.Sales
{
    public class Sale
    {
        public int SaleId { get; set; }
        public int EventId { get; set; }
        public int UserId { get; set; }
        public DateTime SaleDate { get; set; }
        public string PaymentMethod { get; set; } = PaymentMethodType.EFECTIVO.ToString(); // EFECTIVO|TARJETA|MIXTO|OTRO
        public string Status { get; set; } = SaleStatus.CERRADA.ToString();                // ABIERTA|CERRADA|ANULADA
        public decimal Total { get; set; }
    }
}
