namespace MusicXmlReaderModel
{
    
    public abstract class Element : MusicXmlObject
    {
        public virtual string Caption { get { return ""; } }
        // abstract public Element Create(XmlNode node);
        // abstract public string Format();
    }
}
