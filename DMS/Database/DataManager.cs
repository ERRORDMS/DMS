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
using System.ServiceModel;
using DMS.Controllers;
using Newtonsoft.Json;
using System.Globalization;
using  static DMS.Controllers.AuthorizationController;
using DevExpress.Web;
using System.Text.RegularExpressions;
using Dapper;
using System.Text;
using DevExpress.Utils.Text;
using Newtonsoft.Json.Linq;
using System.Dynamic;
using Microsoft.CSharp.RuntimeBinder;

namespace DMS.Database
{
    public class DataManager
    {
        private  SQLHelper sqlHelper;
        private  ServiceReference1.AlSahlServiceClient client;
        private  Settings settings;
        public DataManager(string userID)
        {
            try
            {
                settings = JsonConvert.DeserializeObject<Settings>(SettingsController.GetSettings());
                client = new ServiceReference1.AlSahlServiceClient(new BasicHttpBinding(), new EndpointAddress(new Uri(settings.ServiceEndpoint)));

                var con = GetConnectionString("");

                sqlHelper = new SQLHelper(con);


                if (!string.IsNullOrEmpty(userID))
                {
                    con = GetConnectionString(userID);

                    sqlHelper = new SQLHelper(con);
                }
            }   
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
            }
        }

        public void SetAllCatPermissions(string userID, bool value)
        {
            sqlHelper.Update("[" + userID + "]",
                new string[] { "CanEdit", "CanDelete" },
                new string[] { value.ToString(), value.ToString() }, "");
        }

        public AlSahlServiceClient GetClient() { return client; }
        
        public int DeleteCategory(long autoKey)
        {
            try
            {
                if (sqlHelper.Delete(Tables.Categories, "AutoKey = '" + autoKey + "'"))
                {

                    /*if (sqlHelper.Delete(Tables.CategoryUserRel, "CatAutoKey = '" + autoKey + "'"))
                    {
                        sqlHelper.ExecuteNonQuery("UPDATE " + Tables.Categories + " SET FatherAutoKey = 0 where FatherAutoKey = " + autoKey);
                        return (int)ErrorCodes.SUCCESS;
                    }
                    else
                        return (int)ErrorCodes.INTERNAL_ERROR;*/
                    return (int)ErrorCodes.SUCCESS;

                }
                else
                {
                    return (int)ErrorCodes.INTERNAL_ERROR;
                }
            }catch(Exception ex)
            {
                Logger.Log(ex.Message);
                return (int)ErrorCodes.INTERNAL_ERROR;

            }
        }

        public  int SaveCategory(Category cat, string userId)
        {
            try
            {/*
                string query = "select top 1 UserID ";
                query += " from " + Tables.CategoryUserRel + " as CUR";
                query += " where UserID = '" + userId + "'";
                query += " and";
                query += " EXISTS(SELECT Name from " + Tables.Categories + " where AutoKey = CUR.CatAutoKey and Name = '" + cat.Name + "')";

                if (sqlHelper.ExecuteReader<object>(query).Count > 0)
                    return (int)ErrorCodes.ALREADY_EXISTS;
                */
                Dictionary<string, string> wheres = new Dictionary<string, string>();
                wheres.Add("Name", cat.Name);
                wheres.Add("FatherAutoKey", cat.FatherAutoKey.ToString());

                if (sqlHelper.Exists(Tables.Categories, wheres))
                    return (int)ErrorCodes.ALREADY_EXISTS;

                if (sqlHelper.Update(Tables.Categories,
                    new string[] { "Name"},
                    new string[] { cat.Name},
                    "AutoKey = '" + cat.AutoKey + "'"))
                {

                    return (int)ErrorCodes.SUCCESS;
                }

                else
                {
                    return (int)ErrorCodes.INTERNAL_ERROR;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
                return (int)ErrorCodes.INTERNAL_ERROR;

            }

        }

        public int SaveUser(string userID, List<Permission> permissions)
        {
            try
            {
                sqlHelper.Delete(Tables.UserPermissions, "UserID = '" + userID + "'");

                foreach(var permission in permissions)
                {
                    if(!sqlHelper.Insert(Tables.UserPermissions,
                        new string[] { "PermissionID", "UserID"},
                        new string[] { permission.AutoKey.ToString(), userID }))
                    {
                        return (int)ErrorCodes.INTERNAL_ERROR;
                    }
                }



                return (int)ErrorCodes.SUCCESS;
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
                return (int)ErrorCodes.INTERNAL_ERROR;

            }

        }

        public int SaveRole(string roleID, List<Permission> permissions)
        {
            try
            {
                sqlHelper.Delete(Tables.RolePermissions, "RoleID = '" + roleID + "'");

                foreach (var permission in permissions)
                {
                    if (!sqlHelper.Insert(Tables.RolePermissions,
                        new string[] { "PermissionID", "RoleID" },
                        new string[] { permission.AutoKey.ToString(), roleID }))
                    {
                        return (int)ErrorCodes.INTERNAL_ERROR;
                    }
                }



                return (int)ErrorCodes.SUCCESS;
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
                return (int)ErrorCodes.INTERNAL_ERROR;

            }

        }
        public  int SetParent(Category cat, string userId)
        {
            try
            {

                if (sqlHelper.Update(Tables.Categories,
                    new string[] { "FatherAutoKey" },
                    new string[] { cat.FatherAutoKey.ToString() },
                    "AutoKey = '" + cat.AutoKey + "'"))
                {

                    return (int)ErrorCodes.SUCCESS;
                }

                else
                {
                    return (int)ErrorCodes.INTERNAL_ERROR;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
                return (int)ErrorCodes.INTERNAL_ERROR;

            }

        }

        public  string GetCatName(long autokey)
        {
            return sqlHelper.SelectWithWhere(Tables.Categories,
                "Name",
                "AutoKey = '" + autokey + "'");
        }

        public  string GetFather(long autokey)
        {
            return sqlHelper.SelectWithWhere(Tables.Categories,
                "FatherAutoKey",
                "AutoKey = '" + autokey + "'");
        }
        public  int AddCategory(Category category, string userId)
        {
            try
            {/*
                string query = "select top 1 UserID ";
                query += " from " + Tables.CategoryUserRel + " as CUR";
                query += " where Userpiu[ID = '" + userId + "'";
                query += " and";
                query += " EXISTS(SELECT Name from " + Tables.Categories + " where AutoKey = CUR.CatAutoKey and Name = '" + category.Name + "')";

                if (sqlHelper.ExecuteReader<object>(query).Count > 0)
                    return (int)ErrorCodes.ALREADY_EXISTS;
                */
                Dictionary<string, string> wheres = new Dictionary<string, string>();
                wheres.Add("Name", category.Name);
                wheres.Add("FatherAutoKey", category.FatherAutoKey.ToString());

                if (sqlHelper.Exists(Tables.Categories, wheres))
                    return (int)ErrorCodes.ALREADY_EXISTS;

                string autoKey = sqlHelper.InsertWithID(Tables.Categories,
                    new string[] { "Name", "FatherAutoKey" },
                    new string[] { category.Name, category.FatherAutoKey.ToString() });
                if (!string.IsNullOrEmpty(autoKey))
                {
                    /*

                    if (sqlHelper.Insert(Tables.CategoryUserRel,
                        new string[] { "CatAutoKey", "UserID" },
                        new string[] { autoKey, userId }))
                    {

                        return (int)ErrorCodes.SUCCESS;
                    }

                    else
                    {
                        return (int)ErrorCodes.INTERNAL_ERROR;
                    }*/
                    return (int)ErrorCodes.SUCCESS;
                }
                else
                {
                    return (int)ErrorCodes.INTERNAL_ERROR;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
                return (int)ErrorCodes.INTERNAL_ERROR;

            }
        }


        public  int DeleteContact(long autoKey)
        {
            try
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
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
                return (int)ErrorCodes.INTERNAL_ERROR;

            }
        }


        public  int DeleteFile(long AutoKey)
        {
            try
            {
                var LineAutoKey = sqlHelper.SelectWithWhere(Tables.DocumentLines,
                "AutoKey",
                "InfoAutoKey = '" + AutoKey + "'");
                var InfoAutoKey = AutoKey;

                var Ext = sqlHelper.SelectWithWhere(Tables.DocumentLines,
                    "Ext",
                    "InfoAutoKey = '" + AutoKey + "'");

                string filename =  InfoAutoKey + "_" + LineAutoKey + Ext;


                sqlHelper.Delete(Tables.DocumentInfo, "InfoAutoKey = '" + AutoKey + "'");
                sqlHelper.Delete(Tables.DocumentLines, "InfoAutoKey = '" + AutoKey + "'");
                sqlHelper.Delete(Tables.DocumentCategoryRel, "DocumentAutoKey = '" + AutoKey + "'");
                sqlHelper.Delete(Tables.DocumentContactRel, "DocumentAutoKey = '" + AutoKey + "'");
                sqlHelper.Delete(Tables.DocumentSearchKeysRel, "DocumentAutoKey = '" + AutoKey + "'");
                sqlHelper.Delete(Tables.Images, "name = '" + filename + "'");

                return (int)ErrorCodes.SUCCESS;
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
                return (int)ErrorCodes.INTERNAL_ERROR;

            }
        }

        public  int DeleteKey(long autoKey)
        {
            try
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
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
                return (int)ErrorCodes.INTERNAL_ERROR;

            }
        }

        public  int SaveContact(Contact con, string userId)
        {
            try
            {
                Dictionary<string, string> wheres = new Dictionary<string, string>();

                wheres.Add("Name", con.Name);
                wheres.Add("DMSUserID", userId);

                if (sqlHelper.Exists(Tables.Contacts, wheres))
                    return (int)ErrorCodes.ALREADY_EXISTS;

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
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
                return (int)ErrorCodes.INTERNAL_ERROR;

            }
        }

        public  int SaveKey(SearchKey con, string userId)
        {
            try
            {
                Dictionary<string, string> wheres = new Dictionary<string, string>();

                wheres.Add("Name", con.Name);
                wheres.Add("DMSUserID", userId);

                if (sqlHelper.Exists(Tables.SearchKeys, wheres))
                    return (int)ErrorCodes.ALREADY_EXISTS;


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
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
                return (int)ErrorCodes.INTERNAL_ERROR;

            }

        }

        public  string GetConName(long autokey)
        {
            return sqlHelper.SelectWithWhere(Tables.Contacts,
                "Name",
                "AutoKey = '" + autokey + "'");
        }
        public  string GetKeyName(long autokey)
        {
            return sqlHelper.SelectWithWhere(Tables.SearchKeys,
                "Name",
                "AutoKey = '" + autokey + "'");
        }
        public  int AddContact(Contact contact, DateTime birthDate,  string userId)
        {
            try
            {
                Dictionary<string, string> wheres = new Dictionary<string, string>();

                wheres.Add("Name", contact.Name);
            //    wheres.Add("DMSUserID", userId);

                if (sqlHelper.Exists(Tables.Contacts, wheres))
                    return (int)ErrorCodes.ALREADY_EXISTS;

                var genId = GetAutoIcrementID(birthDate, "", "");

                string autoKey = sqlHelper.InsertWithID(Tables.Contacts,
                    new string[] { "ID", "Name", "DMSUserID" },
                    new string[] { genId, contact.Name, userId });
                if (!string.IsNullOrEmpty(autoKey))
                {
                    return (int)ErrorCodes.SUCCESS;
                }
                else
                {
                    return (int)ErrorCodes.INTERNAL_ERROR;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
                return (int)ErrorCodes.INTERNAL_ERROR;

            }
        }

        private  string GetAutoIcrementID(DateTime Birthdate, string MainCatID = "", string GenderID = "")
        {
            string NewNo = "";

            NewNo += Birthdate.Year.ToString().Substring(2, 2);
            NewNo += "-";

            string Query = "select max(ID) from " + Tables.Contacts + " where id like '" + NewNo + "%%'";

              
                string max = sqlHelper.ExecuteScalar<string>(Query);
                string[] mycount = max.Split('-');
                // MessageBox.Show(mycount[1].ToString());
                UInt64 count = 0;

                if (mycount.Length != 1 && mycount.Length != 0)
                {
                    if (!UInt64.TryParse(mycount[1], out count))
                        count = 0;
                }
                count++;
                string nn = count.ToString();
                nn = nn.PadLeft(5, '0');
                NewNo += nn + "-";
                if (!string.IsNullOrEmpty(GenderID))
                {
                NewNo += GenderID;
                }
                else
                {
                    NewNo += "2";
                }
            
            return NewNo;
        }

        public void FixExt()
        {
            var databases = sqlHelper.GetSqlConnection().Query<string>("SELECT DBName from " + Tables.UserDatabases); // sqlHelper.Select<string>(Tables.UserDatabases, "DBName");

            foreach (var db in databases)
            {
                string query = "USE [" + db + "]";

                query += " UPDATE " + Tables.DocumentLines + " SET [Ext] = REPLACE([Ext], '.', '')";

                sqlHelper.ExecuteNonQuery(query);

            }

        }
        public  int AddKey(string name, string userId)
        {
            try
            {
                Dictionary<string, string> wheres = new Dictionary<string, string>();

                wheres.Add("Name", name);
              //  wheres.Add("DMSUserID", userId);

                if (sqlHelper.Exists(Tables.SearchKeys, wheres))
                    return (int)ErrorCodes.ALREADY_EXISTS;

                string autoKey = sqlHelper.InsertWithID(Tables.SearchKeys,
                    new string[] { "Name", "DMSUserID" },
                    new string[] { name, userId });

                if (!string.IsNullOrEmpty(autoKey))
                {
                    return (int)ErrorCodes.SUCCESS;
                }
                else
                {
                    return (int)ErrorCodes.INTERNAL_ERROR;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
                return (int)ErrorCodes.INTERNAL_ERROR;

            }
        }
        public  List<Document> GetCatDocuments(long CatID)
        {
            try
            {
                string query = "select ROW_NUMBER() OVER(ORDER BY AutoKey ASC) AS ID, DocumentAutoKey,";
                query += " (select DateTimeAdded from " + Tables.DocumentInfo + " where InfoAutoKey = DCR.DocumentAutoKey) as DateTimeAdded,";
                query += " (select Name from " + Tables.DocumentLines + " where InfoAutoKey = DCR.DocumentAutoKey) as Name,";
                query += " (select AutoKey from " + Tables.DocumentLines + " where InfoAutoKey = DCR.DocumentAutoKey) as LineAutoKey,";
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
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
                return null;

            }
        }

        public  byte[] GetFile(long InfoAutoKey, long LineAutoKey, string Ext)
        {

            var fn = InfoAutoKey + "_" + LineAutoKey + "." +  Ext;

            var arr = sqlHelper.ExecuteScalar<byte[]>("select TOP 1 file_stream from " + DMS.Database.Tables.Images + " where name = '" + fn + "'");

            return arr;

        }
        public  string GetFileName(long AutoKey)
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
        public  Controllers.FileInfo GetFileInfo(long AutoKey)
        {
            try
            {
                Controllers.FileInfo info = new Controllers.FileInfo();

                string query = "select Name, FatherAutoKey, DCT.AutoKey";
                query += " from " + Tables.DocumentCategoryRel + " as DCR";
                query += " INNER JOIN " + Tables.Categories + " DCT on DCT.AutoKey = DCR.CatAutoKey";
                query += " where DocumentAutoKey = '" + AutoKey + "'";

                info.Categories = sqlHelper.ExecuteReader<Category>(query);

                query = "select Name, DCT.AutoKey";
                query += " from " + Tables.DocumentSearchKeysRel + " as DCR";
                query += " INNER JOIN " + Tables.SearchKeys + " DCT on DCT.AutoKey = DCR.SearchAutoKey";
                query += " where DocumentAutoKey = '" + AutoKey + "'";


                info.SearchKeys = sqlHelper.ExecuteReader<SearchKey>(query);

                query = "select Name, DCT.AutoKey";
                query += " from " + Tables.DocumentContactRel + " as DCR";
                query += " INNER JOIN " + Tables.Contacts + " DCT on DCT.AutoKey = DCR.ContactAutoKey";
                query += " where DocumentAutoKey = '" + AutoKey + "'";


                info.Contacts = sqlHelper.ExecuteReader<Contact>(query);
                return info;

            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
                return null;
            }   
        }

        public  int SaveFile(long AutoKey, List<long> categories, List<long> contacts, List<long> SearchKeys)
        {
            try
            {
                string query = "";

                query += " DELETE from " + Tables.DocumentCategoryRel + " where DocumentAutoKey = '" + AutoKey + "'";
                query += " DELETE from " + Tables.DocumentContactRel + " where DocumentAutoKey = '" + AutoKey + "'";
                query += " DELETE from " + Tables.DocumentSearchKeysRel + " where DocumentAutoKey = '" + AutoKey + "'";

                sqlHelper.ExecuteNonQuery(query);

                foreach (var category in categories)
                {
                    sqlHelper.Insert(Tables.DocumentCategoryRel,
                        new string[] { "DocumentAutoKey", "CatAutoKey" },
                        new string[] { AutoKey.ToString(), category.ToString() });
                }

                foreach (var contact in contacts)
                {
                    sqlHelper.Insert(Tables.DocumentContactRel,
                        new string[] { "DocumentAutoKey", "ContactAutoKey" },
                        new string[] { AutoKey.ToString(), contact.ToString() });
                }
                foreach (var key in SearchKeys)
                {
                    sqlHelper.Insert(Tables.DocumentSearchKeysRel,
                        new string[] { "DocumentAutoKey", "SearchAutoKey" },
                        new string[] { AutoKey.ToString(), key.ToString() });
                }

                return (int)ErrorCodes.SUCCESS;
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
                return (int)ErrorCodes.INTERNAL_ERROR;
            }
        }
        public  int AddFile(List<DMSCategory> categories, List<DMSContact> contacts, List<SearchKey> SearchKeys, IFormFile file, string userID, string webRoot)
        {
            try
            {
                            DateTime dt = DateTime.Now;
                //   string date = dt.ToString("MM-dd-yyyy-HH-mm-ss");
                // string fileName = Path.GetFileNameWithoutExtension(file.FileName) + "-" + date.Replace(" ", "") + Path.GetExtension(file.FileName);
                // var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(fileName);
                // fileName = System.Convert.ToBase64String(plainTextBytes) + Path.GetExtension(file.FileName);

                string infoAutoKey = sqlHelper.InsertWithID(Tables.DocumentInfo,
                    new string[] { "AddedByUserID", "DateTimeAdded", "DateTimeCreated", "IsDeleted" },
                    new string[] { userID, dt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture), dt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture), "0" });

                string lineAutoKey = sqlHelper.InsertWithID(Tables.DocumentLines,
                    new string[] { "InfoAutoKey", "Ext", "Name" },
                    new string[] { infoAutoKey, Path.GetExtension(file.FileName).Replace(".", ""), Path.GetFileNameWithoutExtension(file.FileName) });

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
                        new string[] { "DocumentAutoKey", "SearchAutoKey" },
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

                    int i = sqlHelper.ExecuteNonQuery(query, param);

                    if (i > 0)
                    {
                        var fileSize = file.Length / 1048576.0;

                        if (new DataManager(null).AddUsedStorage(Math.Round(fileSize, 2), userID))
                            return (int)ErrorCodes.SUCCESS;
                    }
                }

                return (int)ErrorCodes.INTERNAL_ERROR;
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
                return (int)ErrorCodes.INTERNAL_ERROR;

            }

        }


        public  bool AddUsedStorage(double amount, string userID)
        {

            var oldStorage = GetUserStorage(userID);

            return sqlHelper.Update(Tables.UserStorage,
                new string[] { "UsedStorage" },
                new string[] {  (oldStorage.UsedStorage + amount).ToString() },
                "UserID = '" + userID + "'");
        }

        public  int AddFile(IFormFile file, string userID, out string infoAutoKey)
        {
            infoAutoKey = "";
            try
            {
                DateTime dt = DateTime.Now;
                //   string date = dt.ToString("MM-dd-yyyy-HH-mm-ss");
                // string fileName = Path.GetFileNameWithoutExtension(file.FileName) + "-" + date.Replace(" ", "") + Path.GetExtension(file.FileName);
                // var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(fileName);
                // fileName = System.Convert.ToBase64String(plainTextBytes) + Path.GetExtension(file.FileName);

                infoAutoKey = sqlHelper.InsertWithID(Tables.DocumentInfo,
                    new string[] { "AddedByUserID", "DateTimeAdded", "DateTimeCreated", "IsDeleted" },
                    new string[] { userID, dt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture), dt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture), "0" });

                string lineAutoKey = sqlHelper.InsertWithID(Tables.DocumentLines,
                    new string[] { "InfoAutoKey", "Ext", "Name" },
                    new string[] { infoAutoKey, Path.GetExtension(file.FileName), Path.GetFileNameWithoutExtension(file.FileName) });


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

                    int i = sqlHelper.ExecuteNonQuery(query, param);

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
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
                return (int)ErrorCodes.INTERNAL_ERROR;

            }

        }
        private  string AddDateDirectories()
        {
            string year = DateTime.Now.Year.ToString();
            string month = DateTime.Now.Month.ToString();
            string day = DateTime.Now.Day.ToString();
            AddDirectory( year, "Images", "Images");
            AddDirectory( month, year, @"Images\" + year);
            AddDirectory( day, month, @"Images\" + year + @"\" + month);


            return GetParentPathLocator(day);
        }

        private  void AddDirectory(string name, string parent, string parents)
        {
            string query;
            query = @"IF NOT EXISTS ( SELECT 1 FROM Images WHERE path_locator = GetPathLocator(FileTableRootPath() + N'\Images\" + parents + @"\" + name + "'))";
            query += " INSERT INTO Images (name,is_directory,is_archive,path_locator) VALUES ('" + name + "', 1, 0, CAST('" + GetParentPathLocator( parent) + "' AS hierarchyid))";

            sqlHelper.ExecuteNonQuery(query);
        }

        private  string GetParentPathLocator(string parentName)
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

        public  bool SaveFile(IFormFile file, string rootPath, string name)
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
        public  IEnumerable<Category> GetCategories()
        {
                return sqlHelper.Select<Category>(Tables.Categories, new string[] { "*" });
            /*
            string query = "select ";
            query += " (select Name from " + Tables.Categories + " where AutoKey = CUR.CatAutoKey) as Name,";
            query += " (select AutoKey from " + Tables.Categories + " where AutoKey = CUR.CatAutoKey) as AutoKey,";
            query += "(select FatherAutoKey from " + Tables.Categories + " where AutoKey = CUR.CatAutoKey) as FatherAutoKey";
            query += " from " + Tables.CategoryUserRel + " as CUR";
            query += " where UserID = '" + userID + "'";

            
            return sqlHelper.ExecuteReader<Category>(query);*/
        }

        public IEnumerable<UserCategory> GetEnterpriseCategories(string userID)
        {
            string query = string.Format(@"select 
AutoKey, FatherAutoKey = 0, Name,
ISNULL(CanEdit, 0) as CanEdit,
ISNULL(CanView, 0) as CanView,
ISNULL(CanAdd, 0) as CanAdd,
ISNULL(CanDelete, 0) as CanDelete
 from {0}
 left join [{1}] ON [{1}].CatID = {0}.AutoKey
 where CanView = 1", Tables.Categories, userID);


            return sqlHelper.ExecuteReader<UserCategory>(query);
        }
        public bool IsEnterprise(string userID)
        {
            string result = sqlHelper.SelectWithWhere(Tables.Users, "EnterpriseCode", "ID = '" + userID + "' AND AccountType <> 'Enterprise'");

            return !string.IsNullOrEmpty(result);
        }

        public IEnumerable<UserCategory> GetUserCategories(string userID)
        {

            string query = string.Format(@"select 
AutoKey, FatherAutoKey, Name,
ISNULL(CanEdit, 0) as CanEdit,
ISNULL(CanAdd, 0) as CanAdd,
ISNULL(CanView, 0) as CanView,
ISNULL(CanDelete, 0) as CanDelete
 from {0}
 left join [{1}] ON [{1}].CatID = {0}.AutoKey", Tables.Categories, userID);


            return sqlHelper.ExecuteReader<UserCategory>(query);
        }

        public bool CanDelete(long AutoKey, string userID)
        {
            string query = string.Format(@"select
ISNULL(CanDelete, 0) as CanDelete
 from {0}
 left join [{1}] ON [{1}].CatID = {0}.AutoKey
 WHERE {0}.AutoKey = {2}", Tables.Categories, userID, AutoKey);

            return sqlHelper.ExecuteScalar<bool>(query);
        }

        public IEnumerable<UserCategory> GetRoleCategories(string roleID)
        {

            string query = string.Format(@"select 
AutoKey, FatherAutoKey, Name,
ISNULL(CanEdit, 0) as CanEdit,
ISNULL(CanView, 0) as CanView,
ISNULL(CanAdd, 0) as CanAdd,
ISNULL(CanDelete, 0) as CanDelete
 from {0}
 left join {1} ON {1}.CatID = {0}.AutoKey AND RoleID = {2}", Tables.Categories, Tables.RoleCategories, roleID);


            return sqlHelper.ExecuteReader<UserCategory>(query);
        }

        public class UpdateInfo
        {
            public string CanEdit { get; set; }
            public string CanView { get; set; }
            public string CanDelete { get; set; }
            public string CanAdd { get; set; }
        }
        public void UpdatePermission(string userId, long key, string values)
        {
            var d = JsonConvert.DeserializeObject<UpdateInfo>(values);

            if (!string.IsNullOrEmpty(d.CanDelete) && Convert.ToBoolean(d.CanDelete))
                d.CanEdit = "true";

            string query = "if(EXISTS(SELECT 1 from [" + userId + "] where CatID = '" + key + "'))";
            query += " BEGIN";

            query += " UPDATE [" + userId + "] SET ";

            if (!string.IsNullOrEmpty(d.CanView))
            {
                query += " CanView = '" + d.CanView + "',";
            }

            if (!string.IsNullOrEmpty(d.CanEdit))
            {
                query += "CanEdit = '" + d.CanEdit + "',";
            }


            if (!string.IsNullOrEmpty(d.CanAdd))
            {
                query += "CanAdd = '" + d.CanAdd + "',";
            }

            if (!string.IsNullOrEmpty(d.CanDelete))
            {
                query += "CanDelete = '" + d.CanDelete + "',";
            }

            query = query.Remove(query.Length - 1);

            query += " where CatID = '" + key + "'";
            query += " END";
            query += " ELSE";
            query += " BEGIN";
            query += " INSERT INTO [" + userId + "] (CatID, CanView, CanEdit, CanAdd, CanDelete) values ('" + key + "','" + Convert.ToBoolean(d.CanView) + "','" + Convert.ToBoolean(d.CanEdit) + "','" + Convert.ToBoolean(d.CanAdd) + "','" + Convert.ToBoolean(d.CanDelete) + "')";
            query += " END";


            sqlHelper.ExecuteNonQuery(query);
        }
        public void UpdateRolePermission(string roleId, long key, string values)
        {
            var d = JsonConvert.DeserializeObject<UpdateInfo>(values);

            string query = "if(EXISTS(SELECT 1 from " + Tables.RoleCategories + " where CatID = '" + key + "' AND RoleID = '" + roleId + "'))";
            query += " BEGIN";

            query += " UPDATE " + Tables.RoleCategories + " SET ";

            if (!string.IsNullOrEmpty(d.CanView))
            {
                query += " CanView = '" + d.CanView + "',";
            }

            if (!string.IsNullOrEmpty(d.CanEdit))
            {
                query += "CanEdit = '" + d.CanEdit + "',";
            }

            if (!string.IsNullOrEmpty(d.CanAdd))
            {
                query += "CanAdd = '" + d.CanAdd + "',";
            }

            if (!string.IsNullOrEmpty(d.CanDelete))
            {
                query += "CanDelete = '" + d.CanDelete + "',";
            }

            query = query.Remove(query.Length - 1);

            query += " where CatID = '" + key + "' AND RoleID = '" + roleId + "'";
            query += " END";
            query += " ELSE";
            query += " BEGIN";
            query += " INSERT INTO " + Tables.RoleCategories + " (CatID, CanView, CanEdit, CanDelete, CanAdd, RoleID) values ('" + key + "','" + Convert.ToBoolean(d.CanView) + "','" + Convert.ToBoolean(d.CanEdit) + "','" + Convert.ToBoolean(d.CanDelete) + "', '" + Convert.ToBoolean(d.CanAdd) + "','" + roleId + "')";
            query += " END";


            sqlHelper.ExecuteNonQuery(query);
        }
        public string Crypt(string text)
        {
            return Convert.ToBase64String(
                    Encoding.Unicode.GetBytes(text));
        }
        public string GetEnterpriseCode(string userID)
        {
            return new string(Crypt(userID).Take(6).ToArray());
        }
        public string GetAccountType(string userID)
        {
            return sqlHelper.ExecuteScalar<string>("SELECT AccountType from Users where ID = '" + userID + "'");
        }
        public IEnumerable<User> GetUsers(string userID)
        {

            Dictionary<string, string> wheres = new Dictionary<string, string>();
            wheres.Add("EnterpriseCode", GetEnterpriseCode(userID));

            return sqlHelper.SelectWithWhere<User>(Tables.Users, new string[] { "ID", "Name as Email", "AccountType" }, wheres);
        
        }
        public IEnumerable<Permission> GetUserPermissions(string roleId)
        {

            string query = "SELECT PermissionID as AutoKey, Name FROM " + Tables.UserPermissions + " inner join " 
                + Tables.Permissions + " ON Permissions.AutoKey = PermissionID WHERE UserID = '" + roleId + "'";

            return sqlHelper.ExecuteReader<Permission>(query);

        }

        public IEnumerable<Permission> GetRolePermissions(string userID)
        {

            string query = "SELECT PermissionID as AutoKey, Name FROM " + Tables.RolePermissions + " inner join "
                + Tables.Permissions + " ON Permissions.AutoKey = PermissionID WHERE RoleID = '" + userID + "'";

            return sqlHelper.ExecuteReader<Permission>(query);

        }

        public IEnumerable<Permission> GetPermissions()
        {

            return sqlHelper.Select<Permission>(Tables.Permissions, new string[] { "*" });

        }

        public IEnumerable<Role> GetRoles()
        {
            return sqlHelper.Select<Role>(Tables.Roles, new string[] { "AutoKey", "Name" });

        }

        public int AddRole(string Name)
        {
            try
            {
                Dictionary<string, string> wheres = new Dictionary<string, string>();
                wheres.Add("Name", Name);

                if (sqlHelper.Exists(Tables.Roles, wheres))
                {
                    return (int)ErrorCodes.ALREADY_EXISTS;
                }


                var id = sqlHelper.InsertWithID(Tables.Roles, new string[] { "Name" }, new string[] { Name });
                if (!string.IsNullOrEmpty(id))
                {
                    return (int)ErrorCodes.SUCCESS;
                }

            }catch(Exception ex)
            {
                Logger.Log(ex.Message);
            }

            return (int)ErrorCodes.INTERNAL_ERROR;

        }

        public int UpdateRole(int AutoKey, string Name)
        {
            try
            {
                Dictionary<string, string> wheres = new Dictionary<string, string>();
                wheres.Add("Name", Name);

                if (sqlHelper.Exists(Tables.Roles, wheres))
                {
                    return (int)ErrorCodes.ALREADY_EXISTS;
                }


                if (sqlHelper.Update(Tables.Roles, new string[] { "Name"}, new string[] { Name }, "AutoKey = '" + AutoKey + "'"))
                {
                    return (int)ErrorCodes.SUCCESS;
                }

            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
            }

            return (int)ErrorCodes.INTERNAL_ERROR;

        }

        public int DeleteRole(long AutoKey)
        {
            try
            {
                if (sqlHelper.Delete(Tables.Roles, "AutoKey = '" + AutoKey + "'"))
                {
                    if (sqlHelper.Delete(Tables.RoleCategories, "RoleID = '" + AutoKey + "'"))
                        return (int)ErrorCodes.SUCCESS;
                    
                }

            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
            }

            return (int)ErrorCodes.INTERNAL_ERROR;

        }
        public string GetUserAutoKey(string name)
        {
            return sqlHelper.SelectWithWhere(Tables.Users, "AutoKey", "Name = '" + name + "'");
        }
        public  IEnumerable<Contact> GetContacts(string userID)
        {
            //    Dictionary<string,string> wheres = new System.Collections.Generic.Dictionary<string, string>();
            //  wheres.Add("DMSUserID", userID);

            //return sqlHelper.SelectWithWhere<Contact>(Tables.Contacts, new string[] { "*" }, wheres);
            return sqlHelper.Select<Contact>(Tables.Contacts, "*");
        }
        public  IEnumerable<SearchKey> GetSearchKeys(string userID)
        {
            /*     Dictionary<string, string> wheres = new System.Collections.Generic.Dictionary<string, string>();
                 wheres.Add("DMSUserID", userID);

                 return sqlHelper.SelectWithWhere<SearchKey>(Tables.SearchKeys, new string[] { "*" }, wheres);*/
            return sqlHelper.Select<SearchKey>(Tables.SearchKeys, "*");

        }
        public  int Login(string username, string password)
        {
        //    try
        //    {
        /*
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

          //  }
        //    catch (Exception ex)
      //      {
    //            Logger.Log(ex.Message);
  //              return (int)ErrorCodes.INTERNAL_ERROR;

//            }
        }

        public  int GenerateTables()
        {
            try
            {
                String query = @"



IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'UserDB'))

BEGIN

CREATE TABLE [dbo].[UserStorage](
	[AutoKey] [bigint] IDENTITY(1,1) NOT NULL,
	[UserID] [nvarchar](max) NULL,
	[UsedStorage] [float] NULL,
	[Storage] [float] NULL,
 CONSTRAINT [PK_UserStorage] PRIMARY KEY CLUSTERED 
(
	[AutoKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

END

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'UserDB'))

BEGIN

CREATE TABLE [dbo].[UserDB](
	[AutoKey] [bigint] IDENTITY(1,1) NOT NULL,
	[UserID] [nvarchar](max) NULL,
	[DBName] [nvarchar](max) NULL,
 CONSTRAINT [PK_UserDatabases] PRIMARY KEY CLUSTERED 
(
	[AutoKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

END

IF (NOT EXISTS (SELECT * 
                 FROM INFORMATION_SCHEMA.TABLES 
                 WHERE TABLE_SCHEMA = 'dbo' 
                 AND  TABLE_NAME = 'CategoryUserRel'))
BEGIN

CREATE TABLE [dbo].[CategoryUserRel](
	[RelAutoKey] [bigint] IDENTITY(1,1) NOT NULL,
	[CatAutoKey] [bigint] NULL,
	[UserID] [nvarchar](max) NULL,
 CONSTRAINT [PK_CategoryUserRel] PRIMARY KEY CLUSTERED 
(
	[RelAutoKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

END


IF COL_LENGTH('dbo.Contacts', 'DMSUserID') IS NULL
BEGIN
  ALTER TABLE [Contacts]
    ADD [DMSUserID] NVARCHAR(MAX) NULL
END


IF COL_LENGTH('dbo.DocumentLine', 'Name') IS NULL
BEGIN
  ALTER TABLE [DocumentLine]
    ADD [Name] NVARCHAR(MAX) NULL
END

IF COL_LENGTH('dbo.SearchKeys', 'DMSUserID') IS NULL
BEGIN
  ALTER TABLE [SearchKeys]
    ADD [DMSUserID] NVARCHAR(MAX) NULL
END

";

                sqlHelper.ExecuteNonQuery(query);
                return (int)ErrorCodes.SUCCESS;
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
                return (int)ErrorCodes.INTERNAL_ERROR;

            }
        }
        
        public  UserStorage GetUserStorage(string UserID)
        {
            try
            {

                Dictionary<string, string> wheres = new Dictionary<string, string>();
                wheres.Add("UserID", UserID);

                return sqlHelper.SelectWithWhere<UserStorage>(Tables.UserStorage,
                    new string[] { "UsedStorage", "Storage" },
                    wheres)[0];
            }

            catch (Exception ex)
            {
                Logger.Log(ex.Message);
                return null;

            }
        }
        public int Verify(string id)
        {

            try
            {
                if (sqlHelper.Update(Tables.Users,
                    new[] { "Activated" },
                    new[] { "1" }, "ID = '" + id + "'"))
                {

                    // Manage enterprise
                    string query = Queries.CreateEnterpriseTable;

                    query = query.Replace("UID", id);

                    sqlHelper.ExecuteNonQuery(query);

                    var wheres = new Dictionary<string, string>();
                    wheres.Add("UserID", "*");

                    if (!sqlHelper.Exists(Tables.UserDatabases, wheres))
                    {

                        sqlHelper.Insert(Tables.UserDatabases,
                            new string[] { "UserID", "DBName" },
                            new string[] { id, id });

                        // Create DB
                        sqlHelper.CreateDatabase(id, settings.DatabasesPath);


                        // Configure DB
                        query = Queries.ConfigureDBQuery;

                        query = query.Replace("DBNAME", id);

                        sqlHelper.ExecuteNonQuery(query);


                        // Add Tables
                        query = Queries.AddTablesQuery;

                        query = query.Replace("DBNAME", id);


                        sqlHelper.ExecuteNonQuery(query);

                    }


                    return (int)ErrorCodes.SUCCESS;
                }

            }

            catch (Exception ex)
            {
                Logger.Log(ex.Message);
                return (int)ErrorCodes.INTERNAL_ERROR;

            }
            return (int)ErrorCodes.INTERNAL_ERROR;
        }

        public  int Register(string email, string password, string enterpriseCode)
        {
            try
            {
                
                Dictionary<string, string> wheres = new Dictionary<string, string>();


                wheres = new Dictionary<string, string>();

                wheres.Add("Name", email);

                if (sqlHelper.Exists(Tables.Users, wheres))
                {
                    return (int)ErrorCodes.USERNAME_EXISTS;
                }


                if (!string.IsNullOrEmpty(enterpriseCode))
                {
                    wheres = new Dictionary<string, string>();

                    wheres.Add("AccountType", "Enterprise");
                    wheres.Add("EnterpriseCode", enterpriseCode);

                    if (!sqlHelper.Exists(Tables.Users, wheres))
                    {
                        return (int)ErrorCodes.CODE_DOES_NOT_EXIST;
                    }

                }

                var id = GetUserAutoIcrementID();

                if (sqlHelper.Insert(Tables.Users,
                    new string[] {  "Name", "ID", "AccountType", "EnterpriseCode" },
                    new string[] {  email,  id, "Free", string.IsNullOrEmpty(enterpriseCode) ? "NULL" : enterpriseCode  }))
                {
                    if (sqlHelper.Insert(Tables.UserStorage,
                        new string[] { "UserID", "UsedStorage", "Storage" },
                        new string[] { id, "0", "10000" }))
                    {
                        client.SetPasswordAsync(email, password);

                        return Verify(id);

                        /*
                        string sql = Queries.NewDBQuery;//String.Join(Environment.NewLine, File.ReadAllLines("script.sql"));
                        
                        sql = sql.Replace("DBNAME", id );
                        sql = sql.Replace("DBPATH", settings.DatabasesPath);

                        Logger.Log(sql);
                        if(sqlHelper.ExecuteNonQuery(sql) > 0)
                            return (int)ErrorCodes.SUCCESS;
                        else
                            return (int)ErrorCodes.INTERNAL_ERROR;*/

                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
                return (int)ErrorCodes.INTERNAL_ERROR;

            }
            return (int)ErrorCodes.INTERNAL_ERROR;

        }


        private  string GetUserAutoIcrementID()
        {
            string NewNo = "";

            NewNo += DateTime.Now.Year.ToString().Substring(2, 2);
            NewNo += "-";

            string Query = "select max(ID) from " + Tables.Users + " where id like '" + NewNo + "%%'";


            string max = sqlHelper.ExecuteScalar<string>(Query);
            string[] mycount = max.Split('-');
            // MessageBox.Show(mycount[1].ToString());
            UInt64 count = 0;

            if (mycount.Length != 1 && mycount.Length != 0)
            {
                if (!UInt64.TryParse(mycount[1], out count))
                    count = 0;
            }
            count++;
            string nn = count.ToString();
            nn = nn.PadLeft(5, '0');
            NewNo += nn + "-";
            NewNo += "2";

            return NewNo;
        }
        public  SqlConnectionStringBuilder GetConnectionString(string userID)
        {
            return new SqlConnectionStringBuilder()
            {
                DataSource = settings.DataSource,
                UserID = "test",
                Password = "test_2008",
                MultipleActiveResultSets = true,
                InitialCatalog = string.IsNullOrEmpty(userID) ? settings.Database : sqlHelper.SelectWithWhere(Tables.UserDatabases, "DBName", "UserID = '*' OR UserID = '" + userID + "'")
            };
        }
    }

    public enum ErrorCodes
    {
        SUCCESS = 0,
        WRONG_CREDENTIALS = 101,
        USERNAME_EXISTS = 102,
        INTERNAL_ERROR = 103,
        EMAIL_EXISTS = 104,
        ALREADY_EXISTS = 105,
        CODE_DOES_NOT_EXIST = 106
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
