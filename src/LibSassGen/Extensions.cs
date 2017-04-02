using ClangSharp;

namespace LibSassGen
{
    /// <summary>
    ///     Extensions methods for clang types
    /// </summary>
    internal static class Extensions
    {
        public static string GetPrimitiveCsTypeOrNull(this CXTypeKind kind)
        {
            switch (kind)
            {
                case CXTypeKind.CXType_Bool:
                    return "bool";
                case CXTypeKind.CXType_Char16:
                case CXTypeKind.CXType_WChar:
                    return "char";
                case CXTypeKind.CXType_UChar:
                case CXTypeKind.CXType_Char_U:
                    return "byte";
                case CXTypeKind.CXType_SChar:
                case CXTypeKind.CXType_Char_S:
                    return "sbyte";
                case CXTypeKind.CXType_UShort:
                    return "ushort";
                case CXTypeKind.CXType_Short:
                    return "short";
                case CXTypeKind.CXType_Float:
                    return "float";
                case CXTypeKind.CXType_Double:
                    return "double";
                case CXTypeKind.CXType_Int:
                    return "int";
                case CXTypeKind.CXType_UInt:
                    return "uint";
                case CXTypeKind.CXType_Pointer:
                case CXTypeKind.CXType_NullPtr: // ugh, what else can I do?
                    return "IntPtr";
                case CXTypeKind.CXType_Long:
                    return "int";
                case CXTypeKind.CXType_ULong:
                    return "uint";
                case CXTypeKind.CXType_LongLong:
                    return "long";
                case CXTypeKind.CXType_ULongLong:
                    return "ulong";
                case CXTypeKind.CXType_Void:
                    return "void";
                default:
                    return null;
            }
        }

        public static bool IsChar(this CXTypeKind kind)
        {
            return kind == CXTypeKind.CXType_UChar || kind == CXTypeKind.CXType_Char_U ||
                   kind == CXTypeKind.CXType_SChar || kind == CXTypeKind.CXType_Char_S;
        }

        public static bool IsInSystemHeader(this CXCursor cursor)
        {
            return clang.Location_isInSystemHeader(clang.getCursorLocation(cursor)) != 0;
        }

        public static string ToPrimitiveCsType(this CXTypeKind kind)
        {
            return GetPrimitiveCsTypeOrNull(kind) ?? "__Invalid__";
        }
    }
}
