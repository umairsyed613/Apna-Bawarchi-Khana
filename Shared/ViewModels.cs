using System.Collections.Generic;

namespace ApnaBawarchiKhana.Shared
{
    public class RecipesListByCategory
    {
        public int RecipeId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public byte[] Thumbnail { get; set; }
        public List<int> Ratings { get; set; }
    }
}
