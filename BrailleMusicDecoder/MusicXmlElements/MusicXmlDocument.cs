using System.Xml;

namespace BrailleMusicDecoder.MusicXmlElements
{

    // Inspired by https://docs.microsoft.com/en-us/dotnet/standard/data/xml/extending-the-dom
    // Works in near collaboration with the MuscXmlElement and classes derived from it.

    public class MusicXmlDocument : XmlDocument
    {
        public MusicXmlDocument() : base()
        { }

        public override XmlElement CreateElement(string s0, string s1, string s2)
        {
            MusicXmlElement element = new MusicXmlElement(s0, s1, s2, this);
            return element;
        }

        public MusicXmlElement CreateMusicXmlElement(string s0, string s1, string s2)
        {
            MusicXmlElement element = new MusicXmlElement(s0, s1, s2, this);
            return element;
        }

        public MusicXmlElement CreateMusicXmlElement(string s0)
        {
            MusicXmlElement element = new MusicXmlElement(s0, this);
            return element;
        }


        public MusicXmlDerivedElement CreateMusicXmlDerivedElement(string s0)
        {
            MusicXmlDerivedElement element = new MusicXmlDerivedElement(s0, this);
            return element;
        }

        public MusicXmlNoteElement CreateMusicXmlNoteElement(string s0,MusicXmlElementFactory factory)
        {
            MusicXmlNoteElement musicXmlNoteElement = new MusicXmlNoteElement(s0, this );
            musicXmlNoteElement.ElementFactory = factory;
            return musicXmlNoteElement;
        }

        public MusicXmlHarmonyElement CreateMusicXmlHarmonyElement(string s0, MusicXmlElementFactory factory)
        {
            MusicXmlHarmonyElement musicXmlHarmonyElement = new MusicXmlHarmonyElement(s0, this);
            musicXmlHarmonyElement.ElementFactory = factory;
            return musicXmlHarmonyElement;
        }
 
       public MusicXmlMeasureElement CreateMusicXmlMeasureElement(string s0, MusicXmlElementFactory factory)
        {
            MusicXmlMeasureElement musicXmlMeasureElement = new MusicXmlMeasureElement(s0, this);
            musicXmlMeasureElement.ElementFactory = factory;
            return musicXmlMeasureElement;
        }


    }

    public class MusicXmlTest
    {
        public static string Test()
        {
            MusicXmlDocument eDoc = new MusicXmlDocument();
            XmlElement element1 = eDoc.CreateElement("name");
            MusicXmlElement xNoteElement1 = eDoc.CreateMusicXmlElement("anyMusicXmlElement");
            // Finally a sample of creating any derived (real world) class, such as MusicXmlNoteElement or MuaicXmlMeasureElement:
            MusicXmlDerivedElement derivedElement = eDoc.CreateMusicXmlDerivedElement("musicXmlDerivedElement");
            derivedElement.Method1();
            derivedElement.Method2();
            string n = derivedElement.ElementName;
            string t = derivedElement.ElementType;
            return string.Format("Name='{0}' Type='{1}'", n, t);
        }
    }


}
