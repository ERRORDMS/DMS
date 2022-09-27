using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DevExpress.Data;
using DMS.Database;
using DMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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

        public bool IsAdmin(string userID = null)
        {
            if (string.IsNullOrEmpty(userID))
                userID = User.FindFirstValue(ClaimTypes.NameIdentifier);

            using (var dm = new DataManager(userID))
            {
                return dm.IsAdmin(userID);
            }
        }

        [Route("Users")]
        [HttpGet]
        public IActionResult GetUsers(string userId = null, bool includeSelf = false)
        {
            using (var dm = new DataManager(null))
            {
                if (string.IsNullOrEmpty(userId))
                    userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                return new JsonResult(dm.GetUsers(userId, includeSelf));
            }   
        }


        [Route("Permissions")]
        [HttpGet]
        public IActionResult GetPermissions(string userId = null)
        {

            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            using (var dm = new DataManager(userId))
            {
                return new JsonResult(dm.GetPermissions());
            }
        }


        [Route("UserPermissions")]
        [HttpGet]
        public IActionResult GetUserPermissions(string userID = null)
        {

            if (string.IsNullOrEmpty(userID))
                userID = User.FindFirstValue(ClaimTypes.NameIdentifier);

            using (var dm = new DataManager(userID))
            {
                return new JsonResult(dm.GetUserPermissions(userID));
            }
        }

        [Route("RolePermissions")]
        [HttpGet]
        public IActionResult GetRolePermissions(string roleID, string userId = null)
        {

            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            using (var dm = new DataManager(userId))
            {

                return new JsonResult(dm.GetRolePermissions(roleID));
            }
        }
        [Route("Save")]
        [HttpPost]
        public IActionResult save(string userID, string permissionsJson, string rolesJson)
        {
            using (var dm = new DataManager(userID))
            {
                var permissions = JsonConvert.DeserializeObject<List<Permission>>(permissionsJson);
                var roles = JsonConvert.DeserializeObject<List<Role>>(rolesJson);
                int i = dm.SaveUser(userID, permissions, roles);

                AuthorizationController.Result result = new AuthorizationController.Result();
                result.StatusName = ((ErrorCodes)i).ToString();
                result.StatusCode = i;

                return new JsonResult(result);
            }
        }

        [Route("SaveRole")]
        [HttpPost]
        public IActionResult saveRole(string roleID, string permissionsJson, string userId = null)
        {
            
            if(string.IsNullOrEmpty(userId))
            userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            using (var dm = new DataManager(userId))
            {
                var permissions = JsonConvert.DeserializeObject<List<Permission>>(permissionsJson);
                int i = dm.SaveRole(roleID, permissions);

                AuthorizationController.Result result = new AuthorizationController.Result();
                result.StatusName = ((ErrorCodes)i).ToString();
                result.StatusCode = i;

                return new JsonResult(result);
            }
        }


        [Route("GetCode")]
        [HttpGet]
        public string GetCode(string userId = null)
        {
            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            using (var dm = new DataManager(null)) {
                return dm.GetEnterpriseCode(userId);
            }
        }

        [Route("Roles")]
        [HttpGet]
        public IActionResult GetRoles(string userId = null)
        {

            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            using (var dm = new DataManager(userId))
            {
                return new JsonResult(dm.GetRoles());
            }
        }


        [Route("GetUserRoles")]
        [HttpGet]
        public IActionResult GetUserRoles(string userId = null)
        {

            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            using (var dm = new DataManager(userId))
            {
                return new JsonResult(dm.GetUserRoles(userId));
            }
        }

        [Route("AddRole")]
        [HttpPost]
        public IActionResult AddRole(string values)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            using (var dm = new DataManager(userId))
            {
                var d = JsonConvert.DeserializeObject<RoleResult>(values);

                int i = dm.AddRole(d.Name);

                AuthorizationController.Result result = new AuthorizationController.Result();
                result.StatusName = ((ErrorCodes)i).ToString();
                result.StatusCode = i;

                return new JsonResult(result);
            }
        }

        [Route("UpdateRole")]
        [HttpPut]
        public IActionResult UpdateRole(int key, string values, string userId = null)
        {
            // Update
            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            using (var dm = new DataManager(userId))
            {
                var d = JsonConvert.DeserializeObject<RoleResult>(values);

                int i = dm.UpdateRole(key, d.Name);

                AuthorizationController.Result result = new AuthorizationController.Result();
                result.StatusName = ((ErrorCodes)i).ToString();
                result.StatusCode = i;

                return new JsonResult(result);
            }
        }

        [Route("DeleteRole")]
        [HttpDelete]
        public void DeleteRole(int key, string userId = null)
        {
            // Delete    
            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            using (var dm = new DataManager(userId)) { 
                dm.DeleteRole(key);
            }

        }

        public class RoleResult
        {
            public string Name { get; set; }
        }
    }
}
