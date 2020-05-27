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
        public StatusController()
        {
        }

        [HttpGet("[action]")]
        public string PingApi()
        {
            return "Pong";
        }
    }
}
