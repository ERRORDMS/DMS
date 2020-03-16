using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMS.Database;
using Microsoft.AspNetCore.Mvc;

namespace DMS.Controllers
{

    [Route("api/[controller]")]
    public class AuthorizationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Route("Login")]
        [HttpPost]
        public IActionResult Login(string Username, string Password)
        {

            int i = DataManager.Login(Username, Password);

            Result result = new Result();
            result.StatusName = ((ErrorCodes)i).ToString();
            result.StatusCode = i;

            return new JsonResult(result);
        }

        [Route("Register")]
        [HttpPost]
        public JsonResult Register(string Email, string Username, string Password)
        {

            int i = DataManager.Register(Email, Username, Password);

            Result result = new Result();
            result.StatusName = ((ErrorCodes)i).ToString();
            result.StatusCode = i;

            return new JsonResult(result);
        }

        public class Result
        {
            public string StatusName { get; set; }
            public int StatusCode { get; set; }
            public string Extra { get; set; }
        }
    }
}