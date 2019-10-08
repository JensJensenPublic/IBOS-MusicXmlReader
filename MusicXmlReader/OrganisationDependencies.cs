using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicXmlReaderModel;

namespace MusicXmlReader
{

    /// <summary>
    /// By using this class we can change selected string variables exposed to the user via the UI
    /// (from for instance the main form and all error message boxes) by just changing the name of the executing assembly!
    /// Change MusicXmlReader.Properties.AssemblyName to change the name of the executing assembly.
    /// This will be reflected by the Application name and the link to new software. 
    /// </summary>
    public class OrganisationDependencies
    {
        private const string TactileMusicXmlReader = "Tactile MusicXmlReader"; // Is compared to the name of the .exe file

        private string linkToNewestSoftware = ResourcesForUI.ToolStripMenuItem_Help_SoftwareUpdateLink; // Set up a default
        private string applicationName = ResourcesForUI.MainForm_ApplicationName; // Set up a default
        // Define more private members here as needed

        public string LinkToNewestSoftware { get { return linkToNewestSoftware; } }
        public string ApplicationName { get { return applicationName; } }

        /// <summary>
        /// Prevent construction
        /// </summary>
        private OrganisationDependencies()
        { }

        private OrganisationDependencies(string executingAssembly)
        {
            switch (executingAssembly)
            {
                case TactileMusicXmlReader:
                    applicationName  = executingAssembly; // Bring your own resource file if you want !
                    linkToNewestSoftware = ""; // Bring your own link if you want !
                    Logger.LogCF(string.Format(": ApplicationName='{0}'  LinkToNewestSoftware='{1}' ", applicationName, linkToNewestSoftware));
                    break;
                default:
                    break; // Keep the default values.
            }
        }


        static public OrganisationDependencies Create(string executingAssembly)
        {
            return new OrganisationDependencies(executingAssembly);
        }


    }
}
