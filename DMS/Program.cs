using AspNetCoreRateLimit;
using DevExpress.AspNetCore;
using DevExpress.DashboardAspNetCore;
using DevExpress.DashboardWeb;
using DevExpress.XtraCharts;
using GleamTech.AspNet.Core;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using GleamTech.AspNet;
using GleamTech.DocumentUltimate;
using GleamTech;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Authorization;

Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(@"MTExMUAzMTM4MmUzMjJlMzBJaWF3ZHF1ZHBMa1lvQmRCUHZlWThOSWhjNlNSVkxjVE92VGVmZ0F5akQ0PQ==");

var bytes = System.Text.Encoding.UTF8.GetBytes(@"<?xml version=""1.0""?>
<License>
  <Data>
    <LicensedTo>DDTWC</LicensedTo>
    <EmailTo>info@devesprit.com</EmailTo>
    <LicenseType>Site OEM</LicenseType>
    <LicenseNote>Up To 10 Developers And Unlimited Deployment Locations</LicenseNote>
    <OrderID>220817165856</OrderID>
    <UserID>828251</UserID>
    <OEM>This is a redistributable license</OEM>
    <Products>
      <Product>Aspose.Total for .NET</Product>
    </Products>
    <EditionType>Enterprise</EditionType>
    <SerialNumber>b3d94fc5-eeab-4756-b465-d1758054eefd</SerialNumber>
    <SubscriptionExpiry>20240116</SubscriptionExpiry>
    <LicenseExpiry>20230216</LicenseExpiry>
    <ExpiryNote>This is a temporary license for non-commercial use only and it will expire on 2023-02-16</ExpiryNote>
    <LicenseVersion>3.0</LicenseVersion>
    <LicenseInstructions>https://purchase.aspose.com/policies/use-license</LicenseInstructions>
  </Data>
  <Signature>BP3LTqvtK7AcB56Gk0oNg/+wbECJjuKFwXdgSclGlSbEteJlf/ezHhsAZGgA1LfCLJKJt9jsg9dfiTsJwfnzrg5EQh4Lov1UGjSP8FWfESnX4aeIqpvTAix0WGolklVs1jNO2+A+VzG75L4I+jfu0r9jJBMmCVIf3+1jKZwhx04=</Signature>
</License>");


new Aspose.Imaging.License().SetLicense(new MemoryStream(bytes));//("Aspose.Total.lic");




var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;


// Add services to the container.
services.AddRazorPages()
    .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null)
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
    .AddDataAnnotationsLocalization()
    .AddMvcOptions(options => options.Filters.Add(new AuthorizeFilter()));

services.AddControllers().AddNewtonsoftJson(options =>
{
    // Use the default property (Pascal) casing
    options.SerializerSettings.ContractResolver = new DefaultContractResolver();

});
services.AddLocalization(options => options.ResourcesPath = "Resources");

// needed to load configuration from appsettings.json
services.AddOptions();

// needed to store rate limit counters and ip rules
services.AddMemoryCache();

//load general configuration from appsettings.json
services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));

//load ip rules from appsettings.json
services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("IpRateLimitPolicies"));


services.AddInMemoryRateLimiting();
services.AddHttpContextAccessor();

services.AddSingleton<IIpPolicyStore, DistributedCacheIpPolicyStore>();
services.AddSingleton<IRateLimitCounterStore, DistributedCacheRateLimitCounterStore>();
services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();


//Add GleamTech to the ASP.NET Core services container. 
//----------------------
services.AddGleamTech();
//----------------------
IFileProvider fileProvider = builder.Environment.ContentRootFileProvider;
IConfiguration configuration = builder.Configuration;

services.AddDevExpressControls();
services.AddScoped<DashboardConfigurator>((IServiceProvider serviceProvider) =>
{
    DashboardConfigurator configurator = new DashboardConfigurator();
    configurator.SetDashboardStorage(new DashboardFileStorage(fileProvider.GetFileInfo("Data/Dashboards").PhysicalPath));
    configurator.SetConnectionStringsProvider(new DashboardConnectionStringsProvider(configuration));
    return configurator;
});

services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
.AddCookie(options =>
{

    options.Cookie.Name = "MalafateeCookie";
    options.LoginPath = "/Login";
    options.LogoutPath = "/SignOut";
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
    ? CookieSecurePolicy.None : CookieSecurePolicy.Always;
    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
});

services.Configure<CookiePolicyOptions>(options =>
{

    options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
    options.HttpOnly = HttpOnlyPolicy.None;
    options.Secure = builder.Environment.IsDevelopment()
      ? CookieSecurePolicy.None : CookieSecurePolicy.Always;
});

services.Configure<FormOptions>(x =>
{
    x.ValueLengthLimit = int.MaxValue;
    x.MultipartBodyLengthLimit = int.MaxValue; // In case of multipart
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();


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

//app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapDefaultControllerRoute();

app.Run();
