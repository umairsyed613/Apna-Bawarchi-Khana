using System;
using System.Linq;
using System.Threading.Tasks;

using ApnaBawarchiKhana.Server.Database;
using ApnaBawarchiKhana.Server.Services;

using Microsoft.AspNetCore.Mvc;

namespace ApnaBawarchiKhana.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly ApnaBawarchiKhanaDbContext _databaseContext;
        private readonly IPathProvider _pathProvider;

        public StatusController(ApnaBawarchiKhanaDbContext databaseContext, IPathProvider pathProvider)
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
        public string PingDb()
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
