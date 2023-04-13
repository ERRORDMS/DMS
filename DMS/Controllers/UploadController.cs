using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
        private TaskQueue queue;
        public UploadController()
        {
            queue = new TaskQueue();
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
        public string GetFile(long InfoAutoKey, long LineAutoKey, string Ext, string userId = null)
        {
            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return Convert.ToBase64String(new DataManager(userId).GetFile(InfoAutoKey, LineAutoKey, Ext));
        }
        /*

        [Route("FileNameExists")]
        [HttpGet]
        public string FileNameExists(string Name, string categories)
        {

        }
        */
        [Route("DeleteFile")]
        [HttpPost]
        public IActionResult DeleteFile(long AutoKey)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int i = new DataManager(userId).DeleteFile(AutoKey, userId);
            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            using (var dm = new DataManager(userId))
            {
                dm.CreateNotification(new Notification() { Title = "Deleted file" });
            }
                Result result = new Result();
            result.StatusName = ((ErrorCodes)i).ToString();
            result.StatusCode = i;

            return new JsonResult(result);
        }

        [Route("Update")]
        [HttpPost]
        public void UpdateFile(IList<IFormFile> uploadFiles, long InfoAutoKey, long LineAutoKey)
        {
            if (uploadFiles != null && uploadFiles.Count > 0)
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                int i = new DataManager(userId).UpdateFile(uploadFiles[0], InfoAutoKey, LineAutoKey, userId);

                using (var dm = new DataManager(userId))
                {
                    dm.CreateNotification(new Notification() { Title = "Updated file" });
                }
            }

        }

        [Route("GetDeletedFiles")]
        [HttpGet]
        public IActionResult GetDeletedFiles()
        {

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var deletedFiles = new DataManager(userId).GetDeletedFiles();

            var users = new DataManager(null).GetUsers(userId).ToList();
            users.Add(new Models.User() { ID = userId, Email = "You" });

            deletedFiles.ForEach((S) =>
            {
                if (!users.Any(U => U.ID == S.Email))
                    S.Email = "Unknown";
                else
                    S.Email = users.First(U => U.ID == S.Email).Email;
            });

            return new JsonResult(deletedFiles);

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
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            using (var dm = new DataManager(userId))
            {
                dm.CreateNotification(new Notification() { Title = "Edited file" });
            }
            return new JsonResult(result);
        }

        [Route("GetFileInfo")]
        [HttpGet]
        public FileInfo GetFileInfo(long InfoAutoKey)
        {
            return new DataManager(User.FindFirstValue(ClaimTypes.NameIdentifier)).GetFileInfo(InfoAutoKey);
        }

        [Route("GetEncryptedString")]
        [HttpGet]
        public string GetEncryptedString(string key, string userId = null)
        {
            if (string.IsNullOrEmpty(userId))
            {
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }

            return StringCipher.Encrypt(userId + "|" + key);
        }
        [Route("GetDocuments")]
        [HttpGet]
        public List<Document> GetCatDocuments(long CatID, string userId = null)
        {
            if (string.IsNullOrEmpty(userId))
            {
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier); 
            }
            return new DataManager(userId).GetCatDocuments(CatID);
        }

        [Route("GetDocumentTypesSizes")]
        [HttpGet]
        public List<FileTypeSize> GetDocumentTypesSizes(string userId = null)
        {
            if (string.IsNullOrEmpty(userId))
            {
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }

            return new DataManager(userId).GetDocumentTypesSizes();
        }

        [Route("GetMonthlyUsage")]
        [HttpGet]
        public List<MonthlyUsageData> GetMonthlyUsage(string userId = null)
        {
            if (string.IsNullOrEmpty(userId))
            {
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }

            return new DataManager(userId).GetMonthlyUsages();
        }

        [Route("Save")]
        [HttpPost]
        public IActionResult Save(IList<IFormFile> uploadFiles, string categories, string contacts, string keys, string note, string userId = null)
        {
            try
            {
                if (uploadFiles != null && uploadFiles.Count!= 0 && !string.IsNullOrEmpty(categories))
                {
                    var cats = JsonConvert.DeserializeObject<List<long>>(categories);
                    var cons = JsonConvert.DeserializeObject<List<DMSContact>>(contacts);
                    var sKeys = JsonConvert.DeserializeObject<List<SearchKey>>(keys);

                    if (string.IsNullOrEmpty(userId))
                        userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                    foreach (var file in uploadFiles)
                    {
                        Task<int> t = new DataManager(userId).AddFileAsync(cats, cons, sKeys, file, userId, note);

                        //t.Start();
                        t.Wait();
                        


                        if (file == uploadFiles.Last())
                        {
                            Result result = new Result();
                            result.StatusName = ((ErrorCodes)t.Result).ToString();
                            result.StatusCode = t.Result;

                            if(t.Result == (int)ErrorCodes.INTERNAL_ERROR)
                            {
                                Response.Clear();
                                Response.StatusCode = 204;
                                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "File failed to upload";
                                return BadRequest();
                            }

                            using (var dm = new DataManager(userId))
                            {
                                dm.CreateNotification(new Notification() { Title = "Uploaded file" });
                            }

                            return new JsonResult(result);
                        }



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

            return BadRequest();
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

    public class MonthlyUsageData
    {
        public string Month { get; set; }
        public long Files { get; set; }
    }

    public class FileInfo
    {
        public List<Category> Categories { get; set; }
        public List<Contact> Contacts { get; set; }
        public List<SearchKey> SearchKeys { get; set; }
    }

    public class TaskQueue
    {
        private SemaphoreSlim semaphore;
        public TaskQueue()
        {
            semaphore = new SemaphoreSlim(1);
        }

        public async Task<T> Enqueue<T>(Func<Task<T>> taskGenerator)
        {
            await semaphore.WaitAsync();
            try
            {
                return await taskGenerator();
            }
            finally
            {
                semaphore.Release();
            }
        }
        public async Task Enqueue(Func<Task> taskGenerator)
        {
            await semaphore.WaitAsync();
            try
            {
                await taskGenerator();
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}