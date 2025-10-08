using System.Data;
using Dapper;
using INESPRE.Core.Models.Sales;

namespace INESPRE.Core.Services
{
    public interface ISalesService
    {
        Task<int> CreateAsync(Sale s);
        Task<IEnumerable<Sale>> GetAllAsync();
        Task<IEnumerable<Sale>> GetByEventAsync(int eventId);
        Task<Sale?> GetByIdAsync(int saleId);
        Task UpdateAsync(Sale s);
        Task DeleteAsync(int saleId);
    }

    public sealed class SalesService : ISalesService
    {
        private readonly IDbConnectionFactory _factory;
        public SalesService(IDbConnectionFactory factory) => _factory = factory;

        public async Task<int> CreateAsync(Sale s)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters();
            p.Add("@EventId", s.EventId);
            p.Add("@UserId", s.UserId);
            p.Add("@PaymentMethod", s.PaymentMethod);
            p.Add("@Status", s.Status);
            p.Add("@Total", s.Total);
            return await conn.ExecuteScalarAsync<int>("dbo.spSales_Create", p, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Sale>> GetAllAsync()
        {
            using var conn = _factory.CreateConnection();
            return await conn.QueryAsync<Sale>("dbo.spSales_Read", commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Sale>> GetByEventAsync(int eventId)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters();
            p.Add("@EventId", eventId);
            return await conn.QueryAsync<Sale>("dbo.spSales_Read", p, commandType: CommandType.StoredProcedure);
        }

        public async Task<Sale?> GetByIdAsync(int saleId)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters();
            p.Add("@SaleId", saleId);
            return await conn.QueryFirstOrDefaultAsync<Sale>("dbo.spSales_Read", p, commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateAsync(Sale s)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters();
            p.Add("@SaleId", s.SaleId);
            p.Add("@PaymentMethod", s.PaymentMethod);
            p.Add("@Status", s.Status);
            p.Add("@Total", s.Total);
            await conn.ExecuteAsync("dbo.spSales_Update", p, commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteAsync(int saleId)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters();
            p.Add("@SaleId", saleId);
            await conn.ExecuteAsync("dbo.spSales_Delete", p, commandType: CommandType.StoredProcedure);
        }
    }
}
