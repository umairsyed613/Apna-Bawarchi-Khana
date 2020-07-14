using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ApnaBawarchiKhana.Shared;

namespace ApnaBawarchiKhana.Server.Services
{
    public interface IRecipeService
    {
        Task<Recipe> GetRecipeById(int recipeId);

        Task<IEnumerable<RecipesListByCategory>> GetAllRecipesByCategoryId(int categoryId);

        Task<IEnumerable<Category>> GetAllCategories();

        Task CreateRecipe(CreateRecipeFormData recipeFormData);

        Task DeleteRecipe(int recipeId);

        Task CreateCategory(CreateCategoryFormData categoryFormData);

        Task DeleteCategory(int categoryId);

        Task StoreRecipeRating(RecipeRatingForm recipeRatingForm);
    }
}
