﻿using System;
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
        public IActionResult GetPermissions(string userId = null)
        {

            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return new JsonResult(new DataManager(userId).GetPermissions());
        }


        [Route("UserPermissions")]
        [HttpGet]
        public IActionResult GetUserPermissions(string userID = null)
        {

            if (string.IsNullOrEmpty(userID))
                userID = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return new JsonResult(new DataManager(userID).GetUserPermissions(userID));
        }

        [Route("RolePermissions")]
        [HttpGet]
        public IActionResult GetRolePermissions(string roleID, string userId = null)
        {
            if(string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return new JsonResult(new DataManager(userId).GetRolePermissions(roleID));
        }
        [Route("Save")]
        [HttpPost]
        public IActionResult save(string userID, string permissionsJson, string rolesJson)
        {
            var permissions = JsonConvert.DeserializeObject<List<Permission>>(permissionsJson);
            var roles = JsonConvert.DeserializeObject<List<Role>>(rolesJson);
            int i = new DataManager(userID).SaveUser(userID, permissions, roles);

            AuthorizationController.Result result = new AuthorizationController.Result();
            result.StatusName = ((ErrorCodes)i).ToString();
            result.StatusCode = i;

            return new JsonResult(result);
        }

        [Route("SaveRole")]
        [HttpPost]
        public IActionResult saveRole(string roleID, string permissionsJson, string userId = null)
        {
            if(string.IsNullOrEmpty(userId))
            userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var permissions = JsonConvert.DeserializeObject<List<Permission>>(permissionsJson);
            int i = new DataManager(userId).SaveRole(roleID, permissions);

            AuthorizationController.Result result = new AuthorizationController.Result();
            result.StatusName = ((ErrorCodes)i).ToString();
            result.StatusCode = i;

            return new JsonResult(result);
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

            return new JsonResult(new DataManager(userId).GetRoles());
        }


        [Route("GetUserRoles")]
        [HttpGet]
        public IActionResult GetUserRoles(string userId = null)
        {

            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return new JsonResult(new DataManager(userId).GetUserRoles(userId));
        }

        [Route("AddRole")]
        [HttpPost]
        public IActionResult AddRole(string values)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var d = JsonConvert.DeserializeObject<RoleResult>(values);

            int i = new DataManager(userId).AddRole(d.Name);

            AuthorizationController.Result result = new AuthorizationController.Result();
            result.StatusName = ((ErrorCodes)i).ToString();
            result.StatusCode = i;

            return new JsonResult(result);
        }

        [Route("UpdateRole")]
        [HttpPut]
        public IActionResult UpdateRole(int key, string values, string userId = null)
        {
            // Update
            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


            var d = JsonConvert.DeserializeObject<RoleResult>(values);

            int i = new DataManager(userId).UpdateRole(key, d.Name);

            AuthorizationController.Result result = new AuthorizationController.Result();
            result.StatusName = ((ErrorCodes)i).ToString();
            result.StatusCode = i;

            return new JsonResult(result);
        }

        [Route("DeleteRole")]
        [HttpDelete]
        public void DeleteRole(int key, string userId = null)
        {
            // Delete    
            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


            new DataManager(userId).DeleteRole(key);

        }

        public class RoleResult
        {
            public string Name { get; set; }
        }
    }
}
