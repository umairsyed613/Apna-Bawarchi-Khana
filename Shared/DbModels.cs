using System;
using System.Collections.Generic;

namespace AmnasKitchen.Shared
{
    public partial class Category
    {
        public Category()
        {
            this.Recipes = new HashSet<Recipe>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int? ImageId { get; set; }

        public UploadedImage UploadedImage { get; set; }

        public ICollection<Recipe> Recipes { get; set; }
    }

    public partial class Direction
    {
        public int Id { get; set; }

        public int RecipeId { get; set; }

        public int StepNr { get; set; }

        public string Step { get; set; }

        public Recipe Recipe { get; set; }
    }

    public partial class Ingredient
    {
        public int Id { get; set; }

        public int RecipeId { get; set; }

        public int StepNr { get; set; }

        public string Quantity { get; set; }

        public string Description { get; set; }

        public Recipe Recipe { get; set; }
    }

    public partial class RecipeImage
    {
        public int Id { get; set; }

        public int RecipeId { get; set; }

        public int ImageId { get; set; }

        public Recipe Recipe { get; set; }

        public UploadedImage UploadedImage { get; set; }
    }

    public partial class Recipe
    {
        public Recipe()
        {
            this.Directions = new HashSet<Direction>();
            this.Ingredients = new HashSet<Ingredient>();
            this.RecipeImages = new HashSet<RecipeImage>();
        }

        public int Id { get; set; }

        public int CategoryId { get; set; }

        public int Time { get; set; }

        public string TimeUnit { get; set; }

        public int Difficulty { get; set; }

        public int Serving { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public Category Category { get; set; }

        public ICollection<Direction> Directions { get; set; }

        public ICollection<Ingredient> Ingredients { get; set; }

        public ICollection<RecipeImage> RecipeImages { get; set; }
    }

    public partial class UploadedImage
    {
        public UploadedImage()
        {
            this.Categories = new HashSet<Category>();
            this.RecipeImages = new HashSet<RecipeImage>();
        }

        public int Id { get; set; }

        public byte[] ImageData { get; set; }

        public ICollection<Category> Categories { get; set; }

        public ICollection<RecipeImage> RecipeImages { get; set; }
    }

    public partial class User
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }
    }
}
