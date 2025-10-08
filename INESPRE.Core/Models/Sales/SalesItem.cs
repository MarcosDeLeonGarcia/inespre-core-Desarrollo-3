namespace INESPRE.Core.Models.Sales
{
    public class SaleItem
    {
        public int SaleItemId { get; set; }
        public int SaleId { get; set; }
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }
}
