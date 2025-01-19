using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Policy;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using NAudio.Gui;
using PlatformDependencies;
using static MusicXmlReaderModel.BrailleBuilderForMusic;

namespace MusicXmlReaderModel
{
    /// <summary>
    /// Class for setting up filetype associations allowing the end user to open the MusicXmlReader applicationby clicking ant .musicxml file.
    /// Note that these operations do NOT require administrative rights!
    /// The following information assumes that the following parameters are used:
    ///  extension=         ".musicxml" 
    ///  progId=            "IBOS MusicXmlReader" 
    ///  applicationPath =  "C:\Program Files (x86)\IBOS MusicXmlReader\IBOS MusicXmlReader.exe"
    ///  
    /// Manipulates and inspects a number of registry keys, all subkeys to HKEY_CURRENT_USER\SOFTWARE:
    /// 
    ///  \Classes:                              Creates subkey ".musicxml\shell\open\command" if not already found.
    ///  \Classes\.musicxml:                    Changes standard value to "IBOS MusicXmlReader"
    ///  \Classes\.musicxml\shell\open\command: Changes standard value to "C:\Program Files (x86)\IBOS MusicXmlReader\IBOS MusicXmlReader.exe" "%1"
    ///  
    ///  \Classes:                                          Creates subkey "IBOS_MusicXmlReader\shell\open\command" if not already found
    ///  \Classes\IBOS_MusicXmlReader:                      Changes standard value to ""
    ///  \Classes\IBOS_MusicXmlReader\shell\open\command:   Changes standard value to "C:\Program Files (x86)\IBOS MusicXmlReader\IBOS MusicXmlReader.exe" "%1"
    ///  
    ///  \Microsoft\Windows\CurrentVersion\Explorer\FileExts\.musicxml: Deletes subkey "UserChoise"   
    /// 
    /// The changes below HKEY_CURRENT_USER\SOFTWARE\Classes will map the ".musicxml" extension to progId="IBOS_MusicXmlReader" and to the default installation path for MusicXmlReader.exe.
    /// The changes below HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.musicxml will force Windows Explorer to recalculate the association for ".musicxml" files.
    /// 
    /// After each change NativeMethods.ShellChangeNotify() is called in order to notify the shall that changes have been made.
    /// </summary>
    internal class FileAssociations
    {

      
        private  string LogRegistryInformation(string keyName,string valueName)
        {
            string result = "";
            RegistryKey key = null;
            try
            {
                key = Registry.CurrentUser.OpenSubKey(keyName);
                if (null == key)
                {
                    result = string.Format(@" {0} DOES NOT EXIST!", PadRight100( @"HKEY_CURRENT_USER\" + keyName)); // Add "HKEY_CURRENT_USER" because it is implic to Registry.CurrentUser.OpenSubKey()
                }
                else
                {
                    object o = key.GetValue(valueName);
                    string value = (null == o) ? "DOES NOT EXIST!" : string.Format("Value={0}", o.ToString());
                    result = string.Format(@" {0} ValueName={1} {2}", PadRight100(key.Name), PadRight10(valueName),value);                 
                }
                Logger.Log(result);
            }
            catch (Exception e)
            { 
                Logger.LogCFE(e);
                result = "The operation failed witn an exception. Please see LogFile";
            }
            if (null != key) key.Close();   
            return result;
        }

        /// <summary>
        /// Simple mechanism for aligning multiline occurances, Will not always give the nicest possible result, but is simple !
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private string PadRight100(string s)
        {
            return string.Format("{0,-100}", s);
        }

        private string PadRight10(string s)
        {
            return string.Format("{0,-10}", s);
        }


        public List<string> LogAssociationInformation(string extension, List<string> progIds)
        {
            List<string> result = new List<string>();
            //result.Add("Extension= " + extension+ ":");
            result.Add(LogRegistryInformation(GetSoftwareClassesKeyName(extension),""));
            foreach (string progId in progIds)
            {
                // result.Add("ProgId= " + progId + ":");
                result.Add(GetProgIdInformation(progId));
            }

            result.Add(LogRegistryInformation(GetExplorerFileExtsUserChoiseKeyName(extension), progIdValueName));
            result.Add(LogRegistryInformation(GetExplorerFileAppPathKeyName("MuseScore4stable.exe"), ""));
            result.Add(LogRegistryInformation(GetExplorerFileAppPathKeyName("MuseScore Studio 4 stable.exe"), ""));
            return result;
        }

        /// <summary>
        /// Return the name of the Registry key generally used for looking up the ProgId for the specified extension
        /// (Typically HKCU\Software\\Classes\.musicxml)
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        private string GetSoftwareClassesKeyName(string extension)
        { 
            return @"Software\Classes\" + extension;
        }

        /// <summary>
        /// Return the name of the Registry key used by Windows Explorer for looking up the ProgId chosen by the user for the specified extension
        /// (Typically "Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.musicxml\UserChoise")
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        private string GetExplorerFileExtsUserChoiseKeyName(string extension)
        {
            return @"Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\" + extension + @"\UserChoice";
        }
        private string GetExplorerFileAppPathKeyName(string programName)
        {
            return @"Software\Microsoft\Windows\CurrentVersion\App Paths\" + programName;
        }

        /// <summary>
        ///  Return the name of the Registry key generally used for looking up the execution-command for the specified progId
        /// (For instance "Software\Classes\MuseScore Studio\shell\open\command"
        /// </summary>
        /// <param name="progId"></param>
        /// <returns></returns>
        private string GetSoftwareClassesProgIdShellOpenCommandKeyName(string progId)
        {
            return @"Software\Classes\" + progId + @"\shell\open\command";
        }

        private void CloseOpenKeys(List<RegistryKey> keys)
        {
            foreach (RegistryKey key in keys)
            {
                if (null != key) key.Close();
            }    
        }

        private const string exceptionMessage = "The operation failed with an exception. Please see LogFile";

        private const string progIdValueName = "ProgId";  

        public string GetProgIdInformation(string progId)
        {
            //Logger.LogCF(string.Format("({0})+", progId));
            string result;
            try // Be sure to leave all registry keys closed !
            {          
                string keyName = GetSoftwareClassesProgIdShellOpenCommandKeyName(progId);
                result=LogRegistryInformation(keyName,"");
            }
            catch (Exception e)
            {
                Logger.LogCFE(e);
                result = exceptionMessage;
            }
            //Logger.LogCF(": " + result);
            //Logger.LogCF("()-");
            return result;
        }


        /// <summary>
        /// Associate the extension with the application
        /// </summary>
        /// <param name="extension"></param>
        /// <param name="applicationPath"></param>
        /// <returns></returns>
        public bool ChangeGlobalAssociation(string extension, string progId, string applicationPath)
        {
            bool result = false;
            RegistryKey key = null;
            RegistryKey progIdKey = null;
            RegistryKey commandKey = null;
            try // Be sure to close all keys
            {
                string keyName;
                // AI - generated by Bing:
                // Create a new key for the file extension
                keyName = GetSoftwareClassesKeyName(extension);
                key = Registry.CurrentUser.CreateSubKey(keyName); // Create or open an existing key for write access
                key.SetValue("", progId);

                // Create a new key for the ProgID
                keyName = GetSoftwareClassesKeyName(progId);
                progIdKey = Registry.CurrentUser.CreateSubKey(keyName); // Create or open an existing subkey for write access
                progIdKey.SetValue("", ""); // Could be used for supplementary information

                // Create a new key for the application
                commandKey = progIdKey.CreateSubKey(@"shell\open\command");          
                string value = "\""+applicationPath+"\" \"%1\""; // For instance:  "C:\Program Files\MuseScore 4\bin\MuseScore4.exe" "%1"
                commandKey.SetValue("",value);
                result = true;

            }
            catch (Exception e)
            {
                Logger.LogCFE(e);
            }

            CloseOpenKeys(new List<RegistryKey> { key, progIdKey, commandKey }); 
 
            return result;
        }



        /// <summary>
        /// Modifies the access rules for the specified subkey in order to make it writable for the current user, even if the useer is not an administrator
        /// Returns true iff it was possible to modify the subkey.
        /// Assumes that neither parameter is null.
        /// Inspired by
        /// https://stackoverflow.com/questions/6108128/remove-a-deny-rule-permission-from-the-userchoice-key-in-the-registry-via/41290208#41290208
        /// </summary>
        /// <param name="key"></param>
        /// <param name="subKeyName"></param>
        /// <returns></returns>
        private bool ModifyAccessRules(RegistryKey key, string subKeyName)
        {    
            RegistryKey subKey = null;
            bool result = true;
            try
            {
                subKey = key.OpenSubKey(subKeyName, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.ChangePermissions);
                if (subKey == null) { return false; } // Failed to modify access rules

                string upperCaseUserName = WindowsIdentity.GetCurrent().Name.ToUpper(); 
                RegistrySecurity security = subKey.GetAccessControl();
                AuthorizationRuleCollection accRules = security.GetAccessRules(true, true, typeof(NTAccount));
                foreach (RegistryAccessRule ar in accRules)
                {
                    if (0 == string.Compare(ar.IdentityReference.Value.ToUpper(), upperCaseUserName)) // Be sure the case matches!
                    {
                        if (ar.AccessControlType == AccessControlType.Deny)
                        {
                            security.RemoveAccessRuleSpecific(ar); // remove the 'Deny' permission
                        }
                    }
                }
                subKey.SetAccessControl(security); // restore all original permissions *except* for the 'Deny' permission
            }
            catch (Exception e)
            {              
                Logger.LogCFE(e); // Write details to the logfile
                result = false; 
            }
            if (null != subKey) subKey.Close();   
            return result;
        }




        /// <summary>
        /// When this key was set up by Windows Explorer->OpenWith->Choose another App->Always it can not be removed by normal means.
        /// Thie mechanism overcomes this.
        /// Inspired by
        /// https://stackoverflow.com/questions/6108128/remove-a-deny-rule-permission-from-the-userchoice-key-in-the-registry-via/41290208#41290208
        /// </summary>
        /// <param name="key"></param>
        private bool DeleteSubKey(RegistryKey key,string subKeyName)
        {
            if (!key.GetSubKeyNames().Contains(subKeyName)) { return true; } // The sudkey does not exist. Nothing to delete.
            ModifyAccessRules(key, subKeyName);
            try
            {
                key.DeleteSubKeyTree(subKeyName, true);
            }
            catch (Exception e)
            {
                Logger.LogCFE(e); // Write details to the logfile
            }
            string[] subKeys = key.GetSubKeyNames();
            bool wasDeleted = !subKeys.Contains(subKeyName);
            string message =  string.Format("{0} SubKey={1} within Key={2}",  wasDeleted ? "Deleted" : "Failed to delete", subKeyName, key.Name);
            Logger.LogCF(": " + message);
            return wasDeleted;
        }


        //// Older method
        ////  // c# - Associate File Extension with Application - Stack Overflow
        //RegistryKey FileReg = Registry.CurrentUser.CreateSubKey("Software\\Classes\\" + extension);
        //FileReg.CreateSubKey("shell\\open\\command").SetValue("", $"\"{applicationPath}\" \"%1\"");
        //FileReg.Close();


        /// <summary>
        /// The place where Windows Explorer casches information about User expensin preferences!
        /// </summary>
        private const string ExplorerFileExtsKeyName = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\";
        private const string UserChoiseSubKeyName = "UserChoice";

        /// <summary>
        /// Another method inspired by 
        ///  https://stackoverflow.com/questions/2681878/associate-file-extension-with-application/2697804#2697804
        /// where the author says:
        /// "The answer was a lot simpler than I expected.
        ///  Windows Explorer has its own override for the open with application, and I was trying to modify it in the last lines of code.
        ///  If you just delete the Explorer override, then the file association will work.
        ///  I also told explorer that I had changed a file association by calling the unmanaged function"
        /// </summary>
        /// <param name="Extension"></param>
        public bool ChangeExplorerAssociation(string Extension,string progIdValue, string applicationPath)
        {
            bool result = true;
            RegistryKey key = null;
            RegistryKey newKey = null;
            RegistryKey newSubKey = null; // UserChoise
            RegistryKey pathKey = null;
            RegistryKey newPathKey = null;
            try
            {
                // The key will typically be: "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts\\.musicXml"
                // Delete the key instead of trying to change it
                key = Registry.CurrentUser.OpenSubKey(ExplorerFileExtsKeyName, true);
                if (null != key)
                {
                    RegistryKey extensionKey = key.OpenSubKey(Extension, true);
                    if (null != extensionKey)
                    {
                        string[] subkeyNames = extensionKey.GetSubKeyNames();
                        foreach (string subkeyName in subkeyNames)
                        {
                            this.DeleteSubKey(extensionKey, subkeyName  ); // Typically: UserChoise, OpenWithList, OpenWithProgids
                        }
                    }
                }
#if true
                // The following code is inspired by looking at
                // "Computer\HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\App Paths\MuseScore4stable.exe" where info about MuseScore4 seems to be stored 
                // The new key actually does not seem not to be needed. Deleting the old one will suffice ! But we create it anyway !
                newKey = key.CreateSubKey(Extension);
                // Create a new subkey "progIdValue"  for UserChoise
                newSubKey = newKey.CreateSubKey(UserChoiseSubKeyName); // "UserChoice";
                newSubKey.SetValue(progIdValueName, progIdValue); // Create a valuepair: {Name="progId" , Value=<progIdValue>} where <progIdValue is for instance "IBOS MusicXmlReader">

                // Create a subKey "progIdValue" under "Computer\HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\App Paths" and fill it in
                pathKey = Registry.CurrentUser.OpenSubKey(ExplorerAppPathsKeyName, true);
                newPathKey = pathKey.CreateSubKey(progIdValue);
                newPathKey.SetValue("",applicationPath);
                newPathKey.SetValue(PathValueName, Path.GetDirectoryName(applicationPath));
#endif
            }
            catch (Exception e)
            {
                Logger.LogCFE(e);
                result = false;
            }
            CloseOpenKeys(new List<RegistryKey>() { key, newKey, newSubKey, pathKey, newPathKey });
            return result;
        }

        const string ExplorerAppPathsKeyName = "Software\\Microsoft\\Windows\\CurrentVersion\\App Paths";
        const string PathValueName = "Path";

        //***********************************************************************************************************************************************
        //
        // The above code assumes that Windows Explorer works in the following way:
        //
        // 1: Read HKCU\Software\Microsoft\Windows\CurrentVersion\Explorer\FileExts\.musicxml\UserChoise\ProgId for instance "MuseScore Studio"
        // 2: Read HKCU\Software\Classes\MuseScore Studio\shell\open\command                                    for instance "C:\Program Files\MuseScore4\bin\MuseScore4.exe" "%1"
        // 3: Run the latest command
        //
        //***************************************************************************************************************************************************


        private FileAssociations()
        { }


        public static FileAssociations Create()
        {
            return new FileAssociations();
        }

    }
}
