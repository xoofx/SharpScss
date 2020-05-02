// Copyright (c) Alexandre Mutel. All rights reserved.
// This file is licensed under the BSD-Clause 2 license. 
// See the license.txt file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace SharpScss
{
    /// <summary>
    /// Struct and functions of libsass used manually. All other functions/types are defined in LibSass.Generated.cs
    /// </summary>
    internal static partial class LibSass
    {
        private const string LibSassDll = "libsass";

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate Sass_Import_List sass_importer_delegate(LibSass.StringUtf8 cur_path, LibSass.Sass_Importer_Entry cb, LibSass.Sass_Compiler compiler);

        public partial struct Sass_File_Context
        {
            public static implicit operator Sass_Context(Sass_File_Context context)
            {
                return new Sass_Context(context.Handle);
            }
            public static explicit operator Sass_File_Context(Sass_Context context)
            {
                return new Sass_File_Context(context.Handle);
            }
        }

        public partial struct Sass_Data_Context
        {
            public static implicit operator Sass_Context(Sass_Data_Context context)
            {
                return new Sass_Context(context.Handle);
            }
            public static explicit operator Sass_Data_Context(Sass_Context context)
            {
                return new Sass_Data_Context(context.Handle);
            }
        }
        public partial struct size_t
        {
            public size_t(int value)
            {
                Value = new IntPtr(value);
            }

            public size_t(long value)
            {
                Value = new IntPtr(value);
            }


            public static implicit operator long(size_t size)
            {
                return size.Value.ToInt64();
            }

            public static implicit operator int(size_t size)
            {
                return size.Value.ToInt32();
            }

            public static implicit operator size_t(long size)
            {
                return new size_t(size);
            }

            public static implicit operator size_t(int size)
            {
                return new size_t(size);
            }
        }

        public struct StringUtf8
        {
            public StringUtf8(IntPtr pointer)
            {
                this.pointer = pointer;
            }

            private readonly IntPtr pointer;

            public bool IsEmpty => pointer == IntPtr.Zero;

            public static implicit operator string(StringUtf8 stringUtf8)
            {
                if (stringUtf8.pointer != IntPtr.Zero)
                {
                    return Utf8ToString(stringUtf8.pointer);
                }
                return null;
            }

            public static unsafe implicit operator StringUtf8(string text)
            {
                if (text == null)
                {
                    return new StringUtf8(IntPtr.Zero);
                }

#if SUPPORT_UTF8_GETBYTES_UNSAFE
                var length = Encoding.UTF8.GetByteCount(text);
                var pointer = sass_alloc_memory(length+1);
                fixed (char* pText = text)
                    Encoding.UTF8.GetBytes(pText, text.Length, (byte*) pointer, length);
#else

                var buffer = Encoding.UTF8.GetBytes(text);
                var length = buffer.Length;
                var pointer = sass_alloc_memory(length+1);
                Marshal.Copy(buffer, 0, pointer, length);
#endif
                ((byte*)pointer)[length] = 0;
                return new StringUtf8(pointer);
            }

            private static unsafe string Utf8ToString(IntPtr utfString)
            {
                var pText = (byte*)utfString;
                // find the end of the string
                while (*pText != 0)
                {
                    pText++;
                }
                int length = (int)(pText - (byte*)utfString);
#if SUPPORT_UTF8_GETSTRING_UNSAFE
                var text = Encoding.UTF8.GetString((byte*)utfString, length);
#else
                // should not be null terminated
                byte[] strbuf = new byte[length]; // TODO: keep a buffer bool
                // skip the trailing null
                Marshal.Copy(utfString, strbuf, 0, length);
                var text = Encoding.UTF8.GetString(strbuf,0, length);
#endif
                return text;
            }
        }

        private sealed class UTF8EncodingRelaxed : UTF8Encoding
        {
            public new static readonly UTF8EncodingRelaxed Default = new UTF8EncodingRelaxed();

            public UTF8EncodingRelaxed() : base(false, false)
            {
            }
        }

        internal abstract class SassAllocator
        {
            [ThreadStatic]
            private static List<IntPtr> PointersToFree;

            public static void FreeNative()
            {
                var pointersToFree = PointersToFree;
                if (pointersToFree == null) return;
                try
                {
                    foreach (var pData in pointersToFree)
                    {
                        sass_free_memory(pData);
                    }

                }
                finally
                {
                    pointersToFree.Clear();
                }
            }

            public static void RecordFreeNative(IntPtr pNativeData)
            {
                if (pNativeData == IntPtr.Zero) return;
                if (PointersToFree == null)
                {
                    PointersToFree = new List<IntPtr>();
                }

                var pointersToFree = PointersToFree;
                pointersToFree.Add(pNativeData);
            }
        }

        private class UTF8MarshallerBase<T> : SassAllocator, ICustomMarshaler where T : Encoding, new()
        {
            private static readonly Encoding EncodingUsed = new T();

            private readonly bool _freeNative;

            protected UTF8MarshallerBase(bool freeNative)
            {
                _freeNative = freeNative;
            }

            public static unsafe string FromNative(IntPtr pNativeData)
            {
                var pBuffer = (byte*)pNativeData;
                if (pBuffer == null) return string.Empty;
                int length = 0;
                while (pBuffer[length] != 0)
                {
                    length++;
                }
                return EncodingUsed.GetString((byte*)pNativeData, length);
            }

            public static unsafe IntPtr ToNative(string text, bool strict = false)
            {
                if (text == null) return IntPtr.Zero;

                var length = EncodingUsed.GetByteCount(text);
                var pBuffer = (byte*)sass_alloc_memory(length + 1);
                if (pBuffer == null) return IntPtr.Zero;

                if (length > 0)
                {
                    fixed (char* ptr = text)
                    {
                        EncodingUsed.GetBytes(ptr, text.Length, pBuffer, length);
                    }
                }

                pBuffer[length] = 0;

                return new IntPtr(pBuffer);
            }

            public virtual object MarshalNativeToManaged(IntPtr pNativeData)
            {
                return FromNative(pNativeData);
            }

            public virtual IntPtr MarshalManagedToNative(object managedObj)
            {
                var text = (string)managedObj;
                return ToNative(text);
            }

            public virtual void CleanUpNativeData(IntPtr pNativeData)
            {
                if (!_freeNative) return;
                RecordFreeNative(pNativeData);
            }

            public virtual void CleanUpManagedData(object ManagedObj)
            {
            }

            public int GetNativeDataSize()
            {
                return -1;
            }
        }

        private sealed class UTF8MarshallerNoFree : UTF8MarshallerBase<UTF8EncodingRelaxed>
        {
            private static readonly UTF8MarshallerNoFree Instance = new UTF8MarshallerNoFree(false);

            public UTF8MarshallerNoFree(bool freeNative) : base(freeNative)
            {
            }

            public static ICustomMarshaler GetInstance(string cookie)
            {
                return Instance;
            }
        }
    }
}