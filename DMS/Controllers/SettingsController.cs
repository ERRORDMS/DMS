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
                         DatabasesPath = @"D:\AlSahl\Data"
                     }

                     ));

                    writer.Flush();
                    writer.Close();
                }
            }


            return System.IO.File.ReadAllText("settings.json");
            
        }
        
        public static Settings GetSettingsPARSED()
        {
            return JsonConvert.DeserializeObject<Settings>(GetSettings());
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
                         IsCompany = false,
                         CompanyCode = "HKMLNF"
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
            new DataManager(null).GenerateAllTables();

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
        public  ActionResult SaveSettings(string serviceendpoint, string datasource, string database, string DatabasesPath, string FtpUrl,string FtpUsername,string FtpPassword)
        {
            Settings settings = new Settings();
            settings.ServiceEndpoint = serviceendpoint;
            settings.DataSource = datasource;
            settings.Database = database;
            settings.DatabasesPath = DatabasesPath;
            settings.FtpUrl = FtpUrl;
            settings.FtpUsername = FtpUsername;
            settings.FtpPassword = FtpPassword;


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
        public string FtpUrl { get; set; }
        public string FtpUsername { get; set; }
        public string FtpPassword { get; set; }
        public bool IsCompany { get; set; }
        public string CompanyCode { get; set; }
    }
}
