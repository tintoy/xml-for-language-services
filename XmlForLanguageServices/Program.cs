using Serilog;
using System;
using System.IO;
using System.Xml.Linq;
using System.Collections.Generic;

namespace XmlForLanguageServices
{
    /// <summary>
    ///     A quick demo of XML-for-language-services functionality.
    /// </summary>
    /// <remarks>
    ///     One tricky aspect of implementing a language service for XML is that most parsers make it hard to keep track of node locations
    ///     (e.g. they ignore whitespace that you probably care about, such as the whitespace between attributes).
    /// 
    ///     Using a custom XmlTextReader, you can keep track of actual positions in the text and add these as annotations to your XML.
    /// </remarks>
    static class Program
    {
        /// <summary>
        ///     The main program entry-point.
        /// </summary>
        static void Main()
        {
            ConfigureLogging();

            try
            {
                Log.Information("Loading...");
                XDocument document = LocatingXmlTextReader.LoadWithLocations("Test.xml");
                Log.Information("Loaded...");

                Log.Information("=======================================");

                DumpElement(document.Root, depth: 0);
            }
            catch (Exception eUnexpected)
            {
                Log.Error(eUnexpected, eUnexpected.Message);
            }
        }

        /// <summary>
        ///     Recursively dump out an element and its attributes.
        /// </summary>
        /// <param name="element">
        ///     The target element.
        /// </param>
        /// <param name="depth">
        ///     The current element depth.
        /// </param>
        static void DumpElement(XElement element, int depth)
        {
            ElementLocation location = element.Annotation<ElementLocation>();

            Log.Information("{Indent}{Name} ({StartLine},{StartColumn}) to ({EndLine},{EndColumn})",
                new String(' ', depth * 4),
                element.Name,
                location.Start.LineNumber, location.Start.ColumnNumber,
                location.End.LineNumber, location.End.ColumnNumber
            );

            foreach (XAttribute attribute in element.Attributes())
                DumpAttribute(attribute, depth);

            foreach (XElement childElement in element.Elements())
                DumpElement(childElement, depth + 1);
        }

        /// <summary>
        ///     Dump out an attribute.
        /// </summary>
        /// <param name="attribute">
        ///     The target attribute.
        /// </param>
        /// <param name="depth">
        ///     The current element depth.
        /// </param>
        static void DumpAttribute(XAttribute attribute, int depth)
        {
            AttributeLocation location = attribute.Annotation<AttributeLocation>();

            Log.Information("{Indent}@{Name}='{Value}' ({StartLine},{StartColumn}) to ({EndLine},{EndColumn})",
                new String(' ', depth * 4 + 4 /* Extra indent for attributes */),
                attribute.Name,
                attribute.Value,
                location.Start.LineNumber, location.Start.ColumnNumber,
                location.End.LineNumber, location.End.ColumnNumber
            );
        }

        /// <summary>
        ///     Configure logging.
        /// </summary>
        static void ConfigureLogging()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information() // Set this to Verbose if you want to see the parsing process in action
                .WriteTo.LiterateConsole(outputTemplate: "[{Level:u3}] {Message}{NewLine}{Exception}")
                .CreateLogger();
        }
    }
}
