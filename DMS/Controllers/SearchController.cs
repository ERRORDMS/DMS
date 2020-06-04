using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DMS.Database;
using DMS.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DMS.Controllers
{
    [Authorize]
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
            var userId = User.FindFirstValue(ClaimTypes.UserData);

            SQLHelper helper = new SQLHelper(DataManager.GetConnectionString());
            string[] words = sQuery.Split(' ');

            string query = "SELECT";

            query += " D.InfoAutoKey,";
            query += " DL.Name,";
            query += " DL.Ext,";
            query += " C.Name as ConName,";
            query += " Cat.FatherAutoKey,";
            query += " Cat.Name as CatName,";
            query += " Cat.AutoKey as CatAutoKey";
            query += " FROM";
            query += " " + Tables.DocumentInfo + " D";
            query += " INNER JOIN " + Tables.DocumentLines + " DL on DL.InfoAutoKey = D.InfoAutoKey";
            query += " LEFT JOIN " + Tables.DocumentContactRel + " DCR on DCR.DocumentAutoKey = D.InfoAutoKey";
            query += " LEFT JOIN " + Tables.DocumentCategoryRel + " DCL on DCL.DocumentAutoKey = D.InfoAutoKey";
            query += " LEFT JOIN " + Tables.Categories + " Cat on Cat.AutoKey = DCL.CatAutoKey";
            query += " LEFT JOIN " + Tables.Contacts + " C on C.AutoKey = DCR.ContactAutoKey";
            query += " LEFT JOIN " + Tables.DocumentSearchKeysRel + " DSKR on DSKR.DocumentAutoKey = D.InfoAutoKey";
            query += " LEFT JOIN " + Tables.SearchKeys + " S on S.AutoKey = DSKR.SearchAutoKey";
            query += " WHERE";

            query += " D.AddedByUserID = '" + userId + "'";


            for (int i = 0; i < words.Length; i++)
            {
                string word = words[i];
                if (i == 0)
                {
                    query += " AND";
                    query += " (";
                    query += " DL.Name like '%" + word + "%'";
                    query += " OR C.Name like '%" + word + "%'";
                    query += " OR S.Name like '%" + word + "%'";
                    query += " OR Cat.Name like '%" + word + "%'";
                    query += " )";
                }
                else
                {

                    query += " OR ";
                    query += " (";
                    query += " DL.Name like '%" + word + "%'";
                    query += " OR C.Name like '%" + word + "%'";
                    query += " OR S.Name like '%" + word + "%'";
                    query += " OR Cat.Name like '%" + word + "%'";
                    query += " )";
                }
            }

            var reader = helper.ExecuteReader(query);
            while (reader.Read())
            {
                long autokey = Convert.ToInt64(reader["CatAutoKey"]);

                if (!categories.Any(S => S.AutoKey == autokey))
                {
                    Category category = new Category();
                    category.Name = Convert.ToString(reader["CatName"]);
                    category.FatherID = 0;//Convert.ToInt64(reader["FatherAutoKey"]);
                    category.AutoKey = autokey;

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
            var userId = User.FindFirstValue(ClaimTypes.UserData);

            SQLHelper helper = new SQLHelper(DataManager.GetConnectionString());
            string[] words = sQuery.Split(' ');

            string query = "SELECT";

            query += " D.InfoAutoKey,";
            query += " DL.Name,";
            query += " DL.AutoKey as LineAutoKey, ";
            query += " DL.Ext,";
            query += " C.Name as ConName,";
            query += " Cat.FatherAutoKey,";
            query += " Cat.Name as CatName,";
            query += " Cat.AutoKey as CatAutoKey";
            query += " FROM";
            query += " " + Tables.DocumentInfo + " D";
            query += " INNER JOIN " + Tables.DocumentLines + " DL on DL.InfoAutoKey = D.InfoAutoKey";
            query += " LEFT JOIN " + Tables.DocumentContactRel + " DCR on DCR.DocumentAutoKey = D.InfoAutoKey";
            query += " LEFT JOIN " + Tables.DocumentCategoryRel + " DCL on DCL.DocumentAutoKey = D.InfoAutoKey";
            query += " LEFT JOIN " + Tables.Categories + " Cat on Cat.AutoKey = DCL.CatAutoKey";
            query += " LEFT JOIN " + Tables.DocumentSearchKeysRel + " DSKR on DSKR.DocumentAutoKey = D.InfoAutoKey";
            query += " LEFT JOIN " + Tables.Contacts + " C on C.AutoKey = DCR.ContactAutoKey";
            query += " LEFT JOIN " + Tables.SearchKeys + " S on S.AutoKey = DSKR.SearchAutoKey";
            query += " WHERE";
            query += " D.AddedByUserID = '" + userId + "'";


            for (int i = 0; i < words.Length; i++)
            {
                string word = words[i];
                if (i == 0)
                {
                    query += " AND";
                    query += " (";
                    query += " DL.Name like '%" + word + "%'";
                    query += " OR C.Name like '%" + word + "%'";
                    query += " OR S.Name like '%" + word + "%'";
                    query += " OR Cat.Name like '%" + word + "%'";
                    query += " )";
                }
                else
                {

                    query += " OR ";
                    query += " (";
                    query += " DL.Name like '%" + word + "%'";
                    query += " OR C.Name like '%" + word + "%'";
                    query += " OR S.Name like '%" + word + "%'";
                    query += " OR Cat.Name like '%" + word + "%'";
                    query += " )";
                }
            }

            SearchResult sResult;
            var reader = helper.ExecuteReader(query);
            while (reader.Read())
            {
                sResult = new SearchResult();
                sResult.DocumentAutoKey = Convert.ToInt64(reader["InfoAutoKey"]);
                sResult.LineAutoKey = Convert.ToInt64(reader["LineAutoKey"]);
                sResult.CatAutoKey = Convert.ToInt64(reader["CatAutoKey"]);
                sResult.Ext = Convert.ToString(reader["Ext"]);
                sResult.Name = Convert.ToString(reader["Name"]);
                sResult.ConName = Convert.ToString(reader["ConName"]);

                result.Add(sResult);
            }

            helper.Dispose();


            return new JsonResult(result);


        }


    }


    public class SearchResult
    {
        public long DocumentAutoKey { get; set; }
        public long LineAutoKey { get; set; }
        public string Name { get; set; }
        public string Ext { get; set; }
        public string ConName { get; set; }
        public long CatAutoKey { get; set; }
    }

}