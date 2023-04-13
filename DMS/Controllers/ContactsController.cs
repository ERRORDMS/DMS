﻿using System;
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
using static DMS.Controllers.AuthorizationController;

namespace DMS.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class ContactsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public LoadResult Get(string userId = null)
        {
            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            using (var dm = new DataManager(userId))
            {
                return DataSourceLoader.Load(dm.GetContacts(userId), new DataSourceLoadOptionsBase());
            }
        }

        [Route("GetContactCategories")]
        [HttpGet]
        public LoadResult GetContactCategories(string userId = null)
        {
            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            using (var dm = new DataManager(userId))
            {
                return DataSourceLoader.Load(dm.GetContactCategories(), new DataSourceLoadOptionsBase());
            }
        }
        [Route("GetDocumentContacts")]
        [HttpGet]
        public LoadResult GetDocumentContacts(long AutoKey, string userId = null)
        {
            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            using (var dm = new DataManager(userId))
            {
                return DataSourceLoader.Load(dm.GetDocumentContacts(AutoKey), new DataSourceLoadOptionsBase());
            }
        }

        [Route("Exists")]
        [HttpGet]
        public IActionResult Exists(string name, long AutoKey)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            using (var dm = new DataManager(userId))
            {

                return Json(!dm.SQLHelper.Exists(Tables.Contacts, "Name = '" + name + "' AND AutoKey <> " + AutoKey));
            }
        }

        [Route("CatExists")]
        [HttpGet]
        public IActionResult CatExists(string name, long AutoKey)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            using (var dm = new DataManager(userId))
            {
                return Json(!dm.SQLHelper.Exists(Tables.ContactCategories, "Name = '" + name + "' AND AutoKey <> " + AutoKey));

            }
        }

        [Route("AddContactCategory")]
        [HttpPost]
        public IActionResult AddContactCategory(string values)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            using (var dm = new DataManager(userId))
            {
                var d = JsonConvert.DeserializeObject<ContactCategory>(values);

                int i = dm.AddContactCategory(d);
                dm.CreateNotification(new Notification() { Title = "Add Contact Gategory"});
                AuthorizationController.Result result = new AuthorizationController.Result();
                result.StatusName = ((ErrorCodes)i).ToString();
                result.StatusCode = i;

                return new JsonResult(result);
            }
        }

        [Route("UpdateContactCategory")]
        [HttpPut]
        public IActionResult UpdateContactCategory(int key, string values, string userId = null)
        {
            // Update
            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            using (var dm = new DataManager(userId))
            {
                var d = JsonConvert.DeserializeObject<ContactCategory>(values);

                d.AutoKey = Convert.ToInt64(key);
                int i = dm.SaveContactCategory(d);
                dm.CreateNotification(new Notification() { Title = "Update Contact" });
                AuthorizationController.Result result = new AuthorizationController.Result();
                result.StatusName = ((ErrorCodes)i).ToString();
                result.StatusCode = i;

                return new JsonResult(result);
            }
        }
        [Route("DeleteContactCategory")]
        [HttpDelete]
        public void DeleteContactCategory(int key, string userId = null)
        {
            // Delete    
            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            using (var dm = new DataManager(userId))
            {
                dm.DeleteContactCategory(Convert.ToInt64(key));
                dm.CreateNotification(new Notification() { Title = "Delete Contact"});
            }

        }



        [Route("AddContact")]
        [HttpPost]
        public IActionResult AddContact(string values)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            using (var dm = new DataManager(userId))
            {
                var d = JsonConvert.DeserializeObject<Contact>(values);

                int i = dm.AddContact(d, userId);
                dm.CreateNotification(new Notification() { Title = "Add Contact"});

                AuthorizationController.Result result = new AuthorizationController.Result();
                result.StatusName = ((ErrorCodes)i).ToString();
                result.StatusCode = i;

                return new JsonResult(result);
            }
        }

        [Route("UpdateContact")]
        [HttpPut]
        public IActionResult UpdateContact(int key, string values, string userId = null)
        {
            // Update
                if (string.IsNullOrEmpty(userId))
                    userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                using (var dm = new DataManager(userId))
                {
                var d = JsonConvert.DeserializeObject<Contact>(values);

                d.AutoKey = Convert.ToInt64(key);
                int i = dm.SaveContact(d);
                dm.CreateNotification(new Notification() { Title = "Update Contaact"});
                AuthorizationController.Result result = new AuthorizationController.Result();
                result.StatusName = ((ErrorCodes)i).ToString();
                result.StatusCode = i;

                return new JsonResult(result);
            }
        }

        [Route("DeleteContact")]
        [HttpDelete]
        public void DeleteContact(int key, string userId = null)
        {
            // Delete    
            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            using (var dm = new DataManager(userId))
            {
                dm.DeleteContact(Convert.ToInt64(key));
                dm.CreateNotification(new Notification() { Title = "Delete Contact"});
            }

        }

        public class ContactResult
        {
            public string Name { get; set; }
        }

    }
}