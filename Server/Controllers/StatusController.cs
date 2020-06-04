using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AmnasKitchen.Server.Services;
using AmnasKitchen.Shared;

using Dapper;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace AmnasKitchen.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly DatabaseConnectionHandler _databaseConnectionHandler;
        private readonly IPathProvider _pathProvider;

        public StatusController(DatabaseConnectionHandler connectionHandler, IPathProvider pathProvider)
        {
            _databaseConnectionHandler = connectionHandler ?? throw new ArgumentNullException(nameof(connectionHandler));
            _pathProvider = pathProvider ?? throw new ArgumentNullException(nameof(pathProvider));
        }

        [HttpGet("[action]")]
        public string PingApi()
        {
            return "Pong";
        }

        [HttpGet("[action]")]
        public async Task<string> PingDb()
        {
            try
            {
                using var connection = new SqlConnection(_databaseConnectionHandler.GetDbConnectionString());
                connection.Open();
                var data = await connection.QueryAsync<User>(@"SELECT * FROM sa_amna.Users");

                return data?.Count().ToString() ?? "No Data but have connection to Db";
            }
            catch
            {
                return "No Connection to Database";
            }
        }

        [HttpGet("[action]")]
        public async Task<string> GetRootPath()
        {
            try
            {
                return _pathProvider.GetRootPath();
            }
            catch
            {
                return "Failed to get Root Path";
            }
        }
    }
}
