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
            return DataSourceLoader.Load(new DataManager(userId).GetContacts(userId), new DataSourceLoadOptionsBase());
        }

        [Route("Save")]
        [HttpPost]
        public IActionResult saveCon(Contact cat, string userId = null)
        {

            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            int i = new DataManager(userId).SaveContact(cat, userId);

            Result result = new Result();
            result.StatusName = ((ErrorCodes)i).ToString();
            result.StatusCode = i;

            return new JsonResult(result);
        }

        [Route("Name")]
        public string GetName(long autokey, string userId = null)
        {
            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return new DataManager(userId).GetConName(autokey);
        }

        [Route("Delete")]
        [HttpPost]
        public IActionResult delCon(long autokey, string userId = null)
        {
            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            int i = new DataManager(userId).DeleteContact(autokey);

            Result result = new Result();
            result.StatusName = ((ErrorCodes)i).ToString();
            result.StatusCode = i;

            return new JsonResult(result);
        }
        [Route("addContact")]
        [HttpPost]
        public IActionResult addContact(Contact contact,DateTime birthday, string userId = null)
        {
            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int i = new DataManager(userId).AddContact(contact,birthday, userId);

            Result result = new Result();
            result.StatusName = ((ErrorCodes)i).ToString();
            result.StatusCode = i;

            return new JsonResult(result);
        }
    }
}