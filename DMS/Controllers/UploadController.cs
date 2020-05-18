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
        
        [Route("GetFileName")]
        [HttpGet]
        public string GetFileName(long AutoKey)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(DataManager.GetFileName(AutoKey));
            return System.Convert.ToBase64String(plainTextBytes);

        }
        
        [HttpPost]
        public ActionResult FileSelection(string categories, string contacts, IFormFile photo)
        {

            
            if (photo != null)
            {

                var cats = JsonConvert.DeserializeObject<List<DMSCategory>>(categories);   
                var cons = JsonConvert.DeserializeObject<List<DMSContact>>(contacts);
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                int result = DataManager.AddFile(cats, cons, photo, userId, _hostingEnvironment.WebRootPath);

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

    }
}