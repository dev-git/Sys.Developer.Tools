using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace Sys.Developer.Tools.Bdl.Data
{
    public class DataObject : IDataObject
    {
        public void GetTable(string connectStr)
        {
            throw new NotImplementedException();
        }

        public void GetStoredProcedure(string connectStr, string spText, bool createXSD)
        {
            throw new NotImplementedException();
        }
    }
}
