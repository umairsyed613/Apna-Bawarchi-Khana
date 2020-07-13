
using System.Collections.Generic;

using ApnaBawarchiKhana.Server.Helper;

using Microsoft.AspNetCore.Mvc;

namespace ApnaBawarchiKhana.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        [HttpGet]
        public List<string> GetAllLogs()
        {
            var result = new List<string>();

            while(InMemorySink.Events.TryDequeue(out string item))
            {
                result.Add(item);
            }

            return result;
        }
    }
}
