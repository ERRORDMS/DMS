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
using System.Threading;
using Microsoft.AspNetCore.Http.Internal;
using System.Net;
using Aspose.Imaging;
using Aspose.Imaging.ImageOptions;
using LZ4;

namespace DMS.Database
{
    public class DataManager : IDisposable
    {
        private  SQLHelper sqlHelper;
        private  ServiceReference1.AlSahlServiceClient client;
        public  Settings settings;
        private bool isUserIDProvided = false;
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
                    isUserIDProvided = true;
                    con = GetConnectionString(userID);

                    sqlHelper = new SQLHelper(con);
                }
            }   
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
            }
        }


        public SQLHelper SQLHelper { get { return sqlHelper; } }
        public void SetAllCatPermissions(string userID, bool value)
        {
            sqlHelper.Delete(Tables.UserCategories, "UserID = '" + userID + "'");
            sqlHelper.ExecuteNonQuery(@"INSERT INTO " +  Tables.UserCategories +" (CatID, UserID) "
                                      + " SELECT AutoKey, '" +  userID +  "' from DocumentCatTree");
            sqlHelper.Update("[" + Tables.UserCategories + "]",
                new string[] { "CanView", "CanEdit", "CanDelete", "CanAdd", "CanDownload"},
                new string[] { value.ToString(), value.ToString(), value.ToString(), value.ToString(), value.ToString() },
                "UserID = '" + userID + "'");
        }

        public void SetAllRoleCatPermissions(string roleId, bool value)
        {
            sqlHelper.Delete(Tables.RoleCategories, "RoleID = '" + roleId + "'");
            sqlHelper.ExecuteNonQuery(@"INSERT INTO " + Tables.RoleCategories + " (CatID, RoleID) "
                                      + " SELECT AutoKey, '" + roleId + "' from DocumentCatTree");
            sqlHelper.Update("[" + Tables.UserCategories + "]",
                new string[] { "CanView", "CanEdit", "CanDelete", "CanAdd", "CanDownload" },
                new string[] { value.ToString(), value.ToString(), value.ToString(), value.ToString(), value.ToString() },
                "RoleID = '" + roleId + "'");
        }


        public void ChangePasword(string userID, string password)
        {
            var email = sqlHelper.SelectWithWhere(Tables.Users, "Name", "ID = '" + userID + "'");
            client.SetPasswordAsync(email, password);
        }
        public AlSahlServiceClient GetClient() { return client; }

        public bool IsAdmin(string userID = null)
        {
            if (new DataManager(null).GetAccountType(userID) == "Enterprise") return true;
            else if (GetUserPermissions(userID).Any(S => S.Name == "Administrator")) return true;

            return false;

        }
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

        public Info GetInfo()
        {
            try
            {

                string query = string.Format(@"DECLARE @sql varchar(max);

SELECT @sql = Coalesce(@sql + ' UNION ALL ', '') + 'SELECT count(InfoAutoKey) as Files FROM ' + QuoteName(name) + '.dbo.DocumentLine'

FROM   sys.databases
where database_id > 4 AND name like '20%'
;

EXEC('SELECT SUM(Files) as Files, (SELECT COUNT(*) FROM [{0}].dbo.Users) as Users FROM (' + @sql +  ') dt')
", GetConnectionString(null).InitialCatalog);

                return sqlHelper.ExecuteReader<Info>(query)[0];
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
                return null;

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


                var id = sqlHelper.ExecuteScalar<long>("SELECT FatherAutoKey FROM DocumentCatTree WHERE AutoKey = '" + cat.AutoKey + "'");
                cat.FatherAutoKey = id;

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
        class TempDocument
        {
            public string Ext { get; set; }
            public long FileSize { get; set; }
        }

        public List<MonthlyUsageData> GetMonthlyUsages()
        {
            var reader = sqlHelper.ExecuteReader(@"SELECT DateName( month , DateAdd( month , MonthNumber , -1 ) ) as Month, TOTALCOUNT FROM (SELECT MONTH(DateTimeCreated) as MonthNumber, COUNT(InfoAutoKey) AS TOTALCOUNT 
FROM DocumentsInfo 
WHERE YEAR(DateTimeCreated) = YEAR(GetDate())

GROUP BY YEAR(DateTimeCreated), MONTH(DateTimeCreated)
) x
");

            List<MonthlyUsageData> data = new List<MonthlyUsageData>();
            while (reader.Read())
            {
                MonthlyUsageData d = new MonthlyUsageData();
                d.Month = Convert.ToString(reader["Month"]);
                d.Files = Convert.ToInt64(reader["TOTALCOUNT"]);
                data.Add(d);
            }

            return data;
        }

        public List<FileTypeSize> GetDocumentTypesSizes()
        {
            var imgExts = new string[]{

                "ase",
    "art",
    "bmp",
    "blp",
    "cd5",
    "cit",
    "cpt",
    "cr2",
    "cut",
    "dds",
    "dib",
    "djvu",
    "egt",
    "exif",
    "gif",
    "gpl",
    "grf",
    "icns",
    "ico",
    "iff",
    "jng",
    "jpeg",
    "jpg",
    "jfif",
    "jp2",
    "jps",
    "lbm",
    "max",
    "miff",
    "mng",
    "msp",
    "nitf",
    "ota",
    "pbm",
    "pc1",
    "pc2",
    "pc3",
    "pcf",
    "pcx",
    "pdn",
    "pgm",
    "PI1",
    "PI2",
    "PI3",
    "pict",
    "pct",
    "pnm",
    "pns",
    "ppm",
    "psb",
    "psd",
    "pdd",
    "psp",
    "px",
    "pxm",
    "pxr",
    "qfx",
    "raw",
    "rle",
    "sct",
    "sgi",
    "rgb",
    "int",
    "bw",
    "tga",
    "tiff",
    "tif",
    "vtf",
    "xbm",
    "xcf",
    "xpm",
    "3dv",
    "amf",
    "ai",
    "awg",
    "cgm",
    "cdr",
    "cmx",
    "dxf",
    "e2d",
    "egt",
    "eps",
    "fs",
    "gbr",
    "odg",
    "svg",
    "stl",
    "vrml",
    "x3d",
    "sxd",
    "v2d",
    "vnd",
    "wmf",
    "emf",
    "art",
    "xar",
    "png",
    "webp",
    "jxr",
    "hdp",
    "wdp",
    "cur",
    "ecw",
    "iff",
    "lbm",
    "liff",
    "nrrd",
    "pam",
    "pcx",
    "pgf",
    "sgi",
    "rgb",
    "rgba",
    "bw",
    "int",
    "inta",
    "sid",
    "ras",
    "sun",
    "tga"
};
            var docExts = new string[]
            {
                "doc",
                "dot",
                "wbk",
                "docm",
                "docx",
                "dotm",
                "dotx",
                "pdf",
                "rtf",
                "txt",
                "xml",
                "csv",
                "xls",
                "xlsb",
                "xlsm",
                "xlsx",
                "ppt"
            };

            var videoExts = new string[]
            {
                "webm",
                "mpg",
                "mp2",
                "mpeg",
                "mpe",
                "mpv",
                "ogg",
                "mp4",
                "m4p",
                "m4v",
                "avi",
                "wmv",
                "mov",
                "qt",
                "flv",
                "swf",
                "avchd"
            };
            var docs = sqlHelper.Select<TempDocument>(Tables.DocumentLines, "Ext", "FileSize");

            var imgDocs = docs.Where(S => imgExts.Contains(S.Ext.ToLower()));
            var docDocs = docs.Where(S => docExts.Contains(S.Ext.ToLower()));
            var vidDocs = docs.Where(S => videoExts.Contains(S.Ext.ToLower()));
            var otherDocs = docs.Where(S => !imgDocs.Contains(S) && !docDocs.Contains(S) && !vidDocs.Contains(S));

            List<FileTypeSize> fileTypeSizes = new List<FileTypeSize>();
            
            FileTypeSize img = new FileTypeSize();
            img.Files = imgDocs.Count();
            img.Name = "Images";
            img.Size = imgDocs.Sum(S => S.FileSize);

            FileTypeSize doc = new FileTypeSize();
            doc.Files = docDocs.Count();
            doc.Name = "Documents";
            doc.Size = docDocs.Sum(S => S.FileSize);

            FileTypeSize vid = new FileTypeSize();
            vid.Files = vidDocs.Count();
            vid.Name = "Videos";
            vid.Size = vidDocs.Sum(S => S.FileSize);

            FileTypeSize other = new FileTypeSize();
            other.Files = otherDocs.Count();
            other.Name = "Other";
            other.Size = otherDocs.Sum(S => S.FileSize);


            fileTypeSizes.Add(img);
            fileTypeSizes.Add(doc);
            fileTypeSizes.Add(vid); 
            fileTypeSizes.Add(other);


            return fileTypeSizes;
        }

        public int SaveUser(string userID, List<Permission> permissions, List<Role> roles)
        {
            try
            {
                sqlHelper.Delete(Tables.UserPermissions, "UserID = '" + userID + "'");
                sqlHelper.Delete(Tables.UserRoles, "UserID = '" + userID + "'");

                foreach (var permission in permissions)
                {
                    if(!sqlHelper.Insert(Tables.UserPermissions,
                        new string[] { "PermissionID", "UserID"},
                        new string[] { permission.AutoKey.ToString(), userID }))
                    {

                        return (int)ErrorCodes.INTERNAL_ERROR;
                    }
                }
                foreach(var role in roles)
                {
                    if (!sqlHelper.Insert(Tables.UserRoles,
                    new string[] { "UserID", "RoleID" },
                    new string[] { userID, role.AutoKey.ToString() }))
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
        public  int AddCategory(Category category, string userId, out string catID)
        {
            catID = "";

            try
            {
                /*
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
                    catID = autoKey;
                    if (IsEnterpriseSubUser(userId))
                    {
                        if(sqlHelper.Insert("[" + Tables.UserCategories + "]",
                            new string[] { "CatID", "Fathers", "CanView", "CanEdit", "CanAdd", "CanDelete", "CanDownload", "UserID" },
                            new string[] { autoKey, "", "true", "true", "true", "true", "true", userId }))
                        {
                            return (int)ErrorCodes.SUCCESS;

                        }
                    }
                    else
                    {
                        return (int)ErrorCodes.SUCCESS;

                    }
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
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
                return (int)ErrorCodes.INTERNAL_ERROR;

            }

            return (int)ErrorCodes.INTERNAL_ERROR;

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

        public int DeleteContactCategory(long autoKey)
        {
            try
            {
                if (sqlHelper.Delete(Tables.ContactCategories, "AutoKey = '" + autoKey + "'"))
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

        public List<DeletedFile> GetDeletedFiles()
        {
            try
            {
                string query = "select AddedByUserID as Email,";
                query += " (select top 1 Name + '.' + Ext from " + Tables.DocumentLines +" where InfoAutoKey = " + Tables.DocumentInfo + ".InfoAutoKey) as FileName,";
                query += " DateTimeDeleted as DeletionDate,";
                query += " InfoAutoKey as DocumentAutoKey,";
                query += " (select top 1 AutoKey from " + Tables.DocumentLines + " where InfoAutoKey = " + Tables.DocumentInfo + ".InfoAutoKey) as LineAutoKey";

                query += " from " + Tables.DocumentInfo + " where IsDeleted = 1";
                return sqlHelper.ExecuteReader<DeletedFile>(query);
                
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
                return null;

            }
        }

        public int UpdateFile(IFormFile file, long InfoAutoKey, long LineAutoKey, string userId)
        {
            try
            {
                var lastVersion = sqlHelper.ExecuteScalar<long>("SELECT ISNULL(MAX(Version), 1) as Version from " + Tables.DocumentVersions + " WHERE InfoAutoKey = '" + InfoAutoKey + "' and LineAutoKey = '" + LineAutoKey + "'");
                AddDateDirectories();

                string year = DateTime.Now.Year.ToString();
                string month = DateTime.Now.Month.ToString();
                string day = DateTime.Now.Day.ToString();

                string filename = InfoAutoKey + "_" + LineAutoKey + "v" + (lastVersion+ 1) + Path.GetExtension(file.FileName);

                string fileTablePath = sqlHelper.ExecuteScalar<string>("select FileTableRootPath()");

                string path = Path.Combine(settings.FtpUrl, fileTablePath.Split('\\').Last(), fileTablePath.Split('\\').Last(), $@"Images\{year}\{month}\{day}").Replace("\\", "//");

                // Get the object used to communicate with the server.
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(Path.Combine(path, filename));
                request.Method = WebRequestMethods.Ftp.UploadFile;

                // This example assumes the FTP site uses anonymous logon.
                request.Credentials = new NetworkCredential(settings.FtpUsername, settings.FtpPassword);

                request.ContentLength = file.Length;

                using (Stream requestStream = request.GetRequestStream())
                {
                    //requestStream.Write(file., 0, fileContents.Length);
                    file.CopyTo(requestStream);
                }

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    Console.WriteLine($"Upload File Complete, status {response.StatusDescription}");
                }

                var fileSize = file.Length / 1048576.0;

                new DataManager(null).AddUsedStorage(Math.Round(fileSize, 2), userId);

                sqlHelper.Insert(Tables.DocumentVersions,
                    new string[] { "Version", "InfoAutoKey", "LineAutoKey", "FileName" },
                    new string[] { (lastVersion+1).ToString(), InfoAutoKey.ToString(), LineAutoKey.ToString(), filename});

                return (int)ErrorCodes.INTERNAL_ERROR;
            }
            catch (Exception ex)
            {

                Logger.Log(ex.Message);
                return (int)ErrorCodes.INTERNAL_ERROR;
            }
        }
        public  int DeleteFile(long AutoKey, string userId)
        {
            try
            {/*
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
                
                return (int)ErrorCodes.SUCCESS;*/

                sqlHelper.Update(Tables.DocumentInfo,
                    new string[] { "IsDeleted", "UserDeleteit", "DateTimeDeleted", },
                    new string[] { "1", userId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)},
                    "InfoAutoKey = '" + AutoKey + "'");

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

        public  int SaveContact(Contact con)
        {
            try
            {
                Dictionary<string, string> wheres = new Dictionary<string, string>();

                wheres.Add("Name", con.Name);

                if (sqlHelper.Exists(Tables.Contacts, wheres))
                    return (int)ErrorCodes.ALREADY_EXISTS;


                 List<string> columns = new List<string>();
                List<string> values = new List<string>();
                if (!string.IsNullOrEmpty(con.Name))
                {
                    columns.Add("Name");
                    values.Add(con.Name);
                }

                if (!string.IsNullOrEmpty(con.Phone))
                {
                    columns.Add("MobilePhone");
                    values.Add(con.Phone);
                }

                if (!string.IsNullOrEmpty(con.Email))
                {
                    columns.Add("Email1");
                    values.Add(con.Email);
                }


                if (!string.IsNullOrEmpty(con.CategoryID))
                {
                    columns.Add("CategoriesID");
                    values.Add(con.CategoryID);
                }


                if (sqlHelper.Update(Tables.Contacts,
                    columns.ToArray(),
                    values.ToArray(),
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
        public int SaveContactCategory(ContactCategory con)
        {
            try
            {
                Dictionary<string, string> wheres = new Dictionary<string, string>();

                wheres.Add("Name", con.Name);

                if (sqlHelper.Exists(Tables.ContactCategories, wheres))
                    return (int)ErrorCodes.ALREADY_EXISTS;


                List<string> columns = new List<string>();
                List<string> values = new List<string>();
                if (!string.IsNullOrEmpty(con.Name))
                {
                    columns.Add("Name");
                    values.Add(con.Name);
                }

                if (sqlHelper.Update(Tables.ContactCategories,
                    columns.ToArray(),
                    values.ToArray(),
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

        public int SaveKey(SearchKey con, string userId)
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
        public  int AddContact(Contact contact,  string userId)
        {
            try
            {
                Dictionary<string, string> wheres = new Dictionary<string, string>();

                wheres.Add("Name", contact.Name);
            //    wheres.Add("DMSUserID", userId);

                if (sqlHelper.Exists(Tables.Contacts, wheres))
                    return (int)ErrorCodes.ALREADY_EXISTS;

                var genId = GetAutoIcrementID(DateTime.Now, "", "");

                string autoKey = sqlHelper.InsertWithID(Tables.Contacts,
                    new string[] { "ID", "Name", "MobilePhone", "Email1", "CategoriesID", "DMSUserID" },
                    new string[] { genId, contact.Name, contact.Phone, contact.Email, contact.CategoryID, userId });
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

        public int AddContactCategory(ContactCategory cat)
        {
            try
            {
                Dictionary<string, string> wheres = new Dictionary<string, string>();

                wheres.Add("Name", cat.Name);
                //    wheres.Add("DMSUserID", userId);

                if (sqlHelper.Exists(Tables.ContactCategories, wheres))
                    return (int)ErrorCodes.ALREADY_EXISTS;

                string autoKey = sqlHelper.InsertWithID(Tables.ContactCategories,
                    new string[] { "Name"},
                    new string[] { cat.Name });
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
            var databases = sqlHelper.GetSqlConnection().Query<string>("SELECT distinct DBName from " + Tables.UserDatabases); // sqlHelper.Select<string>(Tables.UserDatabases, "DBName");

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
        public List<Document> GetCatDocuments(long CatID)
        {
            try
            {
                string query = "select ROW_NUMBER() OVER(ORDER BY  DateTimeAdded desc) AS ID, DocumentAutoKey, AddedByUserID as UserID,";
                query += " DateTimeAdded,  Note,";
                query += " (select Name from " + Tables.DocumentLines + " where InfoAutoKey = DCR.DocumentAutoKey) as Name,";
                query += " (select FileSize from " + Tables.DocumentLines + " where InfoAutoKey = DCR.DocumentAutoKey) as Size,";
                query += " (select AutoKey from " + Tables.DocumentLines + " where InfoAutoKey = DCR.DocumentAutoKey) as LineAutoKey,";
                query += " (select Ext from " + Tables.DocumentLines + " where InfoAutoKey = DCR.DocumentAutoKey) as Ext";
                query += " from " + Tables.DocumentCategoryRel + " as DCR";
                query += " inner join " + Tables.DocumentInfo + " on " + Tables.DocumentInfo + ".InfoAutoKey = DCR.DocumentAutoKey";
                query += " where CatAutoKey = " + CatID + " and (IsDeleted = 0 OR IsDeleted is null) order by DateTimeAdded desc";

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
        
        public  Aspose.Imaging.ImageOptions.VectorRasterizationOptions[] CreatePageOptions<TOptions>(VectorMultipageImage image) where TOptions : VectorRasterizationOptions
        {
            // Create page rasterization options for each page in the image
            return image.Pages.Select(x => x.Size).Select(CreatePageOptions<TOptions>).ToArray();
        }

        public Aspose.Imaging.ImageOptions.VectorRasterizationOptions CreatePageOptions<TOptions>(Size pageSize) where TOptions : VectorRasterizationOptions
        {
            // Create the instance of rasterization options
            var options = Activator.CreateInstance<TOptions>();

            // Set the page size
            options.PageSize = pageSize;
            return options;
        }


        public byte[] GetFile(long InfoAutoKey, long LineAutoKey, string Ext)
        {

            
            //  long lastVersion = 0;
            //lastVersion = sqlHelper.ExecuteScalar<long>("SELECT MAX(Version) as Version from " + Tables.DocumentVersions + " WHERE InfoAutoKey = '" + InfoAutoKey + "' and LineAutoKey = '" + LineAutoKey + "'");
            string fn = sqlHelper.ExecuteScalar<string>("SELECT top 1 FileName from " + Tables.DocumentVersions + " WHERE InfoAutoKey = '" + InfoAutoKey + "' and LineAutoKey = '" + LineAutoKey + "' order by Version desc");
            if(string.IsNullOrEmpty(fn))
                 fn = InfoAutoKey + "_" + LineAutoKey + "." + Ext;

            var arr = sqlHelper.ExecuteScalar<byte[]>("select TOP 1 file_stream from " + DMS.Database.Tables.Images + " where name = '" + fn + "'");

            try
            {
                
                arr = LZ4Codec.Unwrap(arr);
            }
            catch (Exception) { }

            return arr;

        }

        public string GetExt(long InfoAutoKey)
        {
            return sqlHelper.SelectWithWhere(Tables.DocumentLines, "Ext", "InfoAutoKey = '" + InfoAutoKey + "'");
        }
        public  string GetFileName(long AutoKey)
        {
            string fn = sqlHelper.ExecuteScalar<string>("SELECT top 1 FileName from " + Tables.DocumentVersions + " WHERE InfoAutoKey = '" + AutoKey + "' and LineAutoKey = '" + AutoKey + "' order by Version desc");
            if (string.IsNullOrEmpty(fn))
                fn = AutoKey + "_" + AutoKey + "." + GetExt(AutoKey);
            return fn;
        }

        public string GetActualFileName(long AutoKey, string Ext)
        {
            string fn = sqlHelper.ExecuteScalar<string>("SELECT top 1 Name from " + Tables.DocumentLines + " WHERE InfoAutoKey = '" + AutoKey + "'");

            return fn + "." + Ext;
        }
        public  Controllers.FileInfo GetFileInfo(long AutoKey)
        {
            try
            {
                Controllers.FileInfo info = new Controllers.FileInfo();

                info.Categories = GetDocumentCategories(AutoKey);
                info.SearchKeys = GetDocumentSearchKeys(AutoKey);
                info.Contacts = GetDocumentContacts(AutoKey);
                return info;

            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
                return null;
            }   
        }

        public List<Category> GetDocumentCategories(long AutoKey)
        {
            string query = "select Name, FatherAutoKey, DCT.AutoKey";
            query += " from " + Tables.DocumentCategoryRel + " as DCR";
            query += " INNER JOIN " + Tables.Categories + " DCT on DCT.AutoKey = DCR.CatAutoKey";
            query += " where DocumentAutoKey = '" + AutoKey + "'";

            return sqlHelper.ExecuteReader<Category>(query);
        }
        public List<SearchKey> GetDocumentSearchKeys(long AutoKey)
        {
            var query = "select Name, DCT.AutoKey";
            query += " from " + Tables.DocumentSearchKeysRel + " as DCR";
            query += " INNER JOIN " + Tables.SearchKeys + " DCT on DCT.AutoKey = DCR.SearchAutoKey";
            query += " where DocumentAutoKey = '" + AutoKey + "'";


             return sqlHelper.ExecuteReader<SearchKey>(query);

        }
        public List<Contact> GetDocumentContacts(long AutoKey)
        {
            var query = "select Name, MobilePhone as Phone, Email1 as Email, CategoriesID as CategoryID, DCT.AutoKey";
            query += " from " + Tables.DocumentContactRel + " as DCR";
            query += " INNER JOIN " + Tables.Contacts + " DCT on DCT.AutoKey = DCR.ContactAutoKey";
            query += " where DocumentAutoKey = '" + AutoKey + "'";


            return sqlHelper.ExecuteReader<Contact>(query);
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
        public async Task<int> AddFileAsync(List<long> categories, List<DMSContact> contacts, List<SearchKey> SearchKeys, IFormFile file, string userID, string note)
        {
            string infoAutoKey = "";
            try
            {
                DateTime dt = DateTime.Now;
                //   string date = dt.ToString("MM-dd-yyyy-HH-mm-ss");
                // string fileName = Path.GetFileNameWithoutExtension(file.FileName) + "-" + date.Replace(" ", "") + Path.GetExtension(file.FileName);
                // var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(fileName);
                // fileName = System.Convert.ToBase64String(plainTextBytes) + Path.GetExtension(file.FileName);

                 infoAutoKey = sqlHelper.InsertWithID(Tables.DocumentInfo,
                    new string[] { "AddedByUserID", "DateTimeAdded", "DateTimeCreated", "IsDeleted", "Note" },
                    new string[] { userID, dt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture), dt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture), "0", note });

                string lineAutoKey = sqlHelper.InsertWithID(Tables.DocumentLines,
                    new string[] { "InfoAutoKey", "Ext", "Name" },
                    new string[] { infoAutoKey, Path.GetExtension(file.FileName).Replace(".", ""), Path.GetFileNameWithoutExtension(file.FileName) });

                foreach (var category in categories)
                {
                    sqlHelper.Insert(Tables.DocumentCategoryRel,
                        new string[] { "DocumentAutoKey", "CatAutoKey" },
                        new string[] { infoAutoKey, category.ToString() });
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

                    //AddDateDirectories();
                    string fileTablePath = sqlHelper.ExecuteScalar<string>("select FileTableRootPath()");

                    string year = DateTime.Now.Year.ToString();
                    string month = DateTime.Now.Month.ToString();
                    string day = DateTime.Now.Day.ToString();

                    CreateFTPDirectory(year, fileTablePath);
                    CreateFTPDirectory(year + @"\" + month, fileTablePath);
                    CreateFTPDirectory(year + @"\" + month + @"\" + day, fileTablePath);

                    string filename = infoAutoKey + "_" + lineAutoKey + Path.GetExtension(file.FileName);


                    string path = Path.Combine(settings.FtpUrl, fileTablePath.Split('\\').Last(), fileTablePath.Split('\\').Last(), $@"Images\{year}\{month}\{day}").Replace("\\", "//");

                    //byte[] fileBytes = SevenZip.Compression.LZMA.SevenZipHelper.Compress(file.GetBytes().Result);

                    // Get the object used to communicate with the server.
                    var request = WebRequest.Create(Path.Combine(path, filename));
                    request.Method = WebRequestMethods.Ftp.UploadFile;

                    // This example assumes the FTP site uses anonymous logon.
                    request.Credentials = new NetworkCredential(settings.FtpUsername, settings.FtpPassword);

                    request.ContentLength = file.Length;

                    byte[] fileBytes = LZ4Codec.Wrap(await file.GetBytes());

                    using (Stream requestStream = request.GetRequestStream())
                    {
                        await requestStream.WriteAsync(fileBytes, 0, fileBytes.Length);
                        //compressedStream.CopyTo(requestStream);
                    }

                    var fileSize = file.Length / 1048576.0;
                    sqlHelper.Update(Tables.DocumentLines, new string[] { "FileSize" }, new string[] { file.Length.ToString() }, "AutoKey = '" + lineAutoKey + "'");
                    new DataManager(null).AddUsedStorage(Math.Round(fileSize, 2), userID);

                    if (sqlHelper.Insert(Tables.DocumentVersions,
                        new string[] { "Version", "InfoAutoKey", "LineAutoKey", "FileName" },
                        new string[] { "1", infoAutoKey.ToString(), lineAutoKey.ToString(), filename }))
                        return (int)ErrorCodes.SUCCESS;

                }

            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);

                sqlHelper.Delete(Tables.DocumentInfo, "InfoAutoKey = '" + infoAutoKey + "'");
                sqlHelper.Delete(Tables.DocumentLines, "InfoAutoKey = '" + infoAutoKey + "'");
                sqlHelper.Delete(Tables.DocumentCategoryRel, "DocumentAutoKey = '" + infoAutoKey + "'");
                sqlHelper.Delete(Tables.DocumentContactRel, "DocumentAutoKey = '" + infoAutoKey + "'");
                sqlHelper.Delete(Tables.DocumentSearchKeysRel, "DocumentAutoKey = '" + infoAutoKey + "'");
                

                return (int)ErrorCodes.INTERNAL_ERROR;

            }

            return (int)ErrorCodes.INTERNAL_ERROR;


        }


        private bool CreateFTPDirectory(string directory, string fileTablePath)
        {

            try
            {
                //create the directory
                var path = Path.Combine(settings.FtpUrl, fileTablePath.Split('\\').Last(), fileTablePath.Split('\\').Last(), "Images", directory).Replace("\\", "//");

                FtpWebRequest requestDir = (FtpWebRequest)FtpWebRequest.Create(path);
                requestDir.Method = WebRequestMethods.Ftp.MakeDirectory;
                requestDir.Credentials = new NetworkCredential(settings.FtpUsername, settings.FtpPassword);
                requestDir.UsePassive = true;
                requestDir.UseBinary = true;
                requestDir.KeepAlive = false;
                FtpWebResponse response = (FtpWebResponse)requestDir.GetResponse();
                Stream ftpStream = response.GetResponseStream();

                ftpStream.Close();
                response.Close();

                return true;
            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    response.Close();
                    return true;
                }
                else
                {
                    response.Close();
                    return false;
                }
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
            string fileTablePath = sqlHelper.ExecuteScalar<string>("select FileTableRootPath()");

            string query;
            
            query = @"IF NOT EXISTS ( SELECT 1 FROM Images WHERE path_locator = GetPathLocator(FileTableRootPath() + N'\" + fileTablePath.Split('\\').Last() + @"\" + parents + @"\" + name + "'))";
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
            string query = string.Format(@"
select 

AutoKey, FatherAutoKey, Name,
    CAST(MAX(CAST(CanEdit as int)) as bit) as CanEdit,
CAST(MAX(CAST(CanAdd as int)) as bit) as CanAdd,
CAST(MAX(CAST(CanView as int)) as bit) as CanView,
CAST(MAX(CAST(CanDownload as int)) as bit) as CanDownload,
CAST(MAX(CAST(CanDelete as int)) as bit) as CanDelete
 
 from 
(
select 
{0}.AutoKey, FatherAutoKey, Name,
ISNULL(CanEdit, 0) as CanEdit,
ISNULL(CanAdd, 0) as CanAdd,
ISNULL(CanView, 0) as CanView,
ISNULL(CanDownload, 0) as CanDownload,
ISNULL(CanDelete, 0) as CanDelete
 from {0}
 left join {1} ON {1}.CatID = {0}.AutoKey AND {1}.RoleID = RoleID
 left join {2} ON {2}.UserID = '{3}'
 where CanView = 1
 AND UserID = '{4}'
 UNION ALL

 select 
{0}.AutoKey, FatherAutoKey, Name,
ISNULL(CanEdit, 0) as CanEdit,
ISNULL(CanAdd, 0) as CanAdd,
ISNULL(CanView, 0) as CanView,
ISNULL(CanDownload, 0) as CanDownload,
ISNULL(CanDelete, 0) as CanDelete
 from {0}
 left join [{3}] ON [{3}].CatID = {0}.AutoKey
 where CanView = 1
 AND UserID = '{4}'

) as t

group by AutoKey, FatherAutoKey, Name", Tables.Categories, Tables.RoleCategories, Tables.UserRoles, Tables.UserCategories, userID);

            var userCategories = sqlHelper.ExecuteReader<UserCategory>(query);

            var categories = GetUserCategories(userID);

            var autoKeys = userCategories.Select(S => S.FatherAutoKey);

            userCategories.AddRange(categories.Where(S => autoKeys.Contains(S.AutoKey)));
            
            return userCategories.GroupBy(x => x.AutoKey).Select(x => x.First());

        }
        public bool IsEnterpriseSubUser(string userID)
        {
            if (isUserIDProvided)
            {
                return new DataManager(null).IsEnterpriseSubUser(userID);

            }

            string result = sqlHelper.SelectWithWhere(Tables.Users, "AccountType", "ID = '" + userID + "'");

            return result == "EnterpriseSubUser" && !new DataManager(userID).IsAdmin(userID);
        }

        public IEnumerable<UserCategory> GetUserCategories(string userID)
        {

            string query = string.Format(@"select 
{0}.AutoKey, FatherAutoKey, Name,
ISNULL(CanEdit, 0) as CanEdit,
ISNULL(CanAdd, 0) as CanAdd,
ISNULL(CanView, 0) as CanView,
ISNULL(CanDownload, 0) as CanDownload,
ISNULL(CanDelete, 0) as CanDelete
 from {0}
 left join [{1}] ON [{1}].CatID = {0}.AutoKey
 AND UserID = '{2}'", Tables.Categories, Tables.UserCategories, userID);


            return sqlHelper.ExecuteReader<UserCategory>(query);
        }

        public bool CanDelete(long AutoKey, string userID)
        {
            string query = string.Format(@"select
ISNULL(CanDelete, 0) as CanDelete
 from {0}
 left join [{1}] ON [{1}].CatID = {0}.AutoKey
 AND UserID = '{2}'
 WHERE {0}.AutoKey = {3}", Tables.Categories, Tables.UserCategories, userID, AutoKey);

            return sqlHelper.ExecuteScalar<bool>(query);
        }

        public bool CanAdd(long AutoKey, string userID)
        {
            string query = string.Format(@"select
ISNULL(CanAdd, 0) as CanAdd
 from {0}
 left join [{1}] ON [{1}].CatID = {0}.AutoKey
 AND UserID = '{2}'
 WHERE {0}.AutoKey = {3}", Tables.Categories, Tables.UserCategories, userID, AutoKey);

            return sqlHelper.ExecuteScalar<bool>(query);
        }

        public bool CanDownload(long AutoKey, string userID)
        {
            string query = string.Format(@"select
ISNULL(CanDownload, 0) as CanDownload
 from {0}
 left join [{1}] ON [{1}].CatID = {0}.AutoKey
 AND UserID = '{2}'
 WHERE {0}.AutoKey = {3}", Tables.Categories, Tables.UserCategories, userID, AutoKey);

            return sqlHelper.ExecuteScalar<bool>(query);
        }


        public bool AllowDeletingCategory(long AutoKey)
        {
            string query = "SELECT COUNT(AutoKey) FROM " + Tables.Categories + " where FatherAutoKey = '" + AutoKey + "'";
            int i = sqlHelper.ExecuteScalar<int>(query);
            if(i > 1)
            {
                return false;
            }

            query = "SELECT COUNT(IsDeleted) from " + Tables.DocumentCategoryRel;
            query += " INNER JOIN " + Tables.DocumentInfo + " ON " + Tables.DocumentInfo + ".InfoAutoKey = " + Tables.DocumentCategoryRel + " .DocumentAutoKey";
            query += " where IsDeleted <> 1  AND CatAutoKey = '" + AutoKey + "'";

            i = sqlHelper.ExecuteScalar<int>(query);
            if (i > 1)
            {
                return false;
            }

            return true;
        }

        public IEnumerable<UserCategory> GetRoleCategories(string roleID)
        {

            string query = string.Format(@"select 
{0}.AutoKey, FatherAutoKey, Name,
ISNULL(CanEdit, 0) as CanEdit,
ISNULL(CanView, 0) as CanView,
ISNULL(CanAdd, 0) as CanAdd,
ISNULL(CanDownload, 0) as CanDownload,
ISNULL(CanDelete, 0) as CanDelete
 from {0}
 left join {1} ON {1}.CatID = {0}.AutoKey AND RoleID = {2}", Tables.Categories, Tables.RoleCategories, roleID);


            return sqlHelper.ExecuteReader<UserCategory>(query);
        }

        public class UpdateInfo
        {
             public long CatID { get; set; }
            public string CanEdit { get; set; }
            public string CanView { get; set; }
            public string CanDelete { get; set; }
            public string CanAdd { get; set; }
            public string CanDownload { get; set; }

        }

        public void UpdatePermission(string userId, UpdateInfo d)
        {

            if (!string.IsNullOrEmpty(d.CanDelete) && Convert.ToBoolean(d.CanDelete))
                d.CanEdit = "true";

            string query = "if(EXISTS(SELECT 1 from [" + Tables.UserCategories + "] where CatID = '" + d.CatID + "' AND UserID = '" + userId +  "'))";
            query += " BEGIN";

            query += " UPDATE [" + Tables.UserCategories + "] SET ";

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
            if (!string.IsNullOrEmpty(d.CanDownload))
            {
                query += "CanDownload = '" + d.CanDownload + "',";
            }



            query = query.Remove(query.Length - 1);

            query += " where CatID = '" + d.CatID + "' AND UserID = '" + userId + "'";
            query += " END";
            query += " ELSE";
            query += " BEGIN";
            query += " INSERT INTO [" + Tables.UserCategories + "] (CatID, CanView, CanEdit, CanAdd, CanDelete, CanDownload, UserID) values ('" + d.CatID + "','" + Convert.ToBoolean(d.CanView) + "','" + Convert.ToBoolean(d.CanEdit) + "','" + Convert.ToBoolean(d.CanAdd) + "','" + Convert.ToBoolean(d.CanDelete) + "','" + Convert.ToBoolean(d.CanDownload) + "', '" + userId + "')";
            query += " END";


            sqlHelper.ExecuteNonQuery(query);
        }
        public void UpdateRolePermission(string roleId, UpdateInfo d)
        {

            string query = "if(EXISTS(SELECT 1 from " + Tables.RoleCategories + " where CatID = '" + d.CatID + "' AND RoleID = '" + roleId + "'))";
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
            if (!string.IsNullOrEmpty(d.CanDownload))
            {
                query += "CanDownload = '" + d.CanDownload + "',";
            }

            query = query.Remove(query.Length - 1);

            query += " where CatID = '" + d.CatID + "' AND RoleID = '" + roleId + "'";
            query += " END";
            query += " ELSE";
            query += " BEGIN";
            query += " INSERT INTO " + Tables.RoleCategories + " (CatID, CanView, CanEdit, CanDelete, CanAdd, CanDownload, RoleID) values ('" + d.CatID + "','" + Convert.ToBoolean(d.CanView) + "','" + Convert.ToBoolean(d.CanEdit) + "','" + Convert.ToBoolean(d.CanDelete) + "', '" + Convert.ToBoolean(d.CanAdd) + "','" + Convert.ToBoolean(d.CanDownload) + "','" + roleId + "')";
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
            string res;

            if(IsEnterpriseSubUser(userID))
                res = sqlHelper.SelectWithWhere(Tables.Users, "EnterpriseCode", "ID = '" + userID + "'");// Crypt(userID.Split('-')[1]);
            else
                res = sqlHelper.SelectWithWhere(Tables.EnterpriseCodes, "Code", "UserID = '" + userID + "'");// Crypt(userID.Split('-')[1]);
            return res;//new string(Crypt(userID).Take(6).ToArray());
        }
        public string GetAccountType(string userID)
        {
            return sqlHelper.ExecuteScalar<string>("SELECT AccountType from Users where ID = '" + userID + "'");
        }
        public string GetPhoneNumber(string userID)
        {
            return sqlHelper.ExecuteScalar<string>("SELECT Phone from Users where ID = '" + userID + "'");
        }
        public bool Get2FAEnabled(string userID)
        {
            return sqlHelper.ExecuteScalar<bool>("SELECT [2FA] from Users where ID = '" + userID + "'");
        }

        public bool IsIPTrusted(string userID, string IP)
        {
            Dictionary<string, string> wheres = new Dictionary<string, string>();
            wheres.Add("UserID", userID);
            wheres.Add("IP", IP);
            return sqlHelper.Exists(Tables.TrustedIPs, wheres);
        }

        public void TrustIP(string userID, string IP) {
            sqlHelper.Insert(Tables.TrustedIPs,
            new string[] { "UserID", "IP" },
            new string[] { userID, IP });
        }
        public IEnumerable<User> GetUsers(string userID, bool includeSelf = false)
        {/*

            Dictionary<string, string> wheres = new Dictionary<string, string>();
            wheres.Add("EnterpriseCode", GetEnterpriseCode(userID));

            return sqlHelper.SelectWithWhere<User>(Tables.Users, new string[] { "ID", "Name as Email", "AccountType" }, wheres);
        */
            List<User> users = new List<User>();

            try { 
                var enterpriseCode = GetEnterpriseCode(userID);
                users = sqlHelper.ExecuteReader<User>("SELECT ID, Name as Email, AccountType, Phone from Users where EnterpriseCode <> '' AND EnterpriseCode = '" + enterpriseCode + "'");

                if (includeSelf)
                {
                    var newUserID = sqlHelper.SelectWithWhere(Tables.EnterpriseCodes, "UserID", "Code = '" + enterpriseCode + "'");

                    Dictionary<string, string> wheres = new Dictionary<string, string>();
                    wheres.Add("ID", newUserID);

                    users.Add(sqlHelper.SelectWithWhere<User>(Tables.Users,
                        new string[] { "ID", "Name as Email", "AccountType", "Phone" }
                        , wheres).First());
                }
            }
            catch(Exception)
            {

                Dictionary<string, string> wheres = new Dictionary<string, string>();
                wheres.Add("ID", userID);

                users.Add(sqlHelper.SelectWithWhere<User>(Tables.Users,
                    new string[] { "ID", "Name as Email", "AccountType", "Phone" }
                    , wheres).First());
            }
            return users;
        }
        public IEnumerable<Permission> GetUserPermissions(string userId)
        {
            /*
            string query = "SELECT PermissionID as AutoKey, Name FROM " + Tables.UserPermissions + " inner join " 
                + Tables.Permissions + " ON Permissions.AutoKey = PermissionID WHERE UserID = '" + userId + "'";
            */
            string query = string.Format(@"
 select AutoKey, Name
 from
 (
 SELECT PermissionID as AutoKey, Name 
 FROM {0}
  inner join {1} 
  ON {1}.AutoKey = PermissionID 
  WHERE UserID = '{2}'

  UNION ALL

   SELECT PermissionID as AutoKey, Name 
 FROM {3}
  inner join {1} 
  ON {1}.AutoKey = PermissionID 
  left join {4} ON {4}.UserID = '{2}'

  WHERE {3}.RoleID = {4}.RoleID

) as t
group by AutoKey, Name", Tables.UserPermissions, Tables.Permissions, userId, Tables.RolePermissions, Tables.UserRoles);
            return sqlHelper.ExecuteReader<Permission>(query);

        }

        public IEnumerable<Permission> GetRolePermissions(string roleID)
        {

            string query = "SELECT PermissionID as AutoKey, Name FROM " + Tables.RolePermissions + " inner join "
                + Tables.Permissions + " ON Permissions.AutoKey = PermissionID WHERE RoleID = '" + roleID + "'";

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
        public IEnumerable<Role> GetUserRoles(string userId)
        {
            string query = string.Format(@"select {0}.AutoKey, {0}.Name 
from {1}
left join {0} on {0}.AutoKey = {1}.RoleID
where UserID = '{2}'", Tables.Roles, Tables.UserRoles, userId);


            return sqlHelper.ExecuteReader<Role>(query);

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
            return sqlHelper.Select<Contact>(Tables.Contacts, "AutoKey", "Name", "MobilePhone as Phone", "Email1 as Email", "CategoriesID as CategoryID");
        }

        public IEnumerable<ContactCategory> GetContactCategories()
        {
            return sqlHelper.Select<ContactCategory>(Tables.ContactCategories, "AutoKey", "Name");
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

            if (client.loginAsync(username, password).Result)
            {
                var isActivated = Convert.ToBoolean(sqlHelper.SelectWithWhere(Tables.Users, "Activated", "Name = '" + username + "'"));

                if (!isActivated)
                    return (int)ErrorCodes.EMAIL_NOT_VERIFIED;
                else
                    return 0;
            }
            else
                return (int)ErrorCodes.WRONG_CREDENTIALS;

          //  }
        //    catch (Exception ex)
      //      {
    //            Logger.Log(ex.Message);
  //              return (int)ErrorCodes.INTERNAL_ERROR;

//            }
        }

        public  int GenerateAllTables()
        {
            try
            {
                var databases = sqlHelper.GetSqlConnection().Query<string>("SELECT distinct DBName from " + Tables.UserDatabases); // sqlHelper.Select<string>(Tables.UserDatabases, "DBName");

                foreach (var db in databases)
                {
                    String query = Queries.UpdateTables;

                    sqlHelper.ExecuteNonQuery(query);
                }
               
                return (int)ErrorCodes.SUCCESS;
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
                return (int)ErrorCodes.INTERNAL_ERROR;

            }
        }

        public int GenerateTables()
        {
            try
            {
                String query = Queries.UpdateTables;

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
                   
                    if (IsEnterpriseSubUser(id))
                    {


                        string enterpriseCode = sqlHelper.SelectWithWhere(Tables.Users, "EnterpriseCode", "ID = '" + id + "'");
                        string enterpriseAccountID = sqlHelper.SelectWithWhere(Tables.EnterpriseCodes, "UserID", "Code = '" + enterpriseCode + "'");

                        var wheres = new Dictionary<string, string>();
                        wheres.Add("UserID", "*");

                        if (!sqlHelper.Exists(Tables.UserDatabases, wheres))
                        {
                            sqlHelper.Insert(Tables.UserDatabases,
                              new string[] { "UserID", "DBName" },
                              new string[] { id, enterpriseAccountID });
                        }

                        sqlHelper.Insert(Tables.UserStorage,
    new string[] { "UserID", "UsedStorage", "Storage" },
    new string[] { id, "0", "-1" });
                        /*
                        string query = Queries.CreateEnterpriseTable;

                        query = query.Replace("UID", id);
                        query = query.Replace("DBNAME", enterpriseAccountID);

                        sqlHelper.ExecuteNonQuery(query);*/
                    }
                    else
                    {
                        sqlHelper.Insert(Tables.UserStorage,
new string[] { "UserID", "UsedStorage", "Storage" },
new string[] { id, "0", "3072" });

                        var wheres = new Dictionary<string, string>();
                        wheres.Add("UserID", "*");

                        if (!sqlHelper.Exists(Tables.UserDatabases, wheres))
                        {

                            sqlHelper.Insert(Tables.UserDatabases,
                                new string[] { "UserID", "DBName" },
                                new string[] { id, id });

                            //                            DatabaseManager.CreateDatabase(id, this);
                            sqlHelper.CreateDatabase(id, settings.DatabasesPath);


                            // Configure DB
                            string query = Queries.ConfigureDBQuery;

                            query = query.Replace("DBNAME", id);

                            sqlHelper.ExecuteNonQuery(query);


                            // Add Tables
                            query = Queries.AddTablesQuery;

                            query = query.Replace("DBNAME", id);


                            sqlHelper.ExecuteNonQuery(query);
                        }


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

        public  int Register(string email, string password, string phone, string enterpriseCode)
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

                    wheres.Add("Code", enterpriseCode);

                    if (!sqlHelper.Exists(Tables.EnterpriseCodes, wheres))
                    {
                        return (int)ErrorCodes.CODE_DOES_NOT_EXIST;
                    }

                }

                var id = GetUserAutoIcrementID();


                Logger.Log(id);

                if (sqlHelper.Insert(Tables.Users,
                    new string[] {  "Name", "ID", "AccountType", "EnterpriseCode", "Phone", "Activated", "[2FA]" },
                    new string[] {  email,  id, string.IsNullOrEmpty(enterpriseCode) ? "Free" : "EnterpriseSubUser", string.IsNullOrEmpty(enterpriseCode) ? null : enterpriseCode, phone, "0","0" }))
                {

                    client.SetPasswordAsync(email, password);
                    
                    return (int)ErrorCodes.SUCCESS;

                    /*
                    string sql = Queries.NewDBQuery;//String.Join(Environment.NewLine, File.ReadAllLines("script.sql"));

                    sql = sql.Replace("DBNAME", id );
                    sql = sql.Replace("DBPATH", settings.DatabasesPath);

                    Logger.Log(sql);
                    if(sqlHelper.ExecuteNonQuery(sql) > 0)
                    else
                        return (int)ErrorCodes.INTERNAL_ERROR;*/

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

        public void Dispose()
        {
            sqlHelper.Dispose();

        }

        public List<EmailItem> GetEmails()
        {
            return sqlHelper.Select<EmailItem>(Tables.Emails, "*");
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
        CODE_DOES_NOT_EXIST = 106,
        EMAIL_NOT_VERIFIED = 107
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
