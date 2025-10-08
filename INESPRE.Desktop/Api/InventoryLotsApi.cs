namespace INESPRE.Desktop.Api
{
    public sealed class InventoryLotsApi : ApiClient
    {
        private readonly string _endpoint;
        public InventoryLotsApi(string endpoint = "/api/inventorylots") => _endpoint = endpoint;

        // GET /api/inventorylots?productId=&eventId=
        public async Task<List<InventoryLotDto>> GetAllAsync(int? productId = null, int? eventId = null)
        {
            var url = BuildUrl(_endpoint, new Dictionary<string, string?>
            {
                ["productId"] = productId?.ToString(),
                ["eventId"]   = eventId?.ToString()
            });

            return await GetMaybeAsync<List<InventoryLotDto>>(url) ?? new List<InventoryLotDto>();
        }

        public Task<InventoryLotDto?> GetByIdAsync(int id)
            => GetMaybeAsync<InventoryLotDto>($"{_endpoint}/{id}");

        public Task CreateAsync(InventoryLotCreateRequest dto)
            => PostAsync(_endpoint, dto); // la API suele devolver 204/201

        public Task UpdateAsync(int id, InventoryLotUpdateRequest dto)
        {
            dto.LotId = id;                      // ruta == body
            return PutAsync($"{_endpoint}/{id}", dto); // 204 NoContent esperado
        }

        public Task DeleteAsync(int id)
            => DeleteAsync($"{_endpoint}/{id}");
    }
}
