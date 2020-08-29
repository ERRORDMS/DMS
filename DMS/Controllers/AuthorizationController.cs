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
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;
using DMS.Models;
using Microsoft.AspNetCore.DataProtection;

namespace DMS.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthorizationController : Controller
    {
        IDataProtector _protector;

        public AuthorizationController(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector(GetType().FullName);
        }

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

        [Route("Verify")]
        [HttpPost]
        public IActionResult Verify(string Key)
        {
            var email = _protector.Unprotect(Key);

            int i = new DataManager(null).Verify(GetUserID(email));

            Result result = new Result();
            result.StatusName = ((ErrorCodes)i).ToString();
            result.StatusCode = i;

            return new JsonResult(result);
        }

        [Route("Register")]
        [HttpPost]
        public JsonResult Register(string Email, string Password, string code)
        {

            int i = new DataManager(null).Register(Email, Password, code);

            Result result = new Result();
            result.StatusName = ((ErrorCodes)i).ToString();
            result.StatusCode = i;

            if (i == (int)ErrorCodes.SUCCESS)
            {
            /*    
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Malafatee", "support@malafatee.com"));
                message.To.Add(new MailboxAddress(Email, Email));
                message.Subject = "Verify your email!";


                string link = "https://malafatee.com/VerifyEmail?Key=" + _protector.Protect(Email);
                Logger.Log(link);
                message.Body = new TextPart("plain")
                {
                    Text = "Hello, " + Email
                    + Environment.NewLine + "Open this link to verify your account: " + link
                };

                using (var client = new SmtpClient())
                {
                    client.Connect("mail.malafatee.com", 25, false);

                    // Note: only needed if the SMTP server requires authentication
                    client.Authenticate("support@malafatee.com", "123");

                    client.Send(message);
                    client.Disconnect(true);
                }
                */
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
  new Claim(ClaimTypes.UserData, new DataManager(null).GetAccountType(id)),
  new Claim(ClaimTypes.NameIdentifier, id )
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
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return new DataManager(null).GetUserStorage(userId);
        }


        [Route("GetInfo")]
        [HttpGet]
        public IActionResult GetInfo()
        {
            return new JsonResult(new DataManager(null).GetInfo());
        }

        public class Info
        {
            public long Users { get; set; }
            public long Files { get; set; }
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