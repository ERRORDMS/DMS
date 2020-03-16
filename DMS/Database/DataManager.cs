﻿using System;
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
}
