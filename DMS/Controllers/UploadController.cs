using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DMS.Models;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DMS.Controllers
{
    [Route("api/[controller]")]
    public class UploadController : Controller
    {
        public class FormData
        {
            public List<Category> categories { get; set; }
            public List<Contacts> contacts { get; set; }
            public IFormFile photo { get; set; }
        }
        HostingEnvironment _hostingEnvironment;
        public UploadController(HostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        /*
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
        */
        [HttpPost]
        public ActionResult FileSelection(FormData fd)
        {

            if (fd != null)
            {
                SaveFile(fd.photo);
            }

            return Ok();
        }

        void SaveFile(IFormFile file)
        {
            try
            {
                var path = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
                // Uncomment to save the file
                if(!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                using(var fileStream = System.IO.File.Create(Path.Combine(path, file.FileName))) {
                    file.CopyTo(fileStream);
                }
            }
            catch
            {
                Response.StatusCode = 400;
            }
        }
    }
}