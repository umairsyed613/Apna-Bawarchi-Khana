using Microsoft.Extensions.Configuration;
using System;
using System.Data;

using AmnasKitchen.Shared;

using Dapper;

using Microsoft.Data.SqlClient;

namespace AmnasKitchen.Server.Services
{
    public class DatabaseConnectionHandler
    {
        private readonly IConfiguration _configuration;
        public DatabaseConnectionHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetDbConnectionString()
        {
#if DEBUG
            var conn = _configuration.GetConnectionString("DATABASE_URL");
#else
                var conn = Environment.GetEnvironmentVariable("DATABASE_URL");
#endif
            return conn;
        }

        public IDbConnection GetDbConnection()
        {
            try
            {
                return new SqlConnection(GetDbConnectionString());
            }
            catch
            {
                return null;
            }
        }
    }
}
