using System;
using System.Collections.Generic;
using System.Dynamic;
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
                    result = string.Format("  Key{0} DOES NOT EXIST!", keyName);
                }
                else
                {
                    object o = key.GetValue(valueName);
                    string value = (null == o) ? "DOES NOT EXIST!" : string.Format("Value='{0}", o.ToString());
                    result = string.Format("  {0} ValueName='{1}' {2}", key.Name, valueName,value);                 
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

        public List<string> LogAssociationInformation(string extension, List<string> progIds)
        {
            List<string> result = new List<string>();
            //result.Add("Extension= " + extension+ ":");
            result.Add(LogRegistryInformation(GetSoftwareClassesKeyName(extension),""));
            foreach (string progId in progIds)
            {
                // result.Add("ProgId= " + progId + ":");
                result.AddRange(LogProgIdInformation(progId));
            }

            result.Add(LogRegistryInformation(GetExplorerFileExtsUserChoiseKeyName(extension), progIdValueName));
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

        public List<string> LogProgIdInformation(string progId)
        {
            Logger.LogCF(string.Format("({0})+", progId));
            List<string> result = new List<string>();
            try // Be sure to leave all registry keys closed !
            {          
                string keyName = GetSoftwareClassesProgIdShellOpenCommandKeyName(progId);
                result.Add(LogRegistryInformation(keyName,""));
            }
            catch (Exception e)
            {
                Logger.LogCFE(e);
                result.Add(exceptionMessage);
            }
            Logger.LogCF("()-");
            return result;
        }


        /// <summary>
        /// Associate the extension with the application
        /// </summary>
        /// <param name="extension"></param>
        /// <param name="applicationPath"></param>
        /// <returns></returns>
        public bool RegisterForFileExtension(string extension, string progId, string applicationPath)
        {
            bool classesOK = true;
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

            }
            catch (Exception e)
            {
                Logger.LogCFE(e);
                classesOK = false;
            }

            CloseOpenKeys(new List<RegistryKey> { key, progIdKey, commandKey });

            bool explorerOK = ChangeExplorerAssociation(extension,progId); // See comment below

            NativeMethods.ShellChangeNotify();
            return (classesOK && explorerOK);
        }


        /// <summary>
        /// When this key was set up by Windows Explorer->OpenWith->Choose another App->Always it can not be removed by normal means.
        /// Thie mechanism overcomes this.
        /// Inspired by
        /// https://stackoverflow.com/questions/6108128/remove-a-deny-rule-permission-from-the-userchoice-key-in-the-registry-via/41290208#41290208
        /// </summary>
        /// <param name="extensionKey"></param>
        private void DeleteUserChoiceKey(RegistryKey extensionKey)
        {
            const string userChoiceKeyName = "UserChoice";

            using (RegistryKey userChoiceKey =
                extensionKey.OpenSubKey(userChoiceKeyName,
                    RegistryKeyPermissionCheck.ReadWriteSubTree,
                    RegistryRights.ChangePermissions))
            {
                if (userChoiceKey == null) { return; }
                string userName = WindowsIdentity.GetCurrent().Name;
                RegistrySecurity security = userChoiceKey.GetAccessControl();

                AuthorizationRuleCollection accRules =
                    security.GetAccessRules(true, true, typeof(NTAccount));

                foreach (RegistryAccessRule ar in accRules)
                {
                    if (0 == string.Compare(ar.IdentityReference.Value.ToLower(), userName.ToLower()))
                    {
                        if (ar.AccessControlType == AccessControlType.Deny)
                        {
                            security.RemoveAccessRuleSpecific(ar); // remove the 'Deny' permission
                        }
                    }
                }

                userChoiceKey.SetAccessControl(security); // restore all original permissions
                                                          // *except* for the 'Deny' permission
            }

            extensionKey.DeleteSubKeyTree(userChoiceKeyName, true);
            
            Logger.LogCF(string.Format(": Deleted {0}", extensionKey.Name));
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
        public bool ChangeExplorerAssociation(string Extension,string progIdValue)
        {
            bool result = true;
            RegistryKey key = null;
            RegistryKey newKey = null;
            RegistryKey newSubKey = null; // UserChoise
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
                        this.DeleteUserChoiceKey(extensionKey);                
                    }
                }
                newKey = key.CreateSubKey(Extension);
                // Create a new subkey for UserChoise
                newSubKey = newKey.CreateSubKey(UserChoiseSubKeyName); // "UserChoice";
                newSubKey.SetValue(progIdValueName, progIdValue); // Create a valuepair: {Name="progId" , Value=<progIdValue>} where <progIdValue is for instance "IBOS MusicXmlReader">              
        
            }
            catch (Exception e)
            {
                Logger.LogCFE(e);
                result = false;
            }
            CloseOpenKeys(new List<RegistryKey>() { key, newKey, newSubKey });
            return result;
        }

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
