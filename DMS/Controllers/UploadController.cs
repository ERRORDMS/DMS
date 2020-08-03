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
            string fn = DataManager.GetFileName(AutoKey);

            return fn;

        }
        

        [Route("GetFile")]
        [HttpGet]
        public string GetFile(long InfoAutoKey, long LineAutoKey, string Ext)
        {
            return Convert.ToBase64String(DataManager.GetFile(InfoAutoKey, LineAutoKey, Ext));
        }

        [Route("DeleteFile")]
        [HttpPost]
        public IActionResult DeleteFile(long AutoKey)
        {
            int i = DataManager.DeleteFile(AutoKey);
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

            int i = DataManager.SaveFile(AutoKey, cats, cons, sKeys);
            Result result = new Result();
            result.StatusName = ((ErrorCodes)i).ToString();
            result.StatusCode = i;

            return new JsonResult(result);
        }

        [Route("GetFileInfo")]
        [HttpGet]
        public FileInfo GetFileInfo(long InfoAutoKey)
        {
            return DataManager.GetFileInfo(InfoAutoKey);
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
                    userId =  User.FindFirstValue(ClaimTypes.UserData);

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
        

        /*
        [HttpPost]
        public ActionResult FileSelection(IFormFile photo, string userId = null)
        {


            if (photo != null)
            {
                if (string.IsNullOrEmpty(userId))
                    userId = User.FindFirstValue(ClaimTypes.UserData);

                string autoKey = "";

                int result = DataManager.AddFile(photo, userId, out autoKey);

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
            return DataManager.GetCatDocuments(CatID);
        }

    }

    public class FileInfo
    {
        public List<Category> Categories { get; set; }
        public List<Contact> Contacts { get; set; }
        public List<SearchKey> SearchKeys { get; set; }
    }
}