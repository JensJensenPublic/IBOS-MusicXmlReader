using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using MusicXmlReaderModel;

namespace MusicBrailleReader
{
    public interface IRegressionTestClient
    {
        /// <summary>
        /// Method to call when a new line is ready for the client
        /// </summary>
        /// <param name="line"></param>
        void OnNewLine(String line);

        /// <summary>
        /// Method to call when the operation has terminated
        /// </summary>
        /// <param name="ok"></param>
        void OnTermination(bool ok,string referenceDir);


        /// <summary>
        /// Method to call to interpret a MusicBraille file
        /// </summary>
        /// <param name="s"></param>
        /// <param name="fileEncoding"></param>
        /// <param name="musicXmlDocument"></param>
        /// <param name="decoderOptions"></param>
        /// <returns></returns>
        List<MusicXmlReaderModel.DecoderItem> InterpretBrailleMusicFile(string s, BrailleFileHandler.FileEncoding fileEncoding, out XmlDocument musicXmlDocument, DecoderOptions decoderOptions);

    }

}
