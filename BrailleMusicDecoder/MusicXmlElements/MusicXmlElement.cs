using System.Xml;
using MusicXmlReaderModel; // Central access to Logger from this and all derived classes

namespace BrailleMusicDecoder.MusicXmlElements
{
    // Inspired by https://docs.microsoft.com/en-us/dotnet/standard/data/xml/extending-the-dom
    // Works in near collaboration with the MuscXmlDocument and classes derived from it.

    /// <summary>
    /// This is the base class for all classes describing MusicXml elements.
    /// Most MusicXml elements are just instances of this class with childNodes and attributes describing the actual musical contents.
    /// A few more complicated MusicXml elements, such as the Note Element and the Harmony element are described in classes derived from MusicXmlElement.
    /// These classes add extra functionality such as methods and properties to the MusicXml contents.
    /// All of these class ares strongly coupled to the MusicXmlDocument class, which contains creation methods for them.
    /// </summary>
    public class MusicXmlElement : XmlElement
    {
        // Should only be called from MusicXmlDocument.CreateMusicXmlElement when a document is loaded
        internal MusicXmlElement(string s0, string s1, string s2, XmlDocument doc) : base(s0, s1, s2, doc)
        { }

        // Called during building up the MusicXml tree in the application.
        internal MusicXmlElement(string name, XmlDocument doc) : base("", name, "", doc)
        { }

        protected void LogCF(string s)
        {
            Logger.LogCF1(s);
        }

        protected void LogUserWarning(string s)
        {
            UserWarnings.LogUserWarning(s);
        }

    }


    /// <summary>
    /// This class is just a sample of the many usefull classes which can berived from (Experimrntal)MusicXmlElement: For instance
    /// MusicXmlNoteElement     (Described in MusicXmlNoteElement.cs)
    /// MusicXmlHarmonyElement  (Described in MusicXmlHarmonyElement.cs)
    /// Only replace "Derived" by for instance "Barline" to create the new class with the desired functionality.
    /// </summary>
    public class MusicXmlDerivedElement : MusicXmlElement
    {
        // Should only be called from MusicXmlDocument.CreateMusicXmlNoteElement  when a document is loaded from a file
        internal MusicXmlDerivedElement(string s0, string s1, string s2, XmlDocument doc) : base(s0, s1, s2, doc)
        { }

        // Called when building a MusicXmlDocument, for instance from the MusicXmlBuilder class.
        internal MusicXmlDerivedElement(string name, XmlDocument doc) : base("", name, "", doc)
        { }

        public void Method1() { }
        public void Method2() { }
        public string ElementName { get { return this.Name; } }
        public string ElementType { get { return this.GetType().ToString(); } }
    }

}
