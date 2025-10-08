using System;

namespace INESPRE.Core.Models.Events
{
    public class EventCreateRequest
    {
        public string Name { get; set; } = default!;
        public string EventType { get; set; } = "MERCADO"; // MERCADO|BODEGA|OTRO
        public DateTime EventDateTime { get; set; }
        public string? Province { get; set; }
        public string? Municipality { get; set; }
        public string? Venue { get; set; }
        public string? Address { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string Status { get; set; } = "PLANIFICADO";
        public int CreatedBy { get; set; }
    }
}
