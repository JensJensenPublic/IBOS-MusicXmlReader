using System;
using System.Collections.Generic;
using System.IO;

namespace MusicXmlReaderModel
{

    /// <summary>
    /// System-wide usable utility class for colleting and reporting warnings to be reported through the UI
    /// Client code can
    ///  1) Report an error by calling one of the overloads of LogUserWarning.
    ///  2) Clear the acturl list of warnings by calling ClearUserWarnings()
    ///  3) Dump the user warnings collected by DumpLocalUSerWarnings() and DumpGlobalUserWarnings
    ///  4) Get the current list of warnings by calling GetLocalUSerWarnings() and GetGlobalUserWarnings()
    /// Note that while "LocalUserWarnings" contains warnings reported since last call to ClearLocalUserWarnings(),
    /// "GlobalUSerWarnongs" contains all warnings reported sinca program start.    ///  
    /// </summary>
    public class UserWarnings
    {
        /// <summary>
        /// Log a textual warning. It will be extended with information about filename, index in file, formnumber, linenumber spacenumber etc
        /// This information can later be
        ///  Dumped    (to Logfile) by DumpLocalUserWarnings or DumpGlobalUserWarnings
        ///  Retreived (for UI use) by GetLocalUserWarnings  or GetGlobalUserWarnings
        /// </summary>
        /// <param name="s"></param>
        public static void LogUserWarning(string message,UserInfoEnum userInfoEnum)
        {
            string fileName = GetShortFileName(Logger.CurrentMusicBrailleSourceFileName);
            DecoderUserInfo userInfo = new DecoderUserInfo(fileName, userPositionInfo, message); // When userPositionInfo is not specified rely on the local value (Previously coded as a callback)
            userInfo.UserInfoEnum = userInfoEnum;
            localUserWarnings.Add(userInfo);
            globalUserWarnings.Add(userInfo);
        }


        ///// <summary>
        ///// NEW CODE !!!
        ///// </summary>
        ///// <param name="message"></param>
        ///// <param name="userPositionInfo"></param>
        //public static void LogUserWarning(string message, UserPositionInfo userPositionInfo)
        //{
        //    string fileName = GetShortFileName(Logger.CurrentMusicBrailleSourceFileName);
        //    DecoderUserInfo userInfo = new DecoderUserInfo(fileName, userPositionInfo, message); // When userPosition is explicitly specified, use the explicit value
        //    localUserWarnings.Add(userInfo);
        //    globalUserWarnings.Add(userInfo);
        //}



        /// <summary>
        /// Log a textual warning witn most parameters specified as call parameters
        /// </summary>
        /// <param name="s"></param>
        /// <param name="currentIndex"></param>
        /// <param name="formNumber"></param>
        /// <param name="LineNumber"></param>
        /// <param name="spaceNumber"></param>
        public static void LogUserWarning(string s, UserPositionInfo userPositionInfo, string brailleString, string brailleDotNumbers,UserInfoEnum userInfoEnum)
        {
            DecoderUserInfo userInfo = new DecoderUserInfo("", userPositionInfo, brailleString, brailleDotNumbers);
            userInfo.FileName = GetShortFileName(Logger.CurrentMusicBrailleSourceFileName);
            userInfo.Message = s;
            userInfo.UserInfoEnum = userInfoEnum;
            localUserWarnings.Add(userInfo);
            globalUserWarnings.Add(userInfo);
        }

        private static UserPositionInfo userPositionInfo;
        public UserPositionInfo UserpositionInfo { get { return userPositionInfo; } }
        public static void OnUserPositionChanged(UserPositionInfo info)
        {
            userPositionInfo = info;
        }



        /// <summary>
        /// To be used when the Decoder modifies the Music Braille input file by inserting items
        /// </summary>
        /// <param name="message"></param>
        /// <param name="index"></param>
        public static void LogUserInsertionWarning(string message, string target,int index, string unicodeToInsert, string dotsToInsert)
        {
            UserPositionInfo userPositionInfo =  UserPositionInfo.Create(target, index);
            DecoderUserInsertionInfo userInfo = new DecoderUserInsertionInfo(message, userPositionInfo, unicodeToInsert, dotsToInsert);
            userInfo.FileName = GetShortFileName(Logger.CurrentMusicBrailleSourceFileName);
            localUserWarnings.Add(userInfo);
            globalUserWarnings.Add(userInfo);
        }

        public static void LogUserReplacementWarning(string message, string target, int index, string oldContents, string oldContentsAsDots, string newContents, string newContentsAsDots)
        {
            UserPositionInfo userPositionInfo = UserPositionInfo.Create(target, index);
            DecoderUserReplacementInfo userInfo = new DecoderUserReplacementInfo(message,userPositionInfo, oldContents, oldContentsAsDots, newContents, newContentsAsDots);
            userInfo.FileName = GetShortFileName(Logger.CurrentMusicBrailleSourceFileName);
            localUserWarnings.Add(userInfo);
            globalUserWarnings.Add(userInfo);
        }



        public static void ClearLocalUserWarnings()
        {
            localUserWarnings.Clear();
        }
        private static void DumpUserWarnings(List<UserInfoBase> list)
        {
            Logger.LogCF(":+");
            int maxFileNameLength = 0;
            foreach (UserInfoBase userInfo in list)
            {
                maxFileNameLength = Math.Max(maxFileNameLength, userInfo.FileName.Length);
            }

            foreach (UserInfoBase userInfo in list)
            {
                string userInfoString = userInfo.ToString(maxFileNameLength);
                Logger.Log(string.Format("**{0}", userInfoString));
            }
            Logger.LogCF(string.Format(": Found {0} UserWarnings", list.Count));
            Logger.LogCF(":-");
        }
        public static void DumpLocalUserWarnings() { DumpUserWarnings(localUserWarnings); }
        public static void DumpGlobalUserWarnings() { DumpUserWarnings(globalUserWarnings); }
        public static List<UserInfoBase> GetlocalUSerWarnings() { return localUserWarnings; }
        public static List<UserInfoBase> GetGlobalUSerWarnings() { return globalUserWarnings; }

        private static string GetShortFileName(string fileName)
        {
            try
            {
                return Path.GetFileName(fileName);
            }
            catch (Exception) { return ""; };
        }

        // Application-wide mechanism for collecting messages intended for the enduser (or for debugging)
        private static List<UserInfoBase> localUserWarnings = new List<UserInfoBase>();
        private static List<UserInfoBase> globalUserWarnings = new List<UserInfoBase>();
        
    }


    /// <summary>
    /// For reporting a  position within a Unicode file to the UI, described as Form,Line,Space. and Braille contents
    /// </summary>
    public class UserPositionInfo
    {
        private char brailleAsUnicode;
        public char BrailleAsUnicode { get { return brailleAsUnicode; } }
        private string brailleDotNumbers = "";
        public string BrailleDotNumbers { get { return brailleDotNumbers; }  }

        private int index;
        public int Index { get { return index; } }
        // Use 1-based indexing in the UI:
        int form = 1;
        public int Form{ get{return form;}}
        int line = 1;
        public int Line { get { return line; } }
        int space = 1;
        public int Space { get { return space; } }
        private UserPositionInfo() { } // Prevent construction
        private UserPositionInfo(string target, int index)
        {
            this.index = index;
            this.brailleAsUnicode = target[index];
            //this.brailleDotNumbers = "???";        
            for (int i = 0; (i < index); i++)
            {
                switch (target[i])
                {
                    case '\f': form++; line = 1; space = 1; break; // FormFeed
                    case '\r': line++; space = 1; break; // Return
                    case '\n': space = 1;  break;  ; // LineFeed
                    default: space++; break;
                }
            }
        }


        private UserPositionInfo(int index, int form, int line, int space)
        {
            this.index = index;
            this.form = form;
            this.line = line;
            this.space = space;
        }

        public UserPositionInfo(int index, int form, int line, int space, char brailleAsUnicode, string brailleDotNumbers)
        {
            this.index = index;
            this.form = form;
            this.line = line;
            this.space = space;
            this.brailleAsUnicode = brailleAsUnicode;
            this.brailleDotNumbers = brailleDotNumbers;
        }

        public override string ToString()
        {
            return  string.Format("Index={0,-4} Page={1,-2}  Line={2,-3}  Space={3,-3}", index, form, line, space);
        }


        /// <summary>
        /// To be used when only index and target are known.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static UserPositionInfo Create(string target, int index)
        {
            return new UserPositionInfo(target, index);
        }

        /// <summary>
        /// To be used when explicit values of all 4 position parameters are know, such as inside the Decoder class
        /// </summary>
        /// <param name="index"></param>
        /// <param name="form"></param>
        /// <param name="line"></param>
        /// <param name="space"></param>
        /// <returns></returns>
        public static UserPositionInfo Create(int index, int form, int line, int space)
        {
            return new UserPositionInfo(index, form, line, space);
        }
        
        public static UserPositionInfo Create(int index, int form, int line, int space, char brailleAsUnicode, string brailleDotUnumers )
        {
            return new UserPositionInfo(index, form, line, space, brailleAsUnicode,brailleDotUnumers);
        }
    }

}
