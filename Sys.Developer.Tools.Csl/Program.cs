using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;

namespace Sys.Developer.Tools.Csl
{
    class Program
    {
        private const string OUTPUT_FILENAME = "definitions.txt";
        private const string DEV_DIR = @"C:\dev\generic\Delphi_Reports";
        private const string DLL_NAME = "*.dpr";

        public static readonly int myVal;
        static void Main(string[] args)
        {

            var i = 1;
            for (int j = 1; j <= 10; j += 5)
            {
                i += j;
            }
            Console.WriteLine(i);

            Console.ReadLine();
            return;
            object o = null;
            string[][] val = { new string[] {"1"} };
            
            try
            {
                Dictionary<int, string> vals = new Dictionary<int, string>();
                vals[1] = "test";
                Console.WriteLine(vals[1]);
                Console.WriteLine(vals[2]);

                //int? i = (int?)o;
                //int i2 = i ?? 0;
                //Console.WriteLine(i2);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetType());
            }
            Console.WriteLine("Done");

            Console.ReadLine();
            return;
           // myVal = 1;
             string[] definitionFiles = Directory.GetFiles(DEV_DIR, OUTPUT_FILENAME, SearchOption.AllDirectories);

            if (definitionFiles != null)
            {
                for (int xx = 0; xx < definitionFiles.Count(); xx++)
                {
                    string fileName = definitionFiles.GetValue(xx).ToString();
                    foreach (string line in File.ReadLines(fileName))
                    {
                        string dllName = Path.GetFileNameWithoutExtension(Directory.GetFiles(Path.GetDirectoryName(fileName), DLL_NAME).GetValue(0).ToString());
                        if (line.Contains("N:"))
                        {
                            Console.WriteLine(String.Format("{0} - {1}", dllName, line.Remove(line.Length - 2).Substring(line.IndexOf("N:") + 3)));
                        }
                    }
                }
            }

            Console.ReadLine();

            return;
            string dir = "C:\\Data\\DocumentAttachment\\";
          
            
            if (Directory.Exists(dir))
            {
                Console.WriteLine(String.Format("{0} directory does exist.", dir));
            }
            else
            {
                Console.WriteLine(String.Format("{0} directory doesn't exist.", dir));
            }

            Console.ReadLine();
            return;
            string testStr = null;
            string[] testArr = new string[1];
            //testArr[0] = testStr.ToString();

            string newTestStr = testArr[0].ToString() ?? "*";

            Console.WriteLine(String.Format("Customer: {0}", newTestStr));
            Console.ReadLine();
        }

        public void RemoveJim(IList Names)
        {
            foreach (var name in Names)
            {
                /*if (name == "Jim")
                {
                    Names.Remove("Jim");
                }*/
            }
        }
    }
}
