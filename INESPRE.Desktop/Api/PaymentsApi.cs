namespace INESPRE.Desktop.Api
{
    public class PaymentsApi : ApiClient
    {
        public Task<List<PaymentDto>> GetAllAsync() => GetAsync<List<PaymentDto>>("/api/payments");
        public Task CreateAsync(PaymentCreateRequest dto) => PostAsync("/api/payments", dto);
        public Task UpdateAsync(int id, PaymentUpdateRequest dto) { dto.PaymentId = id; return PutAsync($"/api/payments/{id}", dto); }
        public Task DeleteAsync(int id) => DeleteAsync($"/api/payments/{id}");
    }
}
