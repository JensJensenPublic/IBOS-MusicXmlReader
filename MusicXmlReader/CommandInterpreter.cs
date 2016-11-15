using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MusicXmlReaderModel;

namespace MusicXmlReader
{
    class CommandInterpreter
    {

        private Model model;
        private StringBuilder command;
        private TextBox textBox;

        private CommandInterpreter()
        {}

        private CommandInterpreter(TextBox textBox, Model model)
        {
            this.model = model;
            this.textBox = textBox;
            command = new StringBuilder();
        }


        /// <summary>
        /// Convert to chars A..Z or digits 0 .. 9
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        private string KeyToString(Keys keys)
        {
            int key = (int)keys;
            if (((int)Keys.A <= key) && (key <= (int)Keys.Z))
            {
                int c = (int)'A' + key - (int)Keys.A ;
                return ((char)c).ToString();
            }

            if (((int)Keys.D0 <= key) && (key <= (int)Keys.D9))
            {
                int c = (int)'0' + key - (int)Keys.D0;
                return ((char)c).ToString();
            }

            if (Keys.Oemcomma == keys) return ",";   
         

            return ""; 
        }

        private void Repeat(string[] paramaters)
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
               model.StartRepeating(n1, n2); // Means "Repeat [measure n1 to measure n2]"
            }
            else
            {
                model.StopRepeating();
            }
        }


        public void Add(KeyEventArgs args)
        {
            if (args.KeyData == Keys.Return)
            {
                // Interpret and execute command
                string s = command.ToString();
                command.Clear();
        
                if (s.Length > 0)
                {
                    string[] parameters = s.Substring(1).Split(new char[] { ',' });
                    switch (s[0])
                    {
                        case 'R': Repeat(parameters); break;
                        default: break;
                    }
                }

                command.Clear();
            }
            else if (Keys.Back == args.KeyData)
            {
                if (command.Length > 0)
                {
                    command.Remove(command.Length - 1, 1); // Remove last character
                }                   
            }
            else
            {
                command.Append(KeyToString(args.KeyData));
            }

            textBox.Text = command.ToString();

        }

        public static CommandInterpreter Create(TextBox textBox,Model model)
        {
            return new CommandInterpreter(textBox,model);
        }
    
    }
}
