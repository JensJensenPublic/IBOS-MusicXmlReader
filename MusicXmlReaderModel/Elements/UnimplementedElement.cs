using System.Xml;

namespace MusicXmlReaderModel
{
    class UnimplementedElement : Element
    {
        private string name;

        public static UnimplementedElement Create(XmlNode node)
        {
            return new UnimplementedElement(node);
        }
             
        private UnimplementedElement(XmlNode node)
        {
            this.name = node.Name;
        }

        public override string ToString()
        {
            return string.Format("Unimplemented  element: {0}",name);      
        }
    }
}
