using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AmnasKitchen.Server.Services;
using AmnasKitchen.Shared;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AmnasKitchen.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipeController : ControllerBase
    {
        private readonly IRecipeService _recipeService;
        public RecipeController(IRecipeService recipeService)
        {
            _recipeService = recipeService?? throw new ArgumentNullException(nameof(recipeService));
        }

        [HttpPost("[action]")]
        public async Task CreateCategory([FromBody] Category category)
        {
            await _recipeService.CreateCategory(category);
        }

        [HttpPost("[action]")]
        public async Task CreateRecipe([FromBody] Recipe recipe)
        {
            await _recipeService.CreateRecipe(recipe);
        }
    }
}