using System;
using System.Collections.Generic;
using System.Text;

namespace AmnasKitchen.Shared
{
    public class CreateCategoryFormData
    {
        public Category Category { get; set; }

        public string IconPath { get; set; }
    }
    
    public class CreateRecipeFormData
    {
        public Recipe Recipe { get; set; }

        public List<int> CategoryIds { get; set; }

        public List<string> Images { get; set; }
    }
}
