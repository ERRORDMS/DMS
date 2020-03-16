using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace DMS.Database
{
    public class DataManager
    {
        private SQLHelper sqlHelper;
        public DataManager()
        {
            sqlHelper = new SQLHelper(GetConnectionString());
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
