using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Database
{
    public class DataManager
    {
        private static SQLHelper sqlHelper;
        public DataManager()
        {
            sqlHelper = new SQLHelper(GetConnectionString());
        }

        public static bool Login(string username, string password)
        {
            Dictionary<string, string> wheres = new Dictionary<string, string>();

            wheres.Add("Username", username);
            wheres.Add("Password", password);

            return sqlHelper.Exists(Tables.Users, wheres);
        }

        public string GetConnectionString()
        {
            return new SqlConnectionStringBuilder()
            {
                DataSource = @".\ALSAHL",
                UserID = "test",
                Password = "test_2008",
                InitialCatalog = "DMS"
            }.ConnectionString;
        } 
    }
}
