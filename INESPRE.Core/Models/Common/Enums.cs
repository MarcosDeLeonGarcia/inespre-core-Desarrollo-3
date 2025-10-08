namespace INESPRE.Core.Models.Common
{
    public enum EventType { MERCADO, BODEGA, OTRO }
    public enum EventStatus { PLANIFICADO, PUBLICADO, EN_CURSO, CERRADO, CANCELADO }

    public enum ProductType { SIMPLE, COMBO }

    public enum LocationType { ALMACEN, EVENTO, VEHICULO }

    public enum SaleStatus { ABIERTA, CERRADA, ANULADA }

    // Incluyo métodos de pago usados en ventas y en pagos a productores
    public enum PaymentMethodType { EFECTIVO, TARJETA, MIXTO, OTRO, TRANSFERENCIA, CHEQUE }

    public enum ProducerStatus { ACTIVO, INACTIVO }
    public enum PaymentStatus { PENDIENTE, PAGADO, ANULADO }

    // Models/Common/Enums.cs
    
        public enum PurchaseOrderStatus
        {
            PENDIENTE,
            PAGADA,
            CERRADA
        }
    

}
