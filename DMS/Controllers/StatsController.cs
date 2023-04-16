using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using DevExpress.DashboardAspNetCore;
using DevExpress.DashboardWeb;

namespace DMS.Controllers
{
    public class StatsController : DashboardController
    {
        public StatsController(DashboardConfigurator configurator, IDataProtectionProvider dataProtectionProvider = null) : base(configurator, dataProtectionProvider)
        {

        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
