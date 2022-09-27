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
            {
                using (var dm = new DataManager(userId))
                    return DataSourceLoader.Load(dm.GetUserCategories(userId), new DataSourceLoadOptionsBase());
            }
            else
            {
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                using (var dm = new DataManager(userId))
                    return DataSourceLoader.Load(dm.GetRoleCategories(roleId), new DataSourceLoadOptionsBase());
            }

            
        }



        [Route("UpdateCat")]
        [HttpPut]
        public IActionResult UpdateCat(string userID, long key, string values)
        {
            using (var dm = new DataManager(userID))
            {
                var d = JsonConvert.DeserializeObject<UpdateInfo>(values);
                d.CatID = key;
                dm.UpdatePermission(userID, d);
                return Ok();
            }
        }

        [Route("IsEnterprise")]
        [HttpGet]
        public bool IsEnterprise(string userId= null)
        {
            using (var dm = new DataManager(null)) {
                if (string.IsNullOrEmpty(userId))
                    userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                return dm.IsEnterpriseSubUser(userId);
            }

        }
        [Route("SetAll")]
        [HttpPost]
        public IActionResult SetAll(bool value, string userID = null, string roleID = null)
        {
            if(string.IsNullOrEmpty(userID))
                userID = User.FindFirstValue(ClaimTypes.NameIdentifier);

            using (var dm = new DataManager(userID))
            {
                if (string.IsNullOrEmpty(roleID))
                {
                    dm.SetAllCatPermissions(userID, value);
                    return Ok();
                }
                else
                {
                    dm.SetAllRoleCatPermissions(roleID, value);
                    return Ok();
                }
            }

        }


        [Route("UpdateRoleCat")]
        [HttpPut]
        public IActionResult UpdateRoleCat(string roleId, long key, string values)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            using (var dm = new DataManager(userId))
            {
                var d = JsonConvert.DeserializeObject<UpdateInfo>(values);
                d.CatID = key;

                dm.UpdateRolePermission(roleId, d);
                return Ok();
            }
        }
        
        [Route("UpdateCats")]
        [HttpPost]
        public IActionResult UpdateCats(string userID, string catsJson)
        {
            using (var dm = new DataManager(userID)) {
                List<UpdateInfo> cats = JsonConvert.DeserializeObject<List<UpdateInfo>>(catsJson);
                foreach (var cat in cats)
                {
                    dm.UpdatePermission(userID, cat);
                }
                return Ok();
            }
        }
        [Route("UpdateRoleCats")]
        [HttpPost]
        public IActionResult UpdateRoleCats(string roleId, string  catsJson,string userId = null)
        {

            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            using (var dm = new DataManager(userId))
            {
                List<UpdateInfo> cats = JsonConvert.DeserializeObject<List<UpdateInfo>>(catsJson);

                foreach (var cat in cats)
                {
                    new DataManager(userId).UpdateRolePermission(roleId, cat);
                }
                return Ok();
            }
        }

    }
}
