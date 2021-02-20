using System.Globalization;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MusicStore.Helper;
using MusicStore.Properties;
using MusicStore.Services;

namespace MusicStore
{
    public class Startup
    {
        //Test Comment

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            services.ConfigureApplicationCookie(options => options.AccessDeniedPath = "/Home/AccessDenied");

            services.AddCors(options => { options.AddPolicy("CorsPolicy", builder => { builder.WithOrigins("http://example.com"); }); });

            services.AddLogging();

            // Add MVC services to the services container
            services.AddMvc();
            services.AddDatabaseDeveloperPageExceptionFilter();


            // Add memory cache services
            services.AddMemoryCache();
            services.AddDistributedMemoryCache();

            // Configure Auth
            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    "ManageStore",
                    authBuilder => { authBuilder.RequireClaim("ManageStore", "Allowed"); });
            });
            services.AddAuthentication();

            services.AddSingleton<IRestClient, RestClient>();
            services.AddSingleton<ICatalogService, CatalogService>();
            services.AddSingleton<IOrderService, OrderService>();
            //Uses Cookies so a scoped lifetime is required
            services.AddScoped<ICartService, CartService>();

            services.AddControllersWithViews(config =>
            {
            });
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, TelemetryConfiguration telemetryConfiguration)
        {
            telemetryConfiguration.InstrumentationKey = Configuration["ApplicationInsights:InstrumentationKey"];

            // StatusCode pages to gracefully handle status codes 400-599.
            app.UseStatusCodePagesWithRedirects("~/Home/StatusCodePage");

            if (env.IsDevelopment())
            {
                // Display custom error page in production when error occurs
                // During development use the ErrorPage middleware to display error information in the browser
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
                app.UseBrowserLink();
            }
            else
                app.UseExceptionHandler("/Home/Error");


            // force the en-US culture, so that the app behaves the same even on machines with different default culture
            var supportedCultures = new[] {new CultureInfo("en-US")};

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            // Add static files to the request pipeline
            app.UseStaticFiles();

            // Add cookie-based authentication to the request pipeline
            app.UseAuthentication();

            // Add MVC to the request pipeline
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "areaRoute",
                    "{area:exists}/{controller}/{action}",
                    new {action = "Index"});

                endpoints.MapControllerRoute(
                    "default",
                    "{controller}/{action}/{id?}",
                    new {controller = "Home", action = "Index"});

                endpoints.MapControllerRoute(
                    "api",
                    "{controller}/{id?}");
            });
        }
    }
}