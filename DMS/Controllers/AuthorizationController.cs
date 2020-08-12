using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DMS.Database;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMS.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthorizationController : Controller
    {

        [Route("Login")]
        [HttpPost]
        public IActionResult Login(string Username, string Password)
        {

            int i = new DataManager(null).Login(Username, Password);

            Result result = new Result();
            result.StatusName = ((ErrorCodes)i).ToString();
            result.StatusCode = i;
            if (i == (int)ErrorCodes.SUCCESS)
            {
                AddCookies(Username);
            }

            return new JsonResult(result);
        }

        [Route("Register")]
        [HttpPost]
        public JsonResult Register(string Email, string Password)
        {

            int i = new DataManager(null).Register(Email, Password);

            Result result = new Result();
            result.StatusName = ((ErrorCodes)i).ToString();
            result.StatusCode = i;
            if (i == (int)ErrorCodes.SUCCESS)
            {
                AddCookies(Email);
            }   
            return new JsonResult(result);
        }

        [Route("GetUserID")]
        [HttpGet]
        public string GetUserID(string Email)
        {
            return new DataManager(null).GetClient().GetUserIDbyNameAsync(Email).Result; 
        }
        private void AddCookies(string Username)
        {
            var id = GetUserID(Username);

                var claims = new List<Claim>
{
  new Claim(ClaimTypes.Name, Guid.NewGuid().ToString()),
  new Claim(ClaimTypes.UserData, id )
};

                var claimsIdentity = new ClaimsIdentity(
                  claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties();

                HttpContext.SignInAsync(
                  CookieAuthenticationDefaults.AuthenticationScheme,
                  new ClaimsPrincipal(claimsIdentity),
                  authProperties);
        }

        [Route("GetUserStorage")]
        [HttpGet]
        public UserStorage GetStorage(string userId = null)
        {
            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.UserData);
            return new DataManager(null).GetUserStorage(userId);
        }

        public class UserStorage
        {
            public double UsedStorage { get; set; }
            public double Storage { get; set; }
        }

        public class Result
        {
            public string StatusName { get; set; }
            public int StatusCode { get; set; }
            public string Extra { get; set; }
        }
    }
}