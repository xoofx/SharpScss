// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license.
// See the license.txt file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;


namespace SharpScss.Tests
{
    /// <summary>
    /// Basic test for <see cref="Scss.ConvertToCss"/> and <see cref="Scss.ConvertFileToCss"/>.
    /// </summary>
    public class TestScss
    {
        public TestScss()
        {
            // Make sure the CurrentDirectory is the same as this assembly (JetBrain Resharper Unittest bug in 10.x???)
            Environment.CurrentDirectory = Path.GetDirectoryName(typeof(TestScss).Assembly.Location);
        }

        [Test]
        public void TestVersion()
        {
            var version = Scss.Version;
            StringAssert.StartsWith("3.6.4", version);
        }

        [Test]
        public void TestConvertToCss()
        {
            var result = Scss.ConvertToCss(@"div {color: #FFF;}");
            Assert.NotNull(result.Css);
            Assert.Null(result.SourceMap);
            Assert.Null(result.IncludedFiles);
            var css = result.Css.Replace("\n","").Replace(" ", "").Trim();
            Assert.AreEqual("div{color:#FFF;}", css);
        }

        [Test]
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

        [Test]
        public void TestConvertToCssCompressed()
        {
            var result = Scss.ConvertToCss(@"div {color: #FFF;}", new ScssOptions() { OutputStyle = ScssOutputStyle.Compressed});
            Assert.NotNull(result.Css);
            Assert.Null(result.SourceMap);
            Assert.Null(result.IncludedFiles);
            var css = result.Css.Trim();
            Assert.AreEqual("div{color:#FFF}", css);
        }

        [Test]
        public void TestConvertToCssAndSourceMap()
        {
            var result = Scss.ConvertToCss(@"div {color: #FFF;}", new ScssOptions()
            {
                InputFile = "Test.scss",
                OutputFile = "Test.css",
                GenerateSourceMap = true
            });
            Assert.NotNull(result.Css);
            Assert.NotNull(result.SourceMap);
            Assert.Null(result.IncludedFiles);
            var css = result.Css.Replace("\n", "").Replace(" ", "").Trim();
            Assert.AreEqual("div{color:#FFF;}/*#sourceMappingURL=Test.css.map*/", css);
        }

        [Test]
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
            Assert.NotNull(result.Css);
            Assert.Null(result.SourceMap);
            Assert.NotNull(result.IncludedFiles);
            Assert.AreEqual(1, result.IncludedFiles.Count);
            Assert.AreEqual("foo", result.IncludedFiles[0]);
            var css = result.Css.Trim();
            Assert.AreEqual("div{color:#FFF}", css);
        }

        [Test]
        public void TestConvertFileToCssWithIncludes()
        {
            var result = Scss.ConvertFileToCss(@"files\test.scss", new ScssOptions()
            {
                OutputStyle = ScssOutputStyle.Compressed,
                IncludePaths =
                {
                    @"files\subfolder"
                }
            });
            Assert.NotNull(result.Css);
            Assert.Null(result.SourceMap);
            Assert.NotNull(result.IncludedFiles);
            Assert.AreEqual(2, result.IncludedFiles.Count);
            Assert.AreEqual(Path.Combine(Environment.CurrentDirectory, @"files\test.scss"), new FileInfo(result.IncludedFiles[0]).FullName);
            Assert.AreEqual(Path.Combine(Environment.CurrentDirectory, @"files\subfolder\foo.scss"), new FileInfo(result.IncludedFiles[1]).FullName);
            var css = result.Css.Trim();
            Assert.AreEqual("div{color:#FFF}", css);
        }

        [Test]
        public void TestParsingException()
        {
            var exception = Assert.Throws<ScssException>(() => Scss.ConvertToCss(@"div {"));
            Assert.AreEqual(5, exception.Column);
            Assert.AreEqual(1, exception.Line);
            Assert.True(exception.ErrorText.Contains("expected"));
        }

        [Test]
        public void TestExceptionWithTryImport()
        {
            var exception = Assert.Throws<ScssException>(() => Scss.ConvertToCss(@"@import ""foo"";", new ScssOptions()
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
            Assert.True(exception.ErrorText.StartsWith("Unable to find include file for @import"));
        }


        [Test]
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
}