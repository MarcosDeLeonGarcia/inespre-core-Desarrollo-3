using System.Data;
using Dapper;
using INESPRE.Core.Models.Purchasing;
using INESPRE.Core.Models.Common;

namespace INESPRE.Core.Services
{
    public interface IPurchaseOrdersService
    {
        Task<int> CreateAsync(PurchaseOrder po);
        Task<IEnumerable<PurchaseOrder>> GetAllAsync();
        Task<PurchaseOrder?> GetByIdAsync(int id);
        Task UpdateAsync(PurchaseOrder po);
        Task DeleteAsync(int id);
    }

    public sealed class PurchaseOrdersService : IPurchaseOrdersService
    {
        private readonly IDbConnectionFactory _factory;
        public PurchaseOrdersService(IDbConnectionFactory factory) => _factory = factory;

        // ====== AQUÍ EL CAMBIO ======
        public async Task<int> CreateAsync(PurchaseOrder po)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters();
            p.Add("@ProducerId", po.ProducerId);
            p.Add("@CreatedBy", po.CreatedBy);
            p.Add("@EventId", po.EventId);
            p.Add("@ExpectedDate", po.ExpectedDate);
            p.Add("@Notes", po.Notes);
            p.Add("@Total", po.Total);   // <-- ahora sí lo enviamos

            return await conn.ExecuteScalarAsync<int>(
                "dbo.spPO_Create", p, commandType: CommandType.StoredProcedure);
        }

        // ====== FIN DEL CAMBIO ======

        public async Task<IEnumerable<PurchaseOrder>> GetAllAsync()
        {
            using var conn = _factory.CreateConnection();
            return await conn.QueryAsync<PurchaseOrder>(
                "dbo.spPO_Read", commandType: CommandType.StoredProcedure);
        }

        public async Task<PurchaseOrder?> GetByIdAsync(int id)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters();
            p.Add("@POId", id);

            return await conn.QueryFirstOrDefaultAsync<PurchaseOrder>(
                "dbo.spPO_Read", p, commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateAsync(PurchaseOrder po)
        {
            using var conn = _factory.CreateConnection();

            var p = new DynamicParameters();
            p.Add("@POId", po.POId);
            p.Add("@ProducerId", po.ProducerId);
            p.Add("@EventId", po.EventId);
            p.Add("@ExpectedDate", po.ExpectedDate);
            p.Add("@Notes", po.Notes);
            p.Add("@Status", po.Status.ToString()); // spPO_Update sí suele recibirlos
            p.Add("@Total", po.Total);

            await conn.ExecuteAsync(
                "dbo.spPO_Update", p, commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteAsync(int id)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters();
            p.Add("@POId", id);
            await conn.ExecuteAsync("dbo.spPO_Delete", p, commandType: CommandType.StoredProcedure);
        }
    }
}
