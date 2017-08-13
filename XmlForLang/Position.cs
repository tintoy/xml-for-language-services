namespace XmlForLang
{
    public class Position
    {
        public Position(int lineNumber, int columnNumber)
        {
            LineNumber = lineNumber;
            ColumnNumber = columnNumber;
        }

        public int LineNumber { get; private set; }
        public int ColumnNumber { get; private set; }

        public Position WithLineNumber(int lineNumber) => new Position(lineNumber, ColumnNumber);

        public Position WithColumnNumber(int columnNumber) => new Position(LineNumber, columnNumber);

        public Position Move(int lineCount = 0, int columnCount = 0) => new Position(LineNumber + lineCount, ColumnNumber + columnCount);
    }
}