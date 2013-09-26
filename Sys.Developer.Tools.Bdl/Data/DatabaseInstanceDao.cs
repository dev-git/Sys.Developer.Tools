using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sys.Developer.Tools.Bdl.Business;

namespace Sys.Developer.Tools.Bdl.Data
{
    public class DatabaseInstanceDao : IDatabaseInstanceDao
    {
        public IList<SqlServerInstance> GetSqlServerInstances(bool getLocalOnly)
        {
            SqlServerInstance sqlServerInstance = new SqlServerInstance();
            return sqlServerInstance.GetSqlServerInstances(getLocalOnly);
        }

        public IList<SqlServerInstance> GetPersistedSqlServerInstances()
        {
            return SqlServerInstance.GetPersistedSqlServerInstances();
        }

        public Array GetDatabases(string instanceName)
        {
            return SqlServerInstance.GetDatabases(instanceName);
        }

        public Array GetDatabasesExtended(string instanceName)
        {
            return SqlServerInstance.GetDatabasesExtended(instanceName);
        }


        public Array GetTables(string instanceName, string databaseName)
        {
            return SqlServerInstance.GetTables(instanceName, databaseName);
        }


        public IList<DataBase> GetDatabaseList(string instanceName)
        {
            return SqlServerInstance.GetDatabaseList(instanceName);
        }

        public IList<Table> GetTableList(string instanceName, string databaseName, bool onlyCommon)
        {
            return SqlServerInstance.GetTableList(instanceName, databaseName, onlyCommon);
        }

        public IList<object> GetTableData(string serverName, string dbName, string tableName, IList<string> fieldList, string filter, bool lookForJoin)
        {
            return SqlServerInstance.GetTableData(serverName, dbName, tableName, fieldList, filter, lookForJoin);
        }

        public IList<string> GetFieldList(string instanceName, string databaseName, string tableName)
        {
            return SqlServerInstance.GetFieldList(instanceName, databaseName, tableName);
        }

        public int UpdateRecord(string serverName, string dbName, string tableName, string updateField, string updateValue, string identField, string identValue)
        {
            return SqlServerInstance.UpdateRecord(serverName, dbName, tableName, updateField, updateValue, identField, identValue);
        }
    }
}
