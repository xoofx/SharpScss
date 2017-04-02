using ClangSharp;

namespace LibSassGen
{
    /// <summary>
    ///     Helper methods for the codegen (indent, write...etc.)
    /// </summary>
    public partial class CodeGenerator
    {
        private static string EscapeHtml(string text)
        {
            text = text.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
            return text;
        }

        private string GetTypeAsString(CXType type)
        {
            // Try to resolve any primitive type
            var canonical = clang.getCanonicalType(type);
            var csType = canonical.kind.ToPrimitiveCsType();
            if (type.kind == CXTypeKind.CXType_Typedef
                || type.kind == CXTypeKind.CXType_Record
                || type.kind == CXTypeKind.CXType_Enum)
            {
                var alias = clang.getTypeSpelling(type).ToString().Replace("const ", string.Empty);
                csType = alias;
            }

            if (csType == null)
                switch (type.kind)
                {
                    case CXTypeKind.CXType_Unexposed:
                        csType = canonical.kind == CXTypeKind.CXType_Unexposed
                            ? clang.getTypeSpelling(canonical).ToString()
                            : GetTypeAsString(canonical);
                        break;
                    default:
                        csType = "UnknownType";
                        break;
                }

            // Remove const qualifier
            csType = csType.Replace("const ", string.Empty);
            return csType;
        }

        private CodeGenerator Indent()
        {
            _indentLevel++;
            return this;
        }

        private CodeGenerator UnIndent()
        {
            _indentLevel--;
            return this;
        }

        private void WriteCloseBlock()
        {
            UnIndent().WriteLine("}");
        }

        private void WriteComment(string comment)
        {
            var i = 0;

            WriteLine("/// <summary>");
            var previousIndex = 0;
            while (true)
            {
                previousIndex = i;
                i += 80;
                if (i >= comment.Length)
                    break;

                while (i > 0 && !char.IsWhiteSpace(comment[i]))
                    i--;
                if (i > previousIndex)
                    WriteLine("/// " + EscapeHtml(comment.Substring(previousIndex, i - previousIndex).TrimStart()));
            }
            if (i > previousIndex && previousIndex < comment.Length)
                WriteLine("/// " + EscapeHtml(comment.Substring(previousIndex).TrimStart()));

            WriteLine("/// </summary>");
            //if (cppElement.NativeComment != null)
            //{
            //    WriteLine("/// <remarks>");
            //    WriteLine("/// Native declaration: <code>" + EscapeHtml(cppElement.NativeComment) + "</code>");
            //    WriteLine("/// </remarks>");
            //}
        }

        private void WriteIndent()
        {
            for (var i = 0; i < IndentMultiplier * _indentLevel; ++i)
                _writer.Write(" ");
        }

        private CodeGenerator WriteLine()
        {
            _writer.WriteLine();
            return this;
        }

        private CodeGenerator WriteLine(string s)
        {
            WriteIndent();
            _writer.WriteLine(s);
            return this;
        }




        private void WriteOpenBlock()
        {
            WriteLine("{").Indent();
        }
    }
}
