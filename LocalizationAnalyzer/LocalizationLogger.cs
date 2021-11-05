using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace LocalizationAnalyzer
{


    /// <summary>
    /// This class is intensionally named LocalizationLogger in order to show that it does not have the same functionality as the MusicXmlModel.Logger class:
    /// LocalizationAnalyzer project is designed to be used within or without the MusicXmlReader solution  so we need to be independent of MusicXmlModel.Logger
    /// </summary>
    public class LocalizationLogger
    {
        static private bool useConsole = true;
        static public bool UseConsole { get { return useConsole; } set { useConsole = value;  } } 
        static private List<string> GlobalLog = new List<string>();

        static public void Init()
        {
            GlobalLog = new List<string>();
        }

        static public void Log(string s)
        {
            if (useConsole) Console.WriteLine(s);
            GlobalLog.Add(s);
        }

        static public void Log(List<string> strings)
        {
            foreach (string s in strings)
            {
                Log(s);
            }
        }

        /// <summary>
        /// Append id of method calling LogCF() before logging
        /// Intensionally avoid using Logger.LogCF because this assembly is designed to be used outside the normal MusicXmlReader environment.
        /// </summary>
        /// <param name="s"></param>
        static public void LogCF(string s)
        {
            string cf = GetCF(1); // For the method calling LogCF before logging
            Log(string.Format("{0} {1}", cf, s));
        }

        /// <summary>
        /// Append id of method calling method calling LogCF() before logging
        /// </summary>
        /// <param name="s"></param>
        static public void LogCF1(string s)
        {
            string cf = GetCF(2); // For the method calling the method calling LogCF before logging
            Log(string.Format("{0} {1}", cf, s));
        }

        /// <summary>
        /// if (levelsUp <= 0) Returns ClassName.MethodName for the method calling GetCF()
        /// if (levelsup > 0) returns ClassName.MethodName for the methon levelsUp levels up the call chain
        /// </summary>
        /// <param name="levelsUp"></param>
        /// <returns></returns>
        static public string GetCF(int levelsUp)
        {
#warning todo handle levelsUp too large
            int level = 1 + Math.Max(0, levelsUp); // 1 extra for the call to the GetCF method itself !
            System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace();
            MethodBase methodBase = stackTrace.GetFrame(level).GetMethod();
            Type type = methodBase.ReflectedType;
            return string.Format("{0}.{1}", type.Name, methodBase.Name);
        }


        static public void LogAssemblies(Assembly[] assemblies)
        {
            Log("");
            Log("Start Assemblies----------------------------------------------------");
            foreach (Assembly assembly in assemblies)
            {
                Log(string.Format("--> {0}", assembly.FullName));
            }
            Log("End Assemblies------------------------------------------------------");
            Log("");
        }


        static public void WriteToFile(string fileName)
        {
            System.IO.File.WriteAllLines(fileName, GlobalLog);
        }


    }
}
