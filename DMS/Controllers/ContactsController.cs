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
using static DMS.Controllers.AuthorizationController;

namespace DMS.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class ContactsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public LoadResult Get()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return DataSourceLoader.Load(DataManager.GetContacts(userId), new DataSourceLoadOptionsBase());
        }

        [Route("Save")]
        [HttpPost]
        public IActionResult saveCon(Contact cat)
        {
            int i = DataManager.SaveContact(cat);

            Result result = new Result();
            result.StatusName = ((ErrorCodes)i).ToString();
            result.StatusCode = i;

            return new JsonResult(result);
        }

        [Route("Name")]
        public string GetName(long autokey)
        {
            return DataManager.GetConName(autokey);
        }

        [Route("Delete")]
        [HttpPost]
        public IActionResult delCon(long autokey)
        {
            int i = DataManager.DeleteContact(autokey);

            Result result = new Result();
            result.StatusName = ((ErrorCodes)i).ToString();
            result.StatusCode = i;

            return new JsonResult(result);
        }
        [Route("addContact")]
        [HttpPost]
        public IActionResult addContact(Contact contact)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            int i = DataManager.AddContact(contact, userId);

            Result result = new Result();
            result.StatusName = ((ErrorCodes)i).ToString();
            result.StatusCode = i;

            return new JsonResult(result);
        }
    }
}