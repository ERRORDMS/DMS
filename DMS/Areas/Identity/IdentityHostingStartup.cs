using System;
using DMS.Areas.Identity.Data;
using DMS.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(DMS.Areas.Identity.IdentityHostingStartup))]
namespace DMS.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
        /*    builder.ConfigureServices((context, services) => {
                services.AddDbContext<DMSContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("DMSContextConnection")));

                services.AddDefaultIdentity<DMSUser>()
                    .AddEntityFrameworkStores<DMSContext>();
            });*/
        }
    }
}