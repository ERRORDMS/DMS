using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Data.ResponseModel;
using DMS.Database;
using Microsoft.AspNetCore.Mvc;

namespace DMS.Controllers
{
    public class ContactsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public LoadResult Get()
        {
            return DataSourceLoader.Load(DataManager.GetContacts(), new DataSourceLoadOptionsBase());
        }
    }
}