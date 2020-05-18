using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DMS.Pages
{
    public class ViewerModel : PageModel
    {
        public void OnGet()
        {

        }

        [HttpPost]
        public IActionResult OnPostFN()
        {
            // Just to test that it actually gets called
            Console.WriteLine("OnPostGeoLocation CALLED ####################################");

            return new JsonResult("OnPostGeoLocation CALLED ####################################");
        }
    }
}