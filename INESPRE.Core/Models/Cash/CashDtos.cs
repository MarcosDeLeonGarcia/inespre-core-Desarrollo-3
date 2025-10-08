namespace INESPRE.Core.Models.Cash
{
    public class CashOpenRequest
    {
        public int UserId { get; set; }
        public int? EventId { get; set; }
        public decimal OpeningAmount { get; set; }
        public string? Notes { get; set; }
    }

    public class CashCloseRequest
    {
        public decimal ClosingAmount { get; set; }
        public IEnumerable<CashCount> Totals { get; set; } = Array.Empty<CashCount>();
    }

    public class CashCount
    {
        public string PaymentMethod { get; set; } = "EFECTIVO";
        public decimal Counted { get; set; }
    }
}
