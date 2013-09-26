using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sys.Developer.Tools.Fcd
{
    public class DatabaseDto
    {
        public string Name { get; set; }

        public bool IsSystem { get; set; }

        public SqlServerInstanceDto SqlServerInstance { get; set; }

        public List<TableDto> TableList { get; set; }
    }
}
