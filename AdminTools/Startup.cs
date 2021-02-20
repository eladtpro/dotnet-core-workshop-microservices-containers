using System.IO;
using AdminTools.Database;
using AdminTools.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace AdminTools
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Register DataContext           
            services.ConfigureDataContext(Configuration);
            // Register Azure Table Storage - used to pre-populate Catalog Read Table
            services.AddAzureTableStorage(Configuration);

            services.AddScoped<IMusicRepository, MusicRepository>();

            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(DataGeneratorCustomExceptionFilter));
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Data Generator API",
                    Version = "v1",
                    Description =
                        "Utility to generate Product Data for Activate Azure with Microservices Workshop"
                });
                //// Set the comments path for the Swagger JSON and UI.
                //var basePath = System.AppContext.BaseDirectory;
                //var xmlPath = Path.Combine(basePath, "AdminTools.xml"/*"DataGenerator.xml"*/);
                //c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "";
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Shopping Basket API V1");
            });


            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}