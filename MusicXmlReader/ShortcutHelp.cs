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

        // string className = "ShortcutHelp"; 
        StringBuilder sb;

        private string Plus(string s1)
        {
            return s1;
        }

        private string Plus(string s1, string s2)
        {
            return UiUtilities.Plus(s1, s2);
            //string plus = (string.IsNullOrEmpty(s1) || string.IsNullOrEmpty(s2)) ? "" : "+";
            //return s1 + plus + s2;
        }
        

        private string Plus(string s1, string s2, string s3)
        {
            return UiUtilities.Plus(s1, s2, s3);
            ////return s1 + "+" + s2 + "+" + s3;
            //return Plus(Plus(s1, s2), s3);
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
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns>Returns the value of the access key specified by "&" if any</returns>
        private string AddCaption(string text, string extraText)
        {
            AddLine("\r");
            AddLine(ResourcesForHelp.Shortcut_Caption_Menu + " " + Utilities.RemoveAmpersant(text) + " " + extraText);
            return Utilities.GetShortcutName(text);
        }

        private string AddCaption(string text)
        {
            return AddCaption(text, "");
        }

        /// <summary>
        /// Generate a localized line of helptext for complicated items containing ALT and CONTROL shortcuts at the same time
        /// </summary>
        /// <param name="altString">A string containing the ALT string after the first occurance of "&"</param>
        /// <param name="keys">The combination of ALT, CONTROL, SHIFT and a normal character used by teh CTRL shortcut</param>
        /// <param name="text">The localized text to be associated with the two shortcuts</param>
        private void AddAltControlLine(string mainMenuString, string altString, Keys keys, string text)
        {   
            string altText = Plus(ResourcesForHelp.shortcut_Key_alt, mainMenuString, Utilities.GetShortcutName(altString));
            string controlText = UiUtilities.KeysToString(keys);
            string or = (string.IsNullOrEmpty(altText) || string.IsNullOrEmpty(controlText)) ? "" : " " + ResourcesForHelp.Conjunction_Or + " ";
            AddLine(altText + or + controlText, text);
        }

        private void AddAltControlLine(string mainMenuString, string altString, Keys keys)
        {
            AddAltControlLine(mainMenuString, altString, keys, Utilities.RemoveAmpersant(altString));
        }

        private void AddAltControlLine(string mainMenuString, string altString)
        {
            AddAltControlLine(mainMenuString, altString,Keys.None, Utilities.RemoveAmpersant(altString));
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

        private void  AddLine(Keys keys, string text)
        {
            string controlKeys = UiUtilities.LocalizeControlKey(keys);
            string simpleKeys = UiUtilities.LocalizeSimpleKey(keys);
            AddLine(Plus(controlKeys, simpleKeys),  text);        
        }



        /// <summary>
        /// For commands taking parameters as a simple parameter string
        /// </summary>
        /// <param name="firstKeys"></param>
        /// <param name="parameterText"></param>
        /// <param name="lastKeys"></param>
        /// <param name="text"></param>
        private void AddLine(Keys firstKeys, string parameterText, Keys lastKeys,string text)
        {
            string firstControlKeys = UiUtilities.LocalizeControlKey(firstKeys);
            string firstSimpleKeys = UiUtilities.LocalizeSimpleKey(firstKeys);
            string firstString = Plus(firstControlKeys, firstSimpleKeys);

            string lastControlKeys = UiUtilities.LocalizeControlKey(lastKeys);
            string lastSimpleKeys = UiUtilities.LocalizeSimpleKey(lastKeys);
            string lastString = Plus(lastControlKeys, lastSimpleKeys);

            AddLine(firstString+parameterText+lastString, text); // Concatenate without glue characters!
        }


        /// <summary>
        /// For shortcuts starting with a 2-key combination, where the first key is not in {Alt, Control, Shift}
        /// </summary>
        /// <param name="firstKeys"></param>
        /// <param name="lastKeys"></param>
        /// <param name="parameterText"></param>
        /// <param name="text"></param>
        private void AddLine(Keys firstKeys,  Keys lastKeys, string parameterText, string text)
        {
            string firstControlKeys = UiUtilities.LocalizeControlKey(firstKeys);
            string firstSimpleKeys = UiUtilities.LocalizeSimpleKey(firstKeys);
            string firstString = Plus(firstControlKeys, firstSimpleKeys);

            string lastControlKeys = UiUtilities.LocalizeControlKey(lastKeys);
            string lastSimpleKeys = UiUtilities.LocalizeSimpleKey(lastKeys);
            string lastString = Plus(lastControlKeys, lastSimpleKeys);

            AddLine(firstString + "+" + lastString + parameterText, text); // Concatenate without glue characters!
        }





        /// <summary>
        /// Used for generating list of shortcuts. No localization needed.
        /// </summary>
        private static string TempoLimitString
        {
            get
            {
                return string.Format(" ({0}<n<{1})", UiUtilities.TempoFactorMinimum, UiUtilities.TempoFactorMaximum);
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
            AddLine(Keys.Alt, ResourcesForHelp.Shortcut_SelectMenuLine);
            AddLine(Keys.Tab, ResourcesForHelp.Shortcut_ToggleBetweenListAndFilter);
            AddLine(Keys.Escape, ResourcesForHelp.Shortcut_CancelCurrentOperation);
            AddLine(Keys.Control | Keys.O, ResourcesForHelp.Shortcut_OpenFileOpenDialog);
            AddLine(Keys.Control | Keys.P, ResourcesForHelp.Shortcut_PrintUsingWindowsPrintDialog);
            AddLine(Keys.Alt | Keys.F4, ResourcesForHelp.Shortcut_CloseProgram);
            AddLine(Keys.Control | Keys.Home, ResourcesForHelp.Shortcut_GoToTopLine);
            AddLine(Keys.Control | Keys.End, ResourcesForHelp.Shortcut_GoToBottomLine);
             
            AddLine("\r");
            AddLine(ResourcesForHelp.Shortcut_Caption_JAWS);
            AddLine(Keys.Insert, Keys.Space, " s ",ResourcesForHelp.Shortcut_ToggleJAWSSpeechOnOff);
            AddLine(Keys.Insert, Keys.PageDown, "", ResourcesForHelp.Shortcut_ReadStatusLine);
            AddLine(Keys.Insert, Keys.T, "",ResourcesForHelp.Shortcut_ReadTitleLine); 
            AddLine(Keys.Insert, Keys.B, "", ResourcesForHelp.Shortcut_ReadMessagebox);

            string files = AddCaption(ResourcesForUI.ToolStripMenuItem_Files);
            AddAltControlLine(files, ResourcesForUI.ToolStripMenuItem_Files_OpenMusicXmlFile, ShortcutHandler.openMusicXmlFile);
            AddAltControlLine(files, ResourcesForUI.ToolStripMenuItem_Files_ImportDownloads, ShortcutHandler.NoKeys);
            AddAltControlLine(files, ResourcesForUI.ToolStripMenuItem_Files_ImportNewestDownloads, ShortcutHandler.NoKeys);
            AddAltControlLine(files, ResourcesForUI.ToolStripMenuItem_Files_ImportNewestSamples, ShortcutHandler.NoKeys);
            AddAltControlLine(files, ResourcesForUI.ToolStripMenuItem_Files_ExportMusicBrailleToFile, ShortcutHandler.NoKeys);
            AddAltControlLine(files, ResourcesForUI.ToolStripMenuItem_Files_PrintMusicBraille, ShortcutHandler.NoKeys);
            AddLine(ShortcutHandler.PrintOnWindowsPrinter, ResourcesForHelp.Shortcut_PrintUsingWindowsPrintDialog); // We can't handle ALT-sequences in 3 levels here, so we only show the CTRL combinations
            AddLine(ShortcutHandler.PrintUsingExternalProgram, ResourcesForHelp.Shortcut_PrintUsingExternalProgram); // We can't handle ALT-sequences in 3 levels here, so we only show the CTRL combinations
            AddAltControlLine(files, ResourcesForUI.ToolStripMenuItem_Files_Exit, ShortcutHandler.exitApplication);

            // The following special keyboard shortcuts can be used in connection with the node filter
            string edit = AddCaption(ResourcesForUI.ToolStripMenuItem_Edit,ResourcesForHelp.ToolStripMenuItem_Edit_ExtraText);
            AddAltControlLine(edit, ResourcesForUI.TreeView_All_Items, ShortcutHandler.NoKeys, ResourcesForHelp.Shortcut_ExpandAndEditNoteFilter);
            AddAltControlLine(edit, ResourcesForUI.TreeWiew_Items, ShortcutHandler.editFilter, ResourcesForHelp.Shortcut_EditNoteFilter);
            AddAltControlLine(edit, ResourcesForUI.TreeView_MusicAsSound, ShortcutHandler.editMusic, ResourcesForHelp.Shortcut_EditMusicPlaying);
            AddAltControlLine(edit, ResourcesForUI.TreeView_MusicAsSpeech, ShortcutHandler.editText, ResourcesForHelp.Shortcut_EditText);
            AddAltControlLine(edit, ResourcesForUI.TreeView_MusicAsBraille, ShortcutHandler.editBraille, ResourcesForHelp.Shortcut_EditMusicBraille);
            AddAltControlLine(edit, ResourcesForUI.TreeView_MusicAsSpeech_Parts, ShortcutHandler.NoKeys, ResourcesForHelp.Shortcut_EditVoices);
            AddAltControlLine(edit, ResourcesForUI.TreeView_MusicAsSpeech_Details, ShortcutHandler.NoKeys, ResourcesForHelp.Shortcut_EditDetails);
            AddAltControlLine(edit, ResourcesForUI.TreeView_UncheckAll, ShortcutHandler.uncheckAll, ResourcesForHelp.Shortcut_TurnOffGlobally);
            AddAltControlLine(edit, ResourcesForUI.TreeView_CheckAll, ShortcutHandler.checkAll, ResourcesForHelp.Shurtcut_TurnOnGlobally);


            // Start of commands taking parameters and handled by the ParameterInputForm
            AddCaption(ResourcesForUI.ToolStripMenuItem_Edit, ResourcesForHelp.ToolStripMenuItem_Edit_ParameterInputText);
            AddAltControlLine(edit, ResourcesForUI.ParameterInputForm_Repeat, ShortcutHandler.CommandRepeat, "'n,m' : " + ResourcesForHelp.Shortcut_RepeatFromNtoM);
            AddAltControlLine(edit, ResourcesForUI.ParameterInputForm_GoTo, ShortcutHandler.CommandGoto, "'n'   : " + ResourcesForHelp.Shortcut_GoToMeasureN);
            AddAltControlLine(edit, ResourcesForUI.ParameterInputForm_PctOfNominalTempo, ShortcutHandler.CommandTempo, "'n'   : " + ResourcesForHelp.Shortcut_SetTempo + TempoLimitString);

            //AddLine(ShortcutHandler.CommandRepeat, " n,m ", Keys.Enter, ResourcesForHelp.Shortcut_RepeatFromNtoM);
            //AddLine(ShortcutHandler.CommandGoto, " n ", Keys.Enter, ResourcesForHelp.Shortcut_GoToMeasureN);
            //AddLine(ShortcutHandler.CommandTempo, " n ", Keys.Enter, ResourcesForHelp.Shortcut_SetTempo + TempoLimitString);
            // End of commands taking parameters and handled by the ParameterInputForm

            string view = AddCaption(ResourcesForUI.ToolStripMenuItem_View);
            AddAltControlLine(view, ResourcesForUI.ToolsStripMenuItem_View_Instruments, ShortcutHandler.NoKeys);

            string settings = AddCaption(ResourcesForUI.ToolStripMenuItem_Settings);
            AddAltControlLine(settings, ResourcesForUI.ToolStripMenuItem_Settings_General, ShortcutHandler.NoKeys);
            AddAltControlLine(settings, ResourcesForUI.ToolStripMenuItem_Settings_Embosser, ShortcutHandler.NoKeys);
            AddAltControlLine(settings, ResourcesForUI.ToolStripMenuItem_Settings_NoteTaker, ShortcutHandler.NoKeys);
            AddAltControlLine(settings, ResourcesForUI.ToolStripMenuItem_Settings_MusicBraille, ShortcutHandler.NoKeys);
            AddAltControlLine(settings, ResourcesForUI.ToolStripMenuItem_Settings_ResetAll, ShortcutHandler.NoKeys);

            string tools = AddCaption(ResourcesForUI.ToolStripMenuItem_Tools);
            AddAltControlLine(tools, ResourcesForUI.ToolStripMenuItem_Tools_MuseScore);
            AddAltControlLine(tools, ResourcesForUI.ToolStripMenuItem_Tools_Sibelius);
            AddAltControlLine(tools, ResourcesForUI.ToolStripMenuItem_Tools_Capella);
            AddAltControlLine(tools, ResourcesForUI.ToolStripMenuItem_Tools_Finale);
            AddAltControlLine(tools, ResourcesForUI.ToolStripMenuItem_Tools_Logfile);
            AddAltControlLine(tools, ResourcesForUI.ToolStripMenuItem_Tools_Logfile_Location);
            AddAltControlLine(tools, ResourcesForUI.ToolStripMenuItem_Tools_OpenXmlFileLocation);
            AddAltControlLine(tools, ResourcesForUI.ToolStripMenuItem_Tools_InspectAsXml);
            AddAltControlLine(tools, ResourcesForUI.ToolStripMenuItem_Tools_InspectAsXml_Using_Chrome);
            AddAltControlLine(tools, ResourcesForUI.ToolStripMenuItem_Tools_ViewAsInterpretedXml);
            AddAltControlLine(tools, ResourcesForUI.ToolStripMenuItem_Tools_JAWS_Settings);

            string noteArchives = AddCaption(ResourcesForUI.ToolsStripMenuItem_Archives);
            AddLine("(Ingen)");

            string help = AddCaption(ResourcesForUI.ToolStripMenuItem_Help);
            AddAltControlLine(help, ResourcesForUI.ToolStripMenuItem_Help_About); 
            AddAltControlLine(help, ResourcesForUI.ToolStripMenuItem_Help_Shortcuts); 
            AddAltControlLine(help, ResourcesForUI.ToolStripMenuItem_Help_SoftwareUpdate); 
            AddAltControlLine(help, ResourcesForUI.ToolStripMenuItem_Help_ShowUsersManual); 


            // Følgende specielle tastaturgenveje og kommandoer kan anvendes i forbindelse med Nodelisten           
            // The following special keyboard skortcuts and commands can be used in connection with the note list
            AddLine("\r");
            AddLine(ResourcesForHelp.shortcut_Caption_NoteList);
            // First all combinations of ARROWS and CONTROL
            AddLine(ShortcutHandler.PreviousEvent, ResourcesForHelp.Shortcut_PreviousLine);
            AddLine(ShortcutHandler.NextEvent, ResourcesForHelp.Shortcut_NextLine);
            AddLine(ShortcutHandler.PreviousMeasure, ResourcesForHelp.Shortcut_PreviousMeasure); 
            AddLine(ShortcutHandler.NextMeasure, ResourcesForHelp.Shortcut_NextMeasure);
            AddLine(ShortcutHandler.DetailsHarmonyTop, ResourcesForHelp.Shortcut_ChordTop);
            AddLine(ShortcutHandler.DetailsHarmonyBottom, ResourcesForHelp.Shortcut_ChordName);
            AddLine(ShortcutHandler.DetailsStatusTop, ResourcesForHelp.Shortcut_StatusTop);
            AddLine(ShortcutHandler.DetailsStatusBottom, ResourcesForHelp.Shortcut_StatusBottom);
            AddLine(ShortcutHandler.DetailsPartsTop, ResourcesForHelp.Shortcut_PartTop);
            AddLine(ShortcutHandler.DetailsPartsBottom, ResourcesForHelp.Shortcut_PartBottom);
            AddLine(ShortcutHandler.DetailsInstruments, ResourcesForHelp.Shortcut_Instruments);
            // AddLine(Plus(control, "F"), ResourcesForHelp.Shortcut_FixedKeySignature)); // Not implemented yet !
            AddLine(ShortcutHandler.startPlaying, ResourcesForHelp.Shortcut_StartPlaying);
            AddLine(ShortcutHandler.stopPlaying, ResourcesForHelp.Shortcut_StopPlaying);
            AddLine(ShortcutHandler.togglePlaying, ResourcesForHelp.Shortcut_TogglePlay);
            AddLine(ShortcutHandler.StopAllNotesPlaying, ResourcesForHelp.Shortcut_StopCurrentNote);

 

            AddLine(ShortcutHandler.tempoIncrement, ResourcesForHelp.Shortcut_IncreaseTempo);
            AddLine(ShortcutHandler.tempoDecrement, ResourcesForHelp.Shortcut_DecreaseTempo);

            // Følgende specielle tastaturgenveje  kan anvendes i forbindelse med Detaljelisten 
            //The following special keyboard shortcuts can be used in connection with the detail list
            AddLine("\r");
            AddLine(ResourcesForHelp.Shortcut_Caption_DetailList);
            AddLine(Keys.Down, ResourcesForHelp.Shortcut_NextDetail);
            AddLine(Keys.Up, ResourcesForHelp.Shortcut_PreviousDetail);

            // Følgende specielle tastaturgenveje og kommandoer kan anvendes i forbindelse med Nodefilteret           
            // The following special keyboard skortcuts and commands can be used in connection with the note filter
            AddLine("\r");
            AddLine(ResourcesForHelp.Shortcut_Caption_NoteFilter_Special);
            AddLine(ShortcutHandler.uncheckOthers, ResourcesForHelp.Shurtcut_TurnOffOtherPartsOrDetails);
            AddLine(ShortcutHandler.checkOthers, ResourcesForHelp.Shurtcut_TurnOnOtherPartsOrDetails);
            AddLine(Keys.Space, ResourcesForHelp.Shortcut_ToggleValue);
            AddLine(Keys.Control | Keys.L, ResourcesForHelp.Shortcut_SelectNoteList);




            return strings;
        }


        public static ShortcutHelp Create()
        {
            return new ShortcutHelp();
        }
    }
}
