﻿using System;
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
using OtpNet;

using MailKit;
using MimeKit;
using DMS.Models;
using Microsoft.AspNetCore.DataProtection;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using System.Text;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace DMS.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthorizationController : Controller
    {
        private const string accountSid = "AC8fa5eeeefcb205673553285d103b9092";
        private const string authToken = "c5b856d32e2d892ba1d78318c2269139";
        private string secretKey = "Yom@1234";
        private IDataProtector _protector;

        public AuthorizationController(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector(GetType().FullName);
            TwilioClient.Init(accountSid, authToken);

        }

        [Route("ChangePass")]
        [HttpPost]
        public IActionResult ChangePass(string userID, string newPassword)
        {
            Result result = new Result();
            try
            {
                using (var dm = new DataManager(null))
                {
                    dm.ChangePasword(userID, newPassword);


                    result.StatusName = ErrorCodes.SUCCESS.ToString();
                    result.StatusCode = 0;

                }
            }
            catch(Exception ex)
            {
                result.StatusName = ErrorCodes.INTERNAL_ERROR.ToString();
                result.StatusCode = 103;
            }

                return new JsonResult(result);
        }
        [Route("Login")]
        [HttpPost]
        public IActionResult Login(string Username, string Password)
        {
            Result result = new Result();

            using (var dm = new DataManager(null))
            {
                int i = dm.Login(Username, Password);

                result.StatusName = ((ErrorCodes)i).ToString();
                result.StatusCode = i;

                if (i == (int)ErrorCodes.SUCCESS)
                {
                    if (Get2FAEnabled(Username))
                    {
                        SendSMS(GetPhoneNumber(Username));
                        result.Extra = LoginStatus.TFA.ToString();
                    }
                    else
                    {

                        if (!SettingsController.GetSettingsPARSED().IsCompany && !dm.IsIPTrusted(GetUserID(Username), HttpContext.Connection.RemoteIpAddress.ToString()))
                        {


                            SendCodeEmail(Username);
                            result.Extra = LoginStatus.IPNotTrusted.ToString();
                        }
                        else
                        {
                            AddCookies(Username, false);
                        }
                    }

                }

            }
            return new JsonResult(result);
        }

        [Route("Register")]
        [HttpPost]
        public JsonResult Register(string Email, string Password, string code, string phone)
        {
            var stngs = SettingsController.GetSettingsPARSED();
            if (stngs.IsCompany) code = stngs.CompanyCode;
            
            Result result = new Result();
            using (var dm = new DataManager(null))
            {
                int i = dm.Register(Email, Password, phone, code);

                result.StatusName = ((ErrorCodes)i).ToString();
                result.StatusCode = i;

                if (i == (int)ErrorCodes.SUCCESS)
                {

                    if (stngs.IsCompany)
                        dm.Verify(GetUserID(Email));
                    else
                        SendVerificationEmail(Email);
                    //AddCookies(Email);
                }
            }
            return new JsonResult(result);

        }

        public void SendCodeEmail(string Username)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Malafatee", "support@malafatee.com"));
            message.To.Add(new MailboxAddress(Username, Username));
            message.Subject = "New location detected, please verify access!";

            message.Body = new TextPart("plain")
            {
                Text = "Hello, " + Username
                + Environment.NewLine + "You have five minutes to use this code to verify access to Malafatee: " + GenerateCode()
            };

            using (var client = new SmtpClient())
            {
                client.Connect("mail.malafatee.com", 25, false);


                // Note: only needed if the SMTP server requires authentication
                client.Authenticate("support@malafatee.com", "Yom@123");

                client.Send(message);
                client.Disconnect(true);
            }
        }

        [Route("GetOTP")]
        [HttpGet]
        public string GetOTP()
        {
            return StringCipher.Encrypt(GenerateCode());
        }

        [Route("CheckCode")]
        [HttpGet]
        public IActionResult CheckCode(string code, string username)
        {
            if (code == GenerateCode())
            {
                AddCookies(username);
                return Ok();
            }

            return BadRequest();
        }

        [Route("ResendSMS")]
        [HttpPost]
        public IActionResult Resend(string username)
        {
            if (Get2FAEnabled(username))
            {
                SendSMS(GetPhoneNumber(username));
            }
            else // ip not verified
            {
                SendCodeEmail(username);
            }

                return Ok();
        }

        [Route("SendVerificationEmail")]
        [HttpPost]
        public IActionResult SendVerificationEmail(string Email)
        {
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
                client.Authenticate("support@malafatee.com", "Yom@123");

                client.Send(message);
                client.Disconnect(true);
            }

            return Ok();
        }

        public string GenerateCode()
        {
            var totp = new Totp(Encoding.UTF8.GetBytes(secretKey), step: 5 * 60);
            return totp.ComputeTotp();
        }

        public void SendSMS(string phoneNumber)
        {
            MessageResource.Create(
              body: "You have five minutes to use this code to authorize access to Malafatee: " + GenerateCode(),
              from: new Twilio.Types.PhoneNumber("+12107917549"),
              to: new Twilio.Types.PhoneNumber(phoneNumber)
          );
        }
        [Route("Verify")]
        [HttpPost]
        public IActionResult Verify(string Key)
        {
            using (var dm = new DataManager(null))
            {

                var email = _protector.Unprotect(Key);

            AddCookies(email);

                int i = dm.Verify(GetUserID(email));
                Result result = new Result();

                result.StatusName = ((ErrorCodes)i).ToString();
                result.StatusCode = i;

                return new JsonResult(result);


            }

        }



        [Route("GetUserID")]
        [HttpGet]
        public string GetUserID(string Email)
        {
            return "20-00016-2";
            using (var dm = new DataManager(null))
            {
                return dm.GetClient().GetUserIDbyNameAsync(Email).Result;
            }
        }

        [Route("GetPhoneNumber")]
        [HttpGet]
        public string GetPhoneNumber(string Email)
        {
            using (var dm = new DataManager(null))
            {
                return dm.GetPhoneNumber(GetUserID(Email));
            }
        }
        [Route("IsTFAEnabled")]
        [HttpGet]
        public bool Get2FAEnabled(string Email)
        {
            return new DataManager(null).Get2FAEnabled(GetUserID(Email));
        }


        private void AddCookies(string Username, bool addIP = true)
        {
            using (var dm = new DataManager(null))
            {
                var id = GetUserID(Username);

                var claims = new List<Claim>
{
  new Claim(ClaimTypes.Name, Guid.NewGuid().ToString()),
  new Claim(ClaimTypes.UserData, dm.GetAccountType(id)),
  new Claim(ClaimTypes.NameIdentifier, id )
};

                var claimsIdentity = new ClaimsIdentity(
                  claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties();

                HttpContext.SignInAsync(
                  CookieAuthenticationDefaults.AuthenticationScheme,
                  new ClaimsPrincipal(claimsIdentity),
                  authProperties);
                if (addIP)
                    dm.TrustIP(id, HttpContext.Connection.RemoteIpAddress.ToString());

                using (var dm2 = new DataManager(id))
                {
                    dm2.GenerateTables();
                }

            }

        }

        [Route("GetUserStorage")]
        [HttpGet]
        public UserStorage GetStorage(string userId = null)
        {
            using (var dm = new DataManager(null)) {
                if (string.IsNullOrEmpty(userId))
                    userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                return dm.GetUserStorage(userId);
            }
        }


        [Route("GetAccountType")]
        [HttpGet]
        public string GetAccountType(string userId = null)
        {
            using (var dm = new DataManager(null))
            {
                if (string.IsNullOrEmpty(userId))
                    userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                return dm.GetAccountType(userId);
            }
        }


        [Route("GetInfo")]
        [HttpGet]
        public IActionResult GetInfo()
        {
            using (var dm = new DataManager(null))
            {
                return new JsonResult(dm.GetInfo());
            }
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
        public enum LoginStatus
        {
            TFA = 0,
            IPNotTrusted = 1
        }

        public class Result
        {
            public string StatusName { get; set; }
            public int StatusCode { get; set; }
            public string Extra { get; set; }
        }
    }
}