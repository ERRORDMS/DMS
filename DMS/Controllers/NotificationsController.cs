using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using DMS.Database;
using DMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ServiceReference1;

namespace DMS.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class NotificationsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Route("GetNotifications")]
        [HttpGet]
        public IEnumerable<Notification> GetNotifications()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            using (var dm = new DataManager(userId))
            {
                return dm.GetNotifications();

            }
        }

        [Route("MarkAsRead")]
        [HttpPost]
        public IActionResult MarkAsRead(bool markasread)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            using (var dm = new DataManager(userId))
            {
                 dm.MarkNotificationsAsRead();
                Ok();
            }
            return Ok();
        }

        
    }
}
