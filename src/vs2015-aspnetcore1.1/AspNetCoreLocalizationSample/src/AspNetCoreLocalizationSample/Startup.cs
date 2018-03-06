using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Localization;

namespace AspNetCoreLocalizationSample
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLocalization(options => { options.ResourcesPath = "Resources"; });
            services.AddMvc(options =>
            {
                var F = services.BuildServiceProvider().GetService<IStringLocalizerFactory>();
                var L = F.Create("ModelBindingMessages", "AspNetCoreLocalizationSample");
                options.ModelBindingMessageProvider.ValueIsInvalidAccessor =
                    (x) => L["The value '{0}' is invalid."];
                options.ModelBindingMessageProvider.ValueMustBeANumberAccessor =
                    (x) => L["The field {0} must be a number."];
                options.ModelBindingMessageProvider.MissingBindRequiredValueAccessor =
                    (x) => L["A value for the '{0}' property was not provided.", x];
                options.ModelBindingMessageProvider.AttemptedValueIsInvalidAccessor =
                    (x, y) => L["The value '{0}' is not valid for {1}.", x, y];
                options.ModelBindingMessageProvider.MissingKeyOrValueAccessor =
                    () => L["A value is required."];
                options.ModelBindingMessageProvider.UnknownValueIsInvalidAccessor =
                    (x) => L["The supplied value is invalid for {0}.", x];
                options.ModelBindingMessageProvider.ValueMustNotBeNullAccessor =
                    (x) => L["Null value is invalid.", x];
            })
            .AddDataAnnotationsLocalization()
            .AddViewLocalization();
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[] { new CultureInfo("en"), new CultureInfo("fa") };
                options.DefaultRequestCulture = new RequestCulture("en", "en");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var supportedCultures = new[] { new CultureInfo("en"), new CultureInfo("fa") };
            app.UseRequestLocalization(new RequestLocalizationOptions()
            {
                DefaultRequestCulture = new RequestCulture(new CultureInfo("en")),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
