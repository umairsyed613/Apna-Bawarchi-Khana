using Microsoft.Extensions.Configuration;

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
    }
}
