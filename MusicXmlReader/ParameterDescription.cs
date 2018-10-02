using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicXmlReader
{
    public abstract class ParameterDescription
    {
        protected string name;
        public string Name { get { return name;} }
        public abstract bool CheckSyntax(string s, out List<int> values);
        public abstract bool CheckSyntax(string s);
        protected List<int>dummyParameterList;

    }

    public class SingleIntParameterDescription : ParameterDescription
    {
        public SingleIntParameterDescription(string name)
        {
            this.name = name;
        }

        public override bool CheckSyntax(string s, out List<int> values)
        {
            values = new List<int>();
            int value;
            if (0 == s.Length) return true;
            if (int.TryParse(s, out value))
            {
                values.Add(value);
                return true;
            }
            return false;
        }

        public bool CheckSyntax(string s, out int tempo)
        {
            tempo = 0;
            if (CheckSyntax(s, out dummyParameterList) && (1 == dummyParameterList.Count))
            {
                tempo = dummyParameterList[0];
                return true;
            }
            return false;
        }


        public override bool CheckSyntax(string s)
        {
            return CheckSyntax(s, out dummyParameterList);
        }


    }

    public class RepeatParameterDescription : ParameterDescription
    {
        public RepeatParameterDescription()
        {
            name = MusicXmlReaderModel.Utilities.RemoveAmpersant(ResourcesForUI.ParameterInputForm_Repeat);
        }

        private bool IsNullEmptyOrInt(string s,out int value)
        {
            value = 0;
            return (string.IsNullOrEmpty(s) || int.TryParse(s, out value));
        }


        public override bool CheckSyntax(string s, out List<int> values)
        {
            values = new List<int>();
            int temp0 = 0;
            int temp1 = 0;
            if (0 == s.Length) return true;
            char[] separators = new char[1] { ',' };
            string[] strings = s.Split(separators);
            switch (strings.Length)
            {
                case 0: return false;
                case 1:
                    if (int.TryParse(strings[0], out temp0)) // Shorthand for "Repeat a single measure"
                    {
                        values.Add(temp0);
                        values.Add(temp0);
                        return true;
                    }
                    else return false;
                case 2:
                    string s0 = strings[0];
                    string s1 = strings[1];
                    if (IsNullEmptyOrInt(s0, out temp0) && IsNullEmptyOrInt(s1, out temp1))
                    {
                        values.Add(temp0);
                        values.Add(temp1);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                default: return false;
            }
        }

        public bool CheckSyntax(string s, out int first, out int last)
        {
            first = 0;
            last = 0;
            if (CheckSyntax(s, out dummyParameterList) && (2 == dummyParameterList.Count))
            {
                first = dummyParameterList[0];
                last  = dummyParameterList[1];
                return true;
            }
            return false;
        }



        public override bool CheckSyntax(string s)
        {
            return CheckSyntax(s, out dummyParameterList);
        }
    }




    public class ExportMusicBrailleToFileParameterDescription : ParameterDescription
    {
        public ExportMusicBrailleToFileParameterDescription()
        {
            name = ResourcesForUI.ParameterInputForm_BrailleFormatting; // Such as "Insert form size as characters per line , lines per form "
        }

        private bool IsNullEmptyOrInt(string s,out int value)
        {
            value = 0;
            return (string.IsNullOrEmpty(s) || int.TryParse(s, out value));
        }


        public override bool CheckSyntax(string s, out List<int> values)
        {
            values = new List<int>();
            int temp0 = 0;
            int temp1 = 0;
            if (0 == s.Length) return true;
            char[] separators = new char[1] { ',' };
            string[] strings = s.Split(separators);
            switch (strings.Length)
            {
                case 0: return false;
                case 1: return false;
                case 2:
                    string s0 = strings[0];
                    string s1 = strings[1];
                    if (IsNullEmptyOrInt(s0, out temp0) && IsNullEmptyOrInt(s1, out temp1))
                    {
                        values.Add(temp0);
                        values.Add(temp1);
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                default: return false;
            }
        }

        public bool CheckSyntax(string s, out int charsPerLine, out int linesPerForm)
        {
            charsPerLine = 0;
            linesPerForm = 0;
            if (CheckSyntax(s, out dummyParameterList) && (2 == dummyParameterList.Count))
            {
                charsPerLine = dummyParameterList[0];
                linesPerForm = dummyParameterList[1];
                return true;
            }
            return false;
        }


        public override bool CheckSyntax(string s)
        {
            return CheckSyntax(s, out dummyParameterList);
        }
    }
















//    public class TempoParameterDescription : ParameterDescription
//    {
//        public TempoParameterDescription()
//        {
//#warning ToDo Add localization
//            name = "Localize(% af Normalt tempo)";
//        }

//        public override bool CheckSyntax(string s, out List<int> values)
//        {
//            values = new List<int>();
//            int value;
//            if (0 == s.Length) return true;
//            if (int.TryParse(s, out value))
//            {
//                values.Add(value);
//                return true;
//            }
//            return false;
//        }

//        public override bool CheckSyntax(string s)
//        {
//            return CheckSyntax(s, out dummyParameterList);
//        }

//        public bool CheckSyntax(string s,out int tempo)
//        {
//            tempo = 0;
//            if (CheckSyntax(s, out dummyParameterList) && (1 == dummyParameterList.Count))
//            {
//                tempo = dummyParameterList[0];
//                return true;
//            }
//            return false;
//        }        
//    }
}

