using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;


namespace INESPRE.Desktop.Api
{
    // =========================================================
    // ===============            AUTH             =============
    // =========================================================
    public sealed class LoginRequest
    {
        public string username { get; set; } = "";
        public string password { get; set; } = "";
    }

    // Soporta API con JWT o sin JWT
    public sealed class LoginResponse
    {
        // JWT
        public string? token { get; set; }
        public string? role { get; set; }

        // Login simple (sin JWT)
        public bool? isValid { get; set; }
        public int? userId { get; set; }
        public int? roleId { get; set; }
    }

    // Registro (por /api/auth/register)
    public sealed class RegisterRequest
    {
        public string username { get; set; } = "";
        public string? email { get; set; }
        public string? fullName { get; set; }
        public string? phone { get; set; }
        public int roleId { get; set; }
        public string password { get; set; } = "";
    }

    // =========================================================
    // ===============           USUARIOS          =============
    // =========================================================
    public sealed class UserDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = "";
        public string Email { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Phone { get; set; } = "";
        public int RoleId { get; set; }
        public bool IsActive { get; set; }
    }

    public sealed class UserUpdateRequest
    {
        public string UserName { get; set; } = "";
        public string Email { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Phone { get; set; } = "";
        public int RoleId { get; set; }
        public bool IsActive { get; set; }
        public int UserId { get; internal set; }
    }

    // =========================================================
    // ===============           PRODUCTOS         =============
    // =========================================================
    public sealed class ProductDto
    {
        public int ProductId { get; set; }
        public string? Name { get; set; }
        public string? SKU { get; set; }
        public string Unit { get; set; } = "und";
        public string ProductType { get; set; } = "SIMPLE"; // SIMPLE/COMBO
        public bool Perishable { get; set; }
        public decimal? DefaultSalePrice { get; set; }             // no nullable para simplificar el binding
        public bool Active { get; set; }
    }

    public  class ProductCreateRequest
    {
        public string Name { get; set; } = "";
        public string? SKU { get; set; }
        public string? Unit { get; set; }
        public string ProductType { get; set; } = "SIMPLE";
        public bool Perishable { get; set; }
        public decimal DefaultSalePrice { get; set; }
        public bool Active { get; set; } = true;
    }

    public sealed class ProductUpdateRequest : ProductCreateRequest
    {
        public int ProductId { get; set; }
    }

    // =========================================================
    // ===============             CAJA            =============
    // =========================================================
    public sealed class CashOpenRequest
    {
        public int UserId { get; set; }
        public int? EventId { get; set; }
        public decimal OpeningAmount { get; set; }
        public string? Notes { get; set; }
    }

    public sealed class CashCloseRequest
    {
        public decimal ClosingAmount { get; set; }
        public List<CashCount> Totals { get; set; } = new();
    }

    public sealed class CashCount
    {
        public string PaymentMethod { get; set; } = "EFECTIVO";
        public decimal Counted { get; set; }
    }

    public sealed class CashMovementRequest
    {
        public string Method { get; set; } = "EFECTIVO";
        public string Direction { get; set; } = "IN";       // IN/OUT
        public string Category { get; set; } = "EXPENSE";  // EXPENSE/SALE/OTHER
        public decimal Amount { get; set; }
        public string? Notes { get; set; }
        public string? RefType { get; set; }
        public int? RefId { get; set; }
    }

    // =========================================================
    // ===============            EVENTOS          =============
    // =========================================================
    public sealed class EventDto
    {
        public int EventId { get; set; }
        public string? Name { get; set; }
        public string? EventType { get; set; }
        public DateTime EventDateTime { get; set; }
        public string? Province { get; set; }
        public string? Municipality { get; set; }
        public string? Venue { get; set; }
        public string? Address { get; set; }
        public string? Status { get; set; }

        // Para mostrar quién creó el evento
        public int? CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
    }

    public sealed class EventCreateRequest
    {
        public string Name { get; set; } = "";
        public string EventType { get; set; } = "OPERATIVO";
        public DateTime EventDateTime { get; set; } = DateTime.UtcNow;
        public string Province { get; set; } = "SD";
        public string Municipality { get; set; } = "DN";
        public string? Venue { get; set; }
        public string? Address { get; set; }
        public string Status { get; set; } = "PLANIFICADO";
        public int CreatedBy { get; set; }
    }

    public sealed class EventUpdateRequest
    {
        public int EventId { get; set; }
        public string Name { get; set; } = "";
        public string EventType { get; set; } = "OPERATIVO";
        public DateTime EventDateTime { get; set; } = DateTime.UtcNow;
        public string Province { get; set; } = "SD";
        public string Municipality { get; set; } = "DN";
        public string? Venue { get; set; }
        public string? Address { get; set; }
        public string Status { get; set; } = "PLANIFICADO";
        public int? UpdatedBy { get; set; }
    }

    // =========================================================
    // ===============           PRODUCTORES       =============
    // =========================================================
    // =========================================================
    // ===============           PRODUCTORES       =============
    // =========================================================
    public sealed class ProducerDto
    {
        public int ProducerId { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }

        // Viene del backend: "ACTIVO" | "INACTIVO"
        public string? Status { get; set; }

        // Usado por la UI (grid/filtro). Lo rellenamos en ProducersApi.
        public bool Active { get; set; }
    }

    public class ProducerCreateRequest
    {
        public string Name { get; set; } = "";
        public string? Phone { get; set; }
        public string? Address { get; set; }

        // Check del formulario
        public bool Active { get; set; } = true;
    }

    public sealed class ProducerUpdateRequest : ProducerCreateRequest
    {
        public int ProducerId { get; set; }
    }


    // =========================================================
    // ===============             ROLES           =============
    // =========================================================
    public sealed class RoleDto
    {
        public int RoleId { get; set; }
        public string Name { get; set; } = "";
    }

    public class RoleCreateRequest { public string Name { get; set; } = ""; }
    public sealed class RoleUpdateRequest : RoleCreateRequest { public int RoleId { get; set; } }

    // =========================================================
    // ===============        LOTES INVENTARIO     =============
    // =========================================================
    public sealed class InventoryLotDto
    {
        public int LotId { get; set; }
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }

        // Si tu API devuelve availableQty, lo mapeamos aquí (opcional)
        [JsonPropertyName("availableQty")]
        public decimal? AvailableQty { get; set; }

        // <-- AHORA SIEMPRE UnitCost (la API lo llama unitCost)
        [JsonPropertyName("unitCost")]
        public decimal UnitCost { get; set; }

        public int? EventId { get; set; }

        [JsonPropertyName("location")]
        public string? Location { get; set; }

        [JsonPropertyName("locationRef")]
        public string? LocationRef { get; set; }

        // Por si el backend envía fecha de creación
        [JsonPropertyName("createdAt")]
        public DateTime? CreatedAt { get; set; }
    }

    public class InventoryLotCreateRequest
    {
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }

        [JsonPropertyName("availableQty")]
        public decimal? AvailableQty { get; set; }

        // <-- enviar como unitCost
        [JsonPropertyName("unitCost")]
        public decimal UnitCost { get; set; }

        public int? EventId { get; set; }

        [JsonPropertyName("location")]
        public string? Location { get; set; }

        [JsonPropertyName("locationRef")]
        public string? LocationRef { get; set; }
    }

    public sealed class InventoryLotUpdateRequest : InventoryLotCreateRequest
    {
        public int LotId { get; set; }
    }
    // =========================================================
    // ===============        ORDENES COMPRA       =============
    // =========================================================
    public sealed class PurchaseOrderDto
    {
        [JsonPropertyName("poId")]
        public int POId { get; set; }

        [JsonPropertyName("producerId")]
        public int ProducerId { get; set; }

        // La API hoy NO envía poDate; sí envía expectedDate.
        // Aun así dejamos ambos alias por si mañana agregas poDate.
        [JsonPropertyName("poDate")]
        public DateTime? PoDate { get; set; }

        [JsonPropertyName("expectedDate")]
        public DateTime? ExpectedDate { get; set; }

        [JsonPropertyName("eventId")]
        public int? EventId { get; set; }

        [JsonPropertyName("total")]
        public decimal Total { get; set; }

        // IMPORTANTE: la API manda número (0/1/2).
        // Usamos int para evitar conflictos con JsonStringEnumConverter.
        [JsonPropertyName("status")]
        public int Status { get; set; }  // 0=PENDIENTE, 1=PAGADA, 2=CERRADA

        // --------- Propiedades calculadas para la UI ---------
        [JsonIgnore]
        public DateTime DisplayDate => ExpectedDate ?? PoDate ?? DateTime.Today;

        [JsonIgnore]
        public string StatusText => Status switch
        {
            0 => "PENDIENTE",
            1 => "PAGADA",
            2 => "CERRADA",
            _ => Status.ToString()
        };
    }
    public sealed class PurchaseOrderCreateRequest
    {
        public int ProducerId { get; set; }
        public int CreatedBy { get; set; }
        public int? EventId { get; set; }
        public DateTime? ExpectedDate { get; set; }
        public string? Notes { get; set; }
        public decimal? Total { get; set; }
        // NO Status en create
    }

    public sealed class PurchaseOrderUpdateRequest
    {
        public int POId { get; set; }
        public int ProducerId { get; set; }
        public int? EventId { get; set; }
        public DateTime? ExpectedDate { get; set; }
        public string? Notes { get; set; }
        public string Status { get; set; } = "PENDIENTE"; // si tu API de Update espera string
        public decimal Total { get; set; }
    }
    //public sealed class ProducerDto { public int ProducerId { get; set; } public string Name { get; set; } = ""; }
    //public sealed class EventDto { public int EventId { get; set; } public string Name { get; set; } = ""; }
    // =========================================================
    // ===============            PAGOS            =============
    // =========================================================
    public sealed class PaymentDto
    {
        public int PaymentId { get; set; }
        public int POId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Method { get; set; } = "EFECTIVO";  // EFECTIVO/TRANSFERENCIA
        public string Status { get; set; } = "PENDIENTE"; // PENDIENTE/CONFIRMADO
        public string? Notes { get; set; }
    }

    public class PaymentCreateRequest
    {
        public int POId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Method { get; set; } = "EFECTIVO";
        public string Status { get; set; } = "PENDIENTE";
        public string? Notes { get; set; }
    }

    public sealed class PaymentUpdateRequest : PaymentCreateRequest
    {
        public int PaymentId { get; set; }
    }

    // =========================================================
    // ===============             VENTAS          =============
    // =========================================================
    public sealed class SaleDto
    {
        public int SaleId { get; set; }
        public DateTime SaleDate { get; set; }
        public int? EventId { get; set; }
        public int UserId { get; set; }

        // NUEVO: método de pago del JSON
        public string PaymentMethod { get; set; } = "EFECTIVO";

        public decimal Total { get; set; }
        public string Status { get; set; } = "ABIERTA"; // ABIERTA/CERRADA/ANULADA

        // ----- Para mostrar nombres en el grid -----
        [JsonIgnore] public string? EventName { get; set; }
        [JsonIgnore] public string? UserName { get; set; }
    }

    public class SaleCreateRequest
    {
        public DateTime SaleDate { get; set; }
        public int? EventId { get; set; }
        public int UserId { get; set; }
        public string PaymentMethod { get; set; } = "EFECTIVO"; // NUEVO
        public decimal Total { get; set; } = 0;                 // NUEVO
        public string Status { get; set; } = "ABIERTA";
    }

    public sealed class SaleUpdateRequest : SaleCreateRequest
    {
        public int SaleId { get; set; }
    }
}
