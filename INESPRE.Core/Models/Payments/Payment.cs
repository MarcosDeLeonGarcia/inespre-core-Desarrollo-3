using System;
using INESPRE.Core.Models.Common;
using PaymentMethodEnum = INESPRE.Core.Models.Common.PaymentMethodType;
using PaymentStatusEnum = INESPRE.Core.Models.Common.PaymentStatus;

namespace INESPRE.Core.Models.Payments
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public int POId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        // Ahora TRANSFERENCIA existe en el enum y uso alias para claridad
        public string Method { get; set; } = PaymentMethodEnum.TRANSFERENCIA.ToString();
        public string Status { get; set; } = PaymentStatusEnum.PENDIENTE.ToString();
        public string? Notes { get; set; }
    }
}
