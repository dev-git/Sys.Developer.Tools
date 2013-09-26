using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sys.Developer.Tools.Bdl.Business
{
    public class Field : BusinessObject
    {
        public Field()
        {
            WhenCreated = DateTime.Now;
            WhenModified = WhenCreated;
        }

        public Field(string name, string type, int precision, int scale, bool isNullable)
            : this()
        {
            Name = name;
            Type = type;
            Precision = precision;
            Scale = scale;
            IsNullable = isNullable;
        }

        // The parent 
        public Table Table { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public int Precision { get; set; }

        public int Scale { get; set; }

        public bool IsNullable { get; set; }
    }
}
