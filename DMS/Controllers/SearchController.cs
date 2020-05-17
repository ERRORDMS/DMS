using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DMS.Database;
using Microsoft.AspNetCore.Mvc;

namespace DMS.Controllers
{
    [Route("api/[controller]")]
    public class SearchController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public ActionResult Search(string sQuery)
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
                query += " DL.Ext,";
                query += " C.Name as ConName,";
                query += " Cat.FatherID,";
                query += " Cat.Name as CatName";
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

                var list =  helper.ExecuteReader<SearchResult>(query);

                result.AddRange(list);
                
            }
            helper.Dispose();


            return new JsonResult(result);
        }
    }


    public class SearchResult
    {
        public long AutoKey { get; set; }
        public string Name { get; set; }
        public string Ext { get; set; }
        public string ConName { get; set; }
        public long FatherID { get; set; }
        public string CatName { get; set; }
    }
}