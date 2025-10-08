using System.Data;
using Dapper;
using INESPRE.Core.Models.Inventory;

namespace INESPRE.Core.Services
{
    public interface IInventoryLotsService
    {
        Task<int> CreateAsync(InventoryLot lot);
        Task<IEnumerable<InventoryLot>> GetAllAsync();
        Task<IEnumerable<InventoryLot>> GetByProductAsync(int productId);
        Task<IEnumerable<InventoryLot>> GetByEventAsync(int eventId);
        Task<InventoryLot?> GetByIdAsync(int lotId);
        Task UpdateAsync(InventoryLot lot);
        Task DeleteAsync(int lotId);
    }

    public sealed class InventoryLotsService : IInventoryLotsService
    {
        private readonly IDbConnectionFactory _factory;
        public InventoryLotsService(IDbConnectionFactory factory) => _factory = factory;

        public async Task<int> CreateAsync(InventoryLot l)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters();
            p.Add("@ProductId", l.ProductId);
            p.Add("@SourcePOItemId", l.SourcePOItemId);
            p.Add("@EventId", l.EventId);
            p.Add("@Quantity", l.Quantity);
            p.Add("@AvailableQty", l.AvailableQty);
            p.Add("@UnitCost", l.UnitCost);
            p.Add("@Location", l.Location);
            p.Add("@LocationRef", l.LocationRef);
            return await conn.ExecuteScalarAsync<int>("dbo.spLots_Create", p, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<InventoryLot>> GetAllAsync()
        {
            using var conn = _factory.CreateConnection();
            return await conn.QueryAsync<InventoryLot>("dbo.spLots_Read", commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<InventoryLot>> GetByProductAsync(int productId)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters();
            p.Add("@ProductId", productId);
            return await conn.QueryAsync<InventoryLot>("dbo.spLots_Read", p, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<InventoryLot>> GetByEventAsync(int eventId)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters();
            p.Add("@EventId", eventId);
            return await conn.QueryAsync<InventoryLot>("dbo.spLots_Read", p, commandType: CommandType.StoredProcedure);
        }

        public async Task<InventoryLot?> GetByIdAsync(int lotId)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters();
            p.Add("@LotId", lotId);
            return await conn.QueryFirstOrDefaultAsync<InventoryLot>("dbo.spLots_Read", p, commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateAsync(InventoryLot l)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters();
            p.Add("@LotId", l.LotId);
            p.Add("@EventId", l.EventId);
            p.Add("@AvailableQty", l.AvailableQty);
            p.Add("@Location", l.Location);
            p.Add("@LocationRef", l.LocationRef);
            await conn.ExecuteAsync("dbo.spLots_Update", p, commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteAsync(int lotId)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters();
            p.Add("@LotId", lotId);
            await conn.ExecuteAsync("dbo.spLots_Delete", p, commandType: CommandType.StoredProcedure);
        }
    }
}
