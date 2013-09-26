using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sys.Developer.Tools.Bdl.Data
{
    public class DataAccess
    {
        public static IConnection Connection
        {
            get { return new Connection(); }
        }

        public static IDataObject DataObject
        {
            get { return new DataObject(); }
        }

        public static IDatabaseInstanceDao DatabaseInstanceDao
        {
            get { return new DatabaseInstanceDao(); }
        }

        /*public static ITableDao TableDao
        {
            get { return new TableDao(); }
        }*/
    }
}
