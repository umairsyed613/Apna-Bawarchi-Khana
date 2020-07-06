using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using ApnaBawarchiKhana.Server.Database;
using ApnaBawarchiKhana.Shared;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace ApnaBawarchiKhana.Server.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly ApnaBawarchiKhanaDbContext _dbContext;
        private readonly IMemoryCache _memoryCache;
        private readonly IAkImageFileService akImageFileService;

        public RecipeService(IMemoryCache memoryCache, ApnaBawarchiKhanaDbContext dbContext, IAkImageFileService akImageFileService)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            this.akImageFileService = akImageFileService ?? throw new ArgumentNullException(nameof(akImageFileService));
        }

        public async Task<Recipe> GetRecipeById(int recipeId)
        {
            return await _dbContext.Recipes.AsNoTracking().FirstOrDefaultAsync(w => w.Id == recipeId);
        }

        public async Task<IEnumerable<Recipe>> GetAllRecipesByCategoryId(int categoryId)
        {
            return await _dbContext.Recipes.Include(i => i.RecipeCategories).AsNoTracking().Where(w => w.RecipeCategories.Any(a => a.CategoryId == categoryId)).ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            if (_memoryCache.TryGetValue(CacheKeys.AllCategories, out IEnumerable<Category> data))
            {
                return data;
            }
            else
            {
                List<Category> result = null;
                result = await _dbContext.Categories.Include(i => i.UploadedImage).AsNoTracking().ToListAsync();

                if (result == null)
                {
                    return null;
                }

                _memoryCache.Set(CacheKeys.AllCategories, result, DateTimeOffset.UtcNow.AddHours(2));

                return result;
            }
        }

        public async Task CreateRecipe(CreateRecipeFormData recipeFormData)
        {
            if (recipeFormData == null)
            {
                throw new ArgumentNullException(nameof(recipeFormData));
            }

            if (!recipeFormData.Recipe.SelectedCategoriesIds.Any())
            {
                throw new InvalidOperationException("Cannot add recipe without categories");
            }

            using var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.ReadCommitted);

            try
            {
                var recipe = await _dbContext.Recipes.AddAsync(
                                 new Recipe
                                     {
                                         Title = recipeFormData.Recipe.Title,
                                         Description = recipeFormData.Recipe.Description,
                                         Difficulty = recipeFormData.Recipe.Difficulty,
                                         Serving = recipeFormData.Recipe.Serving,
                                         Time = recipeFormData.Recipe.Time,
                                         TimeUnit = recipeFormData.Recipe.TimeUnit,
                                         CreatedAt = DateTime.UtcNow
                                     });

                await _dbContext.SaveChangesAsync();


                foreach (var catId in recipeFormData.Recipe.SelectedCategoriesIds)
                {
                    await _dbContext.RecipeCategories.AddAsync(new RecipeCategory { CategoryId = catId, RecipeId = recipe.Entity.Id });
                }

                await _dbContext.SaveChangesAsync();

                if (recipeFormData.Recipe.Ingredients != null && recipeFormData.Recipe.Ingredients.Any())
                {
                    foreach (var ingredient in recipeFormData.Recipe.Ingredients.OrderBy(o => o.StepNr))
                    {
                        await _dbContext.Ingredients.AddAsync(
                            new Ingredient
                                {
                                    StepNr = ingredient.StepNr, Description = ingredient.Description, Quantity = ingredient.Quantity, RecipeId = recipe.Entity.Id
                                });
                    }

                    await _dbContext.SaveChangesAsync();
                }

                if (recipeFormData.Recipe.Directions != null && recipeFormData.Recipe.Directions.Any())
                {
                    foreach (var direction in recipeFormData.Recipe.Directions.OrderBy(o => o.StepNr))
                    {
                        await _dbContext.Directions.AddAsync(new Direction { StepNr = direction.StepNr, Step = direction.Step, RecipeId = recipe.Entity.Id });
                    }
                    await _dbContext.SaveChangesAsync();
                }

                if (recipeFormData.Images != null && recipeFormData.Images.Any())
                {
                    foreach (var image in recipeFormData.Images)
                    {
                        if (!string.IsNullOrEmpty(image))
                        {
                            var imageData = await akImageFileService.GetFileAsBytes(image);
                            var insertedImageId = await StoreImageInDb(imageData);
                            await _dbContext.RecipeImages.AddAsync(new RecipeImage { ImageId = insertedImageId, RecipeId = recipe.Entity.Id });
                        }
                    }

                    await _dbContext.SaveChangesAsync();
                }

                await transaction.CommitAsync();
            }
            catch
            {

                await transaction.RollbackAsync();
                 throw;
            }
        }

        public async Task CreateCategory(CreateCategoryFormData categoryFormData)
        {
            if (categoryFormData == null)
            {
                throw new ArgumentNullException(nameof(categoryFormData));
            }

            if (categoryFormData.Category == null)
            {
                throw new ArgumentNullException(nameof(categoryFormData.Category));
            }

            int? insertedImageId = null;

            if (!string.IsNullOrEmpty(categoryFormData.IconPath))
            {
                var imageData = await akImageFileService.GetFileAsBytes(categoryFormData.IconPath);
                insertedImageId = await StoreImageInDb(imageData);
            }

            categoryFormData.Category.ImageId = insertedImageId;
            await _dbContext.Categories.AddAsync(categoryFormData.Category);
            await _dbContext.SaveChangesAsync();
            _memoryCache.Remove(CacheKeys.AllCategories);
        }

        public async Task DeleteCategory(int categoryId)
        {
            var category = await _dbContext.Categories.Include(i => i.UploadedImage).FirstOrDefaultAsync(f => f.Id == categoryId);

            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            if (category.ImageId != null)
            {
                _dbContext.UploadedImages.Remove(category.UploadedImage);
            }

            _dbContext.Categories.Remove(category);
            await _dbContext.SaveChangesAsync();
            _memoryCache.Remove(CacheKeys.AllCategories);
        }

        private async Task<int> StoreImageInDb(byte[] imageData)
        {
            var imData = new UploadedImage { ImageData = imageData };
            var uploadedImage = await _dbContext.UploadedImages.AddAsync(imData);
            await _dbContext.SaveChangesAsync();

            return uploadedImage.Entity.Id;
        }
    }
}
