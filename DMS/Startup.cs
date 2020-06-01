using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;   
using DMS.Database;
using DMS.Models;
using GleamTech;
using GleamTech.AspNet.Core;
using GleamTech.DocumentUltimate.AspNet;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;

namespace DMS
{
    public class Startup
    {
        public IHostingEnvironment _environment;
        public Startup(IConfiguration configuration, IHostingEnvironment _environment)
        {
            Configuration = configuration;
            this._environment = _environment;
            new DataManager();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services
                .AddMvc(options => options.Filters.Add(new AuthorizeFilter()))
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver());

            /*
            services.AddDbContext<DMSContext>(options =>
                  options.UseSqlServer(
                       DataManager.GetConnectionString()));

            services.AddDefaultIdentity<DMSUser>()
                .AddEntityFrameworkStores<DMSContext>();
                */

            services.AddHttpContextAccessor();

            //Add GleamTech to the ASP.NET Core services container. 
            //----------------------
            services.AddGleamTech();
            //----------------------
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
  .AddCookie(options =>
  {
      options.Cookie.Name = "AuthCookieAspNetCore";
      options.LoginPath = "/Login";
      //options.LogoutPath = "/Logout";
      options.Cookie.HttpOnly = true;
      options.Cookie.SecurePolicy = _environment.IsDevelopment()
        ? CookieSecurePolicy.None : CookieSecurePolicy.Always;
      options.Cookie.SameSite = SameSiteMode.Lax;
  });
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.MinimumSameSitePolicy = SameSiteMode.Strict;
                options.HttpOnly = HttpOnlyPolicy.None;
                options.Secure = _environment.IsDevelopment()
                  ? CookieSecurePolicy.None : CookieSecurePolicy.Always;
            });

            /*
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = $"/Identity/Account/Login";
                options.LogoutPath = $"/Identity/Account/Logout";
                options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
            });

            */

            /*
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
            });
            
            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.SlidingExpiration = true;
            });
            */
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

             //Register GleamTech to the ASP.NET Core HTTP request pipeline.
            //----------------------
            app.UseGleamTech();
            //----------------------

            app.UseCookiePolicy();
            app.UseAuthentication();

            app.UseStaticFiles();
            //app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

        }
    }
}
