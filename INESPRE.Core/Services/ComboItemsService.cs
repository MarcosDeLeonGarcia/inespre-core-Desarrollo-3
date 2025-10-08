using System.Data;
using Dapper;
using INESPRE.Core.Models.Products;

namespace INESPRE.Core.Services
{
    public interface IComboItemsService
    {
        Task CreateAsync(ComboItem c);
        Task<IEnumerable<ComboItem>> GetAllAsync();
        Task<IEnumerable<ComboItem>> GetByComboAsync(int comboProductId);
        Task UpdateAsync(ComboItem c);
        Task DeleteAsync(int comboProductId, int componentProductId);
    }

    public sealed class ComboItemsService : IComboItemsService
    {
        private readonly IDbConnectionFactory _factory;
        public ComboItemsService(IDbConnectionFactory factory) => _factory = factory;

        public async Task CreateAsync(ComboItem c)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters();
            p.Add("@ComboProductId", c.ComboProductId);
            p.Add("@ComponentProductId", c.ComponentProductId);
            p.Add("@Quantity", c.Quantity);
            await conn.ExecuteAsync("dbo.spComboItems_Create", p, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<ComboItem>> GetAllAsync()
        {
            using var conn = _factory.CreateConnection();
            return await conn.QueryAsync<ComboItem>("dbo.spComboItems_Read", commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<ComboItem>> GetByComboAsync(int comboProductId)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters();
            p.Add("@ComboProductId", comboProductId);
            return await conn.QueryAsync<ComboItem>("dbo.spComboItems_Read", p, commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateAsync(ComboItem c)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters();
            p.Add("@ComboProductId", c.ComboProductId);
            p.Add("@ComponentProductId", c.ComponentProductId);
            p.Add("@Quantity", c.Quantity);
            await conn.ExecuteAsync("dbo.spComboItems_Update", p, commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteAsync(int comboProductId, int componentProductId)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters();
            p.Add("@ComboProductId", comboProductId);
            p.Add("@ComponentProductId", componentProductId);
            await conn.ExecuteAsync("dbo.spComboItems_Delete", p, commandType: CommandType.StoredProcedure);
        }
    }
}
