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
    [AllowAnonymous]
    [Route("api/[controller]")]
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
            string fn = DataManager.GetFileName(AutoKey);

            return fn;

        }
        

        [Route("GetFile")]
        [HttpGet]
        public string GetFile(long InfoAutoKey, long LineAutoKey, string Ext)
        {
            return Convert.ToBase64String(DataManager.GetFile(InfoAutoKey, LineAutoKey, Ext));
        }
        
        [HttpPost]
        public ActionResult FileSelection(string categories, string contacts, string keys, IFormFile photo, string userId = null)
        {

            
            if (photo != null)
            {
                var cats = JsonConvert.DeserializeObject<List<DMSCategory>>(categories);   
                var cons = JsonConvert.DeserializeObject<List<DMSContact>>(contacts);
                var sKeys = JsonConvert.DeserializeObject<List<SearchKey>>(keys);
                if(string.IsNullOrEmpty(userId))
                    userId =  User.FindFirstValue(ClaimTypes.NameIdentifier);

                int result = DataManager.AddFile(cats, cons,sKeys, photo, userId, _hostingEnvironment.WebRootPath);

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