using System;
using INESPRE.Core.Models.Common;
using EventTypeEnum = INESPRE.Core.Models.Common.EventType;
using EventStatusEnum = INESPRE.Core.Models.Common.EventStatus;

namespace INESPRE.Core.Models.Events
{
    public class Event
    {
        public int EventId { get; set; }
        public string Name { get; set; } = default!;
        // La propiedad se llama igual que el enum, por eso uso alias EventTypeEnum
        public string EventType { get; set; } = EventTypeEnum.MERCADO.ToString();
        public DateTime EventDateTime { get; set; }

        public string? Province { get; set; }
        public string? Municipality { get; set; }
        public string? Venue { get; set; }
        public string? Address { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        public string Status { get; set; } = EventStatusEnum.PLANIFICADO.ToString();
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
