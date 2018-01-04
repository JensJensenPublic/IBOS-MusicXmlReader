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

namespace KeyboardDemo
{
    public partial class ParameterInputForm : Form
    {
        public ParameterInputForm(ITestStepReporter testStepReporter,KeyboardNameEnum keyboardName)
        {
            //this.mainForm = mainForm;
            InitializeComponent();
            this.KeyPreview = true;
            this.testStepReporter = testStepReporter;
            this.keyboardName = keyboardName;             
        }

        public string Caption { get { return this.Text; } set { this.Text = value; } }

        public  ParameterDescription parameterDescription;

        private KeyEventNode formPreviewKeyEventNode;
        public  KeyEventNode FormPreviewKeyEventNode {set {formPreviewKeyEventNode = value; } }

        private KeyEventNode comboBoxKeyEventNode;
        public  KeyEventNode ComboBoxKeyEventNode { set { comboBoxKeyEventNode = value; } }

        private KeyEventNode textBoxKeyEventNode;
        public  KeyEventNode TextBoxKeyEventNode { set { textBoxKeyEventNode = value; } }

        public string ComboBoxInput { get { return comboBox1.Text; } }

        private ITestStepReporter testStepReporter;
        private KeyboardNameEnum keyboardName;
        private TestSequencer testSequencer;
        private Keyboard keyboard;


        //***********************************
        // Keyhandlers for ParameterInputForm
        //*********************************** 

        private const string parmFormName = "ParmForm";       

        private void ParameterInputForm_KeyDown(object sender, KeyEventArgs e)
        {
            if ((formPreviewKeyEventNode.Checked) && (formPreviewKeyEventNode.KeyDownNodeChecked))
            {
                Logger.LogKeyDown(parmFormName, e, true);
            }
        }

        private void ParameterInputForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((formPreviewKeyEventNode.Checked) && (formPreviewKeyEventNode.KeyPressedNodeChecked))
            {
                Logger.Log(parmFormName, e);
            }
        }

        private void ParameterInputForm_KeyUp(object sender, KeyEventArgs e)
        {
            if ((formPreviewKeyEventNode.Checked) && (formPreviewKeyEventNode.KeyUpNodeChecked))
            {
                Logger.LogKeyUp(parmFormName, e, true);
            }
        }


        //************************
        // Keyhandlers for ComboBox
        //************************ 

        private const string comboboxName = "Combobox";

        private void comboBox1_KeyDown(object sender, KeyEventArgs e)
        {
            KeyEventNode node = comboBoxKeyEventNode;
            if ((node.Checked) && (node.KeyDownNodeChecked))
            {
                Logger.LogKeyDown(comboboxName, e,false);
            }
        }

        private void comboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyEventNode node = comboBoxKeyEventNode;
            if ((node.Checked) && (node.KeyPressedNodeChecked))
            {
                Logger.Log(comboboxName, e);
            }

            bool ok = parameterDescription.CheckSyntax(comboBox1.Text+ e.KeyChar.ToString());
            if (!ok)
            {
                System.Media.SystemSounds.Beep.Play();
                Console.WriteLine("{0} Syntax error!",comboboxName);
            }
        }


        private void comboBox1_KeyUp(object sender, KeyEventArgs e)
        {
            KeyEventNode node = comboBoxKeyEventNode;
            if ((node.Checked) && (node.KeyUpNodeChecked))
            {
                Logger.LogKeyUp(comboboxName, e,false);
            }
            switch (e.KeyData)
            {
                case Keys.Enter:
                    Console.WriteLine("ComboBox closes");
                    this.Close();
                    break;
                default: break;
            }

        }

        private void ParameterInputForm_Load(object sender, EventArgs e)
        {
            switch (keyboardName)
            {
                case KeyboardNameEnum.Braillant: keyboard = Keyboard.CreateBraillantKeyboard(); break;
                case KeyboardNameEnum.BrailleNoteTouch: keyboard = Keyboard.CreateBrailleNoteTouchKeyboard(); break;
                case KeyboardNameEnum.Focus14: keyboard = Keyboard.CreateFocus14Keyboard(); break;
                case KeyboardNameEnum.HimsEdge: keyboard = Keyboard.CreateKeyboardHimsEdgeKeyboard(); break;
                default:
                    testStepReporter.Report(string.Format("Unsupported keyboard:{0}", keyboardName.ToString())); return;
            }

            testSequencer = TestSequencer.Create(keyboard,TargetControl.ComboBox, testStepReporter);
            testSequencer.NextStep();
        }

        //************************
        // Keyhandlers for TextBox
        //************************ 

        private const string textboxName = "Textbox ";

        private void textBox1_KeyDown_1(object sender, KeyEventArgs e)
        {
            KeyEventNode node = textBoxKeyEventNode;
            if ((node.Checked) && (node.KeyDownNodeChecked))
            {
                Logger.LogKeyDown(textboxName, e,false);
            }
        }

        private void textBox1_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            KeyEventNode node = textBoxKeyEventNode;
            if ((node.Checked) && (node.KeyPressedNodeChecked))
            {
                Logger.Log(textboxName, e);
            }
        }

        private void textBox1_KeyUp_1(object sender, KeyEventArgs e)
        {
            KeyEventNode node = textBoxKeyEventNode;
            if ((node.Checked) && (node.KeyUpNodeChecked))
            {
                Logger.LogKeyUp(textboxName, e,false);
            }
        }

        private void ParameterInputForm_MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left: testSequencer.NextStep(); break;
                case MouseButtons.Middle: break;
//                case MouseButtons.Right: mainForm.ListBox.Items.Clear(); mainForm.ListBox.Refresh(); break;
                case MouseButtons.Right: testStepReporter.Clear(); break;
                case MouseButtons.XButton1: break;
                case MouseButtons.XButton2: break;
                default: break;
            }

        }
    }
}
