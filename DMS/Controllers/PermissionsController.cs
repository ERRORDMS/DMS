using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DMS.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMS.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class PermissionsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Route("Users")]
        [HttpGet]
        public IActionResult GetUsers(string userId = null)
        {

            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return new JsonResult(new DataManager(null).GetUsers(userId));
        }


        [Route("Permissions")]
        [HttpGet]
        public IActionResult GetPermissions()
        {
            return new JsonResult(new DataManager(null).GetPermissions());
        }


        [Route("GetCode")]
        [HttpGet]
        public string GetCode(string userId = null)
        {
            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return new DataManager(null).GetEnterpriseCode(userId);
        }

        [Route("Roles")]
        [HttpGet]
        public IActionResult GetRoles(string userId = null)
        {

            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return new JsonResult(new DataManager(null).GetRoles(userId));
        }
    }
}
