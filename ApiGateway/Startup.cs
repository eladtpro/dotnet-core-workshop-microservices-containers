using System.IO;
using ApiGateway.API.Infrastructure.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using RestCommunication;
using ServiceDiscovery;

namespace ApiGateway.API
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

            // Add custom exception filter to ASP.NET Core MVC
            services.AddMvc(config => { config.Filters.Add(typeof(GatewayCustomExceptionFilter)); });

            // Add Swagger (OpenId Connect) plumbing
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "API Gateway Services API",
                    Version = "v1",
                    Description =
                        "API Gateway service for the Microsoft ActivateAzure with Microservices and Containers Workshop. It is the only service exposed to the client, encapsulating all other services behind it."
                });

                //// Set the comments path for the Swagger JSON and UI.
                //var basePath = System.AppContext.BaseDirectory;
                //var xmlPath = Path.Combine(basePath, "ApiGateway.xml");
                //c.IncludeXmlComments(xmlPath);
            });

            // Plubming class for ServiceLocator
            services.AddSingleton<IServiceLocator, ServiceLocator>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IRestClient, RestClient>();

            services.AddControllers();

            services.AddApiVersioning(x =>
            {
                // Allows for API to return a version in the response header
                x.ReportApiVersions = true;
                // Default version for clients not specifying a version number
                x.AssumeDefaultVersionWhenUnspecified = true;
                // Specifies version to which to default. This is the version
                // to which you are routed if no version is specified
                x.DefaultApiVersion = new ApiVersion(1, 0);
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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Gateway Services API V1");
            });

        }
    }
}