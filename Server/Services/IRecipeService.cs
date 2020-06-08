using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AmnasKitchen.Shared;

namespace AmnasKitchen.Server.Services
{
    public interface IRecipeService
    {
        Task<Recipe> GetRecipeById(int recipeId);

        Task<IEnumerable<Recipe>> GetAllRecipesByCategoryId(int categoryId);

        Task<IEnumerable<Category>> GetAllCategories();

        Task CreateRecipe(CreateRecipeFormData recipeFormData);

        Task CreateCategory(CreateCategoryFormData categoryFormData);

        Task DeleteCategory(int categoryId);
    }
}
