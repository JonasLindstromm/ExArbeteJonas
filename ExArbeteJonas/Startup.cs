using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExArbeteJonas.BusinessLayer;
using ExArbeteJonas.DataLayer;
using ExArbeteJonas.IdentityData;
using ExArbeteJonas.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rotativa.AspNetCore;

namespace ExArbeteJonas
{   
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

           
            //Lägger till en service för Entity Framework och skickar in en connectionsträng            
            services.AddDbContext<MarketContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
          
            //Sätter upp Identity
            services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<MarketContext>()
            .AddDefaultTokenProviders();
            
            // Registrera klasserna som används i Datalagret och Businesslagret
            services.AddScoped<IMarketData, MarketData>();
            services.AddScoped<IMarketBusiness, MarketBusiness>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseStatusCodePages();
            }

            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}"
                    );
            });

            // Used to Generate Pdf
            RotativaConfiguration.Setup(env);

            CreateRoles(serviceProvider).Wait();
        }

        // Skapa rollerna Admin och Member
        private async Task CreateRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            bool roleExists = await roleManager.RoleExistsAsync("Admin");
            if (!roleExists)
            {
                // Skapa Admin rollen    
                var role = new IdentityRole();
                role.Name = "Admin";
                await roleManager.CreateAsync(role);
            }

            roleExists = await roleManager.RoleExistsAsync("Member");
            if (!roleExists)
            {
                // Skapa Regular rollen    
                var role = new IdentityRole();
                role.Name = "Member";
                await roleManager.CreateAsync(role);
            }
        }
    }
}
