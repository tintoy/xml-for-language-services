using System.Xml;

namespace XmlForLang
{
    public class ElementLocation
        : Location
    {
        public override XmlNodeType NodeType => XmlNodeType.Element;

        public bool IsEmptyElement { get; set; }
    }
}