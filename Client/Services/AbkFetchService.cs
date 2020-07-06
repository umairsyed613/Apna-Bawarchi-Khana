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

    }
}
