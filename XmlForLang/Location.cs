using System;
using System.Xml;

namespace XmlForLang
{
    public abstract class Location
    {
        public abstract XmlNodeType NodeType { get; }

        public string Name { get; set; }

        public int Depth { get; set; }

        public Position Start { get; set; }
        public Position End { get; set; }
    }
}