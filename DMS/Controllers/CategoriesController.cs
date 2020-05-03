using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using DMS.Database;
using DMS.Models;
using Microsoft.AspNetCore.Mvc;
using static DMS.Controllers.AuthorizationController;

namespace DMS.Controllers
{
    [Route("api/[controller]")]
    public class CategoriesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public LoadResult Get()
        {
            return DataSourceLoader.Load(DataManager.GetCategories(), new DataSourceLoadOptionsBase());
        }

        [Route("addCategory")]
        [HttpPost]
        public IActionResult addCategory(Category category)
        {
            int i =DataManager.AddCategory(category);

            Result result = new Result();
            result.StatusName = ((ErrorCodes)i).ToString();
            result.StatusCode = i;

            return new JsonResult(result);
        }
    }
}