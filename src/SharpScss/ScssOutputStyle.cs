// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.
namespace SharpScss
{
    /// <summary>
    /// Determines the output format of the final CSS style used by <see cref="Scss.ConvertToCss"/> and <see cref="Scss.ConvertFileToCss"/>.
    /// </summary>
    public enum ScssOutputStyle
    {
        /// <summary>
        /// Nested format.
        /// </summary>
        Nested,

        /// <summary>
        /// Expanded format.
        /// </summary>
        Expanded,

        /// <summary>
        /// Compact format.
        /// </summary>
        Compact,

        /// <summary>
        /// Compressed format.
        /// </summary>
        Compressed,
    }
}