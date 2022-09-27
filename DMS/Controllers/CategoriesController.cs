using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using DMS.Database;
using DMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DMS.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class CategoriesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public LoadResult Get(string userId = null)
        {
            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


            using (var dm = new DataManager(userId))
            using (var dm2 = new DataManager(null))
            {
                if (!dm2.IsEnterpriseSubUser(userId))
                {
                    return DataSourceLoader.Load(dm.GetCategories(), new DataSourceLoadOptionsBase());
                }
                else
                {
                    return DataSourceLoader.Load(dm.GetEnterpriseCategories(userId), new DataSourceLoadOptionsBase());
                }
            }
        }


        [Route("GetDocumentCategories")]
        [HttpGet]
        public LoadResult GetDocumentCategories(long AutoKey, string userId = null)
        {
            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            using (var dm = new DataManager(userId))
            {
                return DataSourceLoader.Load(dm.GetDocumentCategories(AutoKey), new DataSourceLoadOptionsBase());
            }
        }
        [Route("CanAdd")]
        [HttpGet]
        public bool CanAdd(long AutoKey, string userId = null)
        {
            using (var dm = new DataManager(userId)) {
                if (string.IsNullOrEmpty(userId))
                    userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                return dm.CanAdd(AutoKey, userId);
            }
        }


        [Route("AllowDeleteCategory")]
        [HttpGet]
        public bool AllowDeleteCategory(long AutoKey, string userId = null)
        {
            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            using (var dm = new DataManager(userId))
            {

                return dm.AllowDeletingCategory(AutoKey);
            }
        }
        [Route("CanDownload")]
        [HttpGet]
        public bool CanDownload(long AutoKey, string userId = null)
        {
            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            using (var dm = new DataManager(userId)) {
               
                return dm.CanDownload(AutoKey, userId);
            }
        }
        [Route("SetParent")]
        [HttpPost]
        public IActionResult SetParent(Category cat, string userId = null)
        {
            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            using (var dm = new DataManager(userId)) {
           
                int i = dm.SetParent(cat, userId);

                AuthorizationController.Result result = new AuthorizationController.Result();
                result.StatusName = ((ErrorCodes)i).ToString();
                result.StatusCode = i;

                return new JsonResult(result);
            }
        }
        [Route("UpdateCategory")]
        [HttpPost]
        public IActionResult saveCat(int key, string values, string userId = null)
        {
            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            using (var dm = new DataManager(userId)){
               
                var cat = JsonConvert.DeserializeObject<Category>(values);
                cat.AutoKey = Convert.ToInt64(key);
                int i = dm.SaveCategory(cat, userId);

                AuthorizationController.Result result = new AuthorizationController.Result();
                result.StatusName = ((ErrorCodes)i).ToString();
                result.StatusCode = i;

                return new JsonResult(result);
            }
        }

        [Route("Name")]
        [HttpGet]
        public string GetName(long autokey, string userId = null)
        {
            using (var dm = new DataManager(userId)) {
                if (string.IsNullOrEmpty(userId))
                    userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                return dm.GetCatName(autokey);
            }
        }

        [Route("GetFather")]
        [HttpGet]
        public string GetFather(long autokey, string userId = null)
        {
            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            using (var dm = new DataManager(userId))
            {

                return dm.GetFather(autokey);
            }
        }

        [Route("DeleteCategory")]
        [HttpPost]
        public IActionResult deleteCat(int autokey, string userId = null)
        {
            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            using (var dm = new DataManager(userId))
            {
             
                int i = dm.DeleteCategory(Convert.ToInt64(autokey));

                AuthorizationController.Result result = new AuthorizationController.Result();
                result.StatusName = ((ErrorCodes)i).ToString();
                result.StatusCode = i;

                return new JsonResult(result);
            }
        }

        [Route("AddCategory")]
        [HttpPost]
        public IActionResult addCategory(Category category, string userId = null)
        {
            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            using (var dm = new DataManager(userId))
            {
              //  var category = JsonConvert.DeserializeObject<Category>(values);
                int i = dm.AddCategory(category, userId, out string catID);

                AuthorizationController.Result result = new AuthorizationController.Result();
                result.StatusName = ((ErrorCodes)i).ToString();
                result.StatusCode = i;
                result.Extra = catID;
                return new JsonResult(result);
            }
        }
    }
}