using System;
using INESPRE.Core.Models.Common;

namespace INESPRE.Core.Models.Producers
{
    public class Producer
    {
        public int ProducerId { get; set; }
        public string Name { get; set; } = default!;
        public string? DocumentId { get; set; }   // RNC/Cédula
        public string? Province { get; set; }
        public string? Municipality { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string Status { get; set; } = ProducerStatus.ACTIVO.ToString();
        public DateTime CreatedAt { get; set; }
    }
}
