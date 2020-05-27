using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Dapper;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace AmnasKitchen.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public UsersController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public string Get()
        {
            try
            {
#if DEBUG
                var conn = _configuration.GetConnectionString("DATABASE_URL");
#else
                var conn = Environment.GetEnvironmentVariable("DATABASE_URL");
#endif
                using var connection = new SqlConnection(conn);
                connection.Open();

                return connection.QueryFirstOrDefault<string>(@"SELECT top 1 username FROM Tbl_Users");
            }
            catch
            {
                return null;
            }
        }
    }
}
