using System;
using System.Linq;
using System.Threading.Tasks;

using ApnaBawarchiKhana.Server.Database;
using ApnaBawarchiKhana.Server.Services;

using EFDbFactory.Sql;

using Microsoft.AspNetCore.Mvc;

namespace ApnaBawarchiKhana.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly IDbFactory _dbFactory;
        private readonly IPathProvider _pathProvider;

        public StatusController(IDbFactory dbFactory, IPathProvider pathProvider)
        {
            _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
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
                return (await _dbFactory.Create()).FactoryFor<ApnaBawarchiKhanaDbContext>().Categories.Count().ToString();
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
