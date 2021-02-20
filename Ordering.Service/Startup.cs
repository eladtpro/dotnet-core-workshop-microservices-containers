using System;
using System.IO;
using EventBus.EventBus;
using EventBus.Events;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Ordering.API.Application.Filters;
using Ordering.API.Application.StartupExtensions;
using Ordering.API.Commands;
using Ordering.API.Events;
using Ordering.API.Queries;

namespace Ordering.API
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
            // Register cloud resources via extension methods
            services.RegisterMessageBroker(Configuration);
            services.RegisterNoSqlStore(Configuration);

            // Resgister concrete dependencies
            services.AddScoped<IOrderQueries, OrderQueries>();
            services.AddScoped<CreateOrderCommandHandler>();
            services.AddScoped<CreateBuyerCommandHandler>();

            services.AddControllersWithViews(options =>
            {
                options.Filters.Add(typeof(OrderingCustomExceptionFilter));
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Ordering Services API",
                    Version = "v1",
                    Description =
                        "Ordering API service for the Microsoft ActivateAzure with Microservices and Containers Workshop."
                    //Contact = new Contact { Name = "Rob Vettor", Email = "robvet@microsoft.com", Url = "www.thinkinginpaas.com" },
                });

                // Set the comments path for the Swagger JSON and UI.
                var basePath = System.AppContext.BaseDirectory;
                var xmlPath = Path.Combine(basePath, "Ordering.API.xml");
                c.IncludeXmlComments(xmlPath);
            });

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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            // ServiceLocator-based call that instantiates the RegisterMesageHandler class and invokes its
            // Register() method which sets a callback for underlying ServiceBus Subscription.
            ////serviceProvider.GetService<IRegisterEventHandler>().Register();


            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "";
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ordering Services API V1");
            });

            ConfigureEventBus(app, serviceProvider);
        }

        private void ConfigureEventBus(IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            eventBus.ServiceProvider = serviceProvider;

            eventBus.Subscribe(MessageEventEnum.UserCheckoutEvent,
                typeof(UserCheckoutEventHandler));
        }
    }
}