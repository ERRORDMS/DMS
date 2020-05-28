using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DMS.Database;
using DMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DMS.Controllers
{
    [Route("api/[controller]")]
    public class SearchController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        [Route("SearchCats")]
        [HttpGet]
        public ActionResult SearchCats(string sQuery)
        {
            if (string.IsNullOrEmpty(sQuery))
                return BadRequest();

            List<Category> categories = new List<Category>();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            SQLHelper helper = new SQLHelper(DataManager.GetConnectionString());
            foreach (var word in sQuery.Split(" "))
            {
                
                string query = "SELECT";

                query += " D.AutoKey,";
                query += " DL.Name,";
                query += " DL.Ext,";
                query += " C.Name as ConName,";
                query += " Cat.FatherID,";
                query += " Cat.Name as CatName,";
                query += " Cat.AutoKey as CatAutoKey";
                query += " FROM";
                query += " DocumentInfo D";
                query += " INNER JOIN DocumentLines DL on DL.InfoAutoKey = D.AutoKey";
                query += " INNER JOIN DocumentContactRel DCR on DCR.InfoAutoKey = D.AutoKey";
                query += " INNER JOIN DocumentCategoryRel DCL on DCL.InfoAutoKey = D.AutoKey";
                query += " INNER JOIN Categories Cat on Cat.AutoKey = DCL.CategoryAutoKey";
                query += " INNER JOIN Contacts C on C.AutoKey = DCR.ContactAutoKey";
                query += " WHERE";
                query += " D.AddedBy = '" + userId + "'";
                query += " AND DL.Name like '%" + word + "%'";
                query += " OR C.Name like '%" + word + "%'";
                query += " OR Cat.Name like '%" + word + "%'";

                SearchResult sResult = new SearchResult();
                var reader =  helper.ExecuteReader(query);
                while (reader.Read())
                {
                    Category category = new Category();
                    category.Name = Convert.ToString(reader["CatName"]);
                    category.FatherID = Convert.ToInt64(reader["FatherID"]);
                    category.AutoKey = Convert.ToInt64(reader["CatAutoKey"]);

                    categories.Add(category);
                }

                
            }
            helper.Dispose();


            return new JsonResult(categories);
        }
        /*
        [Route("Search")]
        [HttpGet]
        public ActionResult Search(string sQuery)
        {
            if (string.IsNullOrEmpty(sQuery))
                return BadRequest();

            List<SearchResult> result = new List<SearchResult>();
            List<Category> categories = new List<Category>();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            SQLHelper helper = new SQLHelper(DataManager.GetConnectionString());
            foreach (var word in sQuery.Split(" "))
            {

                string query = "SELECT";

                query += " D.AutoKey,";
                query += " DL.Name,";
                query += " DL.Ext,";
                query += " C.Name as ConName,";
                query += " Cat.FatherID,";
                query += " Cat.Name as CatName,";
                query += " Cat.AutoKey as CatAutoKey";
                query += " FROM";
                query += " DocumentInfo D";
                query += " INNER JOIN DocumentLines DL on DL.InfoAutoKey = D.AutoKey";
                query += " INNER JOIN DocumentContactRel DCR on DCR.InfoAutoKey = D.AutoKey";
                query += " INNER JOIN DocumentCategoryRel DCL on DCL.InfoAutoKey = D.AutoKey";
                query += " INNER JOIN Categories Cat on Cat.AutoKey = DCL.CategoryAutoKey";
                query += " INNER JOIN Contacts C on C.AutoKey = DCR.ContactAutoKey";
                query += " WHERE";
                query += " D.AddedBy = '" + userId + "'";
                query += " AND DL.Name like '%" + word + "%'";
                query += " OR C.Name like '%" + word + "%'";
                query += " OR Cat.Name like '%" + word + "%'";

                SearchResult sResult = new SearchResult();
                var reader = helper.ExecuteReader(query);
                while (reader.Read())
                {
                    Category category = new Category();
                    category.Name = Convert.ToString(reader["CatName"]);
                    category.FatherID = Convert.ToInt64(reader["FatherID"]);
                    category.AutoKey = Convert.ToInt64(reader["CatAutoKey"]);

                    categories.Add(category);


                    sResult.AutoKey = Convert.ToInt64(reader["AutoKey"]);
                    sResult.CatAutoKey = Convert.ToInt64(reader["CatAutoKey"]);
                    sResult.Ext = Convert.ToString(reader["Ext"]);
                    sResult.Name = Convert.ToString(reader["Name"]);
                    sResult.ConName = Convert.ToString(reader["ConName"]);

                    result.Add(sResult);
                }


            }
            helper.Dispose();


            return new JsonResult(result);
        }
        */
        [Route("SearchFiles")]
        [HttpGet]
        public ActionResult SearchFiles(string sQuery)
        {
            if (string.IsNullOrEmpty(sQuery))
                return BadRequest();

            List<SearchResult> result = new List<SearchResult>();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            SQLHelper helper = new SQLHelper(DataManager.GetConnectionString());
            foreach (var word in sQuery.Split(" "))
            {

                string query = "SELECT";

                query += " D.AutoKey,";
                query += " DL.Name,";
                query += " DL.InfoAutoKey, ";
                query += " DL.Ext,";
                query += " C.Name as ConName,";
                query += " Cat.FatherID,";
                query += " Cat.Name as CatName,";
                query += " Cat.AutoKey as CatAutoKey";
                query += " FROM";
                query += " DocumentInfo D";
                query += " INNER JOIN DocumentLines DL on DL.InfoAutoKey = D.AutoKey";
                query += " INNER JOIN DocumentContactRel DCR on DCR.InfoAutoKey = D.AutoKey";
                query += " INNER JOIN DocumentCategoryRel DCL on DCL.InfoAutoKey = D.AutoKey";
                query += " INNER JOIN Categories Cat on Cat.AutoKey = DCL.CategoryAutoKey";
                query += " INNER JOIN Contacts C on C.AutoKey = DCR.ContactAutoKey";
                query += " WHERE";
                query += " D.AddedBy = '" + userId + "'";
                query += " AND DL.Name like '%" + word + "%'";
                query += " OR C.Name like '%" + word + "%'";
                query += " OR Cat.Name like '%" + word + "%'";

                SearchResult sResult = new SearchResult();
                var reader = helper.ExecuteReader(query);
                while (reader.Read())
                {
                    sResult.InfoAutoKey = Convert.ToInt64(reader["InfoAutoKey"]);
                    sResult.AutoKey = Convert.ToInt64(reader["AutoKey"]);
                    sResult.CatAutoKey = Convert.ToInt64(reader["CatAutoKey"]);
                    sResult.Ext = Convert.ToString(reader["Ext"]);
                    sResult.Name = Convert.ToString(reader["Name"]);
                    sResult.ConName = Convert.ToString(reader["ConName"]);

                    result.Add(sResult);
                }


            }
            helper.Dispose();


            return new JsonResult(result);
        }
    }


    public class SearchResult
    {
        public long InfoAutoKey { get; set; }
        public long AutoKey { get; set; }
        public string Name { get; set; }
        public string Ext { get; set; }
        public string ConName { get; set; }
        public long CatAutoKey { get; set; }
    }

}