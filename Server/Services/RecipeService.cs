using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using ApnaBawarchiKhana.Server.Database;
using ApnaBawarchiKhana.Shared;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Caching.Memory;

using Serilog;

namespace ApnaBawarchiKhana.Server.Services
{
    public class RecipeService : IRecipeService
    {
        private static readonly Serilog.ILogger _logger = Log.ForContext<RecipeService>();

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
            var key = CacheKeys.Recipe + "_" + recipeId;

            if (_memoryCache.TryGetValue(key, out Recipe data))
            {
                return data;
            }
            else
            {
                var result = await _dbContext.Recipes.Include(i => i.RecipeCategories).Include(i => i.Ingredients).Include(d => d.Directions).Include(r => r.RecipeRatings)
                                   .Include(i => i.RecipeImages)
                                   .ThenInclude(i => i.UploadedImage)
                                   .AsNoTracking().Where(w => w.Id == recipeId).FirstOrDefaultAsync();

                if (result == null)
                {
                    throw new InvalidOperationException("No recipe was found with provided Id");
                }

                _memoryCache.Set(key, result, DateTimeOffset.UtcNow.AddHours(2));

                return result;
            }
        }

        public async Task<IEnumerable<RecipesListByCategory>> GetAllRecipesByCategoryId(int categoryId)
        {
            _logger.Information("Fetching Recipes By Category {CategoryId}", categoryId);

            var key = CacheKeys.RecipesByCatId + "_" + categoryId;

            if (_memoryCache.TryGetValue(key, out IEnumerable<RecipesListByCategory> data))
            {
                if(data != null || !data.Any())
                {
                    return data;
                }

                _memoryCache.Remove(key);
            }

            var result = await _dbContext.Recipes.Include(i => i.RecipeCategories).Include(r => r.RecipeRatings)
                               .Include(i => i.RecipeImages)
                               .ThenInclude(i => i.UploadedImage)
                               .AsNoTracking().OrderByDescending(o => o.CreatedAt).Where(w => w.RecipeCategories.Any(a => a.CategoryId == categoryId))
                               .Select(r => new RecipesListByCategory
                               {
                                   RecipeId = r.Id,
                                   Description = r.Description,
                                   Title = r.Title,
                                   Thumbnail = r.RecipeImages.Any() ? r.RecipeImages.First().UploadedImage.ImageData : null,
                                   Ratings = AverageCalculator.GetAverageRating(r.RecipeRatings.Select(s => s.Rating).ToList())
                               }).ToListAsync();

            if (result == null)
            {
                return null;
            }

            _memoryCache.Set(key, result, DateTimeOffset.UtcNow.AddHours(1));

            return result;

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

                result = result.OrderBy(o => o.Name).ToList();

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
                    foreach (var image in recipeFormData.Images.Where(w => !string.IsNullOrEmpty(w)).ToList())
                    {
                        var imageData = await akImageFileService.GetFileAsBytes(image);
                        var insertedImageId = await StoreImageInDb(akImageFileService.ResizeImage(imageData));
                        await _dbContext.RecipeImages.AddAsync(new RecipeImage { ImageId = insertedImageId, RecipeId = recipe.Entity.Id });
                    }

                    await _dbContext.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                
                _logger.Information("Recipe: {RecipeTitle} has been created sucessfully.", recipeFormData.Recipe.Title);

            }
            catch(Exception ee)
            {
                await transaction.RollbackAsync();
                _logger.Error(ee, "Failed to Create Recipe");
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

            _logger.Information("Category {CategoryId} has been removed.", categoryId);
        }

        private async Task<int> StoreImageInDb(byte[] imageData)
        {
            var imData = new UploadedImage { ImageData = imageData };
            var uploadedImage = await _dbContext.UploadedImages.AddAsync(imData);
            await _dbContext.SaveChangesAsync();

            return uploadedImage.Entity.Id;
        }

        public async Task DeleteRecipe(int recipeId)
        {
            var recipe = await _dbContext.Recipes.Include(i => i.RecipeImages).Include(c => c.RecipeCategories).FirstOrDefaultAsync(f => f.Id == recipeId);

            if (recipe == null)
            {
                throw new ArgumentNullException(nameof(recipe));
            }

            var uploadImagesIds = recipe.RecipeImages.Select(s => s.ImageId).ToList();
            var recipeCatIds = recipe.RecipeCategories.Select(s => s.CategoryId).ToList();

            _dbContext.Recipes.Remove(recipe);


            if (uploadImagesIds.Count > 0)
            {
                var uploadedImages = await _dbContext.UploadedImages.Where(w => uploadImagesIds.Contains(w.Id)).ToListAsync();

                _dbContext.UploadedImages.RemoveRange(uploadedImages);
            }

            await _dbContext.SaveChangesAsync();

            _memoryCache.Remove(CacheKeys.Recipe + "_" + recipe.Id);

            if(recipeCatIds.Count > 0)
            {
                foreach(var catId in recipeCatIds)
                {
                    _memoryCache.Remove(CacheKeys.RecipesByCatId + "_" + catId);
                }
            }

            _logger.Information("Recipe {RecipeId} has been removed.", recipeId);
        }

        public async Task StoreRecipeRating(RecipeRatingForm recipeRatingForm)
        {
            if (recipeRatingForm == null)
            {
                throw new ArgumentNullException(nameof(recipeRatingForm));
            }

            if (recipeRatingForm.RecipeId < 1 || recipeRatingForm.RatingValue > 5)
            {
                throw new InvalidOperationException("Rating cannot be less than 1 and greater than 5");
            }

            await _dbContext.RecipeRatings.AddAsync(new RecipeRating { RecipeId = recipeRatingForm.RecipeId, Rating = recipeRatingForm.RatingValue });
            await _dbContext.SaveChangesAsync();
        }
    }
}
