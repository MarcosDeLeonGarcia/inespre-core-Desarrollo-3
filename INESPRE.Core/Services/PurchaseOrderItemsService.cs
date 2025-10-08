using System.Data;
using Dapper;
using INESPRE.Core.Models.Purchasing;

namespace INESPRE.Core.Services
{
    public interface IPurchaseOrderItemsService
    {
        Task<int> CreateAsync(PurchaseOrderItem item);
        Task<IEnumerable<PurchaseOrderItem>> GetAllAsync();
        Task<IEnumerable<PurchaseOrderItem>> GetByPOAsync(int poId);
        Task<PurchaseOrderItem?> GetByIdAsync(int poItemId);
        Task UpdateAsync(PurchaseOrderItem item);
        Task DeleteAsync(int poItemId);
    }

    public sealed class PurchaseOrderItemsService : IPurchaseOrderItemsService
    {
        private readonly IDbConnectionFactory _factory;
        public PurchaseOrderItemsService(IDbConnectionFactory factory) => _factory = factory;

        public async Task<int> CreateAsync(PurchaseOrderItem item)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters();
            p.Add("@POId", item.POId);
            p.Add("@ProductId", item.ProductId);
            p.Add("@Quantity", item.Quantity);
            p.Add("@UnitPrice", item.UnitPrice);
            return await conn.ExecuteScalarAsync<int>("dbo.spPOItems_Create", p, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<PurchaseOrderItem>> GetAllAsync()
        {
            using var conn = _factory.CreateConnection();
            return await conn.QueryAsync<PurchaseOrderItem>("dbo.spPOItems_Read", commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<PurchaseOrderItem>> GetByPOAsync(int poId)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters();
            p.Add("@POId", poId);
            return await conn.QueryAsync<PurchaseOrderItem>("dbo.spPOItems_Read", p, commandType: CommandType.StoredProcedure);
        }

        public async Task<PurchaseOrderItem?> GetByIdAsync(int poItemId)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters();
            p.Add("@POItemId", poItemId);
            return await conn.QueryFirstOrDefaultAsync<PurchaseOrderItem>("dbo.spPOItems_Read", p, commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateAsync(PurchaseOrderItem item)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters();
            p.Add("@POItemId", item.POItemId);
            p.Add("@ProductId", item.ProductId);
            p.Add("@Quantity", item.Quantity);
            p.Add("@UnitPrice", item.UnitPrice);
            p.Add("@ReceivedQty", item.ReceivedQty);
            await conn.ExecuteAsync("dbo.spPOItems_Update", p, commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteAsync(int poItemId)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters();
            p.Add("@POItemId", poItemId);
            await conn.ExecuteAsync("dbo.spPOItems_Delete", p, commandType: CommandType.StoredProcedure);
        }
    }
}
