using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DMS.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DMS.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class SettingsController : Controller
    {
        public static string GetSettings()
        {
            if (!System.IO.File.Exists("settings.json"))
            {
                using (var writer = System.IO.File.CreateText("settings.json"))
                {
                    writer.Write(JsonConvert.SerializeObject(
                     new Settings()
                     {
                         ServiceEndpoint = "http://localhost:9100/AlSahlService",
                         DataSource = @".\",
                         Database = "DB",
                         DatabasesPath = @"D:\AlSahl\Data",
                         FileTablePath = @"\\127.0.0.1\alsahl\Images\Images\"
                     }

                     ));

                    writer.Flush();
                    writer.Close();
                }
            }


            return System.IO.File.ReadAllText("settings.json");
            
        }

        [HttpGet]
        public string Get()
        {
            if (!System.IO.File.Exists("settings.json"))
            {
                using (var writer = System.IO.File.CreateText("settings.json"))
                {
                    writer.Write(JsonConvert.SerializeObject(
                     new Settings()
                     {
                         ServiceEndpoint = "http://localhost:9100/AlSahlService",
                         DataSource = @".\",
                         Database = "DB",
                          DatabasesPath = @"D:\AlSahl\Data",
                          FileTablePath = @"\\127.0.0.1\alsahl\Images\Images\"
                     }

                     ));

                    writer.Flush();
                    writer.Close();
                }
            }


            return System.IO.File.ReadAllText("settings.json");

        }

        [HttpPost]
        [Route("GenerateTables")]
        public ActionResult GenerateTables()
        {
            new DataManager(null).GenerateTables();

            return Ok();
        }

        [HttpPost]
        [Route("FixExt")]
        public ActionResult FixExt()
        {
            new DataManager(null).FixExt();

            return Ok();
        }


        [HttpPost]
        [Route("Save")]
        public  ActionResult SaveSettings(string serviceendpoint, string datasource, string database, string DatabasesPath, string FileTablePath)
        {
            Settings settings = new Settings();
            settings.ServiceEndpoint = serviceendpoint;
            settings.DataSource = datasource;
            settings.Database = database;
            settings.DatabasesPath = DatabasesPath;
            settings.FileTablePath = FileTablePath;

            System.IO.File.WriteAllText("settings.json", JsonConvert.SerializeObject(settings));

            return Ok();
        }

    }

    public class Settings
    {
        public string ServiceEndpoint { get; set; }
        public string DataSource { get; set; }
        public string Database { get; set; }
        public string DatabasesPath { get; set; }
        public string FileTablePath { get; set; }
    }
}
