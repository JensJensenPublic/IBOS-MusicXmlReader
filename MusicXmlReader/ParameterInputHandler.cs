using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MusicXmlReaderModel;

namespace MusicXmlReader
{
    /// <summary>
    /// Class for handling all parameter input via the ParameterInputForm
    /// </summary>
    public class ParameterInputHandler
    {
        private string className = "ParameterInputHandler";
        private Model model;
        private MainForm mainForm;

        private void ReportSyntax(bool ok, string name, int value, string input)
        {
            const string functionName = "ReportSyntax";
            string s;
            if (ok)
            {
                s = name + value.ToString(); // For test 
            }
            else
            {
                s = name + ": Invalid systax " + input;
            }
            Logger.Log(string.Format("{0}.{1} {2}", className, functionName, s));
        }

        private DialogResult ShowParameterInputForm(ParameterDescription p, out string parameters)
        {
            const string functionName = "ShowParameterInputForm";
            ParameterInputForm parameterInputForm = new ParameterInputForm();
            parameterInputForm.ParameterDescription = p;
            parameterInputForm.Text = p.Name;
            // Show testDialog as a modal dialog and determine if DialogResult = OK.
            DialogResult dialogResult = parameterInputForm.ShowDialog(mainForm);
            Logger.Log(string.Format("{0}.{1} returned {2}", className, functionName, dialogResult));
            parameters = parameterInputForm.ComboBoxInput;
            parameterInputForm.Dispose();
            return dialogResult;
        }        

        public void RepeatToolStripMenuItem_Click()
        {
            const string functionName = "repeatToolStripMenuItem_Click";
            Logger.Log(string.Format("{0}.{1}", className, functionName));
            RepeatParameterDescription p = new RepeatParameterDescription();
            string input = "";
            if (DialogResult.OK != ShowParameterInputForm(p, out input)) return;
            int n1 = 0;
            int n2 = 0;
            bool ok = (p.CheckSyntax(input, out n1, out n2));
            string s;
            if (ok)
            {
                s = "Repeat from " + n1 + " to " + n2;
            }
            else
            {
                UiUtilities.Beep();
                s = "Repeat: Invalid systax " + input;
            }
            Logger.Log(string.Format("{0}.{1} {2}", className, functionName, s));
            int iStart = 0;
            int iStop = 0;
            if (ok && (n1 >= 0) && (n2 >= 0) && (model.MeasureToIndex(n1, ref iStart)) && (model.MeasureToIndex(n2 + 1, ref iStop)))
            {
                model.StartRepeating(iStart, iStop + 1); // Means "Repeat [measure n1 to measure n2]"
            }
            else
            {
                UiUtilities.Beep();
                model.StopRepeating();
            }
        }

        public void exportMusicBrailleToFileToolStripMenuItem_Click()
        {
            Logger.LogCF("");
            ExportMusicBrailleToFileParameterDescription p = new ExportMusicBrailleToFileParameterDescription();
            string input = "";
            if (DialogResult.OK != ShowParameterInputForm(p, out input)) return ;
            int n1 = 0;
            int n2 = 0;
            bool ok = (p.CheckSyntax(input, out n1, out n2));
            string s;
            if (ok)
            {
                s = "Number of character per line = " + n1 + " Number of lines per form = " + n2;
                model.UserPreferences.CharsPerLine = n1;
                model.UserPreferences.LinesPerForm = n2;
            }
            else
            {
                UiUtilities.Beep();
                s = "ExportMusicBrailleToFile: Invalid systax " + input;
            }
            Logger.LogCF(string.Format(": {0}", s));

            return;
        }



        public bool GoToToolStripMenuItem_Click(out int index)
        {
            index = -1;
            string name = Utilities.RemoveAmpersant(ResourcesForUI.ParameterInputForm_GoTo); // "Localize(GoTo)";
            const string functionName = "goToToolStripMenuItem_Click";
            Logger.Log(string.Format("{0}.{1}", className, functionName));
            SingleIntParameterDescription p = new SingleIntParameterDescription(name);
            string input = ""; ;
            if (DialogResult.OK != ShowParameterInputForm(p, out input)) return false;
            int value = 0;
            bool ok = (p.CheckSyntax(input, out value));
            ReportSyntax(ok, name, value, input);

            if (ok && (value >= 0) && model.MeasureToIndex(value, ref index))
            {
                return true;
            }
            else
            {
                UiUtilities.Beep();
                Logger.Log(string.Format("{0}.{1} Illegal GoTo-command:'{2}'", className, functionName, input));
                return false;
            }
        }

        public void OfNominalTempoToolStripMenuItem_Click()
        {
            const string functionName = "ofNominalTempoToolStripMenuItem_Click";
            Logger.Log(string.Format("{0}.{1}", className, functionName));
            string name = Utilities.RemoveAmpersant(ResourcesForUI.ParameterInputForm_PctOfNominalTempo); // "Localize(% of &Normalt Tempo)";
            SingleIntParameterDescription p = new SingleIntParameterDescription(name);
            string input = "";
            if (DialogResult.OK != ShowParameterInputForm(p, out input)) return;
            int value = 0;
            bool ok = (p.CheckSyntax(input, out value));
            ReportSyntax(ok, name, value, input);

            if (ok && (value >= UiUtilities.TempoFactorMinimum) && (value <= UiUtilities.TempoFactorMaximum))
            {
                model.SetUserTempo(value);
            }
            else
            {
                UiUtilities.Beep();
                Logger.Log(string.Format("{0}.{1} Illegal Tempo-Command:'{2}'", className, functionName, input));
            }
        }
        
        private ParameterInputHandler(Model model, MainForm mainForm)
        {
            this.model = model;
            this.mainForm = mainForm;
        }
        
        public static ParameterInputHandler Create(Model model, MainForm mainForm)
        {
            return new ParameterInputHandler(model, mainForm);
        }

    }
}
