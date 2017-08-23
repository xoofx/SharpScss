// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
using System.IO;
using Xunit;
using TestAttribute = Xunit.FactAttribute;


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
            Assert.True(version.StartsWith("3.4.4"));
        }

        [Test]
        public void TestConvertToCss()
        {               
            var result = Scss.ConvertToCss(@"div {color: #FFF;}");
            Assert.NotNull(result.Css);
            Assert.Null(result.SourceMap);
            Assert.Null(result.IncludedFiles);
            var css = result.Css.Replace("\n","").Replace(" ", "").Trim();
            Assert.Equal("div{color:#FFF;}", css);
        }

        [Test]
        public void TestConvertToCssCompressed()
        {
            var result = Scss.ConvertToCss(@"div {color: #FFF;}", new ScssOptions() { OutputStyle = ScssOutputStyle.Compressed});
            Assert.NotNull(result.Css);
            Assert.Null(result.SourceMap);
            Assert.Null(result.IncludedFiles);
            var css = result.Css.Trim();
            Assert.Equal("div{color:#FFF}", css);
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
            Assert.Equal("div{color:#FFF;}/*#sourceMappingURL=Test.css.map*/", css);
        }

        [Test]
        public void TestConvertToCssWithTryImport()
        {
            var result = Scss.ConvertToCss(@"@import ""foo"";", new ScssOptions()
            {
                OutputStyle = ScssOutputStyle.Compressed,
                InputFile = "test.scss",
                TryImport = (string file, string path, out string scss, out string map) =>
                {
                    Assert.Equal("foo", file);
                    Assert.Equal(Path.Combine(Environment.CurrentDirectory, "test.scss"), new FileInfo(path).FullName);
                    scss = "div {color: #FFF;}";
                    map = null;
                    return true;
                }
            });
            Assert.NotNull(result.Css);
            Assert.Null(result.SourceMap);
            Assert.NotNull(result.IncludedFiles);
            Assert.Equal(1, result.IncludedFiles.Count);
            Assert.Equal("foo", result.IncludedFiles[0]);
            var css = result.Css.Trim();
            Assert.Equal("div{color:#FFF}", css);
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
            Assert.Equal(2, result.IncludedFiles.Count);
            Assert.Equal(Path.Combine(Environment.CurrentDirectory, @"files\test.scss"), new FileInfo(result.IncludedFiles[0]).FullName);
            Assert.Equal(Path.Combine(Environment.CurrentDirectory, @"files\subfolder\foo.scss"), new FileInfo(result.IncludedFiles[1]).FullName);
            var css = result.Css.Trim();
            Assert.Equal("div{color:#FFF}", css);
        }

        [Test]
        public void TestParsingException()
        {
            var exception = Assert.Throws<ScssException>(() => Scss.ConvertToCss(@"div {"));
            Assert.Equal(5, exception.Column);
            Assert.Equal(1, exception.Line);
            Assert.True(exception.ErrorText.Contains("expected"));
        }

        [Test]
        public void TestExceptionWithTryImport()
        {
            var exception = Assert.Throws<ScssException>(() => Scss.ConvertToCss(@"@import ""foo"";", new ScssOptions()
            {
                InputFile = "test.scss",
                TryImport = (string file, string path, out string scss, out string map) =>
                {
                    scss = null;
                    map = null;
                    return false;
                }
            }));
            Assert.Equal(1, exception.Line);
            Assert.Equal(9, exception.Column);
            Assert.True(exception.ErrorText.StartsWith("Unable to find include file for @import"));
        }
    }
}