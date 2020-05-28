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

        Task CreateRecipe(Recipe Recipe);

        Task CreateCategory(Category category);
    }
}
