using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using AmnasKitchen.Server.Database;
using AmnasKitchen.Shared;


using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AmnasKitchen.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AmnasKitchenDbContext _dbContext;

        public UsersController(AmnasKitchenDbContext dbContext)
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
