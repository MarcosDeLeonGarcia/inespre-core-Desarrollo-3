namespace INESPRE.Desktop.Api
{
    public class CashApi : ApiClient
    {
        public async Task<int> OpenAsync(CashOpenRequest req)
        {
            var res = await PostAsync<CashOpenRequest, Dictionary<string, int>>("/api/cash/open", req);
            return res.TryGetValue("cashSessionId", out var id) ? id : 0;
        }

        public async Task AddMovementAsync(int cashSessionId, CashMovementRequest req)
            => await PostAsync($"/api/cash/{cashSessionId}/movement", req);

        public async Task PayPOAsync(int cashSessionId, int poId, decimal amount, string method, string? notes)
        {
            var path = $"/api/cash/{cashSessionId}/pay-po?poId={poId}&amount={amount}&method={Uri.EscapeDataString(method)}&notes={Uri.EscapeDataString(notes ?? "")}";
            await PostAsync<object>(path, new { });
        }

        public async Task CloseAsync(int cashSessionId, CashCloseRequest req)
            => await PostAsync($"/api/cash/{cashSessionId}/close", req);
    }
}
