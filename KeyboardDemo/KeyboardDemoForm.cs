using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KeyboardTest;
using System.IO;
using System.Reflection;



namespace KeyboardDemo
{
    public partial class KeyboardDemoForm : Form, ITestStepReporter, IKeyboardSelectorClient
    {
        public KeyboardDemoForm()
        {
            InitializeComponent();
            UserInit();
        }

        private SpeechSynthesizer speechSynthesizer;
        public  SpeechSynthesizer SpeechSynthesizer { get { return speechSynthesizer; } }

        public ListBox ListBox { get { return this.listBox1; } }

        private TestSequencer testSequencer;
        private string applicationName = "KeyboardDemo";

        private void UserInit()
        {

            // Init TreeView

            // Nodes for controlling the main form
            MainFormEventNode = new KeyEventNode("MainFormPreview");
            MainFormEventNode.Checked = false;
            ListboxKeyEventNode = new KeyEventNode("Listbox    ");
            TreeviewKeyEventNode = new KeyEventNode("TreeView   ");
            treeView.Nodes.Add(MainFormEventNode);
            treeView.Nodes.Add(ListboxKeyEventNode);
            treeView.Nodes.Add(TreeviewKeyEventNode);

            // Nodes for controlling ParameterInputForm
            ParmFormKeyEventNode = new KeyEventNode("ParmFormPreview");
            ParmFormKeyEventNode.Checked = false;
            ComboBoxKeyEventNode = new KeyEventNode("ComboBox");
            TextBoxKeyEventNode = new KeyEventNode("TextBox");
            treeView.Nodes.Add(ParmFormKeyEventNode);
            treeView.Nodes.Add(ComboBoxKeyEventNode);
            treeView.Nodes.Add(TextBoxKeyEventNode);

            treeView.ExpandAll();
            treeView.AfterCheck += TreeView_AfterCheck;

            // Init Listbox
            listBox1.Items.Add("Line1");
            listBox1.Items.Add("Line2");
            listBox1.Items.Add("Line3");
            listBox1.Items.Add("Line4");
            listBox1.Items.Add("Line5");

            speechSynthesizer = SpeechSynthesizer.Create();
            Logger.TestStepreporter = this as ITestStepReporter;
            Utilities.LogJawsConfiguration();

            Console.WriteLine("Detecting keyboard");
            KeyboardNameEnum keyboardNameEnum;
            try            {
                keyboardNameEnum = Utilities.LogUsbDevices();
                WriteKeyboardName(keyboardNameEnum);             
            }
            catch (Exception e)
            {
                Console.WriteLine(string.Format("Failed to detect keyboard. MEssage={0}", e.Message));
            }

            // Test();

        }
        private string press = " TRYK ";

        private void Test(Keyboard keyboard,Keys keys)
        {
            PerkinsKeySequence pks = keyboard.GetPerkinsSequence(keys);
            Console.WriteLine(keys.ToString() + press  + pks.ToString());
        }

        private void Test(Keyboard keyboard, KeySequenceList keySequenceList)
        {
            PerkinsKeySequence pks = keyboard.GetPerkinsSequence(keySequenceList);
            Console.WriteLine(keySequenceList.ToString() + press + pks.ToString());
        }


        private void Test()
        {
            Keyboard keyboard = Keyboard.CreateFocus14Keyboard();

            // TEst simple Keys
            Test(keyboard, Keys.A);
            Test(keyboard, Keys.Control | Keys.A);
            Test(keyboard, Keys.Control | Keys.Q); // Not implemented
            Test(keyboard, Keys.Control | Keys.Space);  // Not implemented

            // Test special JAWS sequences
            Test(keyboard, KeySequenceList.JawsToggleSpeech);
            Test(keyboard, KeySequenceList.JAWSReadMessage);
            Test(keyboard, KeySequenceList.JAWSReadTitleLine);
            Test(keyboard, KeySequenceList.JAWSReadStatusLine);
        }


        //// Generic key handlers

        private void TreeView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            string text = e.Node.Text;
            //bool checkValue = e.Node.Checked;
            //switch (text)
            //{
            //    case mainFormKeyPreview: this.KeyPreview = checkValue; Logger.Log(string.Format("{0} is {1}", mainFormKeyPreview, checkValue)); break;
            //    case parmFormKeyPreview: this.parameterFormKeyPreview = checkValue; Logger.Log(string.Format("{0} is {1}", parmFormKeyPreview, checkValue)); break;
            //    default:
            //        Logger.Log("No action");break;
            //}

            Logger.Log("TreeView", e);
        }


        /// <summary>
        /// Implements ITestStepReporter
        /// </summary>
        /// <param name="s"></param>
        public void Report(string s)
        {
            speechSynthesizer.Speak(s);
            //listBox1.Items.Add(s);
            //listBox1.Refresh();
            Console.WriteLine(s);
        }

        public void Clear()
        {
            Report("ITestStepReporter.Clear()");
        }


        private string ShowParameterInputForm(ParameterDescription parameterDescription,KeyEventNode formPreviewKeyEventNode, KeyEventNode comboBoxKeyEventNode, KeyEventNode textBoxKeyEventNode)
        {
            ITestStepReporter testStepReporter = this as ITestStepReporter;
            ParameterInputForm parameterInputForm = new ParameterInputForm(testStepReporter,keyboardName);
            parameterInputForm.parameterDescription = parameterDescription;
            parameterInputForm.KeyPreview = true; // keyEventNode.Checked;
            parameterInputForm.FormPreviewKeyEventNode = formPreviewKeyEventNode;
            parameterInputForm.ComboBoxKeyEventNode = comboBoxKeyEventNode;
            parameterInputForm.TextBoxKeyEventNode  = textBoxKeyEventNode;

            parameterInputForm.Caption = parameterDescription.Name;

            // Show testDialog as a modal dialog and determine if DialogResult = OK.
            if (parameterInputForm.ShowDialog(this) == DialogResult.OK)
            {
                // Read the contents of testDialog's TextBox.
                //this.txtResult.Text = parameterInputForm.TextBox1.Text;
            }
            else
            {
                //this.txtResult.Text = "Cancelled";
            }
            string result = parameterInputForm.ComboBoxInput;
            parameterInputForm.Dispose();
            return result ;
        }

        private KeyEventNode ParmFormKeyEventNode = new KeyEventNode("ParmForm");
        private KeyEventNode ComboBoxKeyEventNode = new KeyEventNode("ComboBox");
        private KeyEventNode TextBoxKeyEventNode = new KeyEventNode("TextBox");


        /// <summary>
        /// Simple convenience method!
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private string ShowParameterInputForm(ParameterDescription p)
        {
            return ShowParameterInputForm(p, ParmFormKeyEventNode, ComboBoxKeyEventNode, TextBoxKeyEventNode);
        }

        /// <summary>
        /// Shortcut keys handled by a menuItem are appearantly not reported as KeyDown. Instead they can be reported in this way:
        /// </summary>
        /// <param name="functionName"></param>
        /// <param name="sender"></param>
        private void ReportMenuItemClick(string caption,object sender)
        {
            ToolStripMenuItem sendingItem = sender as ToolStripMenuItem;
            this.Report(string.Format("Genvej('{0}') : {1}",caption,Utilities.KeysToString(sendingItem.ShortcutKeys)));
        }

        /// <summary>
        /// Tests the CONTROL-G shortcut, but without showing the ParameterInputForm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gotoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string name = "GoTo";
            ReportMenuItemClick(name,sender);
            return;
            //SingleIntParameterDescription p = new SingleIntParameterDescription(name);
            //string input = ShowParameterInputForm(p);
            //int value = 0;
            //bool ok = (p.CheckSyntax(input, out value));
            //ReportSyntax(ok, name, value, input);
        }

        /// <summary>
        /// Tests the CONTROL-R shortcut, but without showing the ParameterInputForm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void repeatToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReportMenuItemClick("Repeat",sender);
            return;
            //RepeatParameterDescription p = new RepeatParameterDescription();
            ////string input = ShowParameterInputForm(p, ParmFormKeyEventNode, ComboBoxKeyEventNode, TextBoxKeyEventNode);
            //string input = ShowParameterInputForm(p); //, ParmFormKeyEventNode, ComboBoxKeyEventNode, TextBoxKeyEventNode);
            //int first = 0;
            //int last  = 0;
            //bool ok = (p.CheckSyntax(input, out first, out last)) ;
            //if (ok)
            //{
            //    this.Text = "Repeat from " + first + " to " + last;
            //}
            //else
            //{
            //    this.Text = "Repeat: Invalid systax " + input;
            //}
    
        }

        /// <summary>
        /// Tests the CONTROL-N "% of &NormalTempo" shortcut, but without showing the ParameterInputForm
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tempoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string name = "% af &Normalt Tempo ";
            ReportMenuItemClick(name,sender);
            return;
            //TempoParameterDescription p = new TempoParameterDescription();
            //// string input = ShowParameterInputForm(p, ParmFormKeyEventNode, ComboBoxKeyEventNode, TextBoxKeyEventNode);
            //string input = ShowParameterInputForm(p); // , ParmFormKeyEventNode, ComboBoxKeyEventNode, TextBoxKeyEventNode);
            //int value = 0;
            //bool ok = (p.CheckSyntax(input, out value));
            //ReportSyntax(ok, name, value, input);
        }


        /// <summary>
        /// Tests the CONTROL-P  shortcut, and show the ParameterInputForm for further testing of input to a ComboBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void parameterInputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string name = "Test ";
            SingleIntParameterDescription p = new SingleIntParameterDescription(name);
            // string input = ShowParameterInputForm(p, ParmFormKeyEventNode, ComboBoxKeyEventNode, TextBoxKeyEventNode);
            string input = ShowParameterInputForm(p); // , ParmFormKeyEventNode, ComboBoxKeyEventNode, TextBoxKeyEventNode);
            int value = 0;
            bool ok = p.CheckSyntax(input, out value);
            ReportSyntax(ok, name, value, input);
        }

        private void ReportSyntax(bool ok, string name, int value, string input)
        {
            if (ok)
            {
                this.Text = name + value.ToString(); // For test 
            }
            else
            {
                this.Text = name + ": Invalid systax " + input;
            }
        }




    // *********************
    // Treeview Key Handlers
    //**********************
    private const string TreeviewName = "Treeview";
        private KeyEventNode TreeviewKeyEventNode = new KeyEventNode(TreeviewName);

        private void treeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (TreeviewKeyEventNode.Checked && TreeviewKeyEventNode.KeyDownNodeChecked)
            {
                Logger.LogKeyDown(TreeviewName, e, false);
            }
        }

        private void treeView_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (TreeviewKeyEventNode.Checked && TreeviewKeyEventNode.KeyPressedNodeChecked)
            {
                Logger.Log(TreeviewName, e);
            }
        }

        private void treeView_KeyUp(object sender, KeyEventArgs e)
        {
            if (TreeviewKeyEventNode.Checked && TreeviewKeyEventNode.KeyUpNodeChecked)
            {
                Logger.LogKeyUp(TreeviewName, e, false);
            }
        }

        //**********************
        // Listbox1 Key Handlers
        //**********************

        private const string ListboxName = "ListBox "; // Add space to align logging
        private KeyEventNode ListboxKeyEventNode = new KeyEventNode(ListboxName);
   

        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (ListboxKeyEventNode.Checked && ListboxKeyEventNode.KeyDownNodeChecked)
            {
                Logger.LogKeyDown(ListboxName, e, false);
            }
        }

        private void listBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (ListboxKeyEventNode.Checked && ListboxKeyEventNode.KeyPressedNodeChecked)
            {
                Logger.Log(ListboxName, e);
            }
        }

        private void listBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (ListboxKeyEventNode.Checked && ListboxKeyEventNode.KeyUpNodeChecked)
            {
                Logger.LogKeyUp(ListboxName, e, false);
            }
        }

        //***************************
        // Form preview Key handlers
        //***************************

        private const string MainFormName = "MainForm";
        private KeyEventNode MainFormEventNode = new KeyEventNode(MainFormName);


        private void KeyboaedDemoForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (MainFormEventNode.Checked && MainFormEventNode.KeyDownNodeChecked)
            {
                Logger.LogKeyDown(MainFormName, e, true);
            }
        }

        private void KeyboaedDemoForm_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (MainFormEventNode.Checked && MainFormEventNode.KeyPressedNodeChecked)
            {
                Logger.Log(MainFormName, e);
            }
        }

        private void KeyboaedDemoForm_KeyUp(object sender, KeyEventArgs e)
        {

            if (MainFormEventNode.Checked && MainFormEventNode.KeyUpNodeChecked)
            {
                Logger.LogKeyUp(MainFormName, e, true);
            }
        }

        private void CommonMouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left: testSequencer.NextStep(); break;
                case MouseButtons.Middle: break;
                case MouseButtons.Right: testSequencer.FirstStep(); break; //   listBox1.Items.Clear(); listBox1.Refresh(); break;
                case MouseButtons.XButton1: break;
                case MouseButtons.XButton2: break;
                default: break;
            }
        }

        private void CommonEnter(TargetControl targetControl)
        {
            //Keyboard keyboard = Keyboard.CreateFocus14Keyboard();
            Keyboard keyboard;
            switch (keyboardName)
            {

                case KeyboardTest.KeyboardNameEnum.Braillant: keyboard = Keyboard.CreateBraillantKeyboard(); break;
                case KeyboardNameEnum.BrailleNoteTouch: keyboard = Keyboard.CreateBrailleNoteTouchKeyboard(); break;
                case KeyboardNameEnum.Focus14:   keyboard = Keyboard.CreateFocus14Keyboard(); break;
                case KeyboardNameEnum.HimsEdge: keyboard = Keyboard.CreateKeyboardHimsEdgeKeyboard(); break;
                case KeyboardNameEnum.HimsU2: keyboard = Keyboard.CreateKeyboardHimsU2Keyboard(); break;
                default:
                    keyboard = Keyboard.CreateFocus14Keyboard();
                    Report(string.Format("Unsupported keyboard:{0}.  Using FOCUS14 instead", keyboardName.ToString())); break;
            }
            testSequencer = TestSequencer.Create(keyboard, targetControl, this as ITestStepReporter);
            testSequencer.NextStep();
        }



    private void listBox1_MouseDown(object sender, MouseEventArgs e)
        {
            CommonMouseDown(sender, e);
        }

        private void listBox1_Enter(object sender, EventArgs e)
        {
            CommonEnter(TargetControl.ListBox);
        }

        private void treeView_Enter(object sender, EventArgs e)
        {
            CommonEnter(TargetControl.TreeView);
        }

        private void treeView_MouseDown(object sender, MouseEventArgs e)
        {
            CommonMouseDown(sender, e);
        }

        private KeyboardSelectorForm keyboardSelectorForm;
        private KeyboardTest.KeyboardNameEnum keyboardName; 
        private void keyboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReportMenuItemClick("Keyboard",sender);
            keyboardSelectorForm = new KeyboardSelectorForm(keyboardName, this as IKeyboardSelectorClient);
            keyboardSelectorForm.Show();
            //keyboardName = keyboardSelectorForm.KeyboardName;
            //Console.WriteLine(keyboardName.ToString());
        }

        // Implementpublic interface IKeyboardSelectorClient

        public void WriteKeyboardName(KeyboardNameEnum keyboardName)
        {
            this.keyboardName = keyboardName;
            Console.WriteLine(string.Format("Keyboard={0}", keyboardName.ToString()));
            this.Text = string.Format("{0} for {1}", applicationName, keyboardName);

            switch (keyboardName)
            {
                case KeyboardNameEnum.HimsEdge: Report("Tryk 2 3 4 5 7 MELLEMRUM for at aktivere Hims Edge indtastningstilstand!"); break;
                default: break;
            }


        }

        public KeyboardNameEnum ReadKeyboardName()
        {
            return keyboardName;
        }

        private void messageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReportMenuItemClick("Message",sender);
            MessageBox.Show("Test JAWS INSERT B:     'Read Message'");
        }

        private void åbnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ReportMenuItemClick("Åbn",sender);
            openFileDialog.FileName = ""; // No default
            openFileDialog.Filter = string.Format("{0}|*.xml;*.mxl", "MusicXml filer"); // Only present .xml files and .mxl files
            string documentPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments); // C:\Users\<Username>\Documents   
            openFileDialog.InitialDirectory = Path.Combine(documentPath, applicationName);              //   C:\Users\<Username>\Documents\KeyboardDemo
            openFileDialog.CheckFileExists = true;
            openFileDialog.CheckPathExists = true;
            openFileDialog.ShowDialog();

        }

    }
}
