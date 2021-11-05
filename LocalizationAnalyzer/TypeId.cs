using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalizationAnalyzer
{
    /// <summary>
    /// Simple identification of a type through assemblyName  and TypeName
    /// </summary>
    public class TypeId
    {
        private TypeId() { }
        private TypeId(string assemblyName, string typeName, string nameSpaceName)
        {
            this.assemblyName = assemblyName;
            this.typeName = typeName;
            this.nameSpaceName = nameSpaceName;
        }
        public bool Equals(string assemblyName, string typeName)
        {
            if (!this.assemblyName.Equals(assemblyName)) return false;
            if (!this.typeName.Equals(typeName)) return false;
            if (!this.nameSpaceName.Equals(nameSpaceName)) return false;
            return true;
        }
        public static TypeId Create(string assemblyName, string typeName, string nameSpaceName)
        {
            return new TypeId(assemblyName, typeName, nameSpaceName);
        }
        string assemblyName;
        public string AssemblyName { get { return this.assemblyName; } }
        string typeName;
        public string TypeName { get { return this.typeName; } }
        string nameSpaceName;
        public string NameSpaceName { get { return nameSpaceName; } } 
    }
}
