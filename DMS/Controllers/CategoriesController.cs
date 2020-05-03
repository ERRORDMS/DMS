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
    public class CategoriesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public LoadResult Get()
        {
            return DataSourceLoader.Load(DataManager.GetCategories(), new DataSourceLoadOptionsBase());
        }

        [Route("addCategory")]
        [HttpPost]
        public IActionResult addCategory(Category category)
        {
            DataManager.
            string Connection = GetConnString();
            using (SqlConnection sqlconnect = new SqlConnection(Connection))
            {
                string sqlQuery = 
                using (SqlCommand sqlcomm = new SqlCommand(sqlQuery, sqlconnect))
                {
                    sqlconnect.Open();
                    sqlcomm.ExecuteNonQuery();
                    return Ok();
                }
            }
        }
    }
}