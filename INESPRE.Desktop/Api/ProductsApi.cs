// ProductsApi
namespace INESPRE.Desktop.Api
{
    public sealed class ProductsApi : ApiClient
    {
        private readonly string _endpoint;
        public ProductsApi(string endpoint = "/api/products") => _endpoint = endpoint;

        public async Task<List<ProductDto>> GetAllAsync(
            string? name = null,
            string? sku = null,
            string? productType = null,
            bool? active = null)
        {
            var url = BuildUrl(_endpoint, new Dictionary<string, string?>
            {
                ["name"]   = name,
                ["sku"]    = sku,
                ["type"]   = productType,
                ["active"] = active?.ToString()?.ToLowerInvariant()
            });

            return await GetMaybeAsync<List<ProductDto>>(url) ?? new List<ProductDto>();
        }

        public Task<ProductDto?> GetByIdAsync(int id)
            => GetMaybeAsync<ProductDto>($"{_endpoint}/{id}");

        public Task<ProductDto?> CreateAsync(ProductCreateRequest dto)
            => PostMaybeAsync<ProductCreateRequest, ProductDto>(_endpoint, dto);

        public async Task UpdateAsync(int id, ProductUpdateRequest dto)
        {
            dto.ProductId = id;
            await PutAsync($"{_endpoint}/{id}", dto);
        }

        public Task DeleteAsync(int id)
            => DeleteAsync($"{_endpoint}/{id}");
    }
}
