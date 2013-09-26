using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sys.Developer.Tools.Bdl.Business
{
    public class Table : BusinessObject
    {
        public Table()
        {
            //Fields = new Field();
            WhenCreated = DateTime.Now;
            WhenModified = WhenCreated;
            FieldList = new List<Field>();
            CommonList = new List<string>();
            //Table[] tabList = new Table[] { new Table("Config"), new Table("S_This"), new Table("S_Stations")};
            //
        }


        public Table(string tableName)
            : this()
        {
            Name = tableName;
        }

        public Table(string tableName, Field fields)
            : this()
        {
            Name = tableName;
            //FieldList = fields;
        }

        public string Name { get; set; }

        public IList<Field> FieldList { get; set; }

        public List<String> CommonList { get; set; }

        public void SetCommonList(String[] commTabList)
        {
            if (commTabList == null)
            {
                CommonList.AddRange(new String[] { "Config", "S_This", "S_Stations", "Items", "Customers", "TransHeaders", 
                    "TransLines", "TransPayments", "Orders", "OrderLines", "OrderDeliveries", "OrderNotes", "OrderPayments"});
            }
            else
            {
                CommonList.AddRange(commTabList);
            }

            
        }
    }
}
