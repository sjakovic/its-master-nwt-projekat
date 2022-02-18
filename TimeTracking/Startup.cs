using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using TimeTracking.Data;

namespace TimeTracking
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            ConnectionString = Configuration["ConnectionString"];
        }

        public IConfiguration Configuration { get; }
        public static string ConnectionString { get; internal set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.AddMvc()
                .AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor.LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider = (type, factory) =>
                    {
                        var assemblyName = new AssemblyName(typeof(Resource).GetTypeInfo().Assembly.FullName);
                        return factory.Create("Resource", assemblyName.Name);
                    };
                });

            services.Configure<RequestLocalizationOptions>(
                opts =>
                {
                    var supported = new List<CultureInfo>
                    {
                        new CultureInfo("en"),
                        new CultureInfo("sr")
                    };
                    opts.DefaultRequestCulture = new RequestCulture("sr");
                    opts.SupportedCultures = supported;
                    opts.SupportedUICultures = supported;
                });

            services.AddControllersWithViews();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            var requestLocalizationOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(requestLocalizationOptions.Value);

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Projects}/{action=Index}");

                endpoints.MapControllerRoute(
                    name: "projects",
                    pattern: "projekti",
                    defaults: new { controller = "Projects", action = "Index" });

                endpoints.MapControllerRoute(
                    name: "timetable-projects",
                    pattern: "projekti/{ProjectId}",
                    defaults: new { controller = "TimeTable", action = "Index" });

                endpoints.MapControllerRoute(
                    name: "projects-delete",
                    pattern: "projekti/delete/{id}",
                    defaults: new { controller = "Projects", action = "DeleteProject" });

                endpoints.MapControllerRoute(
                    name: "timetable-delete",
                    pattern: "timetable/delete/{id}",
                    defaults: new { controller = "TimeTable", action = "DeleteTimeTable" });

                endpoints.MapControllerRoute(
                    name: "projects-update",
                    pattern: "projekti/update",
                    defaults: new { controller = "Projects", action = "UpdateProject" });

                endpoints.MapControllerRoute(
                    name: "projects-download",
                    pattern: "projekti/{ProjectId}/download",
                    defaults: new { controller = "TimeTable", action = "DownloadPDF" });

                endpoints.MapRazorPages();
            });
        }
    }
}
