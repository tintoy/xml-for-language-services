# XML for language services

Writing a language service for XML is hard:

* Most XML parsers, even if they do capture location information, tend to:
  * Capture _positional_ information, rather than ranges; they may tell you where an element or attribute starts, but not where it ends. A language service needs to know, given a position, what element or attribute (if any) spans that position.
  * Ignore stuff like whitespace between attributes
* You should be able to handle broken XML (this demo doesn't do that).
