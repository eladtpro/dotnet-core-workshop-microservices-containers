using System.IO;
using Catalog.API.Application.Filters;
using Catalog.API.Application.StartupExtensions;
using Catalog.API.Contracts;
using Catalog.API.Infrastructure.DataStore;
using Catalog.API.Infrastructure.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Catalog.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // Invoked at startup by the runtime.
        // Use to add services to the Dependency Injection container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Register cloud resources via extension methods 
            services.RegsiterDataContext(Configuration);
            services.RegisterMessageBroker(Configuration);

            // Resgister concrete dependencies
            services.AddScoped<ICatalogBusinessServices, Domain.CatalogBusinessServices>();
            services.AddScoped<IMusicRepository, MusicRepository>();
            services.AddScoped<IArtistRepository, ArtistRepository>();
            services.AddScoped<IGenreRepository, GenreRepository>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddControllersWithViews(options =>
            {
                options.Filters.Add(typeof(CatalogCustomExceptionFilter));
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Product Catalog API",
                    Version = "v1",
                    Description =
                        "Exposed as microservice for the Microsoft ActivateAzure with Microservices and Containers Workshop. Manages Catalog items."
                });
                // Set the comments path for the Swagger JSON and UI.
                var basePath = System.AppContext.BaseDirectory;
                var xmlPath = Path.Combine(basePath, "Catalog.Service.xml");
                c.IncludeXmlComments(xmlPath);
            });
        }

        // Invoked at startup by the runtime.
        // Use to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();


            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "";
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product Catalog API V1");
            });

            // Ensures Product database is created and fully-populated
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                DataInitializer.InitializeDatabaseAsync(serviceScope).Wait();
            }
        }
    }
}