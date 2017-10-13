using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReader
{
    class ShortcutHelp
    {
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

        private List<string> strings = new List<string>();

        public List<string> ToStrings()
        {
            strings = new List<string>();
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
            AddLine(Combine(Plus(control, "O"),  ResourcesForHelp.Shortcut_OpenFileOpenDialog));
            AddLine(Combine(Plus(alt, "F4"),     ResourcesForHelp.Shortcut_CloseProgram));
            AddLine(Combine(alt, ResourcesForHelp.Shortcut_SelectMenuLine));
            AddLine(Combine(tab, ResourcesForHelp.Shortcut_ToggleBetweenListAndFilter));

            AddLine(Combine(Plus(control, home), ResourcesForHelp.Shortcut_GoToTopLine));
            AddLine(Combine(Plus(control, end), ResourcesForHelp.Shortcut_GoToBottomLine));

            AddLine("\r");
            AddLine(ResourcesForHelp.Shortcut_Caption_JAWS);
            AddLine(Combine(Plus(insert,space) + " " + followedBy + " s",ResourcesForHelp.Shortcut_ToggleJAWSSpeechOnOff));
            AddLine(Combine(Plus(insert,pageDown), ResourcesForHelp.Shortcut_ReadStatusLine));
            AddLine(Combine(Plus(insert, "T"), ResourcesForHelp.Shortcut_ReadTitleLine));
            AddLine(Combine(Plus(insert, "B"), ResourcesForHelp.Shortcut_ReadMessagebox));

            // Følgende specielle tastaturgenveje og kommandoer kan anvendes i forbindelse med Nodelisten           
            // The following special keyboard skortcuts and commands can be used in connection with the note list
            AddLine("\r");
            AddLine(ResourcesForHelp.shortcut_Caption_NoteList);
            AddLine(Combine(arrowLeft, ResourcesForHelp.Shortcut_PreviousLine));
            AddLine(Combine(arrowRight, ResourcesForHelp.Shortcut_NextLine));
            AddLine(Combine(Plus(control,arrowLeft), ResourcesForHelp.Shortcut_PreviousMeasure)); 
            AddLine(Combine(Plus(control,arrowRight), ResourcesForHelp.Shortcut_NextMeasure)); 
            //AddLine(Combine(arrowDown, ResourcesForHelp.Shortcut_NextPart));   // Description has been moved to the description of the  detail list
            //AddLine(Combine(arrowUp, ResourcesForHelp.Shortcut_PreviousPart)); // Description has been moved to the description of the  detail list
            AddLine(Combine(Plus(control, "L"), ResourcesForHelp.Shortcut_SelectNoteList));
            AddLine(Combine(Plus(control, "P"), ResourcesForHelp.Shortcut_StartPlaying));
            AddLine(Combine(Plus(control, shift, "P"), ResourcesForHelp.Shortcut_StopPlaying));
            AddLine(Combine(Plus(space), ResourcesForHelp.Shortcut_TogglePlay));
            AddLine(Combine(Plus(control,"Tn " + enter), ResourcesForHelp.Shortcut_SetTempo));
            AddLine(Combine(Plus(control,"Rn,m " + enter), ResourcesForHelp.Shortcut_RepeatFromNtoM));
            AddLine(Combine(Plus(control,"Gn " + enter), ResourcesForHelp.Shortcut_GoToMeasureN));
            AddLine(Combine("ESC", ResourcesForHelp.Shortcut_StopCurrentNote));
            AddLine(Combine(Plus(control, pageUp), ResourcesForHelp.Shortcut_IncreaseTempo));
            AddLine(Combine(Plus(control, pageDown), ResourcesForHelp.Shortcut_DecreaseTempo));

            // Følgende specielle tastaturgenveje kan bruges i forbindelse med redigering af nodefilteret:
            // The following special keyboard shortcuts can be used in connection with the node filter
            AddLine("\r");
            AddLine(ResourcesForHelp.shortcut_Caption_NoteFilter);
            AddLine(Combine(Plus(alt, "A"), ResourcesForHelp.Shortcut_ExpandAndEditNoteFilter));
            AddLine(Combine(Plus(alt, "F"), ResourcesForHelp.Shortcut_EditNoteFilter));
            AddLine(Combine(Plus(alt, "M"), ResourcesForHelp.Shortcut_EditMusicPlaying));
            AddLine(Combine(Plus(alt, "T"), ResourcesForHelp.Shortcut_EditText));
            AddLine(Combine(Plus(alt, "B"), ResourcesForHelp.Shortcut_EditMusicBraille));
            AddLine(Combine(Plus(alt, "S"), ResourcesForHelp.Shortcut_EditVoices));
            AddLine(Combine(Plus(alt, "0"), ResourcesForHelp.Shortcut_TurnOffGlobally));
            AddLine(Combine(Plus(alt, "1"), ResourcesForHelp.Shurtcut_TurnOnGlobally));
            AddLine(Combine(space, ResourcesForHelp.Shortcut_ToggleValue));

            // Følgende specielle tastaturgenveje  kan anvendes i forbindelse med Detaljelisten 
            //The following special keyboard shortcuts can be used in connection with the detail list
            AddLine("\r");
            AddLine(ResourcesForHelp.Shortcut_Caption_DetailList);
            AddLine(Combine(arrowUp, ResourcesForHelp.Shortcut_ChordTop));
            AddLine(Combine(arrowDown, ResourcesForHelp.Shortcut_ChordName));
            AddLine(Combine(Plus(control, arrowUp), ResourcesForHelp.Shortcut_PartTop));
            AddLine(Combine(Plus(control, arrowDown), ResourcesForHelp.Shortcut_PartBottom));
            AddLine(Combine(Plus(control, "I"), ResourcesForHelp.Shortcut_Instruments));
            // AddLine(Combine(Plus(control, "F"), ResourcesForHelp.Shortcut_FixedKeySignature)); // Not implemented yet !
            AddLine(Combine(arrowDown, ResourcesForHelp.Shortcut_NextDetail));
            AddLine(Combine(arrowUp, ResourcesForHelp.Shortcut_PreviousDetail));

            return strings;
        }


        public static ShortcutHelp Create()
        {
            return new ShortcutHelp();
        }
    }
}
