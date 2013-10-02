using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Xml.Serialization;

using Sys.Developer.Tools.Bdl.Business;

namespace Sys.Developer.Tools.Bdl
{
    public class Utils
    {
        //private const string TAB_LIST_FILE = @"D:\Data\Google Drive\Development\src\Sys.Developer.Tools\Sys.Developer.Tools.Bdl\CommonTableList.xml";
        // Todo: ...
        //private const string TAB_LIST_FILE = @"C:\Data\Google Drive\Development\src\Sys.Developer.Tools\Sys.Developer.Tools.Bdl\Sys.Developer.Tools.BdlCommonTableList.xml";
        //private static readonly string TAB_LIST_FILE = Path.Combine(Environment.SpecialFolder.CommonApplicationData.ToString(), "Sys.Developer.Tools.BdlCommonTableList.xml");
        private static readonly string TAB_LIST_FILE = Path.Combine(Environment.CurrentDirectory, "Sys.Developer.Tools.BdlCommonTableList.xml");
        private static readonly string PRELOAD_SERVER = Path.Combine(Environment.SpecialFolder.CommonApplicationData.ToString(), "Sys.Developer.Tools.BdlSqlServerInstanceList.xml");


        /// <summary>
        /// Gets the SQL server instance list.
        /// </summary>
        /// <returns></returns>
        public static List<SqlServerInstance> GetSqlServerInstanceList()
        {
            List<SqlServerInstance> sqlList = new List<SqlServerInstance>();

            // Create default list
            if (File.Exists(TAB_LIST_FILE))
            {
                // Deserialise
                using (var sr = new StreamReader(TAB_LIST_FILE))
                {
                    var deserializer = new XmlSerializer(typeof(List<SqlServerInstance>));
                    sqlList = (List<SqlServerInstance>)deserializer.Deserialize(sr);
                    sr.Close();
                }
            }

            return sqlList;
        }


        /// <summary>
        /// Sets the SQL server instance list.
        /// </summary>
        /// <param name="sqlList">The SQL list.</param>
        public static void SetSqlServerInstanceList(List<SqlServerInstance> sqlList)
        {
            // Serialise
            using (var sw = new StreamWriter(PRELOAD_SERVER))
            {
                var serializer = new XmlSerializer(typeof(List<SqlServerInstance>));
                serializer.Serialize(sw, sqlList);
                sw.Close();
            }
        }


        /// <summary>
        /// Gets the common table list.
        /// </summary>
        /// <returns></returns>
        public static List<CommonTableModel> GetCommonTableList()
        {
            List<CommonTableModel> ctm = new List<CommonTableModel>();

            // Create default list
            if (!File.Exists(TAB_LIST_FILE))
            {
                List<CommonTableModel> defaultCtmList = new List<CommonTableModel>();
                CommonTableModel defaultCtm = new CommonTableModel();
                defaultCtm.Database = "AKPOS";
                defaultCtm.TableList = new List<string>();
                defaultCtm.TableList.AddRange(new String[] { "Config", "S_This", "S_Stations", "Items", "Customers", "TransHeaders", 
                    "TransLines", "TransPayments", "Orders", "OrderLines", "OrderDeliveries", "OrderNotes", "OrderPayments"});
                defaultCtmList.Add(defaultCtm);

                SetCommonTableList(defaultCtmList);

                ctm = defaultCtmList;
            }
            else
            {
                // Deserialise
                using (var sr = new StreamReader(TAB_LIST_FILE))
                {
                    var deserializer = new XmlSerializer(typeof(List<CommonTableModel>));
                    ctm = (List<CommonTableModel>)deserializer.Deserialize(sr);
                    sr.Close();
                }
            }

            return ctm;
        }

        /// <summary>
        /// Sets the common table list.
        /// </summary>
        /// <param name="ctm">The CTM.</param>
        public static void SetCommonTableList(List<CommonTableModel> ctm)
        {
            // Serialise
            using (var sw = new StreamWriter(TAB_LIST_FILE))
            {
                var serializer = new XmlSerializer(typeof(List<CommonTableModel>));
                serializer.Serialize(sw, ctm);
                sw.Close();
            }
        }
    }
}
