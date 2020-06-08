using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AmnasKitchen.Shared
{
    [Table("sa_amna.Category")]
    public partial class Category
    {
        public Category()
        {
            this.RecipeCategories = new HashSet<RecipeCategory>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }

        [MaxLength(512)]
        [StringLength(512)]
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [MaxLength(512)]
        [StringLength(512)]
        public string Description { get; set; }

        public int? ImageId { get; set; }

        public UploadedImage UploadedImage { get; set; }

        public ICollection<RecipeCategory> RecipeCategories { get; set; }
    }

    [Table("sa_amna.Direction")]
    public partial class Direction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Recipe Id is required")]
        public int RecipeId { get; set; }

        [Required(ErrorMessage = "Step Nr is required")]
        public int StepNr { get; set; }

        [MaxLength]
        [Required(ErrorMessage = "Step is required")]
        public string Step { get; set; }

        public Recipe Recipe { get; set; }
    }

    [Table("sa_amna.Ingredient")]
    public partial class Ingredient
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Recipe Id is required")]
        public int RecipeId { get; set; }

        [Required(ErrorMessage = "Step Nr is required")]
        public int StepNr { get; set; }

        [MaxLength(50)]
        [StringLength(50)]
        [Required(ErrorMessage = "Quantity is required")]
        public string Quantity { get; set; }

        [MaxLength]
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        public Recipe Recipe { get; set; }
    }

    [Table("sa_amna.Recipe")]
    public partial class Recipe
    {
        public Recipe()
        {
            this.Directions = new HashSet<Direction>();
            this.Ingredients = new HashSet<Ingredient>();
            this.RecipeCategories = new HashSet<RecipeCategory>();
            this.RecipeImages = new HashSet<RecipeImage>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Time is required")]
        public int Time { get; set; }

        [MaxLength(1)]
        [StringLength(1)]
        [Required(ErrorMessage = "Time Unit is required")]
        public string TimeUnit { get; set; }

        [Required(ErrorMessage = "Difficulty is required")]
        public int Difficulty { get; set; }

        [Required(ErrorMessage = "Serving is required")]
        public int Serving { get; set; }

        [MaxLength(512)]
        [StringLength(512)]
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        [MaxLength]
        public string Description { get; set; }

        [Required(ErrorMessage = "Created At is required")]
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public ICollection<Direction> Directions { get; set; }

        public ICollection<Ingredient> Ingredients { get; set; }

        public ICollection<RecipeCategory> RecipeCategories { get; set; }

        public ICollection<RecipeImage> RecipeImages { get; set; }
    }

    [Table("sa_amna.RecipeCategory")]
    public partial class RecipeCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Recipe Id is required")]
        public int RecipeId { get; set; }

        [Required(ErrorMessage = "Category Id is required")]
        public int CategoryId { get; set; }

        public Recipe Recipe { get; set; }

        public Category Category { get; set; }
    }

    [Table("sa_amna.RecipeImage")]
    public partial class RecipeImage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Recipe Id is required")]
        public int RecipeId { get; set; }

        [Required(ErrorMessage = "Image Id is required")]
        public int ImageId { get; set; }

        public Recipe Recipe { get; set; }

        public UploadedImage UploadedImage { get; set; }
    }

    [Table("sa_amna.UploadedImage")]
    public partial class UploadedImage
    {
        public UploadedImage()
        {
            this.Categories = new HashSet<Category>();
            this.RecipeImages = new HashSet<RecipeImage>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }

        [MaxLength]
        [Required(ErrorMessage = "Image Data is required")]
        public byte[] ImageData { get; set; }

        public ICollection<Category> Categories { get; set; }

        public ICollection<RecipeImage> RecipeImages { get; set; }
    }

    [Table("sa_amna.User")]
    public partial class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }

        [MaxLength(512)]
        [StringLength(512)]
        [Required(ErrorMessage = "User Name is required")]
        public string UserName { get; set; }

        [MaxLength(512)]
        [StringLength(512)]
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        [MaxLength(512)]
        [StringLength(512)]
        public string Email { get; set; }
    }

}
