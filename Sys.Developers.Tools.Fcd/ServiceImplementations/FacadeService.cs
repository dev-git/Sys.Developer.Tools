using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Sys.Developer.Tools.Bdl.Data;
using Sys.Developer.Tools.Bdl.Business;
using Sys.Developer.Tools.Fcd.ServiceContracts;
using Sys.Developer.Tools.Fcd.DataTransferObjectMapper;


namespace Sys.Developer.Tools.Fcd
{
    public class FacadeService : IFacadeService, IDisposable
    {
        private IDatabaseInstanceDao databaseInstanceDao = DataAccess.DatabaseInstanceDao;

        public bool Connect(string connectionString)
        {
            throw new NotImplementedException();
        }

        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        public void GetStoredProcedure(string connectStr, string spText, bool createXSD)
        {
            throw new NotImplementedException();
        }


        public IEnumerable<SqlServerInstanceDto> GetSqlServerInstances(bool getLocalOnly)
        {
            IEnumerable<SqlServerInstance> sqlServerInstances = databaseInstanceDao.GetSqlServerInstances(getLocalOnly);
            return sqlServerInstances.Select(c => Mapper.ToDataTransferObject(c)).ToList();
        }


        public void GetTableData(string sqlServer, string table)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DatabaseDto> GetDatabaseList(string instance)
        {
            IEnumerable<DataBase> db = databaseInstanceDao.GetDatabaseList(instance);
            return db.Select(c => Mapper.ToDataTransferObject(c)).ToList();
        }

        public IEnumerable<TableDto> GetTableList(string sqlServer, string database, bool onlyCommon)
        {
            IEnumerable<Table> tab = databaseInstanceDao.GetTableList(sqlServer, database, onlyCommon);
            return tab.Select(t => Mapper.ToDataTransferObject(t)).ToList();
        }

        public IList<object> GetTableData(string serverName, string dbName, string tableName, IList<string> fieldList, string filter, bool lookForJoin)
        {
            return databaseInstanceDao.GetTableData(serverName, dbName, tableName, fieldList, filter, lookForJoin);
        }

        public IList<string> GetFieldList(string serverName, string dbName, string tableName)
        {
            return databaseInstanceDao.GetFieldList(serverName, dbName, tableName);
        }

        public int UpdateRecord(string serverName, string dbName, string tableName, string updateField, string updateValue, string identField, string identValue)
        {
            return databaseInstanceDao.UpdateRecord(serverName, dbName, tableName, updateField, updateValue, identField, identValue);
        }

        public void Dispose()
        {
            if (databaseInstanceDao != null)
            {
                databaseInstanceDao = null;
            }
        }

        public void SetSqlServerInstance(SqlServerInstanceDto newSsid)
        {
            throw new NotImplementedException();
        }
    }
}
