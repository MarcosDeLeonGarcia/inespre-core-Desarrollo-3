using System;
using INESPRE.Core.Models.Common;
using ProductTypeEnum = INESPRE.Core.Models.Common.ProductType;

namespace INESPRE.Core.Models.Products
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = default!;
        public string? SKU { get; set; }
        public string Unit { get; set; } = default!;
        // Evito colisión con alias ProductTypeEnum
        public string ProductType { get; set; } = ProductTypeEnum.SIMPLE.ToString();
        public bool Perishable { get; set; }
        public decimal? DefaultPurchasePrice { get; set; }
        public decimal? DefaultSalePrice { get; set; }
        public decimal? SocialPrice { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
