using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Linq;

namespace XmlForLang
{
    public class LocatingXmlTextReader
        : XmlTextReader
    {
        readonly List<Location> _locations = new List<Location>();
        readonly Stack<ElementLocation> _elementLocationStack = new Stack<ElementLocation>();

        bool _emitEndElement;

        public LocatingXmlTextReader(TextReader input)
            : base(input)
        {
        }

        public IReadOnlyList<Location> Locations => _locations;

        Position CurrentPosition => new Position(LineNumber, LinePosition);

        public override bool Read()
        {
            Log.Information("{NodeType} {Name} ({LineNumber},{ColumnNumber}) -> [Read]",
                NodeType, Name, LineNumber, LinePosition
            );

            bool result = base.Read();
            if (!result)
                return false;

            Log.Information("[Read] -> {NodeType} {Name} ({LineNumber},{ColumnNumber})",
                NodeType, Name, LineNumber, LinePosition
            );

            CaptureLocation();

            return true;
        }

        public override bool MoveToFirstAttribute()
        {
            Log.Information("{NodeType} {Name} ({LineNumber},{LinePosition}) -> [FirstAttribute]",
                NodeType, Name, LineNumber, LinePosition
            );

            bool result = base.MoveToFirstAttribute();
            if (!result)
                return false;

            Log.Information("-> [FirstAttribute] {NodeType} {Name} ({LineNumber},{LinePosition})",
                NodeType, Name, LineNumber, LinePosition
            );

            CaptureLocation();

            return true;
        }

        public override bool MoveToNextAttribute()
        {
            Log.Information("{NodeType} {Name} ({LineNumber},{LinePosition}) -> [NextAttribute]",
                NodeType, Name, LineNumber, LinePosition
            );

            bool result = base.MoveToNextAttribute();
            if (!result)
                return false;

            Log.Information("-> [NextAttribute] {NodeType} {Name} ({LineNumber},{LinePosition})",
                NodeType, Name, LineNumber, LinePosition
            );

            CaptureLocation();

            return true;
        }

        public override bool MoveToElement()
        {
            Log.Information("{NodeType} {Name} ({LineNumber},{LinePosition}) -> [Element]",
                NodeType, Name, LineNumber, LinePosition
            );

            bool result = base.MoveToElement();
            if (!result)
                return false;

            // If this is an empty element, capture its end position.
            if (IsEmptyElement)
                _emitEndElement = true;

            Log.Information("-> [Element] {NodeType} {Name} ({LineNumber},{LinePosition}) (IsEmpty={IsEmptyElement}, EmitEndElement={EmitEndElement})",
                NodeType, Name, LineNumber, LinePosition, IsEmptyElement, _emitEndElement
            );

            return true;
        }

        void CaptureLocation()
        {
            Log.Information("[Capture{NodeType}Location] {Name} ({LineNumber},{ColumnNumber}) (IsEmpty={IsEmptyElement}, EmitEndElement={EmitEndElement}, ElementLocationStack={StackDepth})",
                NodeType, Name, LineNumber, LinePosition, IsEmptyElement, _emitEndElement, DumpElementStack()
            );

            if (NodeType == XmlNodeType.Element)
            {
                ElementLocation elementLocation;
                if (_emitEndElement)
                {
                    elementLocation = _elementLocationStack.Pop();
                    elementLocation.End = CurrentPosition.Move(columnCount: -1);

                    Log.Information("[Capture{NodeType}LocationEnd] {Name} ({StartLineNumber},{StartColumnNumber}-{EndLineNumber},{EndColumnNumber})",
                        NodeType, Name,
                        elementLocation.Start.LineNumber, elementLocation.Start.ColumnNumber,
                        elementLocation.End.LineNumber, elementLocation.End.ColumnNumber
                    );

                    _emitEndElement = false;
                }

                Log.Information("[Capture{NodeType}LocationStart] {Name} ({LineNumber},{ColumnNumber})",
                    NodeType, Name, LineNumber, LinePosition
                );

                elementLocation = new ElementLocation
                {
                    Name = Name,
                    Depth = Depth,
                    Start = CurrentPosition.Move(columnCount: -1),
                    IsEmptyElement = IsEmptyElement
                };
                _locations.Add(elementLocation);
                _elementLocationStack.Push(elementLocation);
            }
            else if (NodeType == XmlNodeType.EndElement || _emitEndElement)
            {
                ElementLocation elementLocation = _elementLocationStack.Pop();
                elementLocation.End = CurrentPosition;

                Log.Information("[Capture{NodeType}LocationEnd] {Name} ({StartLineNumber},{StartColumnNumber}-{EndLineNumber},{EndColumnNumber})",
                    NodeType, Name,
                    elementLocation.Start.LineNumber, elementLocation.Start.ColumnNumber,
                    elementLocation.End.LineNumber, elementLocation.End.ColumnNumber
                );

                _emitEndElement = false;
            }
            else if (NodeType == XmlNodeType.Attribute)
            {
                _locations.Add(new AttributeLocation
                {
                    Name = Name,
                    Start = CurrentPosition,
                    End = CurrentPosition.Move(
                        columnCount: Name.Length + 2 /* =" */ + Value.Length + 1 /* " */
                    )
                });
            }
            else
            {
                Log.Information("[SkipCapture{NodeType}Location] {Name} ({LineNumber},{ColumnNumber})",
                    NodeType, Name, LineNumber, LinePosition
                );
            }
        }

        void CaptureEndElementLocation()
        {
            Log.Information("[CaptureEndElementLocation] {NodeType} {Name} ({LineNumber},{ColumnNumber}) (IsEmpty={IsEmptyElement}, EmitEndElement={EmitEndElement})",
                NodeType, Name, LineNumber, LinePosition, IsEmptyElement, _emitEndElement
            );

            ElementLocation elementLocation = _elementLocationStack.Pop();
            elementLocation.End = CurrentPosition;

            Log.Information("[Capture{NodeType}/EndElementLocation] {Name} ({StartLineNumber},{StartColumnNumber}-{EndLineNumber},{EndColumnNumber})",
                NodeType, Name,
                elementLocation.Start.LineNumber, elementLocation.Start.ColumnNumber,
                elementLocation.End.LineNumber, elementLocation.End.ColumnNumber
            );

            _emitEndElement = false;
        }

        string DumpElementStack()
        {
            return "[" + String.Join(", ", _elementLocationStack.Reverse().Select(location => location.Name)) + "]";
        }
    }
}
