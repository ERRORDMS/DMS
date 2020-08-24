using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using DMS.Database;
using DMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
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
        UploadFileTask uploadTask;
        public UploadController(HostingEnvironment hostingEnvironment, UploadFileTask _task)
        {
            _hostingEnvironment = hostingEnvironment;
            uploadTask = _task;
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
        /*
        [HttpPost]
        public ActionResult FileSelection(string categories, string contacts, string keys, IFormFile photo, string userId = null)
        {


            if (photo != null)
            {

                var cats = JsonConvert.DeserializeObject<List<DMSCategory>>(categories);
                var cons = JsonConvert.DeserializeObject<List<DMSContact>>(contacts);
                var sKeys = JsonConvert.DeserializeObject<List<SearchKey>>(keys);
                //var photos = JsonConvert.DeserializeObject<List<IFormFile>>(photosJson);

                if (string.IsNullOrEmpty(userId))
                    userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                int result = new DataManager(User.FindFirstValue(ClaimTypes.NameIdentifier)).AddFile(cats, cons, sKeys, photo, userId, _hostingEnvironment.WebRootPath);

                if (result == (int)ErrorCodes.SUCCESS)
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

        [Route("Save")]
        [HttpPost]
        public void Save(IList<IFormFile> uploadFiles, string categories, string contacts, string keys, string userId = null)
        {
            try
            {
                uploadTask.AddTask(Task.Run(() => { }));
                if (uploadFiles != null && uploadFiles.Count > 0 && !string.IsNullOrEmpty(categories))
                {
                    var cats = JsonConvert.DeserializeObject<List<DMSCategory>>(categories);
                    var cons = JsonConvert.DeserializeObject<List<DMSContact>>(contacts);
                    var sKeys = JsonConvert.DeserializeObject<List<SearchKey>>(keys);

                    if (string.IsNullOrEmpty(userId))
                        userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    foreach (var file in uploadFiles)
                    {


                        new DataManager(userId).AddFile(cats, cons, sKeys, file, userId, uploadTask);

                    }
                }
            }
            catch (Exception e)
            {
                Response.Clear();
                Response.StatusCode = 204;
                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "File failed to upload";
                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = e.Message;
            }
        }

        public void Remove(IList<IFormFile> UploadFiles)
        {
            /*
            try
            {
                foreach (var file in UploadFiles)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var filePath = Path.Combine(hostingEnv.WebRootPath);
                    var fileSavePath = filePath + "\\" + fileName;
                    if (System.IO.File.Exists(fileSavePath))
                    {
                        System.IO.File.Delete(fileSavePath);
                    }
                }
            }
            catch (Exception e)
            {
                Response.Clear();
                Response.StatusCode = 200;
                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "File removed successfully";
                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = e.Message;
            }*/
        }

    }

    public class FileInfo
    {
        public List<Category> Categories { get; set; }
        public List<Contact> Contacts { get; set; }
        public List<SearchKey> SearchKeys { get; set; }
    }
}