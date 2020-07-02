using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;

using ApnaBawarchiKhana.Server.Database;
using ApnaBawarchiKhana.Server.Services;
using Microsoft.EntityFrameworkCore;
using System;

namespace ApnaBawarchiKhana.Server
{
    public class Startup
    {

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApnaBawarchiKhanaDbContext>(options => options.UseSqlServer(GetDbConnectionString(Configuration)), ServiceLifetime.Singleton);

            services.AddControllersWithViews().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            services.AddRazorPages();
            services.AddMemoryCache();

            services.AddSingleton<IPathProvider, PathProvider>();
            services.AddSingleton<IAkImageFileService, AkImageFileService>();
            services.AddSingleton<IRecipeService, RecipeService>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();

                endpoints.MapFallbackToFile("index.html");
            });
        }

        private static string GetDbConnectionString(IConfiguration configuration)
        {
#if DEBUG
            var conn = configuration.GetConnectionString("DATABASE_URL");
#else
                var conn = System.Environment.GetEnvironmentVariable("DATABASE_URL");
#endif
            return conn;
        }
    }
}
