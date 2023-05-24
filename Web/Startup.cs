using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using NetCore.AutoRegisterDi;
using ConfigurationManager = Core.Common.Configuration.ConfigurationManager;
using Core.Common.Configuration;
namespace WebApplicationOutline {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            InitSettings();
            services.AddControllersWithViews();

            var assembliesToScan = new[] {
                Assembly.GetExecutingAssembly(),
                Assembly.GetAssembly(typeof(Outline.Business.IDependency)),
                Assembly.GetAssembly(typeof(Outline.Data.IDependency)),
            };
            // register services only
            services.RegisterAssemblyPublicNonGenericClasses(assembliesToScan)
                .Where(c => c.Name.EndsWith("Service") || c.Name.EndsWith("Repository")).AsPublicImplementedInterfaces();
            //  services.RegisterAssemblyPublicNonGenericClasses(assembliesToScan).AsPublicImplementedInterfaces();

            services.AddHttpContextAccessor();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            else {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

           




            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private void InitSettings() {
            const string CONNECTIONS_SECTION = "ConnectionStrings";
            const string APPSETTINGS_SECTION = "AppSettings";
            //Connections
            if (Configuration.GetSection(CONNECTIONS_SECTION).Exists()) {
                foreach (var item in Configuration.GetSection(CONNECTIONS_SECTION).AsEnumerable()) {
                    var key = item.Key.Replace(CONNECTIONS_SECTION, "");
                    if (!string.IsNullOrWhiteSpace(key)) {
                        ConfigurationManager.ConnectionStrings.Add(key.TrimStart(':'), new ConfigConnection { ConnectionString = item.Value });
                    }
                }
            }

            //AppSettings
            if (Configuration.GetSection(APPSETTINGS_SECTION).Exists()) {
                foreach (var item in Configuration.GetSection(APPSETTINGS_SECTION).AsEnumerable()) {
                    var key = item.Key.Replace(APPSETTINGS_SECTION, "");
                    if (!string.IsNullOrWhiteSpace(key)) {
                        ConfigurationManager.AppSettings.Add(key.TrimStart(':'), item.Value);
                    }
                }
            }
        }
    }
}
