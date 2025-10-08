using System.Data;
using Dapper;
using INESPRE.Core.Models.Events;

namespace INESPRE.Core.Services
{
    public interface IEventsService
    {
        Task<int> CreateAsync(EventCreateRequest dto);
        Task<IEnumerable<Event>> GetAllAsync();
        Task<Event?> GetByIdAsync(int id);
        Task UpdateAsync(EventUpdateRequest dto);
        Task DeleteAsync(int id);
    }

    public sealed class EventsService : IEventsService
    {
        private readonly IDbConnectionFactory _factory;
        public EventsService(IDbConnectionFactory factory) => _factory = factory;

        public async Task<int> CreateAsync(EventCreateRequest dto)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters(dto);
            return await conn.ExecuteScalarAsync<int>("dbo.spEvents_Create", p, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Event>> GetAllAsync()
        {
            using var conn = _factory.CreateConnection();
            return await conn.QueryAsync<Event>("dbo.spEvents_Read", commandType: CommandType.StoredProcedure);
        }

        public async Task<Event?> GetByIdAsync(int id)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters();
            p.Add("@EventId", id);
            return await conn.QueryFirstOrDefaultAsync<Event>("dbo.spEvents_Read", p, commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateAsync(EventUpdateRequest dto)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters(dto);
            await conn.ExecuteAsync("dbo.spEvents_Update", p, commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteAsync(int id)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters();
            p.Add("@EventId", id);
            await conn.ExecuteAsync("dbo.spEvents_Delete", p, commandType: CommandType.StoredProcedure);
        }
    }
}
