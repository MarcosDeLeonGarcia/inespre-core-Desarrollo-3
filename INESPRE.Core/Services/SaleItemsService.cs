using System.Data;
using Dapper;
using INESPRE.Core.Models.Sales;

namespace INESPRE.Core.Services
{
    public interface ISaleItemsService
    {
        Task<int> CreateAsync(SaleItem si);
        Task<IEnumerable<SaleItem>> GetAllAsync();
        Task<IEnumerable<SaleItem>> GetBySaleAsync(int saleId);
        Task<SaleItem?> GetByIdAsync(int saleItemId);
        Task UpdateAsync(SaleItem si);
        Task DeleteAsync(int saleItemId);
    }

    public sealed class SaleItemsService : ISaleItemsService
    {
        private readonly IDbConnectionFactory _factory;
        public SaleItemsService(IDbConnectionFactory factory) => _factory = factory;

        public async Task<int> CreateAsync(SaleItem si)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters();
            p.Add("@SaleId", si.SaleId);
            p.Add("@ProductId", si.ProductId);
            p.Add("@Quantity", si.Quantity);
            p.Add("@UnitPrice", si.UnitPrice);
            return await conn.ExecuteScalarAsync<int>("dbo.spSaleItems_Create", p, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<SaleItem>> GetAllAsync()
        {
            using var conn = _factory.CreateConnection();
            return await conn.QueryAsync<SaleItem>("dbo.spSaleItems_Read", commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<SaleItem>> GetBySaleAsync(int saleId)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters();
            p.Add("@SaleId", saleId);
            return await conn.QueryAsync<SaleItem>("dbo.spSaleItems_Read", p, commandType: CommandType.StoredProcedure);
        }

        public async Task<SaleItem?> GetByIdAsync(int saleItemId)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters();
            p.Add("@SaleItemId", saleItemId);
            return await conn.QueryFirstOrDefaultAsync<SaleItem>("dbo.spSaleItems_Read", p, commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateAsync(SaleItem si)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters();
            p.Add("@SaleItemId", si.SaleItemId);
            p.Add("@ProductId", si.ProductId);
            p.Add("@Quantity", si.Quantity);
            p.Add("@UnitPrice", si.UnitPrice);
            await conn.ExecuteAsync("dbo.spSaleItems_Update", p, commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteAsync(int saleItemId)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters();
            p.Add("@SaleItemId", saleItemId);
            await conn.ExecuteAsync("dbo.spSaleItems_Delete", p, commandType: CommandType.StoredProcedure);
        }
    }
}
