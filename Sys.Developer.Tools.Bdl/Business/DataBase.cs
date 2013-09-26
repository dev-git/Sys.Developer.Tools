using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sys.Developer.Tools.Bdl.Business
{
    public class DataBase : BusinessObject
    {
        public DataBase()
        {
            WhenCreated = DateTime.Now;
            WhenModified = WhenCreated;
        }

        public DataBase(string name)
            : this()
        {
            Name = name;
        }

        public string Name { get; set; }

        public bool IsSystem { get; set; }

        public IList<Table> TableList  { get; set; }

        public SqlServerInstance SqlServerInstance { get; set; }
    }
}
