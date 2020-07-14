using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using ApnaBawarchiKhana.Server.Database;
using ApnaBawarchiKhana.Shared;

using EFDbFactory.Sql;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApnaBawarchiKhana.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IDbFactory _dbFactory;

        public UsersController(IDbFactory dbFactory)
        {
            _dbFactory = dbFactory ?? throw new ArgumentNullException(nameof(dbFactory));
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<User>> GetAllUsers()
        {
            try
            {
                using var factory = await _dbFactory.Create();
                return await factory.FactoryFor<ApnaBawarchiKhanaDbContext>().Users.ToListAsync();
            }
            catch
            {
                return null;
            }
        }
    }
}
