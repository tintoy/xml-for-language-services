namespace XmlForLang
{
    /// <summary>
    ///     Represents a position in a text document.
    /// </summary>
    public class Position
    {
        /// <summary>
        ///     Create a new <see cref="position"/>.
        /// </summary>
        /// <param name="lineNumber">
        ///     The line number (1-based).
        /// </param>
        /// <param name="columnNumber">
        ///     The column number (1-based).
        /// </param>
        public Position(int lineNumber, int columnNumber)
        {
            LineNumber = lineNumber;
            ColumnNumber = columnNumber;
        }

        /// <summary>
        ///     The line number (1-based).
        /// </summary>
        public int LineNumber { get; }

        /// <summary>
        ///     The column number (1-based).
        /// </summary>
        public int ColumnNumber { get; }

        /// <summary>
        ///     Create a copy of the <see cref="Position"/> with the specified line number.
        /// </summary>
        /// <param name="lineNumber">
        ///     The new line number.
        /// </param>
        /// <returns>
        ///     The new <see cref="Position"/>.
        /// </returns>
        public Position WithLineNumber(int lineNumber) => new Position(lineNumber, ColumnNumber);

        /// <summary>
        ///     Create a copy of the <see cref="Position"/> with the specified column number.
        /// </summary>
        /// <param name="columnNumber">
        ///     The new column number.
        /// </param>
        /// <returns>
        ///     The new <see cref="Position"/>.
        /// </returns>
        public Position WithColumnNumber(int columnNumber) => new Position(LineNumber, columnNumber);

        /// <summary>
        ///     Create a copy of the <see cref="Position"/>, moving by the specified number of lines and / or columns.
        /// </summary>
        /// <param name="lineCount">
        ///     The number of lines (if any) to move by.
        /// </param>
        /// <param name="columnCount">
        ///     The number of columns (if any) to move by.
        /// </param>
        /// <returns>
        ///     The new <see cref="Position"/>.
        /// </returns>
        public Position Move(int lineCount = 0, int columnCount = 0) => new Position(LineNumber + lineCount, ColumnNumber + columnCount);
    }
}
