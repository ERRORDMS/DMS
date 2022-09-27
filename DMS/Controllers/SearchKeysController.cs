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
using Newtonsoft.Json;

namespace DMS.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class SearchKeysController : Controller
    {
        public LoadResult Get(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return DataSourceLoader.Load(new DataManager(userId).GetSearchKeys(userId), new DataSourceLoadOptionsBase());
        }
        [Route("GetDocumentSearchKeys")]
        [HttpGet]
        public LoadResult GetDocumentSearchKeys(long AutoKey, string userId = null)
        {
            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            using (var dm = new DataManager(userId))
            {
                return DataSourceLoader.Load(dm.GetDocumentSearchKeys(AutoKey), new DataSourceLoadOptionsBase());
            }
        }

        [Route("UpdateKey")]
        [HttpPut]
        public IActionResult UpdateKey(int key, string values, string userId = null)
        {

            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var sk = JsonConvert.DeserializeObject<SearchKey>(values);
            sk.AutoKey = Convert.ToInt64(key);
            int i = new DataManager(userId).SaveKey(sk, userId);

            AuthorizationController.Result result = new AuthorizationController.Result();
            result.StatusName = ((ErrorCodes)i).ToString();
            result.StatusCode = i;

            return new JsonResult(result);
        }

        [Route("Name")]
        [HttpGet]
        public string GetName(long autokey, string userId = null)
        {
            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return new DataManager(userId).GetKeyName(autokey);
        }

        [Route("DeleteKey")]
        [HttpDelete]
        public IActionResult DeleteKey(int key, string userId =null)
        {
            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            int i = new DataManager(userId).DeleteKey(Convert.ToInt64(key));

            AuthorizationController.Result result = new AuthorizationController.Result();
            result.StatusName = ((ErrorCodes)i).ToString();
            result.StatusCode = i;

            return new JsonResult(result);
        }


        [Route("AddKey")]
        [HttpPost]
        public IActionResult AddKey(string values, string userId = null)
        {
            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int i = new DataManager(userId).AddKey(JsonConvert.DeserializeObject<SearchKey>(values).Name, userId);

            AuthorizationController.Result result = new AuthorizationController.Result();
            result.StatusName = ((ErrorCodes)i).ToString();
            result.StatusCode = i;

            return new JsonResult(result);
        }
    }
}