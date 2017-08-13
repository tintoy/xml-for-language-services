using Serilog;
using System;
using System.IO;
using System.Xml.Linq;
using System.Collections.Generic;

namespace XmlForLang
{
    class Program
    {
        static void Main()
        {
            ConfigureLogging();

            try
            {
                const LoadOptions loadOptions = LoadOptions.PreserveWhitespace | LoadOptions.SetBaseUri | LoadOptions.SetLineInfo;

                XDocument document;
                IReadOnlyList<Location> locations;
                using (StreamReader reader = File.OpenText("Test.xml"))
                using (LocatingXmlTextReader xmlReader = new LocatingXmlTextReader(reader))
                {
                    document = XDocument.Load(xmlReader, loadOptions);
                    locations = xmlReader.Locations;
                }

                Log.Information("=======================================");

                foreach (Location location in locations)
                {
                    Log.Information("[{NodeType}] {Name} ({StartLine},{StartColumn}-{EndLine},{EndColumn})",
                        location.NodeType,
                        location.Name,
                        location.Start.LineNumber, location.Start.ColumnNumber,
                        location.End.LineNumber, location.End.ColumnNumber
                    );
                }
            }
            catch (Exception eUnexpected)
            {
                Log.Error(eUnexpected, eUnexpected.Message);
            }
        }

        static void ConfigureLogging()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.LiterateConsole()
                .CreateLogger();
        }
    }
}
