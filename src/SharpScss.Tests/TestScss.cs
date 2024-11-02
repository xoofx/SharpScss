// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace SharpScss.Tests;

/// <summary>
/// Basic test for <see cref="Scss.ConvertToCss"/> and <see cref="Scss.ConvertFileToCss"/>.
/// </summary>
[TestClass]
public class TestScss
{
    public TestScss()
    {
        // Custom loading of native library
        //string arch;
        //switch (System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture)
        //{
        //    case Architecture.X64:
        //        arch = "x64";
        //        break;
        //    case Architecture.X86:
        //        arch = "x86";
        //        break;
        //    case Architecture.Arm:
        //        arch = "arm";
        //        break;
        //    case Architecture.Arm64:
        //        arch = "arm64";
        //        break;
        //    default:
        //        throw new ArgumentOutOfRangeException();
        //}

        var test = RuntimeInformation.RuntimeIdentifier;

        string rid = RuntimeInformation.RuntimeIdentifier;
        string sharedObjectPostExtension;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            //rid = $"win-{arch}";
            sharedObjectPostExtension = "dll";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            //rid = $"linux-{arch}";
            sharedObjectPostExtension = "so";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            //rid = $"osx-{arch}";
            sharedObjectPostExtension = "dylib";
        }
        else
        {
            throw new PlatformNotSupportedException();
        }

        var fullPath = Path.Combine(AppContext.BaseDirectory, "runtimes", rid, "native", $"libsass.{sharedObjectPostExtension}");
        var libraryPointer = NativeLibrary.Load(fullPath);
        Assert.AreNotEqual(nint.Zero, libraryPointer, $"Unable to load library {fullPath}");
    }

    [TestMethod]
    public void TestVersion()
    {
        var version = Scss.Version;
        StringAssert.StartsWith("3.6.6", version);
    }

    [TestMethod]
    public void TestConvertToCss()
    {
        var result = Scss.ConvertToCss(@"div {color: #FFF;}");
        Assert.IsNotNull(result.Css);
        Assert.IsNull(result.SourceMap);
        Assert.IsNull(result.IncludedFiles);
        var css = result.Css.Replace("\n","").Replace(" ", "").Trim();
        Assert.AreEqual("div{color:#FFF;}", css);
    }

    [TestMethod]
    public void TestConvertToCssMemory()
    {
        Scss.ConvertToCss(@"div {color: #FFF;}");
        var start = GC.GetTotalMemory(true);
        for (int i = 0; i < 10000; i++)
        {
            Scss.ConvertToCss(@"div {color: #FFF;}");
        }
        GC.Collect();
        GC.WaitForFullGCComplete();
        var end = GC.GetTotalMemory(true);
        Console.WriteLine($"Total allocated: {end - start} bytes");
    }

    [TestMethod]
    public void TestConvertToCssCompressed()
    {
        var result = Scss.ConvertToCss(@"div {color: #FFF;}", new ScssOptions() { OutputStyle = ScssOutputStyle.Compressed});
        Assert.IsNotNull(result.Css);
        Assert.IsNull(result.SourceMap);
        Assert.IsNull(result.IncludedFiles);
        var css = result.Css.Trim();
        Assert.AreEqual("div{color:#FFF}", css);
    }

    [TestMethod]
    public void TestConvertToCssAndSourceMap()
    {
        var result = Scss.ConvertToCss(@"div {color: #FFF;}", new ScssOptions()
        {
            InputFile = "Test.scss",
            OutputFile = "Test.css",
            GenerateSourceMap = true
        });
        Assert.IsNotNull(result.Css);
        Assert.IsNotNull(result.SourceMap);
        Assert.IsNull(result.IncludedFiles);
        var css = result.Css.Replace("\n", "").Replace(" ", "").Trim();
        Assert.AreEqual("div{color:#FFF;}/*#sourceMappingURL=Test.css.map*/", css);
    }

    [TestMethod]
    public void TestConvertToCssWithTryImport()
    {
        var result = Scss.ConvertToCss(@"@import ""foo"";", new ScssOptions()
        {
            OutputStyle = ScssOutputStyle.Compressed,
            InputFile = "test.scss",
            TryImport = (ref string file, string path, out string scss, out string map) =>
            {
                Assert.AreEqual("foo", file);
                Assert.AreEqual(Path.Combine(Environment.CurrentDirectory, "test.scss"), new FileInfo(path).FullName);
                scss = "div {color: #FFF;}";
                map = null;
                return true;
            }
        });
        Assert.IsNotNull(result.Css);
        Assert.IsNull(result.SourceMap);
        Assert.IsNotNull(result.IncludedFiles);
        Assert.AreEqual(1, result.IncludedFiles.Count);
        Assert.AreEqual("foo", result.IncludedFiles[0]);
        var css = result.Css.Trim();
        Assert.AreEqual("div{color:#FFF}", css);
    }

    [TestMethod]
    public void TestConvertFileToCssWithIncludes()
    {
        var result = Scss.ConvertFileToCss(Path.Combine("files", "test.scss"), new ScssOptions()
        {
            OutputStyle = ScssOutputStyle.Compressed,
            IncludePaths =
            {
                Path.Combine("files", "subfolder")
            }
        });
        Assert.IsNotNull(result.Css);
        Assert.IsNull(result.SourceMap);
        Assert.IsNotNull(result.IncludedFiles);
        Assert.AreEqual(2, result.IncludedFiles.Count);
        Assert.AreEqual(Path.Combine(Environment.CurrentDirectory, "files", "test.scss"), new FileInfo(result.IncludedFiles[0]).FullName);
        Assert.AreEqual(Path.Combine(Environment.CurrentDirectory, "files", "subfolder", "foo.scss"), new FileInfo(result.IncludedFiles[1]).FullName);
        var css = result.Css.Trim();
        Assert.AreEqual("div{color:#FFF}", css);
    }

    [TestMethod]
    public void TestParsingException()
    {
        var exception = Assert.ThrowsException<ScssException>(() => Scss.ConvertToCss(@"div {"));
        Assert.AreEqual(5, exception.Column);
        Assert.AreEqual(1, exception.Line);
        Assert.IsTrue(exception.ErrorText.Contains("expected"));
    }

    [TestMethod]
    public void TestExceptionWithTryImport()
    {
        var exception = Assert.ThrowsException<ScssException>(() => Scss.ConvertToCss(@"@import ""foo"";", new ScssOptions()
        {
            InputFile = "test.scss",
            TryImport = (ref string file, string path, out string scss, out string map) =>
            {
                scss = null;
                map = null;
                return false;
            }
        }));
        Assert.AreEqual(1, exception.Line);
        Assert.AreEqual(9, exception.Column);
        Assert.IsTrue(exception.ErrorText.StartsWith("Unable to find include file for @import"));
    }


    [TestMethod]
    public void TestTryImportFromMemory()
    {
        var collectPaths = new List<string>();
        var result = Scss.ConvertToCss(@"@import ""foo"";", new ScssOptions()
        {
            InputFile = "/test.scss",
            OutputStyle = ScssOutputStyle.Compressed,
            TryImport = (ref string file, string path, out string scss, out string map) =>
            {
                collectPaths.Add(path);
                if (file == "foo")
                {
                    file = "/this/is/a/sub/folder/" + file + ".scss";
                    scss = "@import 'local/bar';";
                }
                else
                {
                    file = "/this/is/a/sub/folder/" + file + ".css";
                    scss = ".foo { color: red; }";
                }
                map = null;
                return true;
            }
        });
        Assert.AreEqual(".foo{color:red}", result.Css.TrimEnd());

        Assert.AreEqual(2, collectPaths.Count);
        Assert.AreEqual("/test.scss", collectPaths[0]);
        Assert.AreEqual("/this/is/a/sub/folder/foo.scss", collectPaths[1]);

        Assert.AreEqual(2, result.IncludedFiles.Count);
        Assert.AreEqual("/this/is/a/sub/folder/foo.scss", result.IncludedFiles[0]);
        Assert.AreEqual("/this/is/a/sub/folder/local/bar.css", result.IncludedFiles[1]);
    }
}