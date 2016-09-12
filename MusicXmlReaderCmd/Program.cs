using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MusicXmlReaderUI
{
    class Program
    {
        static void Main(string[] args)
        {
            Model model = Model.Create();
            Console.WriteLine(string.Format("Model.Create {0}", (model != null) ? "succeeded" : "failed"));
            string fullFileName = @"C:\Users\Jens\Dropbox\Root\Visual Studio 2015\Projects\MusicXmlReaderUI\MusicXmlReaderUI\bin\Debug\MusicXml samples\OpenMusicScore.org\Revolutionary Study.xml";
            bool ok = model.LoadMusicXmlFile(fullFileName);
            Console.WriteLine(string.Format("Model.LoadMusicXmlFile({0}) {1}", fullFileName, ok? "succeeded" : "failed"));      
            model.StartPlayingPoly();
            Console.ReadLine();

        }
    }
}
