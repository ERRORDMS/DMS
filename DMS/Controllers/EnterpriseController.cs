using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using DMS.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMS.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class EnterpriseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public LoadResult Get(string userId)
        {
            return DataSourceLoader.Load(new DataManager(userId).GetUserCategories(userId), new DataSourceLoadOptionsBase());
        }

        [Route("UpdateCat")]
        [HttpPut]
        public IActionResult UpdateCat(string userID, long key, string values)
        {
            new DataManager(userID).UpdatePermission(userID, key, values);
            return Ok();
        }

    }
}
