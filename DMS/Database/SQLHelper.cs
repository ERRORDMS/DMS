using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.Data;

namespace DMS.Database
{
    public class SQLHelper : IDisposable
    {
        private SqlConnection sqlConnection;
        public SQLHelper(String conString)
        {
            sqlConnection = new SqlConnection(conString);
        }

        /// <summary>
        /// Execute reader query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<T> ExecuteReader<T>(string query)
        {
            if (sqlConnection.State != System.Data.ConnectionState.Open)
                sqlConnection.Open();

            var sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = System.Data.CommandType.Text;

            sqlCommand.CommandText = query;

            List<T> list;

            using (var reader = sqlCommand.ExecuteReader())
            {
                list = DataReaderMapToList<T>(reader);
            }


            if (sqlConnection.State == System.Data.ConnectionState.Open)
                sqlConnection.Close();

            sqlCommand.Dispose();

            return list;
        }

        /// <summary>
        /// Insert into a table.
        /// </summary>
        /// <param name="table">Table to insert into</param>
        /// <param name="columns">Columns to insert values into</param>
        /// <param name="values">Values to insert into specified columns</param>
        /// <returns>Success or failure</returns>
        public bool Insert(string table, string[] columns, string[] values)
        {
            int rv = 0;
            try
            {
                if (sqlConnection.State != System.Data.ConnectionState.Open)
                    sqlConnection.Open();

                var sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandType = System.Data.CommandType.Text;

                String query = "INSERT INTO " + table + " (";

                query += String.Join(',', columns);

                query += ")";

                query += " VALUES (";

                for (int i = 0; i < values.Length; i++)
                {
                    string val = values[i];

                    query += "N'" + val + "'";

                    if (i != values.Length - 1)
                    {
                        query += ",";
                    }
                }

                query += ")";

                sqlCommand.CommandText = query;

                rv = sqlCommand.ExecuteNonQuery();

                if (sqlConnection.State == System.Data.ConnectionState.Open)
                    sqlConnection.Close();
                sqlCommand.Dispose();
            }
            catch(Exception ex)
            {
                var msg = ex.Message;
            }
            return rv >= 1;
        }
        /// <summary>
        /// Insert into a table returning the ID.
        /// </summary>
        /// <param name="table">Table to insert into</param>
        /// <param name="columns">Columns to insert values into</param>
        /// <param name="values">Values to insert into specified columns</param>
        /// <returns>Success or failure</returns>
        public string InsertWithID(string table, string[] columns, string[] values)
        {
            object rv = null;
                if (sqlConnection.State != System.Data.ConnectionState.Open)
                    sqlConnection.Open();

                var sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandType = System.Data.CommandType.Text;

                String query = "INSERT INTO " + table + " (";

                query += String.Join(',', columns);

                query += ")";

                query += " VALUES (";

                for (int i = 0; i < values.Length; i++)
                {
                    string val = values[i];

                    query += "N'" + val + "'";

                    if (i != values.Length - 1)
                    {
                        query += ",";
                    }
                }

                query += ") SELECT SCOPE_IDENTITY()";

                sqlCommand.CommandText = query;

                rv = sqlCommand.ExecuteScalar();

                if (sqlConnection.State == System.Data.ConnectionState.Open)
                    sqlConnection.Close();
                sqlCommand.Dispose();
            return Convert.ToString(rv);
        }


        /// <summary>
        /// Select specified columns from a table
        /// </summary>
        /// <typeparam name="T">Which DataType should it select into</typeparam>
        /// <param name="table">Table to select from</param>
        /// <param name="columns">Columns to be selected</param>
        /// <returns></returns>
        public List<T> Select<T>(string table, params string[] columns)
        {
            List<T> list = new List<T>();

            if (sqlConnection.State != System.Data.ConnectionState.Open)
                sqlConnection.Open();

            var sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = System.Data.CommandType.Text;

            string query = "SELECT " + String.Join(',', columns) + " FROM " + table;

            sqlCommand.CommandText = query;

            using (var reader = sqlCommand.ExecuteReader())
            {
                list = DataReaderMapToList<T>(reader);
            }


             if (sqlConnection.State == System.Data.ConnectionState.Open)
                    sqlConnection.Close();

            sqlCommand.Dispose();
            
            return list;
        }
        /// <summary>
        /// Select specified columns from a table with conditions
        /// </summary>
        /// <typeparam name="T">Which DataType should it select into</typeparam>
        /// <param name="table">Table to select from</param>
        /// <param name="columns">Columns to be selected</param>
        /// <param name="whereValues">Conditions</param>
        /// <returns></returns>
        public List<T> SelectWithWhere<T>(string table, string[] columns, Dictionary<string, string> whereValues)
        {
            List<T> list = new List<T>();

            if (sqlConnection.State != System.Data.ConnectionState.Open)
                sqlConnection.Open();

            var sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = System.Data.CommandType.Text;

            string query = "SELECT " + String.Join(',', columns) + " FROM " + table;

            query += " WHERE ";

            int i = 0;
            foreach(string where in whereValues.Keys)
            {
                i++;
                query += where + " = '" + whereValues[where] + "'";

                if(i != whereValues.Keys.Count)
                {
                    query += " and ";
                }
            }

            sqlCommand.CommandText = query;

            using (var reader = sqlCommand.ExecuteReader())
            {
                list = DataReaderMapToList<T>(reader);
            }


            if (sqlConnection.State == System.Data.ConnectionState.Open)
                sqlConnection.Close();

            sqlCommand.Dispose();

            return list;
        }

        /// <summary>
        /// Check if a row exists in a table.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="whereValues"></param>
        /// <returns></returns>
        public bool Exists(string table, Dictionary<string, string> whereValues)
        {
            if (sqlConnection.State != System.Data.ConnectionState.Open)
                sqlConnection.Open();

            var sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = System.Data.CommandType.Text;
            string query = "SELECT 1 FROM " + table + " WHERE ";

            int i = 0;
            foreach (string where in whereValues.Keys)
            {
                i++;
                query += where + " = '" + whereValues[where] + "'";

                if (i != whereValues.Keys.Count)
                {
                    query += " and ";
                }
            }

            sqlCommand.CommandText = query;

            var reader = sqlCommand.ExecuteReader();

            bool hasRows = reader.HasRows;
            if (sqlConnection.State == System.Data.ConnectionState.Open)
                sqlConnection.Close();

            sqlCommand.Dispose();

            return hasRows;
        }
        /// <summary>
        /// Check if a row exists in a table but using OR instead of AND.
        /// </summary>
        /// <param name="table"></param>
        /// <param name="whereValues"></param>
        /// <returns></returns>
        public bool ExistsOR(string table, Dictionary<string, string> whereValues)
        {
            if (sqlConnection.State != System.Data.ConnectionState.Open)
                sqlConnection.Open();

            var sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = System.Data.CommandType.Text;
            string query = "SELECT 1 FROM " + table + " WHERE ";

            int i = 0;
            foreach (string where in whereValues.Keys)
            {
                i++;
                query += where + " = '" + whereValues[where] + "'";

                if (i != whereValues.Keys.Count)
                {
                    query += " OR ";
                }
            }

            sqlCommand.CommandText = query;

            var reader = sqlCommand.ExecuteReader();

            bool hasRows = reader.HasRows;
            if (sqlConnection.State == System.Data.ConnectionState.Open)
                sqlConnection.Close();

            sqlCommand.Dispose();

            return hasRows;
        }

        public List<T> DataReaderMapToList<T>(IDataReader dr)
        {
            List<T> list = new List<T>();
            T obj = default(T);
            while (dr.Read())
            {
                obj = Activator.CreateInstance<T>();
                foreach (PropertyInfo prop in obj.GetType().GetProperties())
                {
                    if (!object.Equals(dr[prop.Name], DBNull.Value))
                    {
                        prop.SetValue(obj, dr[prop.Name], null);
                    }
                }
                list.Add(obj);
            }
            return list;
        }

        public void Dispose()
        {
            if (sqlConnection.State == System.Data.ConnectionState.Open)
            {
                sqlConnection.Close();
                sqlConnection.Dispose();
            }
        }

    }
}
