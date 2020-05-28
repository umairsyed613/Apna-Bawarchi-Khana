using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using AmnasKitchen.Server.Services;
using AmnasKitchen.Shared;

using Dapper;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace AmnasKitchen.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly DatabaseConnectionHandler _databaseConnectionHandler;

        public UsersController(DatabaseConnectionHandler connectionHandler)
        {
            _databaseConnectionHandler = connectionHandler ?? throw new ArgumentNullException(nameof(connectionHandler));
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<User>> GetAllUsers()
        {
            try
            {
                using var connection = new SqlConnection(_databaseConnectionHandler.GetDbConnectionString());
                connection.Open();

                return await connection.QueryAsync<User>(@"SELECT * FROM sa_amna.Users");
            }
            catch
            {
                return null;
            }
        }
    }
}
