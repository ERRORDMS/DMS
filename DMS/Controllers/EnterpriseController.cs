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
        public LoadResult Get(string userId = null, string roleId = null)
        {
            if (!string.IsNullOrEmpty(userId))
                return DataSourceLoader.Load(new DataManager(userId).GetUserCategories(userId), new DataSourceLoadOptionsBase());
            else
            {
                 userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                return DataSourceLoader.Load(new DataManager(userId).GetRoleCategories(roleId), new DataSourceLoadOptionsBase());
            }   

        }



        [Route("UpdateCat")]
        [HttpPut]
        public IActionResult UpdateCat(string userID, long key, string values)
        {
            new DataManager(userID).UpdatePermission(userID, key, values);
            return Ok();
        }

        [Route("IsEnterprise")]
        [HttpGet]
        public bool IsEnterprise(string userId= null)
        {
            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return new DataManager(null).IsEnterpriseSubUser(userId);

        }
        [Route("SetAll")]
        [HttpPost]
        public IActionResult SetAll(string userID, bool value)
        {
            new DataManager(userID).SetAllCatPermissions(userID, value);
            return Ok();
        }


        [Route("UpdateRoleCat")]
        [HttpPut]
        public IActionResult UpdateRoleCat(string roleId, long key, string values)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            new DataManager(userId).UpdateRolePermission(roleId, key, values);
            return Ok();
        }

    }
}
