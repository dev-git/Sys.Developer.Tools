using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sys.Developer.Tools.Fcd.ServiceContracts
{
    public interface IFacadeService
    {
        bool Connect(string connectionString);

        void Disconnect();

        void GetStoredProcedure(string connectStr, string spText, bool createXSD);

        IEnumerable<SqlServerInstanceDto> GetSqlServerInstances(bool getLocalOnly);

        IEnumerable<DatabaseDto> GetDatabaseList(string instanceName);
        //void GetDatabaseTable(string instance, ref 
    }
}
