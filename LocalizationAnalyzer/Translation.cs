using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace LocalizationAnalyzer
{
    class Translation
    {
        [Flags]
        public enum LogOptions { LogKeyValuePairs = 1, LogIgnoredMethods = 2 }

        private static void Log(string s)
        {
            LocalizationLogger.Log(s);
        }


        public static Translation Create(MemberInfo memberInfo, Counters counters, LogOptions logOptions)
        {
            Translation result = null;

            if (memberInfo is MethodInfo)
            {
                MethodInfo methodInfo = memberInfo as MethodInfo;

                string s = "";
                if ((methodInfo.ReturnType.Name == "String") && (methodInfo.Name.StartsWith("get_")))
                {
                    // Log("Translation Create: +methodInfo.Invoke(null, null)"); // For simple debugging 
                    object res = methodInfo.Invoke(null, null); // The first parameter is null because we call a static method and need no instance reference!
                    // Log("Translation Create: -methodInfo.Invoke(null, null)"); // For simple debugging 
                    s = (res == null) ? "" : res.ToString();
                    if (0 != (logOptions & LogOptions.LogKeyValuePairs))
                    {
                        Log(string.Format("      {0,-60} '{1}'", methodInfo.Name, s));
                    }
                    counters.localizationMethods++;
                    result = Translation.Create(methodInfo, s);
                }
                else
                {
                    if (0 != (logOptions & LogOptions.LogIgnoredMethods))
                    {
                        Log(string.Format("  -->> Ignored Method with Name='{0}' ReturnTypeName='{1}'", methodInfo.Name, methodInfo.ReturnType.Name));
                    }
                    counters.otherMethods++;
                }

            }
            else
            {
                string memberTypeName = memberInfo.GetType().ToString();
                //Log(string.Format("-->Member.Type={0}",memberTypeName));

                switch (memberTypeName)
                {
                    case "System.Reflection.RuntimePropertyInfo": counters.runtimeProtertyInfo++; break;
                    case "System.Reflection.RtFieldInfo":
                        counters.rtFieldInfo++; break;
#warning Find out why unreachable !!
                        counters.otherMembers++;
                }


            }
            return result;
        }




        public static Translation Create(System.Reflection.MemberInfo methodInfo, string translatedString)
        {
            return new Translation(methodInfo, translatedString);
        }
        private Translation() { }
        private Translation(System.Reflection.MemberInfo methodInfo, string translatedString)
        {
            this.methodInfo = methodInfo;
            this.translatedString = translatedString;
        }
        private System.Reflection.MemberInfo methodInfo;
        public System.Reflection.MemberInfo MethodInfo { get { return methodInfo; } }
        private string translatedString;
        public string TransatedString { get { return translatedString; } }
    }
}
