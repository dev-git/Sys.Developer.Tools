using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sys.Developer.Tools.Fcd
{
    public class SqlServerInstanceDto
    {
        public string Name { get; set; }

        public string Machine { get; set; }

        public string Instance { get; set; }

        public bool IsClustered { get; set; }

        public string Version { get; set; }

        public bool IsLocal { get; set; }

        public List<DatabaseDto> DataBase { get; set; }
    }
}
