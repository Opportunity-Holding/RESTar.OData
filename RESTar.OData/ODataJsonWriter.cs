using System.IO;
using Newtonsoft.Json;
using RESTar.Admin;

#pragma warning disable 1591

namespace RESTar.OData
{
    /// <inheritdoc />
    /// <summary>
    /// A JSON text writer for writing OData payloads
    /// </summary>
    internal class ODataJsonWriter : JsonTextWriter
    {
        private const int BaseIndentation = 1;
        private readonly string NewLine;
        private int CurrentDepth;
        public ulong ObjectsWritten { get; private set; }

        public override void WriteStartObject()
        {
            if (CurrentDepth == BaseIndentation)
                ObjectsWritten += 1;
            CurrentDepth += 1;
            base.WriteStartObject();
        }

        public override void WriteEndObject()
        {
            CurrentDepth -= 1;
            base.WriteEndObject();
        }

        public void WriteIndentation()
        {
            if (Formatting == Formatting.Indented)
                WriteIndent();
        }

        public ODataJsonWriter(TextWriter textWriter) : base(textWriter)
        {
            switch (Settings.Instance.LineEndings)
            {
                case LineEndings.Windows:
                    NewLine = "\r\n";
                    break;
                case LineEndings.Linux:
                    NewLine = "\n";
                    break;
                default: return;
            }
        }

        protected override void WriteIndent()
        {
            WriteWhitespace(NewLine);
            var currentIndentCount = Top * Indentation + BaseIndentation;
            for (var i = 0; i < currentIndentCount; i++)
                WriteIndentSpace();
        }

        protected override void Dispose(bool disposing)
        {
            CurrentDepth = 0;
            ObjectsWritten = 0;
            base.Dispose(disposing);
        }
    }
}