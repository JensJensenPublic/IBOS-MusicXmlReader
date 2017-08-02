using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace MusicXmlReaderModel
{

    /// <summary>
    /// For showing progress in UI during long lasting operations, for instance doc.Load()
    /// </summary>
    public class ProgressWriter
    {
        private string className = "ProgressWriter";
        private int delay;
        private int count;
        private bool running;
        private string text;
        IDebugDisplayerClient client;
        private Thread progressThread;
        private string fullText=null;

        private ProgressWriter()
        { }

        private void ProgressThreadStart()
        {
            if (null == client) // Handle the UI-less case
            {
                return;
            }
            count = 0;
            if (running)
            {
                client.WriteStatusInformation(text);
            }
            while (running)
            {
                Thread.Sleep(delay);
                if (running) // May have been stopped during the wait
                {
                    fullText = string.Format("{0} {1}", count++, text);
                    client.WriteStatusInformation(fullText);
                }
                else
                {
                    fullText = "";
                }
            }
        }

        private ProgressWriter(int delay,IDebugDisplayerClient client,string text)
        {
            running = true;
            this.delay = delay;
            this.text = text;
            this.client = client;
            progressThread = new System.Threading.Thread(new System.Threading.ThreadStart(ProgressThreadStart));
            progressThread.Start();
        }

        public void Stop()
        {
            string functionName = "Stop";
            Logger.Log(string.Format("{0}.{1}", className, functionName));
            running = false;

        }

        public static ProgressWriter Create(int delay, IDebugDisplayerClient client, string text)
        {
            return new ProgressWriter(delay, client, text);
        }


    }

}
