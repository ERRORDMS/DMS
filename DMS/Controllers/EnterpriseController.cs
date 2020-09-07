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
using Newtonsoft.Json;
using static DMS.Database.DataManager;

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
            var d = JsonConvert.DeserializeObject<UpdateInfo>(values);
            d.CatID = key;
            new DataManager(userID).UpdatePermission(userID, d);
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
            var d = JsonConvert.DeserializeObject<UpdateInfo>(values);
            d.CatID = key;

            new DataManager(userId).UpdateRolePermission(roleId, d);
            return Ok();
        }
        
        [Route("UpdateCats")]
        [HttpPost]
        public IActionResult UpdateCats(string userID, string catsJson)
        {
            List<UpdateInfo> cats = JsonConvert.DeserializeObject<List<UpdateInfo>>(catsJson);
            foreach (var cat in cats)
            {
                new DataManager(userID).UpdatePermission(userID, cat);
            }
            return Ok();
        }
        [Route("UpdateRoleCats")]
        [HttpPost]
        public IActionResult UpdateRoleCats(string roleId, string  catsJson,string userId = null)
        {
            if(string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            List<UpdateInfo> cats = JsonConvert.DeserializeObject<List<UpdateInfo>>(catsJson);
    
            foreach (var cat in cats)
            {
                new DataManager(userId).UpdateRolePermission(roleId, cat);
            }
            return Ok();
        }

    }
}
