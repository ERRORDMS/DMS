using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DMS.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DMS.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class MailController : Controller
    {
        [HttpGet]
        public List<string> GetEmails(string userId = null)
        {/*
            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            using (DataManager dm = new DataManager(userId))
            {
                return dm.GetEmails();
                
            }*/
            return null;
        }

        [HttpPost]
        [Route("OnMessageReceived")]
        public void OnMessageReceived(string From, string To, string Body, string Subject)
        {
            Logger.Log(From);
        }
    }
}
