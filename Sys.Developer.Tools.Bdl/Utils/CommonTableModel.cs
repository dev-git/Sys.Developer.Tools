using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sys.Developer.Tools.Bdl
{
    [Serializable]
    public class CommonTableModel
    {
        public string Database { get; set; }
        public List<string> TableList { get; set; }
    }
}
