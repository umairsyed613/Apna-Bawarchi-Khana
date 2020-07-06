using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

using System.Threading.Tasks;
using System.Net.Http;
using System;

using Blazored.Toast;

namespace ApnaBawarchiKhana.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");
            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddTelerikBlazor();
            builder.Services.AddBlazoredToast();
            builder.Services.AddSingleton<AbkFetchService>();

            await builder.Build().RunAsync();
        }
    }
}
