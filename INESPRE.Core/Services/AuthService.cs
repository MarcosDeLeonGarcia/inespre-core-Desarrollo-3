using System.Data;
using Dapper;
using INESPRE.Core.Models.Users;

namespace INESPRE.Core.Services
{
    public interface IAuthService
    {
        Task<int> RegisterAsync(RegisterRequest dto);
        Task<LoginResponse> LoginAsync(LoginRequest dto);
        Task ChangePasswordAsync(int userId, string newPassword);
    }

    public sealed class AuthService : IAuthService
    {
        private readonly IDbConnectionFactory _factory;
        public AuthService(IDbConnectionFactory factory) => _factory = factory;

        public async Task<int> RegisterAsync(RegisterRequest dto)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters();
            p.Add("@Username", dto.Username);
            p.Add("@Email", dto.Email);
            p.Add("@FullName", dto.FullName);
            p.Add("@Phone", dto.Phone);
            p.Add("@RoleId", dto.RoleId);
            p.Add("@PlainPassword", dto.Password);
            return await conn.ExecuteScalarAsync<int>("dbo.spUsers_Create", p, commandType: CommandType.StoredProcedure);
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest dto)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters();
            p.Add("@Username", dto.Username);
            p.Add("@PlainPassword", dto.Password);
            return await conn.QueryFirstOrDefaultAsync<LoginResponse>("dbo.spUsers_ValidateLogin", p, commandType: CommandType.StoredProcedure)
                   ?? new LoginResponse { IsValid = false };
        }

        public async Task ChangePasswordAsync(int userId, string newPassword)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters();
            p.Add("@UserId", userId);
            p.Add("@PlainPassword", newPassword);
            await conn.ExecuteAsync("dbo.spUsers_ChangePassword", p, commandType: CommandType.StoredProcedure);
        }
    }
}
