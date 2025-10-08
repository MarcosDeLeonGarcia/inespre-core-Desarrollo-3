namespace INESPRE.Desktop.Api
{
    public class PurchaseOrdersApi : ApiClient
    {
        public Task<List<PurchaseOrderDto>> GetAllAsync()
         => GetAsync<List<PurchaseOrderDto>>("/api/purchaseorders");

        public Task<List<PurchaseOrderDto>> GetAsync(
            int? producerId = null, DateTime? from = null, DateTime? to = null, string? status = null)
        {
            var qs = new List<string>();
            if (producerId is int pid) qs.Add($"producerId={pid}");
            if (from is DateTime f) qs.Add($"from={Uri.EscapeDataString(f.ToString("yyyy-MM-dd"))}");
            if (to is DateTime t) qs.Add($"to={Uri.EscapeDataString(t.ToString("yyyy-MM-dd"))}");
            if (!string.IsNullOrWhiteSpace(status)) qs.Add($"status={Uri.EscapeDataString(status)}");

            var url = "/api/purchaseorders" + (qs.Count > 0 ? "?" + string.Join("&", qs) : "");
            return GetAsync<List<PurchaseOrderDto>>(url);
        }

        public Task CreateAsync(PurchaseOrderCreateRequest dto)
            => PostAsync("/api/purchaseorders", dto);

        public Task UpdateAsync(int id, PurchaseOrderUpdateRequest dto)
        {
            dto.POId = id;
            return PutAsync($"/api/purchaseorders/{id}", dto);
        }

        public Task DeleteAsync(int id)
            => DeleteAsync($"/api/purchaseorders/{id}");
    }
}
