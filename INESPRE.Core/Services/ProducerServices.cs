using System.Data;
using Dapper;
using INESPRE.Core.Models.Producers;

namespace INESPRE.Core.Services
{
    public interface IProducersService
    {
        Task<int> CreateAsync(Producer p);
        Task<IEnumerable<Producer>> GetAllAsync();
        Task<Producer?> GetByIdAsync(int id);
        Task UpdateAsync(Producer p);
        Task DeleteAsync(int id);
    }

    public sealed class ProducersService : IProducersService
    {
        private readonly IDbConnectionFactory _factory;
        public ProducersService(IDbConnectionFactory factory) => _factory = factory;

        public async Task<int> CreateAsync(Producer p)
        {
            using var conn = _factory.CreateConnection();
            var dp = new DynamicParameters();
            dp.Add("@Name", p.Name);
            dp.Add("@DocumentId", p.DocumentId);
            dp.Add("@Province", p.Province);
            dp.Add("@Municipality", p.Municipality);
            dp.Add("@Phone", p.Phone);
            dp.Add("@Email", p.Email);
            dp.Add("@Status", p.Status);
            return await conn.ExecuteScalarAsync<int>("dbo.spProducers_Create", dp, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Producer>> GetAllAsync()
        {
            using var conn = _factory.CreateConnection();
            return await conn.QueryAsync<Producer>("dbo.spProducers_Read", commandType: CommandType.StoredProcedure);
        }

        public async Task<Producer?> GetByIdAsync(int id)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters();
            p.Add("@ProducerId", id);
            return await conn.QueryFirstOrDefaultAsync<Producer>("dbo.spProducers_Read", p, commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateAsync(Producer pr)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters();
            p.Add("@ProducerId", pr.ProducerId);
            p.Add("@Name", pr.Name);
            p.Add("@DocumentId", pr.DocumentId);
            p.Add("@Province", pr.Province);
            p.Add("@Municipality", pr.Municipality);
            p.Add("@Phone", pr.Phone);
            p.Add("@Email", pr.Email);
            p.Add("@Status", pr.Status);
            await conn.ExecuteAsync("dbo.spProducers_Update", p, commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteAsync(int id)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters();
            p.Add("@ProducerId", id);
            await conn.ExecuteAsync("dbo.spProducers_Delete", p, commandType: CommandType.StoredProcedure);
        }
    }
}
