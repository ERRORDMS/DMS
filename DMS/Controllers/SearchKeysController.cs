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
using Microsoft.AspNetCore.Mvc;

namespace DMS.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class SearchKeysController : Controller
    {
        public LoadResult Get()
        {
            var userId = User.FindFirstValue(ClaimTypes.UserData);
            return DataSourceLoader.Load(DataManager.GetSearchKeys(userId), new DataSourceLoadOptionsBase());
        }

        [Route("Save")]
        [HttpPost]
        public IActionResult saveKey(SearchKey key)
        {
            int i = DataManager.SaveKey(key);

            AuthorizationController.Result result = new AuthorizationController.Result();
            result.StatusName = ((ErrorCodes)i).ToString();
            result.StatusCode = i;

            return new JsonResult(result);
        }

        [Route("Name")]
        [HttpGet]
        public string GetName(long autokey)
        {
            return DataManager.GetKeyName(autokey);
        }

        [Route("Delete")]
        [HttpPost]
        public IActionResult saveKey(long autokey)
        {
            int i = DataManager.DeleteKey(autokey);

            AuthorizationController.Result result = new AuthorizationController.Result();
            result.StatusName = ((ErrorCodes)i).ToString();
            result.StatusCode = i;

            return new JsonResult(result);
        }


        [Route("AddKey")]
        [HttpPost]
        public IActionResult AddKey(string name)
        {
            var userId = User.FindFirstValue(ClaimTypes.UserData);
            int i = DataManager.AddKey(name, userId);

            AuthorizationController.Result result = new AuthorizationController.Result();
            result.StatusName = ((ErrorCodes)i).ToString();
            result.StatusCode = i;

            return new JsonResult(result);
        }
    }
}