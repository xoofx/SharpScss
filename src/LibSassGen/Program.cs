using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using CppAst;
using CppAst.CodeGen.Common;
using CppAst.CodeGen.CSharp;
using Zio;
using Zio.FileSystems;

namespace LibSassGen
{
    /// <summary>
    ///     Generates a C# file from api.h
    ///     Runs only on Windows
    /// </summary>
    internal class Program
    {
        static void Main(string[] args)
        {
            var program = new Program();
            program.Run();
        }

        public void Run()
        {
            var srcFolder = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\..\..\..\libsass\include"));
            var destFolder = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\..\..\SharpScss"));

            if (!Directory.Exists(srcFolder))
            {
                throw new DirectoryNotFoundException($"The source folder `{srcFolder}` doesn't exist");
            }
            if (!Directory.Exists(destFolder))
            {
                throw new DirectoryNotFoundException($"The destination folder `{destFolder}` doesn't exist");
            }


            var marshalNoFreeNative = new CSharpMarshalAttribute(CSharpUnmanagedKind.CustomMarshaler) {MarshalTypeRef = "typeof(UTF8MarshallerNoFree)"};

            var csOptions = new CSharpConverterOptions()
            {
                DefaultClassLib = "LibSass",
                DefaultNamespace = "SharpScss",
                DefaultOutputFilePath = "/LibSass.generated.cs",
                DefaultDllImportNameAndArguments = "LibSassDll",
                GenerateAsInternal = true,
                DispatchOutputPerInclude = false,
                DefaultMarshalForString = new CSharpMarshalAttribute(CSharpUnmanagedKind.CustomMarshaler) { MarshalTypeRef = "typeof(UTF8MarshallerNoFree)" },

                MappingRules =
                {
                    // Change Sass_Value to be a struct instead of an union
                    e => e.Map<CppClass>( "Sass_Value").CppAction((converter, element) => ((CppClass)element).ClassKind = CppClassKind.Struct),
                    e => e.Map<CppParameter>("sass_make_import_entry::source").Type("const char*"),
                    e => e.Map<CppParameter>("sass_make_import_entry::srcmap").Type("const char*"),
                    e => e.Map<CppParameter>("sass_make_import::source").Type("const char*"),
                    e => e.Map<CppParameter>("sass_make_import::srcmap").Type("const char*"),
                    e => e.Map<CppParameter>("sass_make_data_context::source_string").Type("const char*"),

                    e => e.Map<CppFunction>("libsass_version").MarshalAs(marshalNoFreeNative),
                    e => e.Map<CppFunction>("libsass_language_version").MarshalAs(marshalNoFreeNative),
                    e => e.Map<CppFunction>("sass_context_get_output_string").MarshalAs(marshalNoFreeNative),
                    e => e.Map<CppFunction>("sass_context_get_source_map_string").MarshalAs(marshalNoFreeNative),
                }
            };
            csOptions.IncludeFolders.Add(srcFolder);
            var files = new List<string>()
            {
                Path.Combine(srcFolder, "sass.h"),
            };

            var csCompilation = CSharpConverter.Convert(files, csOptions);

            if (csCompilation.HasErrors)
            {
                foreach (var message in csCompilation.Diagnostics.Messages)
                {
                    Console.Error.WriteLine(message);
                }
                Console.Error.WriteLine("Unexpected parsing errors");
                Environment.Exit(1);
            }


            var libClass = (CSharpClass) ((CSharpNamespace) ((CSharpGeneratedFile) csCompilation.Members[0]).Members.First(x => x is CSharpNamespace)).Members.FirstOrDefault(x => x is CSharpClass);

            var member = libClass.Members.OfType<CSharpField>().FirstOrDefault(x => x.Name == "SASS2SCSS_FIND_WHITESPACE");
            if (member != null)
            {
                libClass.Members.Remove(member);
            }

            var fs = new PhysicalFileSystem();

            {
                var subfs = new SubFileSystem(fs, fs.ConvertPathFromInternal(destFolder));
                var codeWriter = new CodeWriter(new CodeWriterOptions(subfs));
                csCompilation.DumpTo(codeWriter);
            }
        }
    }
}
