using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sys.Developer.Tools.Fcd
{
    public class TableDto
    {
        public string TableName { get; set; }

        public string FieldName { get; set; }

        public string FieldType { get; set; }

        public int FieldPrecision { get; set; }

        public int FieldScale { get; set; }

        public bool FieldIsNullable { get; set; }
    }
}
