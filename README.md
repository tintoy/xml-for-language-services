# XML for language services

Writing a language service for XML is tricky:

* Most XML parsers, even if they do capture location information, tend to:
  * Capture _positional_ information, rather than ranges; they may tell you where an element or attribute starts, but not where it ends. A language service needs to know, given a position, what element or attribute (if any) spans that position.
  * Ignore stuff like whitespace between attributes
* You should be able to handle broken XML (this demo doesn't do that).

Nevertheless, here's an approach that may be _good enough_ for some purposes.

```csharp
XDocument document = LocatingXmlTextReader.LoadWithLocations("Test.xml");
foreach (XElement element in document.DescendantNodes().OfType<XElement>())
{
    ElementLocation elementLocation = element.Annotation<ElementLocation>();
    Console.WriteLine(
        $"Element '{element.Name}' spans ({elementLocation.Start.LineNumber},{elementLocation.Start.ColumnNumber}) to ({elementLocation.End.LineNumber},{elementLocation.End.ColumnNumber})"
    );

    foreach (XAttribute attribute in element.Attributes())
    {
        AttributeLocation attributeLocation = attribute.Annotation<AttributeLocation>();
        Console.WriteLine(
            $"\tAttribute '{attribute.Name}' spans ({attributeLocation.Start.LineNumber},{attributeLocation.Start.ColumnNumber}) to ({attributeLocation.End.LineNumber},{attributeLocation.End.ColumnNumber})"
        );
        Console.WriteLine(
            $"\tName of attribute '{attribute.Name}' spans ({attributeLocation.NameStart.LineNumber},{attributeLocation.NameStart.ColumnNumber}) to ({attributeLocation.NameEnd.LineNumber},{attributeLocation.NameEnd.ColumnNumber})"
        );
        Console.WriteLine(
            $"\tValue of attribute '{attribute.Name}' spans ({attributeLocation.ValueStart.LineNumber},{attributeLocation.ValueStart.ColumnNumber}) to ({attributeLocation.ValueEnd.LineNumber},{attributeLocation.ValueEnd.ColumnNumber})"
        );
    }
}
```

Since `Position` implements `IComparable`, you can combine a `SortedDictionary` with binary search to easily find the element / attribute that a given position corresponds to.
