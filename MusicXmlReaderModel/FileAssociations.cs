using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.Policy;
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
                    if (null == o)
                    {
                        result = string.Format("  Key {0} ValueName={1} DOES NOT EXIST!", key.Name, valueName);
                    }
                    else
                    {
                        result = string.Format("  Key {0} ValueName={1}  Value={2}", key.Name, valueName, o.ToString());
                    }
                }
                Logger.Log(result);
            }
            catch (Exception e)
            { 
                Logger.LogCFE(e);   
            }
            if (null != key) key.Close();   
            return result;
        }

        public List<string> LogAssociationInformation(string extension, List<string> progIds)
        {
            List<string> result = new List<string>();
            result.Add("Extension= " + extension+ ":");
            result.AddRange(LogExtensionInformation(extension));
            foreach (string progId in progIds)
            {
                result.Add("ProgId= " + progId + ":");
                result.AddRange(LogProgIdInformation(progId));
            }
            return result;
        }

        public List<string> LogExtensionInformation(string extension)
        {
            Logger.LogCF(string.Format("({0})+", extension));
            List<string> result = new List<string>();
            string keyName = null;
            try // Be sure to leave all registry keys closed !
            {
                keyName = "Software\\Classes\\" + extension ; // Typically HKCU\Software\\Classes\.musicxml
                result.Add(LogRegistryInformation(keyName,""));

                keyName = ExplorerFileExtsKeyName + extension + "\\" + UserChoiseSubKeyName;
                result.Add(LogRegistryInformation(keyName,""));  
            }
            catch (Exception e)
            {
                Logger.LogCFE(e);
                result.Add("The operation failed witn an exception. Please see LogFile");
            }
            Logger.LogCF("()-");
            return result;
        }

        public List<string> LogProgIdInformation(string progId)
        {
            Logger.LogCF(string.Format("({0})+", progId));
            List<string> result = new List<string>();
            try // Be sure to leave all registry keys closed !
            {
                string keyName = @"Software\Classes\" + progId + @"\shell\open\command";
                result.Add(LogRegistryInformation(keyName,""));
            }
            catch (Exception e)
            {
                Logger.LogCFE(e);
                result.Add("The operation failed witn an exception. Please see LogFile");
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
        public void RegisterForFileExtension(string extension, string progId, string applicationPath)
        {
            RegistryKey key = null;
            RegistryKey progIdKey = null;
            RegistryKey commandKey = null;
            try // Be sure to close all keys
            {
                // AI - generated by Bing:
                // Create a new key for the file extension
                key = Registry.CurrentUser.CreateSubKey($"Software\\Classes\\{extension}");
                key.SetValue("", progId);

                // Create a new key for the ProgID
                progIdKey = Registry.CurrentUser.CreateSubKey($"Software\\Classes\\{progId}");
                progIdKey.SetValue("", ""); // Could be used for supplementary information

                // Create a new key for the application
                commandKey = progIdKey.CreateSubKey(@"shell\open\command");
                commandKey.SetValue("", $"\"{applicationPath}\" \"%1\"");
            }
            catch (Exception e)
            {
                Logger.LogCFE(e);
            }
            if (null != key) key.Close() ;
            if (null != progIdKey) progIdKey.Close();
            if (null != commandKey) commandKey.Close();

            ChangeExplorerAssociation(extension,progId); // See comment below

            NativeMethods.ShellChangeNotify();          
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
        ///  JSJ: The Explorer also needs a manual refresh (For instance by pressing F5) afterwards !!!!!!!!!!!!!
        /// </summary>
        /// <param name="Extension"></param>
        public void ChangeExplorerAssociation(string Extension,string progIdValue)
        {       
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
                    key.DeleteSubKeyTree(Extension, false); // Delete recursively. (Typically the .musicxml key)
                }
                newKey = key.CreateSubKey(Extension);
                // Create a new subkey for UserChoise
                newSubKey = key.CreateSubKey(UserChoiseSubKeyName); // "UserChoice";
                newSubKey.SetValue("ProgId", progIdValue); // Create a valuepair: {Name="prigId" , Value=<progIdValue>} where <progIdValue is for instance "IBOS MusicXmlReader">                
                
            }
            catch (Exception e)
            {
                Logger.LogCFE(e);
            }
            if (null != key) key.Close();
            if (null != newKey) newKey.Close();
            if (null != newSubKey) newSubKey.Close();
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
