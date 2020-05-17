using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DMS.Database;
using DMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ServiceReference1;

namespace DMS.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class UploadController : Controller
    {
        
        HostingEnvironment _hostingEnvironment;
        public UploadController(HostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        
        [HttpPost]
        public ActionResult FileSelection(string categories, string contacts, IFormFile photo)
        {

            
            if (photo != null)
            {
                SaveFile(photo);

                var cats = JsonConvert.DeserializeObject<List<DMSCategory>>(categories);   
                var cons = JsonConvert.DeserializeObject<List<DMSContact>>(contacts);
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                int result = DataManager.AddFile(cats, cons, photo, userId);

                if(result == (int)ErrorCodes.SUCCESS)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }


            return Ok();
        }

        [Route("GetDocuments")]
        [HttpGet]
        public List<Document> GetCatDocuments(long CatID)
        {
            return DataManager.GetCatDocuments(CatID);
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