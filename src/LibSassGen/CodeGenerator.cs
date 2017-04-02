using System;
using System.IO;

namespace LibSassGen
{
    public partial class CodeGenerator
    {
        private const string DefaultIncludeDir = @"../../../../../libsass/include";

        private const string DefaultOutputFilePath = @"../../../../SharpScss/LibSass.Generated.cs";
        private const int IndentMultiplier = 4;

        private int _indentLevel;

        private TextWriter _writer;

        private TextWriter _writerBody;

        private TextWriter _writerGlobal;

        public bool OutputToConsole { get; set; }

        public void Run()
        {
            var outputFilePath = Path.Combine(Environment.CurrentDirectory, DefaultOutputFilePath);
            outputFilePath = Path.GetFullPath(outputFilePath);

            _writerGlobal = new StringWriter();
            _writerBody = new StringWriter();

            ParseAndWrite();

            var finalWriter = OutputToConsole
                ? Console.Out
                : new StreamWriter(outputFilePath);

            finalWriter.Write(_writerGlobal);

            if (!OutputToConsole)
            {
                finalWriter.Flush();
                finalWriter.Dispose();
                finalWriter = null;
            }
        }
    }
}
