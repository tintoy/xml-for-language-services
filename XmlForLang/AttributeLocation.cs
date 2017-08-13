using System.Xml;

namespace XmlForLang
{
    public class AttributeLocation
        : Location
    {
        public override XmlNodeType NodeType => XmlNodeType.Attribute;
    }
}