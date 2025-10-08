namespace INESPRE.Core.Models.Products
{
    public class ComboItem
    {
        public int ComboProductId { get; set; }      // ProductId del combo
        public int ComponentProductId { get; set; }  // ProductId del componente
        public decimal Quantity { get; set; }        // cantidad del componente
    }
}
