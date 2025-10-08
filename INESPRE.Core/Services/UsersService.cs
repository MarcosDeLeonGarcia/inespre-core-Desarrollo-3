using System.Data;
using Dapper;
using INESPRE.Core.Models.Users;

namespace INESPRE.Core.Services
{
    public interface IUsersService
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int userId);
        Task UpdateAsync(User u);
        Task DisableAsync(int userId); // borrado lógico
    }

    public sealed class UsersService : IUsersService
    {
        private readonly IDbConnectionFactory _factory;

        // Constructor que inicializa el factory para la conexión a la base de datos
        public UsersService(IDbConnectionFactory factory) => _factory = factory;

        // Obtener todos los usuarios
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            using var conn = _factory.CreateConnection();
            return await conn.QueryAsync<User>("dbo.spUsers_Read", commandType: CommandType.StoredProcedure);
        }

        // Obtener un usuario por su ID
        public async Task<User?> GetByIdAsync(int userId)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters();
            p.Add("@UserId", userId);
            return await conn.QueryFirstOrDefaultAsync<User>("dbo.spUsers_Read", p, commandType: CommandType.StoredProcedure);
        }

        // Actualizar un usuario
        public async Task UpdateAsync(User u)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters();
            p.Add("@id", u.UserId);  // Usamos el UserId para identificar al usuario a actualizar
            p.Add("@email", u.Email);
            p.Add("@userName", u.Username);
            p.Add("@fullName", u.FullName);
            p.Add("@phone", u.Phone);
            p.Add("@roleId", u.RoleId);
            p.Add("@isActive", u.IsActive);

            await conn.ExecuteAsync("dbo.spUsers_Update", p, commandType: CommandType.StoredProcedure);
        }

        // Deshabilitar un usuario (borrado lógico)
        public async Task DisableAsync(int userId)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters();
            p.Add("@UserId", userId);

            await conn.ExecuteAsync("dbo.spUsers_Delete", p, commandType: CommandType.StoredProcedure);
        }
    }
}
