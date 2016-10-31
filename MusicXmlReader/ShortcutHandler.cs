using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicXmlReaderModel;

namespace MusicXmlReader
{

    /// <summary>
    /// Class for defining all keyboard shortcuts at on single place instead of scattering them around the code
    /// </summary>
    class ShortcutHandler
    {
        private ShortcutHandler()
        { }

        private MainForm mainForm;
        private Model model;

        private ShortcutHandler(MainForm mainForm, Model model)
        {
            this.mainForm = mainForm;
            this.model = model;
        }

        static public ShortcutHandler Create(MainForm mainForm, Model model)
        {
            return new ShortcutHandler(mainForm, model);
        }
    }
}
