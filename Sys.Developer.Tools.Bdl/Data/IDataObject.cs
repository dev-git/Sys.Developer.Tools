using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sys.Developer.Tools.Bdl.Data
{
    public interface IDataObject
    {
        void GetTable(string connectStr);

        void GetStoredProcedure(string connectStr, string spText, bool createXSD);
    }
}
