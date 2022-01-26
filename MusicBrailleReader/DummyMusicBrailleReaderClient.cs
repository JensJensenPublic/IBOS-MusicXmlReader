using System.IO;
using MusicXmlReaderModel;

namespace MusicBrailleReader
{
    /// <summary>
    /// For implementing a dummy version of IMusicBrailleReaderClient when MusicBrailleReaderMainform is instantiated from a simple commandLine application.
    /// </summary>
    class DummyMusicBrailleReaderClient : IMusicBrailleReaderClient
    {
        private Model model;
        private string defaultPath;
        static public DummyMusicBrailleReaderClient Create(string applicationName, Model model) { return new DummyMusicBrailleReaderClient(applicationName, model); }
        private DummyMusicBrailleReaderClient() { } // Prevent construvtion
        private DummyMusicBrailleReaderClient(string applicationName, Model model)
        {
            this.model = model; // Create a Model instance for our own usage. It contains a lot of usable support code

            // Create a Default directory for the UI
            string documentPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments); // C:\Users\<Username>\Documents
            defaultPath = Path.Combine(documentPath, applicationName);
            if (!Directory.Exists(defaultPath))
            {
                Directory.CreateDirectory(defaultPath);
            }
        }

        public void HideForm() { }
        public void ShowForm() { }
        public void EnableForm(bool b) { }
        public string GetLatestBrailleMusicPath() { return defaultPath; }
        public void SetLatestBrailleMusicPath(string s) { } // We don't want to save this information as a user setting !
        public string GetMyMusicXmlDirectory() { return defaultPath; }
    }
}
