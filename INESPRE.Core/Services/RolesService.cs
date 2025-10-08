using System.Data;
using Dapper;
using INESPRE.Core.Models.Roles;

namespace INESPRE.Core.Services
{
    public interface IRolesService
    {
        Task<int> CreateAsync(Role r);
        Task<IEnumerable<Role>> GetAllAsync();
        Task<Role?> GetByIdAsync(int roleId);
        Task UpdateAsync(Role r);
        Task DeleteAsync(int roleId);
    }

    public sealed class RolesService : IRolesService
    {
        private readonly IDbConnectionFactory _factory;
        public RolesService(IDbConnectionFactory factory) => _factory = factory;

        public async Task<int> CreateAsync(Role r)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters();
            p.Add("@Name", r.Name);
            p.Add("@Description", r.Description);
            p.Add("@IsActive", r.IsActive);
            return await conn.ExecuteScalarAsync<int>("dbo.spRoles_Create", p, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            using var conn = _factory.CreateConnection();
            return await conn.QueryAsync<Role>("dbo.spRoles_Read", commandType: CommandType.StoredProcedure);
        }

        public async Task<Role?> GetByIdAsync(int roleId)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters();
            p.Add("@RoleId", roleId);
            return await conn.QueryFirstOrDefaultAsync<Role>("dbo.spRoles_Read", p, commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateAsync(Role r)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters();
            p.Add("@RoleId", r.RoleId);
            p.Add("@Name", r.Name);
            p.Add("@Description", r.Description);
            p.Add("@IsActive", r.IsActive);
            await conn.ExecuteAsync("dbo.spRoles_Update", p, commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteAsync(int roleId)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters();
            p.Add("@RoleId", roleId);
            await conn.ExecuteAsync("dbo.spRoles_Delete", p, commandType: CommandType.StoredProcedure);
        }
    }
}
