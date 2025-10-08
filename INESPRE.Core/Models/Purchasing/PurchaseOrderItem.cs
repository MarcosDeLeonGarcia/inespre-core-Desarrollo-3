namespace INESPRE.Core.Models.Purchasing
{
    public class PurchaseOrderItem
    {
        public int POItemId { get; set; }
        public int POId { get; set; }
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
        public decimal ReceivedQty { get; set; }
    }
}
