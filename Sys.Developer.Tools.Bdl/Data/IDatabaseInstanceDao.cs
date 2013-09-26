using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sys.Developer.Tools.Bdl.Business;

namespace Sys.Developer.Tools.Bdl.Data
{
    public interface IDatabaseInstanceDao
    {
        IList<SqlServerInstance> GetSqlServerInstances(bool getLocalOnly);

        IList<DataBase> GetDatabaseList(string instanceName);

        Array GetDatabases(string instanceName);

        Array GetDatabasesExtended(string instanceName);

        IList<Table> GetTableList(string instanceName, string database, bool onlyCommon);

        IList<object> GetTableData(string serverName, string dbName, string tableName, IList<string> fieldList, string filter, bool lookForJoin);

        IList<string> GetFieldList(string serverName, string dbName, string tableName);

        int UpdateRecord(string serverName, string dbName, string tableName, string updateField, string updateValue, string identField, string identValue);
    }
}
