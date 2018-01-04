using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KeyboardTest;

namespace KeyboardTest
{
        public class TestSequencer
    {
        private TargetControl targetControl;
        private int step = 0;
        private Keys expectedKeys = Keys.None;
        public Keys ExpectedKeys { get { return expectedKeys; } }
        private ITestStepReporter testStepReporter;
        private Keyboard keyboard = Keyboard.CreateFocus14Keyboard();

        public void Beep()
        {
            System.Media.SystemSounds.Beep.Play();
        }

        public void FirstStep()
        {
            step = 0;
            NextStep();
        }

        public void NextStep()
        {
            bool found = false;
            while ((0 <= step) && (step < keyboard.TestSteps.Count) && (!found))
            {
                TestStep testStep = keyboard.TestSteps.ElementAt(step);
                if (0 != (int)(testStep.TargetControl & this.targetControl))
                {
                    //testStepReporter.Report("");
                    testStepReporter.Report(testStep.ToString(keyboard));
                    //listBox.Items.Add(""); // An empty line to mark start of the next test            
                    //listBox.Items.Add(testStep);
                    //listBox.SelectedIndex = listBox.Items.Count - 1; // Select the newly added line
                    //if (null != testStepReporter) testStepReporter.Report(testStep.ToString());
                    expectedKeys = testStep.ExpectedKeys;
                    found = true;
                }
                step++;
            }

            if (!found)
            {
                Beep();
                testStepReporter.Report("End of tests!");
                //listBox.Items.Add("End of tests!");
                //listBox.SelectedIndex = listBox.Items.Count - 1; // Select the newly added line
            }
        }


        private TestSequencer(Keyboard keyboard, TargetControl targetControl, ITestStepReporter testStepReporter)
        {
            this.keyboard = keyboard;
            this.targetControl = targetControl;
            this.testStepReporter = testStepReporter;
            this.testStepReporter.Report(string.Format("Keyboard={0} TargetControl={1}", keyboard.ToString(), targetControl.ToString()));
        }

        public static TestSequencer Create(Keyboard keyboard, TargetControl targetControl, ITestStepReporter testStepReporter)
        {
            return new TestSequencer(keyboard, targetControl, testStepReporter);
        }

        public static TestSequencer Create(Keyboard keyboard)
        {
            return new TestSequencer(keyboard, TargetControl.All, null);
        }

    }
}
