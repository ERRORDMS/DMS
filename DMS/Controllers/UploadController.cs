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
using static DMS.Controllers.AuthorizationController;

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
            string fn = new DataManager(User.FindFirstValue(ClaimTypes.NameIdentifier)).GetFileName(AutoKey);

            return fn;

        }
        

        [Route("GetFile")]
        [HttpGet]
        public string GetFile(long InfoAutoKey, long LineAutoKey, string Ext)
        {
            return Convert.ToBase64String(new DataManager(User.FindFirstValue(ClaimTypes.NameIdentifier)).GetFile(InfoAutoKey, LineAutoKey, Ext));
        }

        [Route("DeleteFile")]
        [HttpPost]
        public IActionResult DeleteFile(long AutoKey)
        {
            int i = new DataManager(User.FindFirstValue(ClaimTypes.NameIdentifier)).DeleteFile(AutoKey);
            Result result = new Result();
            result.StatusName = ((ErrorCodes)i).ToString();
            result.StatusCode = i;
            
            return new JsonResult(result);
        }

        [Route("EditFile")]
        [HttpPost]
        public ActionResult EditFile(long AutoKey, string categories, string contacts, string keys)
        {
            var cats = JsonConvert.DeserializeObject<List<long>>(categories);
            var cons = JsonConvert.DeserializeObject<List<long>>(contacts);
            var sKeys = JsonConvert.DeserializeObject<List<long>>(keys);

            int i = new DataManager(User.FindFirstValue(ClaimTypes.NameIdentifier)).SaveFile(AutoKey, cats, cons, sKeys);
            Result result = new Result();
            result.StatusName = ((ErrorCodes)i).ToString();
            result.StatusCode = i;

            return new JsonResult(result);
        }

        [Route("GetFileInfo")]
        [HttpGet]
        public FileInfo GetFileInfo(long InfoAutoKey)
        {
            return new DataManager(User.FindFirstValue(ClaimTypes.NameIdentifier)).GetFileInfo(InfoAutoKey);
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

                int result = new DataManager(User.FindFirstValue(ClaimTypes.NameIdentifier)).AddFile(cats, cons,sKeys, photo, userId, _hostingEnvironment.WebRootPath);

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
        

        /*
        [HttpPost]
        public ActionResult FileSelection(IFormFile photo, string userId = null)
        {


            if (photo != null)
            {
                if (string.IsNullOrEmpty(userId))
                    userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                string autoKey = "";

                int result = new DataManager(User.FindFirstValue(ClaimTypes.NameIdentifier)).AddFile(photo, userId, out autoKey);

                if (result == (int)ErrorCodes.SUCCESS)
                {
                    return new JsonResult(autoKey);
                }
                else
                {
                    return BadRequest();
                }
            }


            return Ok();
        }*/
        [Route("GetDocuments")]
        [HttpGet]
        public List<Document> GetCatDocuments(long CatID)
        {
            return new DataManager(User.FindFirstValue(ClaimTypes.NameIdentifier)).GetCatDocuments(CatID);
        }

    }

    public class FileInfo
    {
        public List<Category> Categories { get; set; }
        public List<Contact> Contacts { get; set; }
        public List<SearchKey> SearchKeys { get; set; }
    }
}