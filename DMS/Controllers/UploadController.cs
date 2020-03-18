using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DMS.Controllers
{
    [Route("api/[controller]")]
    public class UploadController : Controller
    {
        HostingEnvironment _hostingEnvironment;
        public UploadController(HostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        [HttpPost]
        public ActionResult UploadImage(IFormFile imageFile)
        {
            try
            {
                string[] imageExtensions = { ".jpg", ".jpeg", ".gif", ".png" };

                var fileName = imageFile.FileName.ToLower();
                var isValidExtenstion = imageExtensions.Any(ext => {
                    return fileName.LastIndexOf(ext) > -1;
                });

                if (isValidExtenstion)
                {
                    // Uncomment to save the file
                    var path = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
                    if(!Directory.Exists(path))
                        Directory.CreateDirectory(path);

                    using(var fileStream = System.IO.File.Create(Path.Combine(path, imageFile.FileName))) {
                        imageFile.CopyTo(fileStream);
                    }
                }
            }
            catch
            {
                Response.StatusCode = 400;
            }

            return new EmptyResult();
        }

    }
}