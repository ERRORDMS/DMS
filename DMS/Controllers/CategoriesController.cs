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
using static DMS.Controllers.AuthorizationController;

namespace DMS.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class CategoriesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public LoadResult Get()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return DataSourceLoader.Load(DataManager.GetCategories(userId), new DataSourceLoadOptionsBase());
        }

        [Route("Save")]
        [HttpPost]
        public IActionResult saveCat(Category cat)
        {
            int i = DataManager.SaveCategory(cat);

            Result result = new Result();
            result.StatusName = ((ErrorCodes)i).ToString();
            result.StatusCode = i;

            return new JsonResult(result);
        }

        [Route("Name")]
        public string GetName(long autokey)
        {
            return DataManager.GetName(autokey);
        }

        [Route("Delete")]
        [HttpPost]
        public IActionResult saveCat(long autokey)
        {
            int i = DataManager.DeleteCategory(autokey);

            Result result = new Result();
            result.StatusName = ((ErrorCodes)i).ToString();
            result.StatusCode = i;

            return new JsonResult(result);
        }
        [Route("addCategory")]
        [HttpPost]
        public IActionResult addCategory(Category category)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            int i =DataManager.AddCategory(category, userId);

            Result result = new Result();
            result.StatusName = ((ErrorCodes)i).ToString();
            result.StatusCode = i;

            return new JsonResult(result);
        }
    }
}