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
        private readonly IAkImageFileService akImageFileService;

        public RecipeService(IMemoryCache memoryCache, DatabaseConnectionHandler connectionHandler, IAkImageFileService akImageFileService)
        {
            _databaseConnectionHandler = connectionHandler ?? throw new ArgumentNullException(nameof(connectionHandler));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            this.akImageFileService = akImageFileService ?? throw new ArgumentNullException(nameof(akImageFileService));
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
                var sql = "Select c.*, ui.* from sa_amna.Categories c inner join sa_amna.UploadedImages ui on c.ImageId = ui.Id";

                var result = await connection.QueryAsync<Category, UploadedImage, Category>(
                                 sql,
                                 map: (c, u) =>
                                     {
                                         c.UploadedImage = u;

                                         return c;
                                     },
                                 splitOn: "ImageId");

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

            await connection.ExecuteAsync(
                sql,
                new
                    {
                        recipe.CategoryId,
                        recipe.Time,
                        recipe.TimeUnit,
                        recipe.Difficulty,
                        recipe.Serving
                    });
        }

        public async Task CreateCategory(CreateCategoryFormData categoryFormData)
        {
            if (categoryFormData == null)
            {
                throw new ArgumentNullException(nameof(categoryFormData));
            }

            using var connection = _databaseConnectionHandler.GetDbConnection();
            connection.Open();
            int? insertedImageId = null;

            if (!string.IsNullOrEmpty(categoryFormData.IconPath))
            {
                var imageData = await akImageFileService.GetFileAsBytes(categoryFormData.IconPath);
                insertedImageId = await StoreImageInDb(imageData);
            }

            var sql = "INSERT INTO sa_amna.Categories (Name, Description, ImageId) VALUES(@name, @description, @imageId)";
            await connection.ExecuteAsync(sql, new { name = categoryFormData.Category.Name, description = categoryFormData.Category.Description, imageId = insertedImageId });
            _memoryCache.Remove(CacheKeys.AllCategories);
        }

        public async Task DeleteCategory(int categoryId)
        {
            using var connection = _databaseConnectionHandler.GetDbConnection();
            connection.Open();
            var category = await connection.QuerySingleAsync<Category>("select * from sa_amna.Categories Where Id = @id", new { id = categoryId });

            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            var sql = "Delete from sa_amna.Categories Where Id = @id";

            await connection.ExecuteAsync(sql, new { id = categoryId });

            if (category.ImageId != null)
            {
                await connection.ExecuteAsync("Delete from sa_amna.UploadedImages Where Id = @id", new { id = category.ImageId });
            }

            _memoryCache.Remove(CacheKeys.AllCategories);
        }

        private async Task<int> StoreImageInDb(byte[] imageData)
        {
            using var connection = _databaseConnectionHandler.GetDbConnection();
            connection.Open();

            return await connection.QuerySingleAsync<int>(
                       @"INSERT INTO sa_amna.UploadedImages (ImageData) VALUES (@imageData); SELECT CAST(SCOPE_IDENTITY() as int)",
                       new { imageData });
        }
    }
}
