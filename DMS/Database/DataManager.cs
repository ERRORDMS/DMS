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

        /*
        public static bool AddFile(List<Category> categories, List<Contacts> contacts, IFormFile file)
        {
            DMSDocument doc = new DMSDocument();

            DMScot

            doc.ContactsList = 
            client.InsertDMSDocumentLineAsync(file.GetBytes(), Path.GetExtension(file.FileName), )
        }
        */
        public static IEnumerable<Category> GetCategories()
        {
            return sqlHelper.Select<Category>(Tables.Categories, new string[] { "*" });
        }
        public static IEnumerable<Contacts> GetContacts()
        {
            return sqlHelper.Select<Contacts>(Tables.Contacts, new string[] { "*" });
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
