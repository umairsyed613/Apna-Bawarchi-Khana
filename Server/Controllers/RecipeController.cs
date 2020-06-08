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

        [HttpGet("[action]")]
        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            return await _recipeService.GetAllCategories();
        }

        [HttpPost("[action]")]
        public async Task CreateCategory([FromBody] CreateCategoryFormData categoryFormData)
        {
            await _recipeService.CreateCategory(categoryFormData);
        }
        
        [HttpDelete("[action]/{categoryId}")]
        public async Task DeleteCategory(int categoryId)
        {
            await _recipeService.DeleteCategory(categoryId);
        }

        [HttpPost("[action]")]
        public async Task CreateRecipe([FromBody] CreateRecipeFormData recipeFormData)
        {
            await _recipeService.CreateRecipe(recipeFormData);
        }
    }
}