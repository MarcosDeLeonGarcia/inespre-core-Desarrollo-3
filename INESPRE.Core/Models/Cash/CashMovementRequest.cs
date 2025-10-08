namespace INESPRE.Core.Models.Cash
{
    public class CashMovementRequest
    {
        public string Method { get; set; } = "EFECTIVO";  // EFECTIVO|TARJETA|MIXTO|OTRO
        public string Direction { get; set; } = "OUT";    // IN|OUT
        public string Category { get; set; } = "EXPENSE"; // EXPENSE|DEPOSIT|REFUND|PAY_PO|OTHER
        public decimal Amount { get; set; }
        public string? RefType { get; set; }              // PAYMENT|PO|SALE|OTHER
        public int? RefId { get; set; }
        public string? Notes { get; set; }
    }
}
