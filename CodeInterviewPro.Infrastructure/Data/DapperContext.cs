using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;

public class DapperContext
{
    private readonly IConfiguration _configuration;

    public DapperContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public IDbConnection CreateConnection()
        => new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
}