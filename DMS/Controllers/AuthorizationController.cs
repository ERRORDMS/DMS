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
            if (DataManager.Login(Username, Password))
                return Ok();

            return BadRequest();
        }
    }
}