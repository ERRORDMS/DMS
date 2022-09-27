using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using AspNetCoreRateLimit;
using GleamTech.AspNet;
using System.IO;
using GleamTech.DocumentUltimate;
using System.Text;

namespace DMS
{
    public class Startup
    {
        public IHostingEnvironment _environment;
        public Startup(IConfiguration configuration, IHostingEnvironment _environment)
        {
            Configuration = configuration;
            this._environment = _environment;


            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(@"MTExMUAzMTM4MmUzMjJlMzBJaWF3ZHF1ZHBMa1lvQmRCUHZlWThOSWhjNlNSVkxjVE92VGVmZ0F5akQ0PQ==");

            var bytes = System.Text.Encoding.UTF8.GetBytes(@"<License>
<Data>
<LicensedTo>Fluor Federal Services</LicensedTo>
<EmailTo>t.j.tumlin@fluor.com</EmailTo>
<LicenseType>Site Small Business</LicenseType>
<LicenseNote>Limited to 10 physical locations, not to exceed 10 developers</LicenseNote>
<OrderID>190501090513</OrderID>
<UserID>412577</UserID>
<OEM>This is not a redistributable license</OEM>
<Products>
<Product>Aspose.Total for .NET</Product>
</Products>
<EditionType>Enterprise</EditionType>
<SerialNumber>57cbdcb5-e657-4ed1-aed2-b2613bdd3517</SerialNumber>
<SubscriptionExpiry>20210106</SubscriptionExpiry>
<LicenseVersion>3.0</LicenseVersion>
<LicenseInstructions>https://purchase.aspose.com/policies/use-license</LicenseInstructions>
</Data>
<Signature>GNczbuoKwEEKCQJaQlTugFt30pBwgAEfPAfICZ6v6+CE+ABgm6cblP8I/KMqJiWFAMTf1/jRXR61SqKRFppFl+W1rvnd26YX9fQkI3b/4Vq2JHDr15cZbFNxHmRAAjW6W/bGRcfVBsnIG8XsD7yrp8146G8zbyX2BJ05JTCT2Yc=</Signature>
</License>");


            new Aspose.Imaging.License().SetLicense(new MemoryStream(bytes));//("Aspose.Total.lic");


            
//            new Aspose.Imaging.License().SetLicense(@"C:\Users\YOGA L380\source\repos\Aspose.Imaging\bin\Debug\Aspose.Total.lic");


        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services
                .AddMvc(options => options.Filters.Add(new AuthorizeFilter()))
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new DefaultContractResolver())
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();


            // needed to load configuration from appsettings.json
            services.AddOptions();

            // needed to store rate limit counters and ip rules
            services.AddMemoryCache();

            //load general configuration from appsettings.json
            services.Configure<IpRateLimitOptions>(Configuration.GetSection("IpRateLimiting"));

            //load ip rules from appsettings.json
            services.Configure<IpRateLimitPolicies>(Configuration.GetSection("IpRateLimitPolicies"));

            // inject counter and rules stores
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();

            services.AddHttpContextAccessor();

            // configuration (resolvers, counter key builders)
         //   services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

            services.AddSingleton<IIpPolicyStore, DistributedCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitCounterStore, DistributedCacheRateLimitCounterStore>();

            /*
            services.AddDbContext<DMSContext>(options =>
                  options.UseSqlServer(
                       DataManager.GetConnectionString()));

            services.AddDefaultIdentity<DMSUser>()
                .AddEntityFrameworkStores<DMSContext>();
                */


            //Add GleamTech to the ASP.NET Core services container. 
            //----------------------
            services.AddGleamTech();
            //----------------------

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
  .AddCookie(options =>
  {
      options.Cookie.Name = "AuthCookieAspNetCore";
      options.LoginPath = "/Login";
      options.LogoutPath = "/SignOut";
      options.Cookie.HttpOnly = true;
      options.Cookie.SecurePolicy = _environment.IsDevelopment()
        ? CookieSecurePolicy.None : CookieSecurePolicy.Always;
      options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
  });

            services.Configure<CookiePolicyOptions>(options =>
            {
            
                options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
                options.HttpOnly = HttpOnlyPolicy.None;
                options.Secure = _environment.IsDevelopment()
                  ? CookieSecurePolicy.None : CookieSecurePolicy.Always;
            });

            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue; // In case of multipart
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
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime)
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

            var gleamTechConfig = Hosting.ResolvePhysicalPath("~/App_Data/GleamTech.config");
            if (File.Exists(gleamTechConfig))
                GleamTechConfiguration.Current.Load(gleamTechConfig);

            var documentUltimateConfig = Hosting.ResolvePhysicalPath("~/App_Data/DocumentUltimate.config");
            if (File.Exists(documentUltimateConfig))
                DocumentUltimateConfiguration.Current.Load(documentUltimateConfig);

            //----------------------

            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseIpRateLimiting();

            var supportedCultures = new[]
            {
                new CultureInfo("en-US"),
                new CultureInfo("ar-SA"),
            };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures,
                 RequestCultureProviders = new[] { new CookieRequestCultureProvider() }
            });


            app.UseHttpsRedirection();

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
