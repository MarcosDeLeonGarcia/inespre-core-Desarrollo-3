using System.Data;
using Dapper;
using INESPRE.Core.Models.Products;

namespace INESPRE.Core.Services
{
    public interface IProductsService
    {
        Task<int> CreateAsync(Product p);
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task UpdateAsync(Product p);
        Task DisableAsync(int id); // borrado lógico
    }

    public sealed class ProductsService : IProductsService
    {
        private readonly IDbConnectionFactory _factory;
        public ProductsService(IDbConnectionFactory factory) => _factory = factory;

        public async Task<int> CreateAsync(Product pr)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters();
            p.Add("@Name", pr.Name);
            p.Add("@SKU", pr.SKU);
            p.Add("@Unit", pr.Unit);
            p.Add("@ProductType", pr.ProductType);
            p.Add("@Perishable", pr.Perishable);
            p.Add("@DefaultPurchasePrice", pr.DefaultPurchasePrice);
            p.Add("@DefaultSalePrice", pr.DefaultSalePrice);
            p.Add("@SocialPrice", pr.SocialPrice);
            p.Add("@Active", pr.Active);
            return await conn.ExecuteScalarAsync<int>("dbo.spProducts_Create", p, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            using var conn = _factory.CreateConnection();
            return await conn.QueryAsync<Product>("dbo.spProducts_Read", commandType: CommandType.StoredProcedure);
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters();
            p.Add("@ProductId", id);
            return await conn.QueryFirstOrDefaultAsync<Product>("dbo.spProducts_Read", p, commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateAsync(Product pr)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters();
            p.Add("@ProductId", pr.ProductId);
            p.Add("@Name", pr.Name);
            p.Add("@SKU", pr.SKU);
            p.Add("@Unit", pr.Unit);
            p.Add("@ProductType", pr.ProductType);
            p.Add("@Perishable", pr.Perishable);
            p.Add("@DefaultPurchasePrice", pr.DefaultPurchasePrice);
            p.Add("@DefaultSalePrice", pr.DefaultSalePrice);
            p.Add("@SocialPrice", pr.SocialPrice);
            p.Add("@Active", pr.Active);
            await conn.ExecuteAsync("dbo.spProducts_Update", p, commandType: CommandType.StoredProcedure);
        }

        public async Task DisableAsync(int id)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters();
            p.Add("@ProductId", id);
            await conn.ExecuteAsync("dbo.spProducts_Delete", p, commandType: CommandType.StoredProcedure);
        }
    }
}
