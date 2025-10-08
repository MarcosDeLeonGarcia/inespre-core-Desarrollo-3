using System.Data;
using System.Text.Json;
using Dapper;
using INESPRE.Core.Models.Cash;

namespace INESPRE.Core.Services
{
    public interface ICashService
    {
        Task<int> OpenAsync(CashOpenRequest req);
        Task<IEnumerable<dynamic>> SessionsAsync(DateTime? from, DateTime? to, int? userId);
        Task<(dynamic header, IEnumerable<dynamic> totals)> SessionDetailAsync(int cashSessionId);
        Task<IEnumerable<dynamic>> CloseAsync(int cashSessionId, CashCloseRequest req);

        Task<int> AddMovementAsync(int cashSessionId, CashMovementRequest req);
        Task<int> PayPOAsync(int cashSessionId, int poId, decimal amount, string method, string? notes);
        Task<IEnumerable<dynamic>> MovementsAsync(int cashSessionId);
    }

    public sealed class CashService : ICashService
    {
        private readonly IDbConnectionFactory _factory;
        public CashService(IDbConnectionFactory factory) => _factory = factory;

        public async Task<int> OpenAsync(CashOpenRequest req)
        {
            using var conn = _factory.CreateConnection();
            var p = new Dapper.DynamicParameters();
            p.Add("@UserId", req.UserId);
            p.Add("@EventId", req.EventId);
            p.Add("@OpeningAmount", req.OpeningAmount);
            p.Add("@Notes", req.Notes);
            return await conn.ExecuteScalarAsync<int>("dbo.spCash_OpenSession", p, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<dynamic>> SessionsAsync(DateTime? from, DateTime? to, int? userId)
        {
            using var conn = _factory.CreateConnection();
            return await conn.QueryAsync("dbo.spCash_Sessions",
                new { From = from, To = to, UserId = userId },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<(dynamic header, IEnumerable<dynamic> totals)> SessionDetailAsync(int cashSessionId)
        {
            using var conn = _factory.CreateConnection();
            using var m = await conn.QueryMultipleAsync("dbo.spCash_SessionDetail",
                new { CashSessionId = cashSessionId }, commandType: CommandType.StoredProcedure);
            var header = await m.ReadFirstOrDefaultAsync();
            var totals = await m.ReadAsync();
            return (header!, totals);
        }

        public async Task<IEnumerable<dynamic>> CloseAsync(int cashSessionId, CashCloseRequest req)
        {
            using var conn = _factory.CreateConnection();
            var json = JsonSerializer.Serialize(req.Totals.Select(t => new { paymentMethod = t.PaymentMethod, counted = t.Counted }));
            var p = new Dapper.DynamicParameters();
            p.Add("@CashSessionId", cashSessionId);
            p.Add("@ClosingAmount", req.ClosingAmount);
            p.Add("@TotalsJson", json);
            return await conn.QueryAsync("dbo.spCash_CloseSession", p, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> AddMovementAsync(int cashSessionId, CashMovementRequest req)
        {
            using var conn = _factory.CreateConnection();
            var p = new Dapper.DynamicParameters();
            p.Add("@CashSessionId", cashSessionId);
            p.Add("@Method", req.Method);
            p.Add("@Direction", req.Direction);
            p.Add("@Category", req.Category);
            p.Add("@Amount", req.Amount);
            p.Add("@RefType", req.RefType);
            p.Add("@RefId", req.RefId);
            p.Add("@Notes", req.Notes);
            return await conn.ExecuteScalarAsync<int>("dbo.spCash_AddMovement", p, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> PayPOAsync(int cashSessionId, int poId, decimal amount, string method, string? notes)
        {
            using var conn = _factory.CreateConnection();
            var p = new Dapper.DynamicParameters();
            p.Add("@CashSessionId", cashSessionId);
            p.Add("@POId", poId);
            p.Add("@Amount", amount);
            p.Add("@Method", method);
            p.Add("@Notes", notes);
            return await conn.ExecuteScalarAsync<int>("dbo.spCash_PayPO", p, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<dynamic>> MovementsAsync(int cashSessionId)
        {
            using var conn = _factory.CreateConnection();
            return await conn.QueryAsync("dbo.spCash_Movements",
                new { CashSessionId = cashSessionId }, commandType: CommandType.StoredProcedure);
        }
    }
}
