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
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class SearchController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        [Route("Search")]
        [HttpGet]
        public ActionResult Search(string sQuery, string userId = null)
        {
            if (string.IsNullOrEmpty(userId))
                userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var dm = new DataManager(userId);
            var cats = dm.GetEnterpriseCategories(userId);
            SQLHelper helper = dm.SQLHelper;
            string query = "";
            try
            {
                if (string.IsNullOrEmpty(sQuery))
                    return BadRequest();

              
                string[] words = sQuery.Split(' ');
                bool isEnterpriseSubUser = new DataManager(null).IsEnterpriseSubUser(userId);

                query = "SELECT";
                query += " ROW_NUMBER() OVER(ORDER BY DL.AutoKey ASC) AS ID,";
                query += " D.DateTimeAdded,";
                query += " D.InfoAutoKey,";
                query += " DL.Name,";
                query += " DL.AutoKey as LineAutoKey, ";
                query += " DL.Ext,";
                query += " DL.FileSize as Size,";
                query += " D.Note as Note,";
                query += " AddedByUserID as UserID,";

                query += " C.Name as ConName,";
                query += " Cat.AutoKey as CatAutoKey,";
                query += " Cat.Name as CatName";
                query += " FROM";
                query += " " + Tables.DocumentInfo + " D";
                query += " INNER JOIN " + Tables.DocumentLines + " DL on DL.InfoAutoKey = D.InfoAutoKey";
                query += " LEFT JOIN " + Tables.DocumentContactRel + " DCR on DCR.DocumentAutoKey = D.InfoAutoKey";
                query += " LEFT JOIN " + Tables.DocumentCategoryRel + " DCL on DCL.DocumentAutoKey = D.InfoAutoKey";
                query += " LEFT JOIN " + Tables.Categories + " Cat on Cat.AutoKey = DCL.CatAutoKey";
                query += " LEFT JOIN " + Tables.DocumentSearchKeysRel + " DSKR on DSKR.DocumentAutoKey = D.InfoAutoKey";
                query += " LEFT JOIN " + Tables.Contacts + " C on C.AutoKey = DCR.ContactAutoKey";
                query += " LEFT JOIN " + Tables.SearchKeys + " S on S.AutoKey = DSKR.SearchAutoKey";
                query += " WHERE IsDeleted <> 1 ";

                for (int i = 0; i < words.Length; i++)
                {
                    string word = words[i];

                    if (i == 0)
                    {
                        query += " AND ";
                    }
                    else
                    {
                        query += " OR ";
                    }

                    query += " (";
                    query += " DL.Name like N'%" + word + "%'";
                    query += " OR C.Name like N'%" + word + "%'";
                    query += " OR S.Name like N'%" + word + "%'";
                    query += " OR Cat.Name like N'%" + word + "%'";
                    query += " OR Note like N'%" + word + "%'";

                    query += " )";

                }
                query += "order by DateTimeAdded desc";



                SearchResult sResult = new SearchResult();
                sResult.Files = new List<SearchResultFile>();
                sResult.Categories = new List<SearchResultCategory>();
                var reader = helper.ExecuteReader(query);
                
                while (reader.Read())
                {
                    object catAutoKey = reader["CatAutoKey"];

                    if (catAutoKey == DBNull.Value)
                        continue;


                    if (isEnterpriseSubUser)
                    {

                        if (!cats.Any(S => S.AutoKey == Convert.ToInt64(catAutoKey)))
                            continue;
                    }

                    SearchResultFile fileResult = new SearchResultFile();
                    fileResult.ID = ConvertToLong(reader["ID"]);
                    fileResult.UserID = Convert.ToString(reader["UserID"]);

                    fileResult.DateTimeAdded = Convert.ToDateTime(reader["DateTimeAdded"]);
                    fileResult.DocumentAutoKey = ConvertToLong(reader["InfoAutoKey"]);
                    fileResult.LineAutoKey = ConvertToLong(reader["LineAutoKey"]);
                    fileResult.Size = Convert.ToInt64(reader["Size"]);

                    fileResult.CatAutoKey = ConvertToLong(catAutoKey);
                    fileResult.Ext = Convert.ToString(reader["Ext"]);
                    fileResult.Name = Convert.ToString(reader["Name"]);
                    fileResult.Note = Convert.ToString(reader["Note"]);

                    fileResult.ConName = Convert.ToString(reader["ConName"]);

                    sResult.Files.Add(fileResult);

                    if (!sResult.Categories.Any(S => S.AutoKey == fileResult.CatAutoKey))
                    {
                        sResult.Categories.Add(new SearchResultCategory() { AutoKey = fileResult.CatAutoKey, Name = Convert.ToString(reader["CatName"]) });
                    }

                }



                return new JsonResult(sResult);

            }
            catch(Exception ex)
            {
                Logger.Log(ex.Message);
                Logger.Log("Query: " + query);
                return BadRequest();
            }
            finally
            {
                dm.Dispose();
                helper.Dispose();

            }
        }


        public long ConvertToLong(object from)
        {
            if (!Int64.TryParse(Convert.ToString(from), out long result))
            {
                result = 0;
            }

            return result;
        }

    }


    public class SearchResult
    {

        public List<SearchResultFile> Files { get; set; }
        public List<SearchResultCategory> Categories { get; set; }
    }

    public class SearchResultFile
    {
        public long ID { get; set; }
        public DateTime DateTimeAdded { get; set; }
        public long DocumentAutoKey { get; set; }
        public long LineAutoKey { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }

        public string Ext { get; set; }
        public string ConName { get; set; }
        public long CatAutoKey { get; set; }
        public long Size { get; set; }
        public string UserID { get; set; }
    }
    public class SearchResultCategory
    {

        public string Name { get; set; }
        public long AutoKey { get; set; }
    }

}