using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace AmnasKitchen.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public StatusController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("[action]")]
        public string GetDbConn()
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