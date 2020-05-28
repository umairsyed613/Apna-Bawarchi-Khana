using System;
using System.Collections.Generic;
using System.Text;

namespace AmnasKitchen.Shared
{
    public partial class Categories
    {
        public Categories()
        {
            this.Recipes = new HashSet<Recipes>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<Recipes> Recipes { get; set; }
    }

    public partial class Directions
    {
        public int Id { get; set; }

        public int RecipeId { get; set; }

        public int StepNr { get; set; }

        public string Step { get; set; }

        public Recipes Recipe { get; set; }
    }

    public partial class Ingredients
    {
        public int Id { get; set; }

        public int RecipeId { get; set; }

        public int StepNr { get; set; }

        public string Quantity { get; set; }

        public string Description { get; set; }

        public Recipes Recipe { get; set; }
    }

    public partial class Recipes
    {
        public Recipes()
        {
            this.Directions = new HashSet<Directions>();
            this.Ingredients = new HashSet<Ingredients>();
        }

        public int Id { get; set; }

        public int CategoryId { get; set; }

        public int Time { get; set; }

        public string TimeUnit { get; set; }

        public int Difficulty { get; set; }

        public int Serving { get; set; }

        public Categories Category { get; set; }

        public ICollection<Directions> Directions { get; set; }

        public ICollection<Ingredients> Ingredients { get; set; }
    }

    public partial class Users
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }
    }

}
