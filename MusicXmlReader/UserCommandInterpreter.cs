using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MusicXmlReaderModel;

namespace MusicXmlReader
{
    class UserCommandInterpreter
    {

        private Model model;
        private StringBuilder command;
        private TextBox textBox;
        private ListBox listBox;
        private string className = "CommandInterpreter";

        private UserCommandInterpreter()
        {}

        private UserCommandInterpreter(TextBox textBox,ListBox listBox, Model model)
        {
            this.model = model;
            this.listBox = listBox;
            this.textBox = textBox;
            command = new StringBuilder();
        }

        private void Beep()
        {
            UiUtilities.Beep();
        }


        /// <summary>
        /// Convert to chars CONTROL+A..CONTROL+Z or digits 0 .. 9
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        private string KeyToString(Keys keys)
        {
            int key = (int)keys;
            // Keys controlKeys = Keys.None;   // When using alphanumeric keys alone
            Keys controlKeys = Keys.Control; // When using alphanumeric keys combined with the CONTROL, SHIFT or ALT
            if (((int)(Keys.A | controlKeys) <= key) && (key <= (int)(Keys.Z | controlKeys)))
            {
                int c = (int)'A' + key - (int)(Keys.A | controlKeys) ;
                return ((char)c).ToString();
            }

            if (((int)Keys.D0 <= key) && (key <= (int)Keys.D9))
            {
                int c = (int)'0' + key - (int)Keys.D0;
                return ((char)c).ToString();
            }

            if (Keys.Oemcomma == keys) return ",";

#warning ToDo Find another key here instead of Keys.Multiply which requires a numeric keyboard !! 
            if ((Keys.Control | Keys.Multiply) == keys)
            {
                return "*";          // On the numeric keyboard
            }
            if ((Keys.Control | Keys.Shift | Keys.Oem2) == keys)
            {
                return "*"; // The asterisk on some keyboards  
            } 

            return ""; 
        }

        /// <summary>
        /// Interpret the parameter as an index
        /// </summary>
        /// <param name="paramaters"></param>
        /// <param name="command"></param>
        private void GotoIndex(string[] paramaters,string command)
        {
            // If the command had the form "Gn1 where n1 represent an integer we go to that index 
            string functionName = "GotoIndex";
            int n1 = 0;
            if ((1 == paramaters.Length)
            && (int.TryParse(paramaters[0], out n1)) // First parameter
            && (n1 >= 0)
            && (n1 < listBox.Items.Count)
            )
            {
                listBox.SelectedIndex = n1;
            }
            else
            {
                Beep();
                Logger.Log(string.Format("{0}.{1} Illegal command:'{2}'", className, functionName, command));
            }    
        }

        /// <summary>
        /// Interpret the parameter as a measure number
        /// </summary>
        /// <param name="paramaters"></param>
        /// <param name="command"></param>
        private void GotoMeasure(string[] paramaters, string command)
        {
            // If the command had the form "Gn1 where n1 represent an integer we go to that index 
            string functionName = "GotoMeasure";
            int index = 0;
            int n1 = 0;
            if ((1 == paramaters.Length)
            && (int.TryParse(paramaters[0], out n1)) // First parameter
            && (n1 >= 0)
            && model.MeasureToIndex(n1,ref index)
            )
            {
                listBox.SelectedIndex = index;
            }
            else
            {
                Beep();
                Logger.Log(string.Format("{0}.{1} Illegal command:'{2}'", className, functionName, command));
            }
        }

        
        /// <summary>
        /// Interpret the parameters as indices
        /// </summary>
        /// <param name="paramaters"></param>
        /// <param name="command"></param>
        private void RepeatIndices(string[] paramaters, string command)
        {
            // If the command had the form "R,n1,n2" where n1 and represent integers we start repeating, else we cancel it:
            int n1 = 0;
            int n2 = 0;
            if ((2 == paramaters.Length)
            && (int.TryParse(paramaters[0], out n1)) // First parameter
            && (int.TryParse(paramaters[1], out n2)) // Second parameter
            && (n1 >= 0)
            && (n2 >= 0)
            && (n2 >= n1)
            )
            {
                model.StartRepeating(n1, n2+1); // Means "Repeat [index n1 to index n2]"
            }
            else
            {
                model.StopRepeating();
            }
        }

        /// <summary>
        /// Interpret the parameters as measure numbers
        /// </summary>
        /// <param name="paramaters"></param>
        /// <param name="command"></param>
        private void RepeatMeasures(string[] paramaters, string command)
        {
            // If the command had the form "R,n1,n2" where n1 and represent integers we start repeating, else we cancel it:
            int n1 = 0;
            int n2 = 0;
            int iStart = 0;
            int iStop = 0;

            if ((2 == paramaters.Length)
            && (int.TryParse(paramaters[0], out n1)) // First parameter
            && (int.TryParse(paramaters[1], out n2)) // Second parameter
            && (n1 >= 0)
            && (n2 >= 0)
            && (model.MeasureToIndex(n1, ref iStart))
            && (model.MeasureToIndex(n2+1, ref iStop))
            )
            {
                model.StartRepeating(iStart, iStop+1); // Means "Repeat [measure n1 to measure n2]"
            }
            else
            {
                model.StopRepeating();
            }
        }
        

        private void Tempo(string[] paramaters, string command)
        {
            // If the command had the form "Tn1 where n1 represent an integer we modify the playback speed to n1% of the value stated by the score.
            string functionName = "Tempo";
            int n1 = 0;
            if ((1 == paramaters.Length)
            && (int.TryParse(paramaters[0], out n1)) // First parameter
            && (n1 >= UiUtilities.TempoFactorMinimum)
            && (n1 <= UiUtilities.TempoFactorMaximum)
            )
            {
                model.SetUserTempo(n1);
            }
            else
            {
                Beep();
                Logger.Log(string.Format("{0}.{1} Illegal TempoCommand:'{2}'", className, functionName, command));
            }
        }




        //public void Add(KeyEventArgs args)
        //{
        //    string functionName = "Add";
        //    if (args.KeyData == Keys.Return)
        //    {
        //        // Interpret and execute command
        //        string s = command.ToString();
        //        command.Clear();
        
        //        if (s.Length > 0)
        //        {
        //            string[] parameters = s.Substring(1).Split(new char[] { ',' });
        //            switch (s[0])
        //            {
        //                case ShortcutHandler.Repeat:        RepeatMeasures(parameters, s); break;
        //                case ShortcutHandler.GoTo:          GotoMeasure(parameters, s); break;
        //                case ShortcutHandler.NormalTempo:   Tempo(parameters, s); break;
        //                default:
        //                    Logger.Log(string.Format("{0}.{1} Illegal Command={2}", className, functionName, s));
        //                    System.Media.SystemSounds.Beep.Play(); break;
        //            }
        //        }

        //        command.Clear();
        //    }
        //    else if (Keys.Back == args.KeyData)
        //    {
        //        if (command.Length > 0)
        //        {
        //            command.Remove(command.Length - 1, 1); // Remove last character
        //        }                   
        //    }
        //    else
        //    {
        //        command.Append(KeyToString(args.KeyData));
        //    }

        //    textBox.Text = command.ToString();

        //}

        public static UserCommandInterpreter Create(TextBox textBox,ListBox listBox,Model model)
        {
            return new UserCommandInterpreter(textBox,listBox,model);
        }
    
    }
}
