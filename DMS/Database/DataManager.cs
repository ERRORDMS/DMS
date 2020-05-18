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

                if (sqlHelper.Delete(Tables.CategoryUserRel, "CategoryAutoKey = '" + autoKey + "'"))
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
            query += " from CategoryUserRel as CUR";
            query += " where UserID = '" + userId + "'";
            query += " and";
            query += " EXISTS(SELECT Name from Categories where AutoKey = CUR.CategoryAutoKey and Name = '" + category.Name + "')";

            if (sqlHelper.ExecuteReader<object>(query).Count > 0)
                return (int)ErrorCodes.CATEGORY_EXISTS;

            string autoKey = sqlHelper.InsertWithID(Tables.Categories,
                new string[] { "Name", "FatherID" },
                new string[] { category.Name, category.FatherID.ToString() });
            if (!string.IsNullOrEmpty(autoKey))
            {


                if (sqlHelper.Insert(Tables.CategoryUserRel,
                    new string[] { "CategoryAutoKey", "UserID" },
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

        public static string GetConName(long autokey)
        {
            return sqlHelper.SelectWithWhere(Tables.Contacts,
                "Name",
                "AutoKey = '" + autokey + "'");
        }
        public static int AddContact(Contact contact, string userId)
        {
            Dictionary<string, string> wheres = new Dictionary<string, string>();

            wheres.Add("Name", contact.Name);
            wheres.Add("UserID", userId);

            if (sqlHelper.Exists(Tables.Contacts, wheres))
                return (int)ErrorCodes.CATEGORY_EXISTS;

            string autoKey = sqlHelper.InsertWithID(Tables.Contacts,
                new string[] { "Name", "UserID" },
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
        public static List<Document> GetCatDocuments(long CatID)
        {
            List<Document> documents = new List<Document>();

            string query = "select InfoAutoKey,";
            query += " (select DateTimeAdded from " + Tables.DocumentInfo + " where AutoKey = DCR.InfoAutoKey) as DateTimeAdded,";
            query += " (select Name from " + Tables.DocumentLines + " where InfoAutoKey = DCR.InfoAutoKey) as Name,";
            query += " (select Ext from " + Tables.DocumentLines + " where InfoAutoKey = DCR.InfoAutoKey) as Ext";
            query += " from " + Tables.DocumentCategoryRel + " as DCR";
            query += " where CategoryAutoKey = " + CatID;

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
            documents = sqlHelper.ExecuteReader<Document>(query);
            return documents;
        }
        public static string GetFileName(long AutoKey)
        {
            


            string query = "select InfoAutoKey,";
            query += " (select DateTimeAdded from " + Tables.DocumentInfo + " where AutoKey = DCR.InfoAutoKey) as DateTimeAdded,";
            query += " (select Name from " + Tables.DocumentLines + " where InfoAutoKey = DCR.InfoAutoKey) as Name,";
            query += " (select Ext from " + Tables.DocumentLines + " where InfoAutoKey = DCR.InfoAutoKey) as Ext";
            query += " from " + Tables.DocumentCategoryRel + " as DCR";
            query += " where InfoAutoKey = " + AutoKey;

            Document document = sqlHelper.ExecuteReader<Document>(query)[0];

            return document.Name + document.DateTimeAdded.ToString("MM-dd-yyyy-HH-mm-ss") + document.Ext;
        }
        public static int AddFile(List<DMSCategory> categories, List<DMSContact> contacts, IFormFile file, string userID, string webRoot)
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
            string date = dt.ToString("MM-dd-yyyy-HH-mm-ss");
            string infoAutoKey = sqlHelper.InsertWithID(Tables.DocumentInfo,
                new string[] { "AddedBy", "DateTimeAdded", "IsDeleted" },
                new string[] { userID, dt.ToString(), "0" });

            bool success = sqlHelper.Insert(Tables.DocumentLines,
                new string[] { "InfoAutoKey", "Ext", "Name" },
                new string[] { infoAutoKey, Path.GetExtension(file.FileName), Path.GetFileNameWithoutExtension(file.FileName) });

            foreach (var category in categories)
            {
                sqlHelper.Insert(Tables.DocumentCategoryRel,
                    new string[] { "InfoAutoKey", "CategoryAutoKey" },
                    new string[] { infoAutoKey, category.AutoKey.ToString() });
            }

            foreach (var contact in contacts)
            {
                sqlHelper.Insert(Tables.DocumentContactRel,
                    new string[] { "InfoAutoKey", "ContactAutoKey" },
                    new string[] { infoAutoKey, contact.AutoKey.ToString() });
            }

            if (success)
            {

                string fileName = Path.GetFileNameWithoutExtension(file.FileName) + "-" + date.Replace(" ", "") + Path.GetExtension(file.FileName);

                if (SaveFile(file, webRoot, fileName))
                    return (int)ErrorCodes.SUCCESS;
                else
                    return (int)ErrorCodes.INTERNAL_ERROR;
            }
            else
            {
                return (int)ErrorCodes.INTERNAL_ERROR;
            }


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
            query += " (select Name from " + Tables.Categories + " where AutoKey = CUR.CategoryAutoKey) as Name,";
            query += " (select AutoKey from " + Tables.Categories + " where AutoKey = CUR.CategoryAutoKey) as AutoKey,";
            query += "(select FatherID from " + Tables.Categories + " where AutoKey = CUR.CategoryAutoKey) as FatherID";
            query += " from " + Tables.CategoryUserRel + " as CUR";
            query += " where UserID = '" + userID + "'";


            return sqlHelper.ExecuteReader<Category>(query);
        }
        public static IEnumerable<Contact> GetContacts()
        {
            return sqlHelper.Select<Contact>(Tables.Contacts, new string[] { "*" });
        }
        public static int Login(string username, string password)
        {
            try
            {
                Dictionary<string, string> wheres = new Dictionary<string, string>();

                wheres.Add("Username", username);
                wheres.Add("Password", password);

                if (sqlHelper.Exists(Tables.Users, wheres))
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
                DataSource = @"192.168.1.235\ALSAHL",
                UserID = "test",
                Password = "test_2008",
                InitialCatalog = "DMS"
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
        CATEGORY_EXISTS = 105
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

public static class ExtensionMethods
{
    public static T GetLoggedInUserId<T>(this ClaimsPrincipal principal)
    {
        if (principal == null)
            throw new ArgumentNullException(nameof(principal));

        var loggedInUserId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

        if (typeof(T) == typeof(string))
        {
            return (T)Convert.ChangeType(loggedInUserId, typeof(T));
        }
        else if (typeof(T) == typeof(int) || typeof(T) == typeof(long))
        {
            return loggedInUserId != null ? (T)Convert.ChangeType(loggedInUserId, typeof(T)) : (T)Convert.ChangeType(0, typeof(T));
        }
        else
        {
            throw new Exception("Invalid type provided");
        }
    }

    public static string GetLoggedInUserName(this ClaimsPrincipal principal)
    {
        if (principal == null)
            throw new ArgumentNullException(nameof(principal));

        return principal.FindFirstValue(ClaimTypes.Name);
    }

    public static string GetLoggedInUserEmail(this ClaimsPrincipal principal)
    {
        if (principal == null)
            throw new ArgumentNullException(nameof(principal));

        return principal.FindFirstValue(ClaimTypes.Email);
    }
}