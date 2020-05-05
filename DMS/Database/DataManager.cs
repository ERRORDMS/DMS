using DMS.Models;
using Microsoft.AspNetCore.Http;
using ServiceReference1;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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

        public static int AddCategory(Category category)
        {
            Dictionary<String, string> existDic = new Dictionary<string, string>();
            existDic.Add("Name", category.Name);

            if (sqlHelper.Exists("Categories", existDic))
                return (int)ErrorCodes.CATEGORY_EXISTS;

            if(sqlHelper.Insert("Categories",
                new string[] { "Name", "FatherID" },
                new string[] { category.Name, category.FatherID.ToString() }))
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
            query += " (select DateTimeAdded from DocumentInfo where AutoKey = DCR.InfoAutoKey) as DateTimeAdded,";
            query += " (select Name from DocumentLines where InfoAutoKey = DCR.InfoAutoKey) as Name,";
            query += " (select Ext from DocumentLines where InfoAutoKey = DCR.InfoAutoKey) as Ext";
            query += " from DocumentCategoryRel as DCR";
            query += " where CategoryAutoKey = " + CatID;

            /*
            var reader = sqlHelper.ExecuteReader(query);

            while (reader.Read())
            {
                Document document = new Document();

                document.InfoAutoKey = Convert.ToInt64(reader["InfoAutoKey"]);
                document.Name = Convert.ToString(reader["Name"]);
                document.Ext = Convert.ToString(reader["Ext"]);
                document.DateTimeAdded = Convert.ToDateTime(reader["DateTimeAdded"]);

                documents.Add(document);
            }
            */
            documents = sqlHelper.ExecuteReader<Document>(query);
            return documents;
        }
        public static int AddFile(List<DMSCategory> categories, List<DMSContact> contacts, IFormFile file)
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

            string infoAutoKey= sqlHelper.InsertWithID("DocumentInfo",
                new string[] { "AddedBy", "DateTimeAdded", "IsDeleted" },
                new string[] { "0", DateTime.Now.ToString(), "0" });

            bool success = sqlHelper.Insert("DocumentLines",
                new string[] { "InfoAutoKey", "Ext", "Name" },
                new string[] { infoAutoKey, Path.GetExtension(file.FileName), Path.GetFileNameWithoutExtension(file.FileName) });

            foreach(var category in categories)
            {
                sqlHelper.Insert("DocumentCategoryRel",
                    new string[] { "InfoAutoKey", "CategoryAutoKey" },
                    new string[] { infoAutoKey, category.AutoKey.ToString() });
            }

            foreach (var contact in contacts)
            {
                sqlHelper.Insert("DocumentContactRel",
                    new string[] { "InfoAutoKey", "ContactAutoKey" },
                    new string[] { infoAutoKey, contact.AutoKey.ToString() });
            }

            if (success)
            {
                return (int)ErrorCodes.SUCCESS;
            }
            else
            {
                return (int)ErrorCodes.INTERNAL_ERROR;
            }


        }
        
        public static IEnumerable<Category> GetCategories()
        {
            return sqlHelper.Select<Category>(Tables.Categories, new string[] { "*" });
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

            }catch(Exception)
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

        public string GetConnectionString()
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
        WRONG_CREDENTIALS  = 101,
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
