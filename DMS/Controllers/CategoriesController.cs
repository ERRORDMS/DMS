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
                userId = User.FindFirstValue(ClaimTypes.UserData);

            return DataSourceLoader.Load(DataManager.GetCategories(userId), new DataSourceLoadOptionsBase());
        }

        [Route("Save")]
        [HttpPost]
        public IActionResult saveCat(Category cat, string userId = null)
        {
            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.UserData);

            int i = DataManager.SaveCategory(cat, userId);

            AuthorizationController.Result result = new AuthorizationController.Result();
            result.StatusName = ((ErrorCodes)i).ToString();
            result.StatusCode = i;

            return new JsonResult(result);
        }

        [Route("Name")]
        [HttpGet]
        public string GetName(long autokey)
        {
            return DataManager.GetCatName(autokey);
        }

        [Route("Delete")]
        [HttpPost]
        public IActionResult deleteCat(long autokey)
        {
            int i = DataManager.DeleteCategory(autokey);

            AuthorizationController.Result result = new AuthorizationController.Result();
            result.StatusName = ((ErrorCodes)i).ToString();
            result.StatusCode = i;

            return new JsonResult(result);
        }

        [Route("addCategory")]
        [HttpPost]
        public IActionResult addCategory(Category category,string userId = null)
        {
            if(string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.UserData);

            int i =DataManager.AddCategory(category, userId);

            AuthorizationController.Result result = new AuthorizationController.Result();
            result.StatusName = ((ErrorCodes)i).ToString();
            result.StatusCode = i;

            return new JsonResult(result);
        }
    }
}