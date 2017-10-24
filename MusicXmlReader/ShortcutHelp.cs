using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicXmlReaderModel;

namespace MusicXmlReader
{
    class ShortcutHelp
    {
        string className = "ShortcutHelp"; 
        StringBuilder sb;

        private ShortcutHelp()
        {
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
            return string.Format("{0,-20} \t {1}", s1, s2); // Make all s1 25 chars long and use a TAB
        }

        private void AddLine(string line)
        {
            strings.Add(line);
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
            // The following texts must be localized!   
            string control = ResourcesForHelp.Shortcut_Key_Control;
            string space = ResourcesForHelp.Shortcut_Key_space;
            string escape = ResourcesForHelp.Shortcut_Key_escape;
            string enter = ResourcesForHelp.Shortcut_Key_enter;
            string insert = ResourcesForHelp.Shortcut_Key_insert;
            string alt = ResourcesForHelp.shortcut_Key_alt;
            string tab = ResourcesForHelp.shortcut_Key_tab;
            string followedBy = ResourcesForHelp.shortcut_Text_followedBy;
            string pageUp = ResourcesForHelp.Shortcut_Key_pageUp;
            string pageDown = ResourcesForHelp.Shortcut_Key_pageDown;
            string shift = ResourcesForHelp.shortcut_Key_shift;
            string arrowLeft = ResourcesForHelp.Shortcut_ArrowLeft;
            string arrowRight = ResourcesForHelp.Shortcut_ArrowRight;
            string arrowUp = ResourcesForHelp.Shortcut_ArrowUp;
            string arrowDown = ResourcesForHelp.Shortcut_ArrowDown;
            string home = ResourcesForHelp.Shortcut_Home;
            string end = ResourcesForHelp.Shortcut_End;


            sb = new StringBuilder();


            // Følgende standard Windows / JAWS tastatur genveje kan anvendes generelt
            // The following standard Windows / JAWS keyboard shortcuts can be used 
            // AddLine("\r");
            AddLine(ResourcesForHelp.Shortcut_Caption_Windows);
            AddLine(Plus(control, "O"),  ResourcesForHelp.Shortcut_OpenFileOpenDialog);
            AddLine(Plus(alt, "F4"),     ResourcesForHelp.Shortcut_CloseProgram);
            AddLine(alt, ResourcesForHelp.Shortcut_SelectMenuLine);
            AddLine(tab, ResourcesForHelp.Shortcut_ToggleBetweenListAndFilter);

            AddLine(Plus(control, home), ResourcesForHelp.Shortcut_GoToTopLine);
            AddLine(Plus(control, end), ResourcesForHelp.Shortcut_GoToBottomLine);

            AddLine("\r");
            AddLine(ResourcesForHelp.Shortcut_Caption_JAWS);
            AddLine(Plus(insert,space) + " " + followedBy + " s",ResourcesForHelp.Shortcut_ToggleJAWSSpeechOnOff);
            AddLine(Plus(insert,pageDown), ResourcesForHelp.Shortcut_ReadStatusLine);
            AddLine(Plus(insert, "T"), ResourcesForHelp.Shortcut_ReadTitleLine);
            AddLine(Plus(insert, "B"), ResourcesForHelp.Shortcut_ReadMessagebox);

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
            AddLine(Plus(control, "L"), ResourcesForHelp.Shortcut_SelectNoteList);
            AddLine(Plus(control, "P"), ResourcesForHelp.Shortcut_StartPlaying);
            AddLine(Plus(control, shift, "P"), ResourcesForHelp.Shortcut_StopPlaying);
            AddLine(Plus(space), ResourcesForHelp.Shortcut_TogglePlay);
            AddLine("Tn " + enter, ResourcesForHelp.Shortcut_SetTempo);
            AddLine("Rn,m " + enter, ResourcesForHelp.Shortcut_RepeatFromNtoM);
            AddLine("Gn " + enter, ResourcesForHelp.Shortcut_GoToMeasureN);
            AddLine("ESC", ResourcesForHelp.Shortcut_StopCurrentNote);
            AddLine(Plus(control, pageUp), ResourcesForHelp.Shortcut_IncreaseTempo);
            AddLine(Plus(control, pageDown), ResourcesForHelp.Shortcut_DecreaseTempo); 

            // Følgende specielle tastaturgenveje kan bruges i forbindelse med redigering af nodefilteret:
            // The following special keyboard shortcuts can be used in connection with the node filter
            AddLine("\r");
            AddLine(ResourcesForHelp.shortcut_Caption_NoteFilter);
            AddLine(Plus(alt, Utilities.GetShortcutName(ResourcesForUI.TreeView_All_Items)),    ResourcesForHelp.Shortcut_ExpandAndEditNoteFilter);
            AddLine(Plus(alt, Utilities.GetShortcutName(ResourcesForUI.TreeWiew_Items)),        ResourcesForHelp.Shortcut_EditNoteFilter);
            AddLine(Plus(alt, Utilities.GetShortcutName(ResourcesForUI.TreeView_MusicAsSound)), ResourcesForHelp.Shortcut_EditMusicPlaying);
            AddLine(Plus(alt, Utilities.GetShortcutName(ResourcesForUI.TreeView_MusicAsSpeech)),ResourcesForHelp.Shortcut_EditText);
            AddLine(Plus(alt, Utilities.GetShortcutName(ResourcesForUI.TreeView_MusicAsBraille)), ResourcesForHelp.Shortcut_EditMusicBraille);
            AddLine(Plus(alt, Utilities.GetShortcutName(ResourcesForUI.TreeView_MusicAsSpeech_Parts)), ResourcesForHelp.Shortcut_EditVoices);
            AddLine(Plus(alt, Utilities.GetShortcutName(ResourcesForUI.TreeView_MusicAsSpeech_Details)), ResourcesForHelp.Shortcut_EditDetails);
            AddLine(Plus(alt, Utilities.GetShortcutName(ResourcesForUI.TreeView_UncheckAll)),   ResourcesForHelp.Shortcut_TurnOffGlobally);
            AddLine(Plus(alt, Utilities.GetShortcutName(ResourcesForUI.TreeView_CheckAll)),     ResourcesForHelp.Shurtcut_TurnOnGlobally);
            AddLine(space, ResourcesForHelp.Shortcut_ToggleValue);

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
