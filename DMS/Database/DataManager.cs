using DMS.Models;
using Microsoft.AspNetCore.Http;
using ServiceReference1;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Security.Principal;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.ServiceModel.Dispatcher;

namespace DMS.Database
{
    public class DataManager
    {
        private static SQLHelper sqlHelper;
        private static ServiceReference1.AlSahlServiceClient client;
        public DataManager()
        {
            sqlHelper = new SQLHelper(GetConnectionString());
            client = new ServiceReference1.AlSahlServiceClient();

        }

        public static int DeleteCategory(long autoKey)
        {
            if (sqlHelper.Delete(Tables.Categories, "AutoKey = '" + autoKey + "'"))
            {

                if (sqlHelper.Delete(Tables.CategoryUserRel, "CatAutoKey = '" + autoKey + "'"))
                    return (int)ErrorCodes.SUCCESS;
                else
                    return (int)ErrorCodes.INTERNAL_ERROR;
            }
            else
            {
                return (int)ErrorCodes.INTERNAL_ERROR;
            }
        }

        public static int SaveCategory(Category cat)
        {
            if (sqlHelper.Update(Tables.Categories,
                new string[] { "Name" },
                new string[] { cat.Name },
                "AutoKey = '" + cat.AutoKey + "'"))
            {

                return (int)ErrorCodes.SUCCESS;
            }

            else
            {
                return (int)ErrorCodes.INTERNAL_ERROR;
            }

        }

        public static string GetCatName(long autokey)
        {
            return sqlHelper.SelectWithWhere(Tables.Categories,
                "Name",
                "AutoKey = '" + autokey + "'");
        }
        public static int AddCategory(Category category, string userId)
        {
            string query = "select top 1 UserID ";
            query += " from " + Tables.CategoryUserRel + " as CUR";
            query += " where UserID = '" + userId + "'";
            query += " and";
            query += " EXISTS(SELECT Name from " + Tables.Categories + " where AutoKey = CUR.CatAutoKey and Name = '" + category.Name + "')";

            if (sqlHelper.ExecuteReader<object>(query).Count > 0)
                return (int)ErrorCodes.ALREADY_EXISTS;

            string autoKey = sqlHelper.InsertWithID(Tables.Categories,
                new string[] { "Name", "FatherAutoKey" },
                new string[] { category.Name, category.FatherID.ToString() });
            if (!string.IsNullOrEmpty(autoKey))
            {


                if (sqlHelper.Insert(Tables.CategoryUserRel,
                    new string[] { "CatAutoKey", "UserID" },
                    new string[] { autoKey, userId }))
                {

                    return (int)ErrorCodes.SUCCESS;
                }

                else
                {
                    return (int)ErrorCodes.INTERNAL_ERROR;
                }
            }
            else
            {
                return (int)ErrorCodes.INTERNAL_ERROR;
            }
        }


        public static int DeleteContact(long autoKey)
        {
            if (sqlHelper.Delete(Tables.Contacts, "AutoKey = '" + autoKey + "'"))
            {

                return (int)ErrorCodes.SUCCESS;
            }
            else
            {
                return (int)ErrorCodes.INTERNAL_ERROR;
            }
        }

        public static int DeleteKey(long autoKey)
        {
            if (sqlHelper.Delete(Tables.SearchKeys, "AutoKey = '" + autoKey + "'"))
            {

                return (int)ErrorCodes.SUCCESS;
            }
            else
            {
                return (int)ErrorCodes.INTERNAL_ERROR;
            }
        }

        public static int SaveContact(Contact con)
        {
            if (sqlHelper.Update(Tables.Contacts,
                new string[] { "Name" },
                new string[] { con.Name },
                "AutoKey = '" + con.AutoKey + "'"))
            {

                return (int)ErrorCodes.SUCCESS;
            }

            else
            {
                return (int)ErrorCodes.INTERNAL_ERROR;
            }

        }

        public static int SaveKey(SearchKey con)
        {
            if (sqlHelper.Update(Tables.SearchKeys,
                new string[] { "Name" },
                new string[] { con.Name },
                "AutoKey = '" + con.AutoKey + "'"))
            {

                return (int)ErrorCodes.SUCCESS;
            }

            else
            {
                return (int)ErrorCodes.INTERNAL_ERROR;
            }

        }

        public static string GetConName(long autokey)
        {
            return sqlHelper.SelectWithWhere(Tables.Contacts,
                "Name",
                "AutoKey = '" + autokey + "'");
        }
        public static string GetKeyName(long autokey)
        {
            return sqlHelper.SelectWithWhere(Tables.SearchKeys,
                "Name",
                "AutoKey = '" + autokey + "'");
        }
        public static int AddContact(Contact contact, string userId)
        {
            Dictionary<string, string> wheres = new Dictionary<string, string>();

            wheres.Add("Name", contact.Name);
            wheres.Add("DMSUserID", userId);

            if (sqlHelper.Exists(Tables.Contacts, wheres))
                return (int)ErrorCodes.ALREADY_EXISTS;

            string autoKey = sqlHelper.InsertWithID(Tables.Contacts,
                new string[] { "ID", "Name", "DMSUserID" },
                new string[] { contact.Name, userId });
            if (!string.IsNullOrEmpty(autoKey))
            {
                return (int)ErrorCodes.SUCCESS;
            }
            else
            {
                return (int)ErrorCodes.INTERNAL_ERROR;
            }
        }

        public static int AddKey(string name, string userId)
        {
            Dictionary<string, string> wheres = new Dictionary<string, string>();

            wheres.Add("Name", name);
            wheres.Add("DMSUserID", userId);

            if (sqlHelper.Exists(Tables.SearchKeys, wheres))
                return (int)ErrorCodes.ALREADY_EXISTS;

            string autoKey = sqlHelper.InsertWithID(Tables.SearchKeys,
                new string[] { "Name", "DMSUserID" },
                new string[] {name, userId });

            if (!string.IsNullOrEmpty(autoKey))
            {
                return (int)ErrorCodes.SUCCESS;
            }
            else
            {
                return (int)ErrorCodes.INTERNAL_ERROR;
            }
        }
        public static List<Document> GetCatDocuments(long CatID)
        {
            string query = "select DocumentAutoKey,";
            query += " (select DateTimeAdded from " + Tables.DocumentInfo + " where AutoKey = DCR.DocumentAutoKey) as DateTimeAdded,";
            query += " (select Name from " + Tables.DocumentLines + " where InfoAutoKey = DCR.DocumentAutoKey) as Name,";
            query += " (select Ext from " + Tables.DocumentLines + " where InfoAutoKey = DCR.DocumentAutoKey) as Ext";
            query += " from " + Tables.DocumentCategoryRel + " as DCR";
            query += " where CatAutoKey = " + CatID;

            /*
            var reader = sqlHelper.ExecuteReader(query);

            while (reader.Read())
            {
                Document document = new Document();

                document.InfoAutoKey = Convert.ToInt64(reader["InfoAutoKey"]);
                document.Name = Convert.ToString(reader["Name"]);
                document.Ext = Convert.ToString(    reader["Ext"]);
                document.DateTimeAdded = Convert.ToDateTime(reader["DateTimeAdded"]);

                documents.Add(document);
            }
            */
            return sqlHelper.ExecuteReader<Document>(query); 
        }
        public static string GetFileName(long AutoKey)
        {



            /*
            string query = "select InfoAutoKey,";
            query += " (select DateTimeAdded from " + Tables.DocumentInfo + " where AutoKey = DCR.InfoAutoKey) as DateTimeAdded,";
            query += " (select Name from " + Tables.DocumentLines + " where InfoAutoKey = DCR.InfoAutoKey) as Name,";
            query += " (select Ext from " + Tables.DocumentLines + " where InfoAutoKey = DCR.InfoAutoKey) as Ext";
            query += " from " + Tables.DocumentCategoryRel + " as DCR";
            query += " where InfoAutoKey = " + AutoKey;*/

            var LineAutoKey = sqlHelper.SelectWithWhere(Tables.DocumentLines,
                "AutoKey",
                "InfoAutoKey = '" + AutoKey + "'");
            var InfoAutoKey = AutoKey;

            var Ext = sqlHelper.SelectWithWhere(Tables.DocumentLines,
                "Ext",
                "InfoAutoKey = '" + AutoKey + "'");

            return InfoAutoKey + "_" + LineAutoKey + Ext;
            /*
            Document document = sqlHelper.ExecuteReader<Document>(query)[0];

            return document.Name + document.DateTimeAdded.ToString("MM-dd-yyyy-HH-mm-ss") + document.Ext;*/
        }
        public static int AddFile(List<DMSCategory> categories, List<DMSContact> contacts, List<SearchKey> SearchKeys, IFormFile file, string userID, string webRoot)
        {/*
            DMSDocument doc = new DMSDocument();    

            doc.ContactsList = contacts.ToArray();
            doc.DMSCategoriesList = categories.ToArray();
            doc.UserID = "0";
            doc.ManualFileNo = "";
            doc.BarCode = "";
            doc.LinesList = new DMSDocumentLine[]
            {
                new DMSDocumentLine() { Pages = 1 }
            };
            doc.SearchKeysList = new DMSSearchKeys[0];
            
            doc.DateTimeCreated = DateTime.Now;
            doc.DateTimeAdded = DateTime.Now;

            byte[] arr = file.GetBytes().Result;

            var ret = client.InsertDMSDocumentLineAsync(arr, Path.GetExtension(file.FileName), doc).Result;
*/

            DateTime dt = DateTime.Now;
         //   string date = dt.ToString("MM-dd-yyyy-HH-mm-ss");
           // string fileName = Path.GetFileNameWithoutExtension(file.FileName) + "-" + date.Replace(" ", "") + Path.GetExtension(file.FileName);
           // var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(fileName);
           // fileName = System.Convert.ToBase64String(plainTextBytes) + Path.GetExtension(file.FileName);

            string infoAutoKey = sqlHelper.InsertWithID(Tables.DocumentInfo,
                new string[] { "AddedByUserID", "DateTimeAdded", "DateTimeCreated",  "IsDeleted" },
                new string[] { userID, dt.ToString(), dt.ToString(), "0" });

            string lineAutoKey = sqlHelper.InsertWithID(Tables.DocumentLines,
                new string[] { "InfoAutoKey", "Ext", "Name"},
                new string[] { infoAutoKey, Path.GetExtension(file.FileName), Path.GetFileNameWithoutExtension(file.FileName) });

            foreach (var category in categories)
            {
                sqlHelper.Insert(Tables.DocumentCategoryRel,
                    new string[] { "DocumentAutoKey", "CatAutoKey" },
                    new string[] { infoAutoKey, category.AutoKey.ToString() });
            }

            foreach (var contact in contacts)
            {
                sqlHelper.Insert(Tables.DocumentContactRel,
                    new string[] { "DocumentAutoKey", "ContactAutoKey" },
                    new string[] { infoAutoKey, contact.AutoKey.ToString() });
            }
            foreach (var key in SearchKeys)
            {
                sqlHelper.Insert(Tables.DocumentSearchKeysRel,
                    new string[] { "DocumentAutoKey", "ID" },
                    new string[] { infoAutoKey, key.AutoKey.ToString() });
            }

            if (!string.IsNullOrEmpty(infoAutoKey) && !string.IsNullOrEmpty(lineAutoKey))
            {
                string filename = infoAutoKey + "_" + lineAutoKey + Path.GetExtension(file.FileName);
               

                string parent = AddDateDirectories(); // GetParentPathLocator(TmpAdo, "Images");
                string query
                    = " INSERT into Images (stream_id, file_stream, name, path_locator) ";
                query += "  values (NEWID(), @File, '" + filename + "', CAST('" + parent + "' AS hierarchyid))";

                var bytes = file.GetBytes().Result;

                SqlParameter param = new SqlParameter("@File", System.Data.SqlDbType.Binary, bytes.Length);
                param.Value = bytes;

                int i =  sqlHelper.ExecuteNonQuery(query, param);

                if (i > 0)
                    return (int)ErrorCodes.SUCCESS;
                else
                    return (int)ErrorCodes.INTERNAL_ERROR;
            }
            else
            {
                return (int)ErrorCodes.INTERNAL_ERROR;
            }


        }
        private static string AddDateDirectories()
        {
            string year = DateTime.Now.Year.ToString();
            string month = DateTime.Now.Month.ToString();
            string day = DateTime.Now.Day.ToString();
            AddDirectory( year, "Images", "Images");
            AddDirectory( month, year, @"Images\" + year);
            AddDirectory( day, month, @"Images\" + year + @"\" + month);


            return GetParentPathLocator(day);
        }

        private static void AddDirectory(string name, string parent, string parents)
        {
            string query;
            query = @"IF NOT EXISTS ( SELECT 1 FROM Images WHERE path_locator = GetPathLocator(FileTableRootPath() + N'\Images\" + parents + @"\" + name + "'))";
            query += " INSERT INTO Images (name,is_directory,is_archive,path_locator) VALUES ('" + name + "', 1, 0, CAST('" + GetParentPathLocator( parent) + "' AS hierarchyid))";

            sqlHelper.ExecuteNonQuery(query);
        }

        private static string GetParentPathLocator(string parentName)
        {
            string query = "DECLARE @path HIERARCHYID";
            query += " DECLARE @new_path VARCHAR(675)";
            query += " SELECT top 1 @path = path_locator";
            query += " FROM Images";
            query += " WHERE name = '" + parentName + "'";
            query += " SELECT @new_path = @path.ToString()";
            query += " + CONVERT(VARCHAR(20), CONVERT(BIGINT, SUBSTRING(CONVERT(BINARY(16), NEWID()), 1,6))) + '.'";
            query += " + CONVERT(VARCHAR(20), CONVERT(BIGINT, SUBSTRING(CONVERT(BINARY(16), NEWID()), 7,6))) + '.'";
            query += " + CONVERT(VARCHAR(20), CONVERT(BIGINT, SUBSTRING(CONVERT(BINARY(16), NEWID()), 13,4))) + '/'";
            query += " SELECT @new_path ";
            return sqlHelper.ExecuteScalar<string>(query);
        }

        public static bool SaveFile(IFormFile file, string rootPath, string name)
        {
            try
            {

                var path = Path.Combine(rootPath, "uploads");
                // Uncomment to save the file
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                using (var fileStream = System.IO.File.Create(Path.Combine(path, name)))
                {
                    file.CopyTo(fileStream);
                }

                return true;
            }
            catch
            {
                //Response.StatusCode = 400;
                return false;
            }
        }
        public static IEnumerable<Category> GetCategories(string userID)
        {
            //return sqlHelper.Select<Category>(Tables.Categories, new string[] { "*" });

            string query = "select ";
            query += " (select Name from " + Tables.Categories + " where AutoKey = CUR.CatAutoKey) as Name,";
            query += " (select AutoKey from " + Tables.Categories + " where AutoKey = CUR.CatAutoKey) as AutoKey,";
            query += "(select FatherAutoKey from " + Tables.Categories + " where AutoKey = CUR.CatAutoKey) as FatherID";
            query += " from " + Tables.CategoryUserRel + " as CUR";
            query += " where UserID = '" + userID + "'";


            return sqlHelper.ExecuteReader<Category>(query);
        }
        public static IEnumerable<Contact> GetContacts(string userID)
        {
            Dictionary<string,string> wheres = new System.Collections.Generic.Dictionary<string, string>();
            wheres.Add("DMSUserID", userID);

            return sqlHelper.SelectWithWhere<Contact>(Tables.Contacts, new string[] { "*" }, wheres);
        }
        public static IEnumerable<SearchKey> GetSearchKeys(string userID)
        {
            Dictionary<string, string> wheres = new System.Collections.Generic.Dictionary<string, string>();
            wheres.Add("DMSUserID", userID);

            return sqlHelper.SelectWithWhere<SearchKey>(Tables.SearchKeys, new string[] { "*" }, wheres);
        }
        public static int Login(string username, string password)
        {
            try
            {/*
                Dictionary<string, string> wheres = new Dictionary<string, string>();

                wheres.Add("Username", username);
                wheres.Add("Password", password);

                if (sqlHelper.Exists(Tables.Users, wheres))
                    return 0;
                else
                    return (int)ErrorCodes.WRONG_CREDENTIALS;*/

                if (client.loginAsync(username, password).Result)
                    return 0;
                else
                    return (int)ErrorCodes.WRONG_CREDENTIALS;

            }
            catch (Exception)
            {
                return (int)ErrorCodes.INTERNAL_ERROR;
            }
        }

        public static int Register(string email, string username, string password)
        {
            try
            {
                
                Dictionary<string, string> wheres = new Dictionary<string, string>();

                wheres.Add("Email", email);

                if (sqlHelper.Exists(Tables.Users, wheres))
                {
                    return (int)ErrorCodes.EMAIL_EXISTS;
                }

                wheres = new Dictionary<string, string>();

                wheres.Add("Username", username);

                if (sqlHelper.Exists(Tables.Users, wheres))
                {
                    return (int)ErrorCodes.USERNAME_EXISTS;
                }
                

                if (sqlHelper.Insert(Tables.Users,
                    new string[] { "Email", "Username", "Password" },
                    new string[] { email, username, password }))
                    return 0;
                else
                    return (int)ErrorCodes.INTERNAL_ERROR;
                    

            }
            catch (Exception)
            {
                return (int)ErrorCodes.INTERNAL_ERROR;
            }
        }

        public static string GetConnectionString()
        {
            return new SqlConnectionStringBuilder()
            {
                DataSource = @"192.168.1.141\ALSAHL",
                UserID = "test",
                Password = "test_2008",
                InitialCatalog = @"D:\AlSahl\Data\MoneySql.MDF"
            }.ConnectionString;
        }
    }

    public enum ErrorCodes
    {
        SUCCESS = 0,
        WRONG_CREDENTIALS = 101,
        USERNAME_EXISTS = 102,
        INTERNAL_ERROR = 103,
        EMAIL_EXISTS = 104,
        ALREADY_EXISTS = 105
    }

    public static class FormFileExtensions
    {
        public static async Task<byte[]> GetBytes(this IFormFile formFile)
        {
            using (var memoryStream = new MemoryStream())
            {
                await formFile.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
