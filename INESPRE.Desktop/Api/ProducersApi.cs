using System.Net.Http.Json;

namespace INESPRE.Desktop.Api
{
    public class ProducersApi : ApiClient
    {
        private static bool ToBool(string? status)
            => (status ?? "").Trim().ToUpperInvariant() == "ACTIVO";

        private static string ToStatus(bool active)
            => active ? "ACTIVO" : "INACTIVO";

        public async Task<List<ProducerDto>> GetAllAsync()
        {
            var list = await GetAsync<List<ProducerDto>>("/api/producers")
                       ?? new List<ProducerDto>();

            // Normaliza Active desde Status
            foreach (var x in list)
                x.Active = ToBool(x.Status);

            return list;
        }

        public async Task<ProducerDto?> GetByIdAsync(int id)
        {
            var dto = await GetAsync<ProducerDto>($"/api/producers/{id}");
            if (dto != null) dto.Active = ToBool(dto.Status);
            return dto;
        }

        public async Task CreateAsync(ProducerCreateRequest dto)
        {
            // El backend espera "status"
            var payload = new
            {
                name = dto.Name,
                phone = dto.Phone,
                address = dto.Address,
                status = ToStatus(dto.Active)
            };

            await PostAsync("/api/producers", payload);
        }

        public async Task UpdateAsync(int id, ProducerUpdateRequest dto)
        {
            dto.ProducerId = id;

            var payload = new
            {
                producerId = dto.ProducerId,
                name = dto.Name,
                phone = dto.Phone,
                address = dto.Address,
                status = ToStatus(dto.Active)
            };

            await PutAsync($"/api/producers/{id}", payload);
        }

        public Task DeleteAsync(int id) => DeleteAsync($"/api/producers/{id}");
    }
}
