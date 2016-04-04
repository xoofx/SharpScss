// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;

namespace SharpScss.Tests
{
    public class Program
    {
        public static void Main()
        {
            // Method only used for quick testing
            var version = Scss.Version;
            var result = Scss.ConvertToCss(@"div {  background-color: #FFF;}", new ScssOptions()
            {
                InputFile = @"C:\Toto\Tata.sass",
                OutputFile = @"C:\Toto\Tata.css",
            });
            Console.WriteLine(result.Css);
        }
    }
}