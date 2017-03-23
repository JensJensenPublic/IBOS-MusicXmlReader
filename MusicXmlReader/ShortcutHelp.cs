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
            string pageDown  = ResourcesForHelp.shortcut_Key_pageDown;
            string shift = ResourcesForHelp.shortcut_Key_shift;


            sb = new StringBuilder();


            // Følgende standard Windows / JAWS tastatur genveje kan anvendes generelt
            sb.Append("\r");
            AddLine(ResourcesForHelp.shortcut_Caption_Windows_JAWS);
            AddLine(Combine(Plus(control, "O"),  ResourcesForHelp.Shortcut_OpenFileOpenDialog));
            AddLine(Combine(Plus(alt, "F4"),     ResourcesForHelp.Shortcut_CloseProgram));
            AddLine(Combine(alt, ResourcesForHelp.Shortcut_SelectMenuLine));
            AddLine(Combine(tab, ResourcesForHelp.Shortcut_ToggleBetweenListAndFilter));
            AddLine(Combine(Plus(insert,space) + " " + followedBy + " s",ResourcesForHelp.Shortcut_ToggleJAWSSpeechOnOff));
            AddLine(Combine(Plus(insert,pageDown), ResourcesForHelp.Shortcut_ReadStatusLine));
            AddLine(Combine(Plus(insert, "T"), ResourcesForHelp.Shortcut_ReadTitleLine));
            AddLine(Combine(Plus(insert, "B"), ResourcesForHelp.Shortcut_ReadMessagebox));

            // Følgende specielle tastaturgenveje og kommandoer kan anvendes i forbindelse med Nodelisten
            sb.Append("\r");
            AddLine(ResourcesForHelp.shortcut_Caption_NoteList);
            AddLine(Combine(Plus(control, "L"), ResourcesForHelp.Shortcut_SelectNoteList));
            AddLine(Combine(Plus(control, "P"), ResourcesForHelp.Shortcut_StartPlaying));
            AddLine(Combine(Plus(control, shift, "P"), ResourcesForHelp.Shortcut_StopPlaying));
            AddLine(Combine(Plus(space), ResourcesForHelp.Shortcut_TogglePlay));
            AddLine(Combine("Tn " + enter, ResourcesForHelp.Shortcut_SetTempo));
            AddLine(Combine("Rn,m " + enter, ResourcesForHelp.Shortcut_RepeatFromNtoM));
            AddLine(Combine("Gn " + enter, ResourcesForHelp.Shortcut_GoToMeasureN));
            AddLine(Combine("ESC", ResourcesForHelp.Shortcut_StopCurrentNote));

            // Følgende specielle tastaturgenveje kan bruges i forbindelse med redigering af nodefilteret:
            sb.Append("\r");
            AddLine(ResourcesForHelp.shortcut_Caption_NoteFilter);
            AddLine(Combine(Plus(control, "A"), ResourcesForHelp.Shortcut_ExpandAndEditNoteFilter));
            AddLine(Combine(Plus(control, "F"), ResourcesForHelp.Shortcut_EditNoteFilter));
            AddLine(Combine(Plus(control, "M"), ResourcesForHelp.Shortcut_EditMusicPlaying));
            AddLine(Combine(Plus(control, "T"), ResourcesForHelp.Shortcut_EditText));
            AddLine(Combine(Plus(control, "B"), ResourcesForHelp.Shortcut_EditMusicBraille));
            AddLine(Combine(Plus(control, "S"), ResourcesForHelp.Shortcut_EditVoices));
            AddLine(Combine(space, ResourcesForHelp.Shortcut_ToggleValue));
            AddLine(Combine(Plus(control, "0"), ResourcesForHelp.Shortcut_TurnOffGlobally));
            AddLine(Combine(Plus(control, "1"), ResourcesForHelp.Shurtcut_TurnOnGlobally));

            return strings;
        }


        public static ShortcutHelp Create()
        {
            return new ShortcutHelp();
        }
    }
}
