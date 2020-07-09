using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ApnaBawarchiKhana.Server.Services;
using ApnaBawarchiKhana.Shared;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApnaBawarchiKhana.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipeController : ControllerBase, IRecipeService
    {
        private readonly IRecipeService _recipeService;

        public RecipeController(IRecipeService recipeService)
        {
            _recipeService = recipeService ?? throw new ArgumentNullException(nameof(recipeService));
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

        [HttpGet("[action]/{categoryId}")]
        public async Task<IEnumerable<RecipesListByCategory>> GetAllRecipesByCategoryId(int categoryId)
        {
            return await _recipeService.GetAllRecipesByCategoryId(categoryId);
        }

        [HttpGet("[action]/{recipeId}")]
        public async Task<Recipe> GetRecipeById(int recipeId)
        {
            return await _recipeService.GetRecipeById(recipeId);
        }

        [HttpPost("[action]")]
        public async Task CreateRecipe([FromBody] CreateRecipeFormData recipeFormData)
        {
            await _recipeService.CreateRecipe(recipeFormData);
        }
    }
}
