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
    ElementLocation location = element.Annotation<ElementLocation>();
    Console.WriteLine
        ($"Element '{element.Name}' spans ({location.Start.LineNumber},{location.Start.ColumnNumber}) to ({location.End.LineNumber},{location.End.ColumnNumber})"
    );
}
```

Since `Position` implements `IComparable`, you can combine a `SortedDictionary` with binary search to easily find the element / attribute that a given position corresponds to.
