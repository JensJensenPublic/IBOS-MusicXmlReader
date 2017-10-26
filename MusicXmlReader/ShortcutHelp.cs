using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicXmlReaderModel;
using System.Windows.Forms;

namespace MusicXmlReader
{
    class ShortcutHelp
    {
        string className = "ShortcutHelp"; 
        StringBuilder sb;
        // The following texts are all localized!   
        string control = "";
        string space = "";
        string escape = "";
        string enter = "";
        string insert = "";
        string alt = "";
        string tab = "";
        string followedBy = "";
        string pageUp = "";
        string pageDown = "";
        string shift = "";
        string arrowLeft = "";
        string arrowRight = "";
        string arrowUp = "";
        string arrowDown = "";
        string home = "";
        string end = "";

        private ShortcutHelp()
        {
            // The following texts are all localized!   
          control = ResourcesForHelp.Shortcut_Key_Control;
          space = ResourcesForHelp.Shortcut_Key_space;
          escape = ResourcesForHelp.Shortcut_Key_escape;
          enter = ResourcesForHelp.Shortcut_Key_enter;
          insert = ResourcesForHelp.Shortcut_Key_insert;
          alt = ResourcesForHelp.shortcut_Key_alt;
          tab = ResourcesForHelp.shortcut_Key_tab;
          followedBy = ResourcesForHelp.shortcut_Text_followedBy;
          pageUp = ResourcesForHelp.Shortcut_Key_pageUp;
          pageDown = ResourcesForHelp.Shortcut_Key_pageDown;
          shift = ResourcesForHelp.shortcut_Key_shift;
          arrowLeft = ResourcesForHelp.Shortcut_ArrowLeft;
          arrowRight = ResourcesForHelp.Shortcut_ArrowRight;
          arrowUp = ResourcesForHelp.Shortcut_ArrowUp;
          arrowDown = ResourcesForHelp.Shortcut_ArrowDown;
          home = ResourcesForHelp.Shortcut_Home;
          end = ResourcesForHelp.Shortcut_End;
        }

        private string Plus(string s1)
        {
            return s1;
        }

        private string Plus(string s1, string s2)
        {
            return s1 + "+" + s2;
        }
        

        private string Plus(string s1, string s2, string s3)
        {
            return s1 + "+" + s2 + "+" + s3;
        }

        private string Combine(string s1, string s2)
        {
            // Accept a simple formatting to save development time !!
            return string.Format("{0,-25} \t {1}", s1, s2); // Make all s1 25 chars long and use a TAB
        }

        private void AddLine(string line)
        {
            strings.Add(line);
        }

        


        /// <summary>
        /// Generate a localized line of helptext for complicated items containing ALT and CONTROL shortcuts at the same time
        /// </summary>
        /// <param name="altString">A string containing the ALT string after the first occurance of "&"</param>
        /// <param name="keys">The combination of ALT, CONTROL, SHIFT and a normal character used by teh CTRL shortcut</param>
        /// <param name="text">The localized text to be associated with the two shortcuts</param>
        private void AddAltControlLine(string mainMenuString, string altString, Keys keys, string text)
        {   
            string altText = Plus(alt, mainMenuString, Utilities.GetShortcutName(altString));
            string controlText = UiUtilities.KeysToString(keys);
            string or = (string.IsNullOrEmpty(altText) || string.IsNullOrEmpty(controlText)) ? "" : " " + ResourcesForHelp.Conjunction_Or + " ";
            AddLine(altText + or + controlText, text);
        }


        private void AddLine(string keys, string text)
        {
            // string functionName = "AddLine";
            strings.Add(Combine(keys,text));
            if (uniqueKeys.Contains(keys))
            {
                // Logger.Log(string.Format("{0}.{1} Duplicate key={2} When adding '{3}'", className, functionName, keys,text));
            }
            else
            {
                uniqueKeys.Add(keys);
            }

        }

        private List<string> strings = new List<string>();
        private List<string> uniqueKeys = new List<string>(); 

        public List<string> ToStrings()
        {
            strings = new List<string>();
            uniqueKeys = new List<string>();

            sb = new StringBuilder();


            // Følgende standard Windows / JAWS tastatur genveje kan anvendes generelt
            // The following standard Windows / JAWS keyboard shortcuts can be used 
            // AddLine("\r");
            AddLine(ResourcesForHelp.Shortcut_Caption_Windows);
            AddLine(alt, ResourcesForHelp.Shortcut_SelectMenuLine);
            AddLine(tab, ResourcesForHelp.Shortcut_ToggleBetweenListAndFilter);
            AddLine(escape, ResourcesForHelp.Shortcut_CancelCurrentOperation);
            AddLine(Plus(control, "O"), ResourcesForHelp.Shortcut_OpenFileOpenDialog);
            AddLine(Plus(alt, "F4"), ResourcesForHelp.Shortcut_CloseProgram);
            AddLine(Plus(control, home), ResourcesForHelp.Shortcut_GoToTopLine);
            AddLine(Plus(control, end), ResourcesForHelp.Shortcut_GoToBottomLine);  

            AddLine("\r");
            AddLine(ResourcesForHelp.Shortcut_Caption_JAWS);
            AddLine(Plus(insert,space) + " " + followedBy + " s",ResourcesForHelp.Shortcut_ToggleJAWSSpeechOnOff);
            AddLine(Plus(insert,pageDown), ResourcesForHelp.Shortcut_ReadStatusLine);
            AddLine(Plus(insert, "T"), ResourcesForHelp.Shortcut_ReadTitleLine);
            AddLine(Plus(insert, "B"), ResourcesForHelp.Shortcut_ReadMessagebox);

            AddLine("\r");
            AddLine("Genvejstaster for menuen 'Filer'");
            string files = Utilities.GetShortcutName(ResourcesForUI.ToolStripMenuItem_Files); // Get a string representing the Acccess key for the "Files" Menu
            AddAltControlLine(files, ResourcesForUI.ToolStripMenuItem_Files_OpenMusicXmlFile, ShortcutHandler.openMusicXmlFile, "");//, ResourcesForHelp.);
            AddAltControlLine(files, ResourcesForUI.ToolStripMenuItem_Files_ImportDownloads, ShortcutHandler.NoKeys, ""); // ResourcesForHelp.);
            AddAltControlLine(files, ResourcesForUI.ToolStripMenuItem_Files_ImportNewestDownloads, ShortcutHandler.NoKeys, "");// ResourcesForHelp.);
            AddAltControlLine(files, ResourcesForUI.ToolStripMenuItem_Files_ImportNewestSamples, ShortcutHandler.NoKeys, "");// ResourcesForHelp.);
            AddAltControlLine(files, ResourcesForUI.ToolStripMenuItem_Files_Exit, ShortcutHandler.exitApplication, "");// ResourcesForHelp.);

            // The following special keyboard shortcuts can be used in connection with the node filter
            AddLine("\r");
            // AddLine(ResourcesForHelp.shortcut_Caption_NoteFilter);
            AddLine("Genvejstaster for menuen 'Redigér'");
            string edit = Utilities.GetShortcutName(ResourcesForUI.ToolStripMenuItem_Edit); // Get a string representing the Acccess key for the "Edit" Menu
            AddAltControlLine(edit, ResourcesForUI.TreeView_All_Items, ShortcutHandler.NoKeys, ResourcesForHelp.Shortcut_ExpandAndEditNoteFilter);
            AddAltControlLine(edit, ResourcesForUI.TreeWiew_Items, ShortcutHandler.editFilter, ResourcesForHelp.Shortcut_EditNoteFilter);
            AddAltControlLine(edit, ResourcesForUI.TreeView_MusicAsSound, ShortcutHandler.editMusic, ResourcesForHelp.Shortcut_EditMusicPlaying);
            AddAltControlLine(edit, ResourcesForUI.TreeView_MusicAsSpeech, ShortcutHandler.editText, ResourcesForHelp.Shortcut_EditText);
            AddAltControlLine(edit, ResourcesForUI.TreeView_MusicAsBraille, ShortcutHandler.editBraille, ResourcesForHelp.Shortcut_EditMusicBraille);
            AddAltControlLine(edit, ResourcesForUI.TreeView_MusicAsSpeech_Parts, ShortcutHandler.NoKeys, ResourcesForHelp.Shortcut_EditVoices);
            AddAltControlLine(edit, ResourcesForUI.TreeView_MusicAsSpeech_Details, ShortcutHandler.NoKeys, ResourcesForHelp.Shortcut_EditDetails);
            AddAltControlLine(edit, ResourcesForUI.TreeView_UncheckAll, ShortcutHandler.uncheckAll, ResourcesForHelp.Shortcut_TurnOffGlobally);
            AddAltControlLine(edit, ResourcesForUI.TreeView_CheckAll, ShortcutHandler.checkAll, ResourcesForHelp.Shurtcut_TurnOnGlobally);
            AddLine(space, ResourcesForHelp.Shortcut_ToggleValue);
            AddLine(Plus(control, "L"), ResourcesForHelp.Shortcut_SelectNoteList);

            AddLine("\r");
            // AddLine(ResourcesForHelp.shortcut_Caption_NoteFilter);
            AddLine("Genvejstaster for menuen 'Vis'");
            string view = Utilities.GetShortcutName(ResourcesForUI.ToolStripMenuItem_View); // Get a string representing the Acccess key for the "View" Menu
            AddAltControlLine(view, ResourcesForUI. ToolsStripMenuItem_View_Instruments, ShortcutHandler.NoKeys, "");


            AddLine("\r");
            // AddLine(ResourcesForHelp.shortcut_Caption_NoteFilter);
            AddLine("Genvejstaster for menuen 'Værktøjer'");
            string tools = Utilities.GetShortcutName(ResourcesForUI.ToolStripMenuItem_Tools); // Get a string representing the Acccess key for the "Tools" Menu
            AddAltControlLine(tools, ResourcesForUI.ToolStripMenuItem_Tools_MuseScore, ShortcutHandler.NoKeys, ""); // ResourcesForHelp);
            AddAltControlLine(tools, ResourcesForUI.ToolStripMenuItem_Tools_Sibelius, ShortcutHandler.NoKeys, ""); // ResourcesForHelp);
            AddAltControlLine(tools, ResourcesForUI.ToolStripMenuItem_Tools_Logfile, ShortcutHandler.NoKeys, ""); // ResourcesForHelp);
            AddAltControlLine(tools, ResourcesForUI.ToolStripMenuItem_Tools_Logfile_Location, ShortcutHandler.NoKeys, ""); // ResourcesForHelp);
            AddAltControlLine(tools, ResourcesForUI.ToolStripMenuItem_Tools_OpenXmlFileLocation, ShortcutHandler.NoKeys, ""); // ResourcesForHelp);
            AddAltControlLine(tools, ResourcesForUI.ToolStripMenuItem_Tools_InspectAsXml, ShortcutHandler.NoKeys, ""); // ResourcesForHelp);
            AddAltControlLine(tools, ResourcesForUI.ToolStripMenuItem_Tools_ViewAsInterpretedXml, ShortcutHandler.NoKeys, ""); // ResourcesForHelp);
            AddAltControlLine(tools, ResourcesForUI.ToolStripMenuItem_Tools_JAWS_Settings, ShortcutHandler.NoKeys, ""); // ResourcesForHelp);


            AddLine("\r");
            // AddLine(ResourcesForHelp.shortcut_Caption_NoteFilter);
            AddLine("Genvejstaster for menuen 'Nodearkiver'");
            string noteArchives = Utilities.GetShortcutName(ResourcesForUI.ToolsStripMenuItem_Archives); // Get a string representing the Acccess key for the "Note Archives" Menu
            AddLine("(Ingen)");

            AddLine("\r");
            // AddLine(ResourcesForHelp.shortcut_Caption_NoteFilter);
            AddLine("Genvejstaster for menuen 'Hjælp'");
            string help = Utilities.GetShortcutName(ResourcesForUI.ToolStripMenuItem_Tools); // Get a string representing the Acccess key for the "Help" Menu
            AddAltControlLine(help, ResourcesForUI.ToolStripMenuItem_Help_About  , ShortcutHandler.NoKeys, ""); // ResourcesForHelp);
            AddAltControlLine(help, ResourcesForUI.ToolStripMenuItem_Help_Shortcuts, ShortcutHandler.NoKeys, ""); // ResourcesForHelp);
            AddAltControlLine(help, ResourcesForUI.ToolStripMenuItem_Help_SoftwareUpdate, ShortcutHandler.NoKeys, ""); // ResourcesForHelp);
            AddAltControlLine(help, ResourcesForUI.ToolStripMenuItem_Help_ShowUsersManual, ShortcutHandler.NoKeys, ""); // ResourcesForHelp);


            // Følgende specielle tastaturgenveje og kommandoer kan anvendes i forbindelse med Nodelisten           
            // The following special keyboard skortcuts and commands can be used in connection with the note list
            AddLine("\r");
            AddLine(ResourcesForHelp.shortcut_Caption_NoteList);
            // First all combinations of ARROWS and CONTROL
            AddLine(arrowLeft, ResourcesForHelp.Shortcut_PreviousLine);
            AddLine(arrowRight, ResourcesForHelp.Shortcut_NextLine);
            AddLine(Plus(control,arrowLeft), ResourcesForHelp.Shortcut_PreviousMeasure); 
            AddLine(Plus(control,arrowRight), ResourcesForHelp.Shortcut_NextMeasure);
            AddLine(arrowUp, ResourcesForHelp.Shortcut_ChordTop);
            AddLine(arrowDown, ResourcesForHelp.Shortcut_ChordName);
            AddLine(Plus(control, arrowUp), ResourcesForHelp.Shortcut_PartTop);
            AddLine(Plus(control, arrowDown), ResourcesForHelp.Shortcut_PartBottom);
            AddLine(Plus(control, "I"), ResourcesForHelp.Shortcut_Instruments);
            // AddLine(Plus(control, "F"), ResourcesForHelp.Shortcut_FixedKeySignature)); // Not implemented yet !
            AddLine(Plus(control, "P"), ResourcesForHelp.Shortcut_StartPlaying);
            AddLine(Plus(control, shift, "P"), ResourcesForHelp.Shortcut_StopPlaying);
            AddLine(Plus(space), ResourcesForHelp.Shortcut_TogglePlay);
            AddLine("Tn " + enter, ResourcesForHelp.Shortcut_SetTempo);
            AddLine("Rn,m " + enter, ResourcesForHelp.Shortcut_RepeatFromNtoM);
            AddLine("Gn " + enter, ResourcesForHelp.Shortcut_GoToMeasureN);
            AddLine("ESC", ResourcesForHelp.Shortcut_StopCurrentNote);
            AddLine(Plus(control, pageUp), ResourcesForHelp.Shortcut_IncreaseTempo);
            AddLine(Plus(control, pageDown), ResourcesForHelp.Shortcut_DecreaseTempo); 

            // Følgende specielle tastaturgenveje  kan anvendes i forbindelse med Detaljelisten 
            //The following special keyboard shortcuts can be used in connection with the detail list
            AddLine("\r");
            AddLine(ResourcesForHelp.Shortcut_Caption_DetailList); 
            AddLine(arrowDown, ResourcesForHelp.Shortcut_NextDetail);
            AddLine(arrowUp, ResourcesForHelp.Shortcut_PreviousDetail);

            return strings;
        }


        public static ShortcutHelp Create()
        {
            return new ShortcutHelp();
        }
    }
}
