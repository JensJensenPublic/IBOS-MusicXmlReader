using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

namespace LocalizationAnalyzer
{
    class Experiment
    {

        public static void Execute(Assembly assembly,TypeId typeId)
        {
            return;

            string nameSpace = Path.GetFileNameWithoutExtension(typeId.AssemblyName);
            // Get the "LocalisationAnalysis" type 
            Type localizationAnalysisType = assembly.GetType(nameSpace + "." + "LocalizationAnalysis");
            // Get the Public GetLocalizationClass
            MethodInfo methodInfo = localizationAnalysisType.GetMethod("GetLocalizationClass");
            object o = methodInfo.Invoke(localizationAnalysisType, null);
            object o1 = methodInfo.Invoke(null, null);

            




            MethodInfo[] allMethodInfos = localizationAnalysisType.GetMethods(BindingFlags.NonPublic | BindingFlags.Static);
            MethodInfo[] allMethodInfos1 = localizationAnalysisType.GetMethods(BindingFlags.Static);
            Type t = o.GetType();
            //MethodInfo[] methodInfos = 
            //foreach ()

        }
    }
}
