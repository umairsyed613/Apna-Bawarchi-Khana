using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

using ApnaBawarchiKhana.Shared;

namespace ApnaBawarchiKhana.Client
{
    public class AbkFetchService
    {
        private readonly HttpClient _httpClient;
        private static IList<Category> Categories;
        private static Dictionary<int, IEnumerable<RecipesListByCategory>> cacheRecipeByCategories;
        private static Dictionary<int, Recipe> cacheRecipeById;

        public AbkFetchService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }


        public async Task<IList<Category>> GetCategories()
        {
            if (Categories == null)
            {
                Categories = await _httpClient.GetFromJsonAsync<IList<Category>>("api/Recipe/GetAllCategories");
            }

            return Categories;
        }

        public async Task<IEnumerable<RecipesListByCategory>> GetRecipesByCategoryId(int catId)
        {
            if (cacheRecipeByCategories == null)
            {
                var data = await _httpClient.GetFromJsonAsync<IEnumerable<RecipesListByCategory>>($"api/Recipe/GetAllRecipesByCategoryId/{catId}");

                if(data == null)
                {
                    return null;
                }

                cacheRecipeByCategories = new Dictionary<int, IEnumerable<RecipesListByCategory>>
                {
                    { catId, data }
                };
                return data;
            }

            if (!cacheRecipeByCategories.ContainsKey(catId))
            {
                var data = await _httpClient.GetFromJsonAsync<IEnumerable<RecipesListByCategory>>($"api/Recipe/GetAllRecipesByCategoryId/{catId}");

                if (data == null)
                {
                    return null;
                }

                cacheRecipeByCategories.Add(catId, data);
                return data;
            }

            return cacheRecipeByCategories.GetValueOrDefault(catId);
        }

        public async Task<Recipe> GetRecipeById(int id)
        {
            if (cacheRecipeById == null)
            {
                var data = await _httpClient.GetFromJsonAsync<Recipe>($"api/Recipe/GetRecipeById/{id}");

                if (data == null)
                {
                    return null;
                }

                cacheRecipeById = new Dictionary<int, Recipe>
                {
                    { id, data }
                };
                return data;
            }

            if (!cacheRecipeById.ContainsKey(id))
            {
                var data = await _httpClient.GetFromJsonAsync<Recipe>($"api/Recipe/GetRecipeById/{id}");

                if (data == null)
                {
                    return null;
                }

                cacheRecipeById.Add(id, data);
                return data;
            }

            return cacheRecipeById.GetValueOrDefault(id);
        }

    }
}
