using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using ApnaBawarchiKhana.Server.Database;
using ApnaBawarchiKhana.Shared;


using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ApnaBawarchiKhana.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ApnaBawarchiKhanaDbContext _dbContext;

        public UsersController(ApnaBawarchiKhanaDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        [HttpGet("[action]")]
        public async Task<IEnumerable<User>> GetAllUsers()
        {
            try
            {
                return await _dbContext.Users.ToListAsync();
            }
            catch
            {
                return null;
            }
        }
    }
}
