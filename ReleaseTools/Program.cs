using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReleaseTools
{
    class Program
    {
        static readonly string value = "modstridende kopi";
        static List<string> directories = new List<string>();
        static int totalCount = 0;
        //string basePath = @"C:\Users\Jens\Dropbox\Root\Visual Studio 2015\Projects\MusicXmlReaderUI";  
        static string basePath = @"C:\Users\Jens\Dropbox\Root\Visual Studio 2015\Projects\MusicXmlReaderUI\SpreadsheetGenerator";


        static void Main(string[] args)
        {
            Console.WriteLine(string.Format("BasePath={0}", basePath));
            Recurse(basePath);
            Console.WriteLine(string.Format("{0} redundant files found in {1}", totalCount, basePath));
        }

        static  private void Recurse(string path)
        {
            string[] fileNames = System.IO.Directory.GetFiles(path);
            List<string> redundantFiles = new List<string>();
            foreach (string fileName in fileNames)
            {
                if (fileName.Contains(value))
                {
                    redundantFiles.Add(fileName);
                }
            }

            int count = redundantFiles.Count();
            if (0 != count)
            {
                Console.WriteLine(string.Format("{0,3} redundant files found in {1}", count,path));
                totalCount += count;
                //foreach (string file in redundantFiles)
                //{
                //    Console.WriteLine(string.Format("  File={0}", file));
                //}
            }

            string[] directories = System.IO.Directory.GetDirectories(path);
            foreach (string directory in directories)
            {
                Recurse(directory);
            }
        }
    }
}
