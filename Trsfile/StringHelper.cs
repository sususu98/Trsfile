﻿using System.Text;
using System.Text.RegularExpressions;

namespace Trsfile
{
    public static class StringHelper
    {
        //------------------------------------------------------------------------------------
        //	This method is used to replace calls to the 2-arg Java String.startsWith method.
        //------------------------------------------------------------------------------------
        public static bool StartsWith(this string self, string prefix, int toffset)
        {
            return self.IndexOf(prefix, toffset, StringComparison.Ordinal) == toffset;
        }

        //------------------------------------------------------------------------------
        //	This method is used to replace most calls to the Java String.split method.
        //------------------------------------------------------------------------------
        public static string[] Split(this string self, string regexDelimiter, bool trimTrailingEmptyStrings)
        {
            string[] splitArray = Regex.Split(self, regexDelimiter);

            if (trimTrailingEmptyStrings)
            {
                if (splitArray.Length > 1)
                {
                    for (int i = splitArray.Length; i > 0; i--)
                    {
                        if (splitArray[i - 1].Length > 0)
                        {
                            if (i < splitArray.Length)
                                Array.Resize(ref splitArray, i);

                            break;
                        }
                    }
                }
            }

            return splitArray;
        }

        //-----------------------------------------------------------------------------
        //	These methods are used to replace calls to some Java String constructors.
        //-----------------------------------------------------------------------------
        public static string NewString(byte[] bytes)
        {
            return NewString(bytes, 0, bytes.Length);
        }
        public static string NewString(byte[] bytes, int index, int count)
        {
            return Encoding.UTF8.GetString((byte[])(object)bytes, index, count);
        }
        public static string NewString(byte[] bytes, string encoding)
        {
            return NewString(bytes, 0, bytes.Length, encoding);
        }
        public static string NewString(byte[] bytes, int index, int count, string encoding)
        {
            return NewString(bytes, index, count, Encoding.GetEncoding(encoding));
        }
        public static string NewString(byte[] bytes, Encoding encoding)
        {
            return NewString(bytes, 0, bytes.Length, encoding);
        }
        public static string NewString(byte[] bytes, int index, int count, Encoding encoding)
        {
            return encoding.GetString((byte[])(object)bytes, index, count);
        }

        //--------------------------------------------------------------------------------
        //	These methods are used to replace calls to the Java String.getBytes methods.
        //--------------------------------------------------------------------------------
        public static byte[] GetBytes(this string self)
        {
            return GetSBytesForEncoding(Encoding.UTF8, self);
        }
        public static byte[] GetBytes(this string self, Encoding encoding)
        {
            return GetSBytesForEncoding(encoding, self);
        }
        public static byte[] GetBytes(this string self, string encoding)
        {
            return GetSBytesForEncoding(Encoding.GetEncoding(encoding), self);
        }
        private static byte[] GetSBytesForEncoding(Encoding encoding, string s)
        {
            byte[] sbytes = new byte[encoding.GetByteCount(s)];
            encoding.GetBytes(s, 0, s.Length, (byte[])(object)sbytes, 0);
            return sbytes;
        }
    }
}