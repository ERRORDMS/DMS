using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using System.Threading.Tasks;
using System.Reflection;
using System.Data;

namespace DMS.Database
{
    public class SQLHelper : IDisposable
    {
        private SqlConnection sqlConnection;
        public SQLHelper(SqlConnectionStringBuilder conString)
        {
            sqlConnection = new SqlConnection(conString.ConnectionString);
        }

        /// <summary>
        /// Execute reader query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<T> ExecuteReader<T>(string query)
        {
            
            if (sqlConnection.State == System.Data.ConnectionState.Closed)
                sqlConnection.Open();

            //var list = sqlConnection.Query<T>(query).ToList();
            var sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = System.Data.CommandType.Text;
            sqlCommand.CommandText = query;

            var reader = sqlCommand.ExecuteReader();

            var list = DataReaderMapToList<T>(reader);


                


            return list;
        }

        public void CreateDatabase(string name, string path)
        {
            string query = string.Format(@"
CREATE DATABASE [{0}]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'MoneySql_dat', FILENAME = N'{1}\{0}.mdf' , SIZE = 786304KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB ), 
 FILEGROUP [FS] CONTAINS FILESTREAM  DEFAULT
( NAME = N'filestream', FILENAME = N'{1}\{0}_filestream' , MAXSIZE = UNLIMITED)
 LOG ON 
( NAME = N'MoneySql_log', FILENAME = N'{1}\{0}_log.ldf' , SIZE = 1024000KB , MAXSIZE = 1024000KB , FILEGROWTH = 10%)
", name, path);
            ExecuteNonQuery(query);
        }
        /// <summary>
        /// Execute reader query
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public SqlDataReader ExecuteReader(string query)
        {
            if (sqlConnection.State == System.Data.ConnectionState.Closed)
                sqlConnection.Open();

            var sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = System.Data.CommandType.Text;

            sqlCommand.CommandText = query;


            return sqlCommand.ExecuteReader();
        }

        public bool TableExists(string tablename)
        {

            if (sqlConnection.State == System.Data.ConnectionState.Closed)
                sqlConnection.Open();

            var sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = System.Data.CommandType.Text;

            sqlCommand.CommandText = "SELECT OBJECT_ID('" + tablename + "', 'U')";

            return sqlCommand.ExecuteScalar() != null;
        }

        public int ExecuteNonQuery(string query, params SqlParameter[] parameters )
        {
            if (sqlConnection.State == System.Data.ConnectionState.Closed)
                sqlConnection.Open();

            var sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = System.Data.CommandType.Text;

            sqlCommand.CommandText = query;
            if (parameters.Length != 0)
                sqlCommand.Parameters.AddRange(parameters);


            return sqlCommand.ExecuteNonQuery();
        }

        public T ExecuteScalar<T>(string query)
        {
            if (sqlConnection.State == System.Data.ConnectionState.Closed)
                sqlConnection.Open();

            var sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = System.Data.CommandType.Text;

            sqlCommand.CommandText = query;


            return (T)Convert.ChangeType(sqlCommand.ExecuteScalar(), typeof(T));
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
                if (sqlConnection.State == System.Data.ConnectionState.Closed)
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
            String query = "";
            try
            {
                object rv = null;
                if (sqlConnection.State == System.Data.ConnectionState.Closed)
                    sqlConnection.Open();

                var sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandType = System.Data.CommandType.Text;

                query = "INSERT INTO " + table + " (";

                query += String.Join(',', columns);

                query += ")";

                query += " VALUES (";

                for (int i = 0; i < values.Length; i++)
                {
                    string val = values[i];

                    if (!DateTime.TryParse(val, out _))
                        query += "N";

                    query += "'" + val + "'";

                    if (i != values.Length - 1)
                    {
                        query += ",";
                    }
                }

                query += ") SELECT SCOPE_IDENTITY()";

                sqlCommand.CommandText = query;

                rv = sqlCommand.ExecuteScalar();



                sqlCommand.Dispose();
                return Convert.ToString(rv);
            }catch(Exception ex)
            {
                Logger.Log(ex.Message + " - " + query);
                return "";
            }
        }
        /// <summary>
        /// Delete from a table
        /// </summary>
        /// <param name="table"></param>
        /// <param name="condition"></param>
        /// <returns></returns>

        public bool Delete(string table, string condition)
        {
            int rv = 0;
            try
            {
                if (sqlConnection.State == System.Data.ConnectionState.Closed)
                    sqlConnection.Open();

                var sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandType = System.Data.CommandType.Text;

                String query = "DELETE from " + table + " WHERE " + condition;


                sqlCommand.CommandText = query;

                rv = sqlCommand.ExecuteNonQuery();

    
                    
                sqlCommand.Dispose();
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
            return rv >= 1;
        }

        /// <summary>
        /// Update rows in a table
        /// </summary>
        /// <param name="table">Table to insert into</param>
        /// <param name="columns">Columns to insert values into</param>
        /// <param name="values">Values to insert into specified columns</param>
        /// <returns>Success or failure</returns>
        public bool Update(string table, string[] columns, string[] values, string condition)
        {
            int rv = 0;
            try
            {
                if (sqlConnection.State == System.Data.ConnectionState.Closed)
                    sqlConnection.Open();

                var sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandType = System.Data.CommandType.Text;

                String query = "UPDATE " + table + " SET ";

                for (int i = 0; i < values.Length; i++)
                {
                    string val = values[i];
                    string col = columns[i];

                    query += col + " = N'" + val + "'";

                    if (i != values.Length - 1)
                    {
                        query += ",";
                    }
                }

                if(!string.IsNullOrEmpty(condition))
                query += " WHERE " + condition ;

                sqlCommand.CommandText = query;

                rv = sqlCommand.ExecuteNonQuery();

    
                    
                sqlCommand.Dispose();
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
            return rv >= 1;
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

            if (sqlConnection.State == System.Data.ConnectionState.Closed)
                sqlConnection.Open();

            var sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = System.Data.CommandType.Text;

            string query = "SELECT " + String.Join(',', columns) + " FROM " + table;

            sqlCommand.CommandText = query;

            using (var reader = sqlCommand.ExecuteReader())
            {
                list = DataReaderMapToList<T>(reader);
            }

            
            sqlCommand.Dispose();
            
            return list;
        }

        /// <summary>
        /// Select specified columns from a table
        /// </summary>
        /// <typeparam name="T">Which DataType should it select into</typeparam>
        /// <param name="table">Table to select from</param>
        /// <param name="column">Columns to be selected</param>
        /// <returns></returns>
        public string Select(string table, string column)
        {
            string rv;

            if (sqlConnection.State == System.Data.ConnectionState.Closed)
                sqlConnection.Open();

            var sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = System.Data.CommandType.Text;

            string query = "SELECT " + column + " FROM " + table;

            sqlCommand.CommandText = query;

            rv = Convert.ToString(sqlCommand.ExecuteScalar());


                

            sqlCommand.Dispose();

            return rv;
        }

        /// <summary>
        /// Select specified columns from a table
        /// </summary>
        /// <param name="table">Table to select from</param>
        /// <param name="column">Columns to be selected</param>
        /// <returns></returns>
        public string SelectWithWhere(string table, string column, string where)
        {
            string rv;

            if (sqlConnection.State == System.Data.ConnectionState.Closed)
                sqlConnection.Open();

            var sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandType = System.Data.CommandType.Text;

            string query = "SELECT " + column + " FROM " + table + " WHERE " + where;

            sqlCommand.CommandText = query;

            rv = Convert.ToString(sqlCommand.ExecuteScalar());


                

            sqlCommand.Dispose();

            return rv;
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

            if (sqlConnection.State == System.Data.ConnectionState.Closed)
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
            if (sqlConnection.State == System.Data.ConnectionState.Closed)
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
            if (sqlConnection.State == System.Data.ConnectionState.Closed)
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

            {
                
                sqlConnection.Dispose();
            }
        }

    }
}
