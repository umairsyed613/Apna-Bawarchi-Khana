using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AmnasKitchen.Shared;

using Dapper;

using Microsoft.Extensions.Caching.Memory;

namespace AmnasKitchen.Server.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly DatabaseConnectionHandler _databaseConnectionHandler;
        private readonly IMemoryCache _memoryCache;

        public RecipeService(IMemoryCache memoryCache ,DatabaseConnectionHandler connectionHandler)
        {
            _databaseConnectionHandler = connectionHandler ?? throw new ArgumentNullException(nameof(connectionHandler));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        public async Task<Recipe> GetRecipeById(int recipeId)
        {
            using var connection = _databaseConnectionHandler.GetDbConnection();
            connection.Open();

            return await connection.QueryFirstOrDefaultAsync<Recipe>("Select * from sa_amna.Recipe where Id = " + recipeId);
        }

        public async Task<IEnumerable<Recipe>> GetAllRecipesByCategoryId(int categoryId)
        {
            using var connection = _databaseConnectionHandler.GetDbConnection();
            connection.Open();

            return await connection.QueryAsync<Recipe>("Select * from sa_amna.Recipe where CategoryId = " + categoryId);
        }

        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            if (_memoryCache.TryGetValue(CacheKeys.AllCategories, out IEnumerable<Category> data))
            {
                return data;
            }
            else
            {
                using var connection = _databaseConnectionHandler.GetDbConnection();
                connection.Open();

                var result = await connection.QueryAsync<Category>("Select * from sa_amna.Categories");
                _memoryCache.Set(CacheKeys.AllCategories, result, DateTimeOffset.UtcNow.AddHours(2));
                return result;
            }
        }

        public async Task CreateRecipe(Recipe recipe)
        {
            if (recipe == null)
            {
                throw new ArgumentNullException(nameof(recipe));
            }

            using var connection = _databaseConnectionHandler.GetDbConnection();
            connection.Open();

            var sql = "INSERT INTO sa_amna.Recipe (CategoryId, Time, TimeUnit, Difficulty, Serving) Values (@categoryId, @time, @timeUnit, @difficulty, @serving)";
            
            await connection.ExecuteAsync(sql, new {recipe.CategoryId, recipe.Time, recipe.TimeUnit, recipe.Difficulty, recipe.Serving});
        }

        public async Task CreateCategory(Category category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            using var connection = _databaseConnectionHandler.GetDbConnection();
            connection.Open();

            var sql = "INSERT INTO sa_amna.Categories (Name, Description, ImageUrl) VALUES(@name, @description, @imageUrl)";

            await connection.ExecuteAsync(sql, new { category.Name, category.Description , category.ImageUrl });
            _memoryCache.Remove(CacheKeys.AllCategories);
        }

        public async Task DeleteCategory(int categoryId)
        {
            using var connection = _databaseConnectionHandler.GetDbConnection();
            connection.Open();

            var sql = "Delete from sa_amna.Categories Where Id = @id";

            await connection.ExecuteAsync(sql, new { id = categoryId });

            _memoryCache.Remove(CacheKeys.AllCategories);
        }
    }
}
