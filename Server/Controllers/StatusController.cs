using System;
using System.Linq;
using System.Threading.Tasks;

using AmnasKitchen.Server.Database;
using AmnasKitchen.Server.Services;

using Microsoft.AspNetCore.Mvc;

namespace AmnasKitchen.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly AmnasKitchenDbContext _databaseContext;
        private readonly IPathProvider _pathProvider;

        public StatusController(AmnasKitchenDbContext databaseContext, IPathProvider pathProvider)
        {
            _databaseContext = databaseContext ?? throw new ArgumentNullException(nameof(databaseContext));
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
                return _databaseContext.Categories.Count().ToString();
            }
            catch
            {
                return "No Connection to Database";
            }
        }

        [HttpGet("[action]")]
        public string GetRootPath()
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
