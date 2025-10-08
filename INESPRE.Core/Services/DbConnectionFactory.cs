using System.Data;
using Microsoft.Data.SqlClient;

namespace INESPRE.Core.Services;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}

public sealed class SqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connString;

    public SqlConnectionFactory(IConfiguration config)
    {
        _connString = config.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection no configurada");
    }

    public IDbConnection CreateConnection() => new SqlConnection(_connString);
}
