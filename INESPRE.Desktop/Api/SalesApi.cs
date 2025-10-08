namespace INESPRE.Desktop.Api
{
    public class SalesApi : ApiClient
    {
        public Task<List<SaleDto>> GetAllAsync() => GetAsync<List<SaleDto>>("/api/sales");
        public Task CreateAsync(SaleCreateRequest dto) => PostAsync("/api/sales", dto);
        public Task UpdateAsync(int id, SaleUpdateRequest dto) { dto.SaleId = id; return PutAsync($"/api/sales/{id}", dto); }
        public Task DeleteAsync(int id) => DeleteAsync($"/api/sales/{id}");
    }
}
