using System;
using System.Xml;

namespace XmlForLang
{
    /// <summary>
    ///     Location information for an XML node.
    /// </summary>
    public abstract class Location
    {
        /// <summary>
        ///     Create a new <see cref="Location"/>.
        /// </summary>
        protected Location()
        {
        }

        /// <summary>
        ///     The starting position of the XML node.
        /// </summary>
        public Position Start { get; internal set; }

        /// <summary>
        ///     The ending position of the XML node.
        /// </summary>
        public Position End { get; internal set; }

        /// <summary>
        ///     The depth of the nearest surrounding element.
        /// </summary>
        public int Depth { get; internal set; }

        /// <summary>
        ///     The name of the node at the location.
        /// </summary>
        /// <remarks>
        ///     For diagnostic purposes only.
        /// </remarks>
        internal string Name { get; set; }
    }
}