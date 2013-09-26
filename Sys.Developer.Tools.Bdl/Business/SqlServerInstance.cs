using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.SqlServer.Management.Common;
using Microsoft.Win32;
using System.Data;
using System.Data.SqlClient;

namespace Sys.Developer.Tools.Bdl.Business
{
    public class SqlServerInstance : BusinessObject
    {
        public SqlServerInstance()
        {
            WhenCreated = DateTime.Now;
            WhenModified = WhenCreated;
        }

        public SqlServerInstance(string name, string server, string instance, bool isClustered, string version, bool isLocal)
            : this()
        {
            Name = name;
            Machine = server;
            Instance = instance;
            IsClustered = isClustered;
            Version = version;
            IsLocal = isLocal;
        }

        public string Name { get; set; }

        public string Machine { get; set; }

        public string Instance { get; set; }

        public bool IsClustered { get; set; }

        public string Version { get; set; }

        public bool IsLocal { get; set; }

        public IList<DataBase> DataBaseList { get; set; }

        public static List<CommonTableModel> CommonTableModelList { get; set; }

        private static ServerConnection Connection = null;
        private static Microsoft.SqlServer.Management.Smo.Server svr;
        private static SqlConnection conn = null;
        //private const string masterConnString = "server={0};initial catalog=master;Trusted_Connection=True;";
        //private const string masterConnString = "server={0};initial catalog=master;user id=sa;password=sa123;";
        private const string masterConnString = "server={0};initial catalog=master;user id=sa;password=Passw0rd;";

        public IList<SqlServerInstance> GetSqlServerInstances(bool getLocalOnly)
        {
            DataTable dt = GetSqlServers(getLocalOnly);
            IList<SqlServerInstance> sqlServerList = new List<SqlServerInstance>();

            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    sqlServerList.Add(new SqlServerInstance(dr.Field<string>(0), dr.Field<string>(1), dr.Field<string>(2), dr.Field<bool>(3), dr.Field<string>(4),
                        dr.Field<bool>(5)));
                }
            }

            // Get the list of any regularly view tables
            CommonTableModelList = Utils.GetCommonTableList();

            return sqlServerList;
        }

        /// <summary>
        /// Gets the SQL servers.
        /// </summary>
        /// <param name="getLocalOnly">if set to <c>true</c> [get local only].</param>
        /// <returns></returns>
        /// <exception cref="System.ApplicationException"></exception>
        private DataTable GetSqlServers(bool getLocalOnly)
        {
            bool found = false;
            DataTable dt = new DataTable();

            try
            {
                dt = Microsoft.SqlServer.Management.Smo.SmoApplication.EnumAvailableSqlServers(getLocalOnly);

                //Search Registry for local server then add then to server list
                RegistryKey rk = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Microsoft SQL Server");

                String[] instances = (String[])rk.GetValue("InstalledInstances");

                if (instances != null && instances.Length > 0)
                {
                    foreach (String element in instances)
                    {
                        found = false;

                        String name = "";

                        //only add if it doesn't exist
                        if (element == "MSSQLSERVER")
                        {
                            name = System.Environment.MachineName;
                        }
                        else
                        {
                            name = System.Environment.MachineName + @"\" + element;
                        }

                        for (int ndx = 0; ndx < dt.Rows.Count; ndx++)
                        {
                            if (dt.Rows[ndx].ItemArray.GetValue(0).ToString() == name)
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            dt.Rows.Add(name, String.Empty, String.Empty, false, String.Empty, true);
                            dt.AcceptChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return dt;
        }

        /// <summary>
        /// Gets the database list.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        /// <exception cref="System.ApplicationException"></exception>
        public static List<DataBase> GetDatabaseList(string instance)
        {
            List<DataBase> dbs = new List<DataBase>();
            if (Connection == null)
            {
                try
                {
                    InitialiseServer(instance);
                    
                    foreach (Microsoft.SqlServer.Management.Smo.Database db in svr.Databases)
                    {
                        DataBase dB = new DataBase(db.Name);
                        dB.IsSystem = db.IsSystemObject;
                        dbs.Add(dB);
                    }

                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message);
                }
                finally
                {
                    Connection.Disconnect();
                    Connection = null;
                }
            }
            
            return dbs;
        }


        /// <summary>
        /// Gets the databases.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        /// <exception cref="System.ApplicationException"></exception>
        public static Array GetDatabases(string instance)
        {
            string[] databaseList = null;
            if (Connection == null)
            {
                try
                {
                    InitialiseServer(instance);

                    databaseList = new string[svr.Databases.Count];
                    int xx = 0;
                    foreach (Microsoft.SqlServer.Management.Smo.Database db in svr.Databases)
                    {
                        DataBase db1 = new DataBase(db.Name);
                        databaseList.SetValue(db.Name, xx);
                        //databaseList.SetValue(db.IsSystemObject, xx + 1);
                        xx++;
                    }
                    //connection.Disconnect();


                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message);
                }
                finally
                {
                    Connection.Disconnect();
                    Connection = null;
                }
            }
            
            return databaseList;
        }

        /// <summary>
        /// Gets the databases extended.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns></returns>
        /// <exception cref="System.ApplicationException"></exception>
        public static Array GetDatabasesExtended(string instance)
        {
            string[,] databaseList = null;
            if (Connection == null)
            {
                try
                {
                    InitialiseServer(instance);

                    databaseList = new string[svr.Databases.Count, 3];
                    int xx = 0;
                    foreach (Microsoft.SqlServer.Management.Smo.Database db in svr.Databases)
                    {
                        int tableCount = GetTables(instance, db.Name).Length;
                        int fieldCount = GetFields(instance, db.Name).Length;
                        databaseList.SetValue(db.Name, xx, 0);
                        databaseList.SetValue(tableCount.ToString(), xx, 1);
                        databaseList.SetValue(fieldCount.ToString(), xx, 2);
                        xx++;
                    }
                    //connection.Disconnect();


                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message);
                }
                finally
                {
                    Connection.Disconnect();
                    Connection = null;
                }


            }
            return databaseList;
        }

        /// <summary>
        /// Initialises the server.
        /// </summary>
        /// <param name="serverInstanceName">Name of the server instance.</param>
        /// <exception cref="System.ApplicationException"></exception>
        private static void InitialiseServer(string serverInstanceName)
        {
            try
            {
                Connection = new ServerConnection(serverInstanceName, "sa", "Passw0rd");
                //Connection.LoginSecure = true;
                //Connection.ServerInstance = String.Format(masterConnString, serverInstanceName);
                svr = new Microsoft.SqlServer.Management.Smo.Server(Connection);

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


        /// <summary>
        /// Gets the table list.
        /// </summary>
        /// <param name="instanceName">Name of the instance.</param>
        /// <param name="databaseName">Name of the database.</param>
        /// <param name="onlyCommon">if set to <c>true</c> [only common].</param>
        /// <returns></returns>
        public static IList<Table> GetTableList(string instanceName, string databaseName, bool? onlyCommon)
        {
            IList<Table> tableList = new List<Table>();
            //if (TableDbList == null)
            //{
            
            TableDbList = (string[])GetTables(instanceName, databaseName);
            //}

            Array tables = TableDbList;
            if (CommonTableModelList != null && onlyCommon != null)
            {
                var ctm = CommonTableModelList.SingleOrDefault(c => c.Database == databaseName);

                if (ctm != null)
                {
                    if ((bool)onlyCommon)
                    {
                        tables = ctm.TableList.Intersect(TableDbList).ToArray();
                    }
                    else
                    {
                        //tables = TableDbList.Except(tab.CommonList).ToArray();
                        tables = TableDbList.Except(ctm.TableList).ToArray();
                    }
                }
                else
                {
                    tables = (string[])GetTables(instanceName, databaseName);
                }
            }
            else
            {
                tables = (string[])GetTables(instanceName, databaseName);
                // Not looking for common tables, just return all tables
                //tables = ;
            }

            // Convert array of tables to Table objects..
            for (int xx = 0; xx < tables.Length; xx++)
            {
                tableList.Add(new Table(tables.GetValue(xx).ToString()));
            }

            return tableList;

        }

        private static string[] TableDbList { get; set; }

        /// <summary>
        /// Gets the table data.
        /// </summary>
        /// <param name="serverName">Name of the server.</param>
        /// <param name="dbName">Name of the db.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="fieldList">The field list.</param>
        /// <param name="filter">The filter.</param>
        /// <param name="lookForJoin">if set to <c>true</c> [look for join].</param>
        /// <returns></returns>
        /// <exception cref="System.ApplicationException"></exception>
        public static IList<object> GetTableData(string serverName, string dbName, string tableName, IList<string>fieldList, string filter, bool lookForJoin)
        {

            string fields = "*";
            string selectionFilter = "select top 100";
            string orderDirection = "asc";
            if (fieldList != null && fieldList.Count > 0)
            {
                fields = String.Join(", ", fieldList.ToArray());
            }

            if (!String.IsNullOrEmpty(filter))
            {
                orderDirection = filter;
            }

            IList<object> tableData = new List<object>();
            try
            {
                string connectionString = String.Format(masterConnString, serverName);
                using (conn = new SqlConnection(connectionString))
                {
                    // Open the connection
                    conn.Open();

                    // Get the number of tables
                    string sqlString = String.Format("use {0};{3} {2} from {1} order by 1 {4} ;", dbName, tableName, fields, selectionFilter, orderDirection);
                    using (SqlCommand cmd = new SqlCommand(sqlString, conn))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                               while (reader.Read())
                            {
                                Object[] vals = new object[reader.FieldCount];
                                reader.GetValues(vals);
                                tableData.Add(vals);
                            }
                        }
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
            }

            return tableData;
        }


        public static Array GetTables(string instanceName, string databaseName)
        {
            string[] tableList = null;

            try
            {
                string connectionString = String.Format(masterConnString, instanceName);

                using (conn = new SqlConnection(connectionString))
                {
                    // Open the connection
                    conn.Open();

                    // Get the number of tables
                    //string sqlString = String.Format("use {0};select count(*) as 'Count' from sysobjects where xtype='u';", databaseName);
                    string sqlString = String.Format("use {0};select COUNT(*) from INFORMATION_SCHEMA.TABLES;", databaseName);
                    using (SqlCommand cmd = new SqlCommand(sqlString, conn))
                    {
                        int tableCount = Int32.Parse(cmd.ExecuteScalar().ToString());
                        tableList = new string[tableCount];

                        // Get the list of tables
                        // Todo: Check here for SQL version... if SQL2K then we comment out the line below...
                        //string sqlTableString = String.Format("use {0};select name as 'Name' from sysobjects where xtype='u' order by name;", databaseName);
                        string sqlTableString = 
                            String.Format("use {0};SELECT TABLE_SCHEMA as 'Schema', TABLE_NAME as 'Name' FROM information_schema.tables order by TABLE_SCHEMA;", databaseName);
                        cmd.CommandText = sqlTableString;

                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            int xx = 0;
                            while (reader.Read())
                            {
                                tableList.SetValue(String.Format("{0}.{1}", reader["Schema"], reader["Name"]), xx);
                                xx++;
                            }
                        }
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
            }

            return tableList;
        }

        public static Array GetFields(string instanceName, string databaseName)
        {
            string[] fieldList = null;

            try
            {
                string connectionString = String.Format(masterConnString, instanceName);
                using (conn = new SqlConnection(connectionString))
                {
                    // Open the connection
                    conn.Open();

                    // Get the number of tables
                    string sqlString = String.Format("use {0};select count(*) as 'Count' from syscolumns;", databaseName);
                    using (SqlCommand cmd = new SqlCommand(sqlString, conn))
                    {
                        int tableCount = Int32.Parse(cmd.ExecuteScalar().ToString());
                        fieldList = new string[tableCount];

                        // Get the list of tables
                        string sqlColumnString = String.Format("use {0};select name as 'Name' from syscolumns;", databaseName);
                        cmd.CommandText = sqlColumnString;

                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            int xx = 0;
                            while (reader.Read())
                            {
                                fieldList.SetValue(reader["Name"], xx);
                                xx++;
                            }
                        }
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
            }

            return fieldList;
        }

        /// <summary>
        /// Gets the field list.
        /// </summary>
        /// <param name="serverName">Name of the server.</param>
        /// <param name="dbName">Name of the db.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        /// <exception cref="System.ApplicationException"></exception>
        public static IList<string> GetFieldList(string serverName, string dbName, string tableName)
        {

            string[] tableSplit = tableName.Split('.');

            string schemaName = tableSplit[0];
            string tabName = tableSplit[1];

            IList<string> fieldList = new List<string>();
            try
            {
                string connectionString = String.Format(masterConnString, serverName);
                using (conn = new SqlConnection(connectionString))
                {
                    // Open the connection
                    conn.Open();

                    // Get the list of fields
                    //string sqlString = String.Format("use {0};select sc.name as columnname from sysobjects as so join syscolumns as sc on so.id = sc.id where so.name = '{1}';", dbName, tableName);
                    string sqlString = String.Format("use {0};select COLUMN_NAME as columnname from INFORMATION_SCHEMA.COLUMNS where TABLE_SCHEMA = '{1}' and TABLE_NAME ='{2}';", 
                        dbName, schemaName, tabName);
                    using (SqlCommand cmd = new SqlCommand(sqlString, conn))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                fieldList.Add(reader.GetString(0));
                            }

                        }
                        reader.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
            }

            return fieldList;
        }

        /// <summary>
        /// Gets the type of the field.
        /// </summary>
        /// <param name="serverName">Name of the server.</param>
        /// <param name="dbName">Name of the db.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        /// <exception cref="System.ApplicationException"></exception>
        public static string GetFieldType(string serverName, string dbName, string tableName, string fieldName)
        {
            string[] tableSplit = tableName.Split('.');

            string schemaName = tableSplit[0];
            string tabName = tableSplit[1];

            string fieldType = String.Empty;
            try
            {
                string connectionString = String.Format(masterConnString, serverName);
                using (conn = new SqlConnection(connectionString))
                {
                    // Open the connection
                    conn.Open();

                    // Get the field type
                    //string sqlString = String.Format("use {0};select TYPE_NAME(sc.[xtype]) from sysobjects as so join syscolumns as sc on so.id = sc.id where so.name = '{1}' and sc.name = '{2}';", 
                      //  dbName, tableName, fieldName);
                    string sqlString = String.Format(@"use {0};select DATA_TYPE as datatype from INFORMATION_SCHEMA.COLUMNS where TABLE_SCHEMA = '{1}' and TABLE_NAME = '{2}' 
                        and COLUMN_NAME = '{3}';", dbName, schemaName, tabName, fieldName);

                    using (SqlCommand cmd = new SqlCommand(sqlString, conn))
                    {
                        object sqlResult = cmd.ExecuteScalar();
                        if (sqlResult != null)
                        {
                            fieldType = (string)sqlResult;

                        }
                     }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
            }

            return fieldType;
        }

        /// <summary>
        /// Gets the field result.
        /// </summary>
        /// <param name="fieldType">Type of the field.</param>
        /// <param name="fieldValue">The field value.</param>
        /// <returns></returns>
        public static string GetFieldResult(string fieldType, string fieldValue)
        {
            string fieldResult = string.Empty;
            switch (fieldType)
            {
                case "varchar":
                case "char":
                case "nvarchar":
                case "nchar":
                    fieldResult = "'" + fieldValue + "'";
                    break;
                case "datetime":
                case "smalldatetime":
                    // Todo...
                    break;
                case "money":
                case "int":
                case "smallint":
                default:
                    fieldResult = fieldValue;
                    break;

            }

            return fieldResult;
        }

        /// <summary>
        /// Updates the record.
        /// </summary>
        /// <param name="serverName">Name of the server.</param>
        /// <param name="dbName">Name of the db.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="updateField">The update field.</param>
        /// <param name="updateValue">The update value.</param>
        /// <param name="identField">The ident field.</param>
        /// <param name="identValue">The ident value.</param>
        /// <returns></returns>
        /// <exception cref="System.ApplicationException"></exception>
        public static int UpdateRecord(string serverName, string dbName, string tableName, string updateField, string updateValue, string identField, string identValue)
        {
            int updateResult = -2;
            string sqlString = string.Empty;
            string updateValueResult = GetFieldResult(GetFieldType(serverName, dbName, tableName, updateField), updateValue);
            string identValueResult = GetFieldResult(GetFieldType(serverName, dbName, tableName, identField), identValue);
            try
            {
                string connectionString = String.Format(masterConnString, serverName);
                using (conn = new SqlConnection(connectionString))
                {
                    // Open the connection
                    conn.Open();
                    
                    // Update the record
                    sqlString = String.Format("use {0};update {1} set {2} = {3} where {4} = {5};", dbName, tableName, updateField, updateValueResult, identField, identValueResult);
                    using (SqlCommand cmd = new SqlCommand(sqlString, conn))
                    {
                        updateResult = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(sqlString,  ex);
            }
            finally
            {
                if (conn.State != ConnectionState.Closed)
                {
                    conn.Close();
                }
            }

            return updateResult;
        }

        internal static IList<SqlServerInstance> GetPersistedSqlServerInstances()
        {
            throw new NotImplementedException();
        }
    }
    
}
