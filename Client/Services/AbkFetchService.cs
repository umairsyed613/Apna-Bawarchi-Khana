using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

using ApnaBawarchiKhana.Shared;

using Polly;

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

        //public async Task<IEnumerable<RecipesListByCategory>> GetRecipesByCategoryId(int catId)
        //{
        //    if (cacheRecipeByCategories == null)
        //    {
        //        var data = await _httpClient.GetFromJsonAsync<IEnumerable<RecipesListByCategory>>($"api/Recipe/GetAllRecipesByCategoryId/{catId}");

        //        if(data == null)
        //        {
        //            return null;
        //        }

        //        cacheRecipeByCategories = new Dictionary<int, IEnumerable<RecipesListByCategory>>
        //        {
        //            { catId, data }
        //        };
        //        return data;
        //    }

        //    if (cacheRecipeByCategories.TryGetValue(catId, out var cachedData))
        //    {
        //        return cachedData;
        //    }
        //    else
        //    {
        //        var data = await _httpClient.GetFromJsonAsync<IEnumerable<RecipesListByCategory>>($"api/Recipe/GetAllRecipesByCategoryId/{catId}");

        //        if (data == null)
        //        {
        //            return null;
        //        }

        //        cacheRecipeByCategories.Add(catId, data);

        //        return data;
        //    }
        //}
        public async Task<IEnumerable<RecipesListByCategory>> GetRecipesByCategoryId(int catId)
        {
            if (cacheRecipeByCategories == null)
            {
                var data = await GetRecipesByCatIdFromServer(catId);

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

            if (cacheRecipeByCategories.TryGetValue(catId, out var cachedData))
            {
                return cachedData;
            }
            else
            {
                var data = await GetRecipesByCatIdFromServer(catId);

                if (data == null)
                {
                    return null;
                }

                cacheRecipeByCategories.Add(catId, data);

                return data;
            }
        }

        private async Task<IEnumerable<RecipesListByCategory>> GetRecipesByCatIdFromServer(int catId)
        {
            IEnumerable<RecipesListByCategory> result = null;
            var maxRetryAttempts = 3;
            var pauseBetweenFailures = TimeSpan.FromSeconds(2);

            var retryPolicy = Policy
                             .Handle<HttpRequestException>()
                             .WaitAndRetryAsync(maxRetryAttempts, i => pauseBetweenFailures);

            await retryPolicy.ExecuteAsync(
                async () =>
                    {
                        result = await _httpClient.GetFromJsonAsync<IEnumerable<RecipesListByCategory>>($"api/Recipe/GetAllRecipesByCategoryId/{catId}");
                    });

            return result;
        }

        public async Task<Recipe> GetRecipeById(int id)
        {
            if (cacheRecipeById == null)
            {
                var data = await GetRecipeIdFromServer(id);

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
                var data = await GetRecipeIdFromServer(id);

                if (data == null)
                {
                    return null;
                }

                cacheRecipeById.Add(id, data);
                return data;
            }

            return cacheRecipeById.GetValueOrDefault(id);
        }

        private async Task<Recipe> GetRecipeIdFromServer(int id)
        {
            Recipe result = null;
            var maxRetryAttempts = 3;
            var pauseBetweenFailures = TimeSpan.FromSeconds(2);

            var retryPolicy = Policy
                             .Handle<HttpRequestException>()
                             .WaitAndRetryAsync(maxRetryAttempts, i => pauseBetweenFailures);

            await retryPolicy.ExecuteAsync(
                async () =>
                    {
                        result = await _httpClient.GetFromJsonAsync<Recipe>($"api/Recipe/GetRecipeById/{id}");
                    });

            return result;
        }

    }
}
