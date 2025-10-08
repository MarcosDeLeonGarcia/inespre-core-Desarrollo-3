using System.Data;
using Dapper;
using INESPRE.Core.Models.Payments;

namespace INESPRE.Core.Services
{
    public interface IPaymentsService
    {
        Task<int> CreateAsync(Payment p);
        Task<IEnumerable<Payment>> GetAllAsync();
        Task<IEnumerable<Payment>> GetByPOAsync(int poId);
        Task<Payment?> GetByIdAsync(int paymentId);
        Task UpdateAsync(Payment p);
        Task DeleteAsync(int paymentId);
    }

    public sealed class PaymentsService : IPaymentsService
    {
        private readonly IDbConnectionFactory _factory;
        public PaymentsService(IDbConnectionFactory factory) => _factory = factory;

        public async Task<int> CreateAsync(Payment pay)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters();
            p.Add("@POId", pay.POId);
            p.Add("@Amount", pay.Amount);
            p.Add("@Method", pay.Method);
            p.Add("@Status", pay.Status);
            p.Add("@Notes", pay.Notes);
            return await conn.ExecuteScalarAsync<int>("dbo.spPayments_Create", p, commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Payment>> GetAllAsync()
        {
            using var conn = _factory.CreateConnection();
            return await conn.QueryAsync<Payment>("dbo.spPayments_Read", commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<Payment>> GetByPOAsync(int poId)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters();
            p.Add("@POId", poId);
            return await conn.QueryAsync<Payment>("dbo.spPayments_Read", p, commandType: CommandType.StoredProcedure);
        }

        public async Task<Payment?> GetByIdAsync(int paymentId)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters();
            p.Add("@PaymentId", paymentId);
            return await conn.QueryFirstOrDefaultAsync<Payment>("dbo.spPayments_Read", p, commandType: CommandType.StoredProcedure);
        }

        public async Task UpdateAsync(Payment pay)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters();
            p.Add("@PaymentId", pay.PaymentId);
            p.Add("@Amount", pay.Amount);
            p.Add("@Method", pay.Method);
            p.Add("@Status", pay.Status);
            p.Add("@Notes", pay.Notes);
            await conn.ExecuteAsync("dbo.spPayments_Update", p, commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteAsync(int paymentId)
        {
            using var conn = _factory.CreateConnection();
            var p = new DynamicParameters();
            p.Add("@PaymentId", paymentId);
            await conn.ExecuteAsync("dbo.spPayments_Delete", p, commandType: CommandType.StoredProcedure);
        }
    }
}
