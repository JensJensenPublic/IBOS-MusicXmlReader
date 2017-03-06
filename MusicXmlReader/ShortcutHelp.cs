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
            return string.Format("{0} : {1}", s1, s2);
        }

        private void AddLine(string line)
        {
            sb.Append(line + "\r");
        }

        public override string ToString()
        {     
            // The following texts must be localized!   
            const string control = "CONTROL";
            const string space = "SPACE";
            const string escape = "ESCAPE";
            const string enter = "ENTER";
            const string insert = "INSERT";
            const string alt = "ALT";
            const string tab = "TAB";
            const string followedBy = "followed by";
            const string pageDown  = "PAGEDOWN";
            const string shift = "SHIFT";


            sb = new StringBuilder();


            // Følgende standard Windows / JAWS tastatur genveje kan anvendes generelt
            AddLine("-----------------------");
            AddLine(Combine(Plus(control, "O"),  ResourcesForHelp.Shortcut_OpenFileOpenDialog));
            AddLine(Combine(Plus(alt, "F4"),     ResourcesForHelp.Shortcut_CloseProgram));
            AddLine(Combine(alt, ResourcesForHelp.Shortcut_SelectMenuLine));
            AddLine(Combine(tab, ResourcesForHelp.Shortcut_ToggleBetweenListAndFilter));
            AddLine(Combine(Plus(insert,space) + " " + followedBy + " s",ResourcesForHelp.Shortcut_ToggleJAWSSpeechOnOff));
            AddLine(Combine(Plus(insert,pageDown), ResourcesForHelp.Shortcut_ReadStatusLine));
            AddLine(Combine(Plus(insert, "T"), ResourcesForHelp.Shortcut_ReadTitleLine));

            // Følgende specielle tastaturgenveje og kommandoer kan anvendes i forbindelse med Nodelisten
            AddLine("-----------------------");
            AddLine(Combine(Plus(control, "L"), ResourcesForHelp.Shortcut_SelectNoteList));
            AddLine(Combine(Plus(control, "P"), ResourcesForHelp.Shortcut_StartPlaying));
            AddLine(Combine(Plus(control, shift, "P"), ResourcesForHelp.Shortcut_StopPlaying));
            AddLine(Combine(Plus(space), ResourcesForHelp.Shortcut_TogglePlay));
            AddLine(Combine("Tn " + enter, ResourcesForHelp.Shortcut_SetTempo));
            AddLine(Combine("Rn,m " + enter, ResourcesForHelp.Shortcut_RepeatFromNtoM));
            AddLine(Combine("Gn " + enter, ResourcesForHelp.Shortcut_GoToMeasureN));
            AddLine(Combine("ESC", ResourcesForHelp.Shortcut_StopCurrentNote));

            // Følgende specielle tastaturgenveje kan bruges i forbindelse med redigering af nodefilteret:
            AddLine("-----------------------");
            AddLine(Combine(Plus(control, "A"), ResourcesForHelp.Shortcut_ExpandAndEditNoteFilter));
            AddLine(Combine(Plus(control, "F"), ResourcesForHelp.Shortcut_EdirNoteFilter));
            AddLine(Combine(Plus(control, "M"), ResourcesForHelp.Shortcut_EditMusicPlaying));
            AddLine(Combine(Plus(control, "T"), ResourcesForHelp.Shortcut_EditText));
            AddLine(Combine(Plus(control, "B"), ResourcesForHelp.Shortcut_EditMusicBraille));
            AddLine(Combine(Plus(control, "S"), ResourcesForHelp.Shortcut_EditVoices));
            AddLine(Combine(space, ResourcesForHelp.Shortcut_ToggleValue));
            AddLine(Combine(Plus(control, "0"), ResourcesForHelp.Shortcut_TurnOffGlobally));
            AddLine(Combine(Plus(control, "1"), ResourcesForHelp.Shurtcut_TurnOnGlobally));

            return sb.ToString();
        }


        public static ShortcutHelp Create()
        {
            return new ShortcutHelp();
        }
    }
}
