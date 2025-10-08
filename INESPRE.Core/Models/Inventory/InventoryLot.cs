using System;
using INESPRE.Core.Models.Common;

namespace INESPRE.Core.Models.Inventory
{
    public class InventoryLot
    {
        public int LotId { get; set; }
        public int ProductId { get; set; }
        public int? SourcePOItemId { get; set; }
        public int? EventId { get; set; }
        public decimal Quantity { get; set; }
        public decimal AvailableQty { get; set; }
        public decimal UnitCost { get; set; }
        public string Location { get; set; } = LocationType.ALMACEN.ToString(); // ALMACEN|EVENTO|VEHICULO
        public string? LocationRef { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
