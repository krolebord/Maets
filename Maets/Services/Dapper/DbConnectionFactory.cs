using System.Data;
using Maets.Attributes;
using Microsoft.Data.SqlClient;

namespace Maets.Services.Dapper;

[Dependency]
public class DbConnectionFactory
{
    private readonly string _authConnectionString;

    private readonly string _dataConnectionString;

    public DbConnectionFactory(IConfiguration configuration)
    {
        _authConnectionString = configuration.GetConnectionString("AuthConnection");
        _dataConnectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task<IDbConnection> GetAuthConnection()
    {
        var connection = new SqlConnection(_authConnectionString);
        await connection.OpenAsync();
        return connection;
    }
    
    public async Task<IDbConnection> GetDataConnection()
    {
        var connection = new SqlConnection(_dataConnectionString);
        await connection.OpenAsync();
        return connection;
    }
}
