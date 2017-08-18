using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSheetBuilder
{
    class Program
    {
        const string className = "Program";
        const string prologFileName = "prolog.xml";
        const string epilogFileName = "epilog.xml";
        const string testSheetFileName = "TestSheet.xml";



        static void Main(string[] args)
        {
            const string functionName = "Main";
            Console.WriteLine(string.Format("{0}.{1}:Starting",className,functionName));    
            string prolog = System.IO.File.ReadAllText(prologFileName);
            Console.WriteLine(string.Format("{0}.{1}: Read {2} bytes from {3}", className, functionName, prolog.Length, prologFileName));
            string epilog = System.IO.File.ReadAllText(epilogFileName);
            Console.WriteLine(string.Format("{0}.{1}: Read {2} bytes from {3}", className, functionName, epilog.Length, epilogFileName));
            string testSheet = prolog;
            System.IO.File.WriteAllText("TestSheet.xml", prolog);
            Console.WriteLine(string.Format("{0}.{1}: wrote {2} bytes to {3}", className, functionName, testSheet.Length, testSheetFileName));
            Console.WriteLine("Press any key to exit");
            Console.Read();
            Console.WriteLine(string.Format("{0}.{1}:Exiting", className, functionName));
        }
    }
}
