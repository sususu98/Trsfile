using System.Text;

namespace Trsfile.IO
{
    public class LittleEndianOutputStream : IDisposable
    {
        /// <summary>
        /// The number of bytes written to the data output stream so far.
        /// If this counter overflows, it will be wrapped to Integer.MAX_VALUE.
        /// </summary>
        protected internal int written;

        private Stream stream;
        private readonly object obj = new();

        /// <summary>
        /// Creates an output stream filter built on top of the specified
        /// underlying output stream.
        /// </summary>
        /// <param name="outS"> the underlying output stream to be assigned to
        ///            the field this.out for later use, or
        ///            null if this instance is to be
        ///            created without an underlying stream. </param>
        public LittleEndianOutputStream(Stream outS)
        {
            stream = outS;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (stream is not null)
                {
                    stream.Close();
                    stream.Dispose();
#pragma warning disable CS8625
                    stream = null;
#pragma warning restore CS8625
                }
            }
        }

        /// <summary>
        /// Increases the written counter by the specified value
        /// until it reaches Integer.MAX_VALUE.
        /// </summary>
        private void IncCount(int value)
        {
            int temp = written + value;
            if (temp < 0)
            {
                temp = int.MaxValue;
            }
            written = temp;
        }



        /// <summary>
        /// Writes the specified byte (the low eight bits of the argument
        /// <paramref name="b"/>) to the underlying output stream. If no exception
        /// is thrown, the counter written is incremented by 1.
        /// <para>
        /// Implements the write method of OutputStream.
        /// </para>
        /// </summary>
        /// <param name="b"> the byte to be written. </param>
        /// <exception cref="IOException"> if an I/O error occurs. </exception>
        public void Write(int b)
        {
            lock (obj)
            {
                stream.Write(BitConverter.GetBytes(b));
                IncCount(1);
            }
        }

        /// <summary>
        /// Writes len bytes from the specified byte array
        /// starting at offset off to the underlying output stream.
        /// If no exception is thrown, the counter written is
        /// incremented by len.
        /// </summary>
        /// <param name="b">   the data. </param>
        /// <param name="off"> the start offset in the data. </param>
        /// <param name="len"> the number of bytes to write. </param>
        /// <exception cref="IOException"> if an I/O error occurs. </exception>
        public void Write(byte[] b, int off, int len)
        {
            lock (obj)
            {
                stream.Write(b, off, len);
                IncCount(len);
            }
        }

        public void Write(ReadOnlySpan<byte> b)
        {
            lock (obj)
            {
                stream.Write(b);
                IncCount(b.Length);
            }
        }

        /// <summary>
        /// Flushes this data output stream. This forces any buffered output
        /// bytes to be written out to the stream.
        /// <para>
        /// The flush method of DataOutputStream
        /// calls the flush method of its underlying output stream.
        /// </para>
        /// </summary>
        /// <exception cref="IOException"> if an I/O error occurs. </exception>
        public void Flush()
        {
            stream.Flush();
        }

        /// <summary>
        /// Writes a boolean to the underlying output stream as
        /// a 1-byte value. The value true is written out as the
        /// value (byte)1; the value false is
        /// written out as the value (byte)0. If no exception is
        /// thrown, the counter written is incremented by 1.
        /// </summary>
        /// <param name="v"> a boolean value to be written. </param>
        /// <exception cref="IOException"> if an I/O error occurs. </exception>
        public void WriteBoolean(bool v)
        {
            stream.WriteByte(v ? (byte)1 : (byte)0);
            IncCount(1);
        }

        /// <summary>
        /// Writes out a byte to the underlying output stream as
        /// a 1-byte value. If no exception is thrown, the counter
        /// written is incremented by 1.
        /// </summary>
        /// <param name="v"> a byte value to be written. </param>
        /// <exception cref="IOException"> if an I/O error occurs. </exception>
        /// <seealso cref="java.io.FilterOutputStream.out"/>
        public void WriteByte(int v)
        {
            stream.WriteByte((byte)v);
            IncCount(1);
        }

        /// <summary>
        /// Writes a short to the underlying output stream as two
        /// bytes, high byte first. If no exception is thrown, the counter
        /// written is incremented by 2.
        /// </summary>
        /// <param name="v"> a short to be written. </param>
        /// <exception cref="IOException"> if an I/O error occurs. </exception>
        public void WriteShort(short v)
        {
            stream.WriteByte((byte)(((int)((uint)v >> 0)) & 0xFF));
            stream.WriteByte((byte)(((int)((uint)v >> 8)) & 0xFF));
            IncCount(2);
        }

        public void WriteShort(int v)
        {
            WriteShort((short)v);
        }

        /// <summary>
        /// Writes a char to the underlying output stream as a
        /// 2-byte value, high byte first. If no exception is thrown, the
        /// counter written is incremented by 2.
        /// </summary>
        /// <param name="v"> a char value to be written. </param>
        /// <exception cref="IOException"> if an I/O error occurs. </exception>
        public void WriteChar(char v)
        {
            stream.WriteByte((byte)(((int)((uint)v >> 0)) & 0xFF));
            stream.WriteByte((byte)(((int)((uint)v >> 8)) & 0xFF));
            IncCount(2);
        }

        /// <summary>
        /// Writes an int to the underlying output stream as four
        /// bytes, high byte first. If no exception is thrown, the counter
        /// written is incremented by 4.
        /// </summary>
        /// <param name="v"> an int to be written. </param>
        /// <exception cref="IOException"> if an I/O error occurs. </exception>
        public void WriteInt(int v)
        {
            stream.WriteByte((byte)(((int)((uint)v >> 0)) & 0xFF));
            stream.WriteByte((byte)(((int)((uint)v >> 8)) & 0xFF));
            stream.WriteByte((byte)(((int)((uint)v >> 16)) & 0xFF));
            stream.WriteByte((byte)(((int)((uint)v >> 24)) & 0xFF));
            IncCount(4);
        }

        private byte[] writeBuffer = new byte[8];

        /// <summary>
        /// Writes a long to the underlying output stream as eight
        /// bytes, high byte first. In no exception is thrown, the counter
        /// written is incremented by 8.
        /// </summary>
        /// <param name="v"> a long to be written. </param>
        /// <exception cref="IOException"> if an I/O error occurs. </exception>
        public void WriteLong(long v)
        {
            writeBuffer[0] = (byte)((long)((ulong)v >> 0));
            writeBuffer[1] = (byte)((long)((ulong)v >> 8));
            writeBuffer[2] = (byte)((long)((ulong)v >> 16));
            writeBuffer[3] = (byte)((long)((ulong)v >> 24));
            writeBuffer[4] = (byte)((long)((ulong)v >> 32));
            writeBuffer[5] = (byte)((long)((ulong)v >> 40));
            writeBuffer[6] = (byte)((long)((ulong)v >> 48));
            writeBuffer[7] = (byte)((long)((ulong)v >> 56));
            stream.Write(writeBuffer, 0, 8);
            IncCount(8);
        }

        /// <summary>
        /// Converts the float argument to an int using the
        /// floatToIntBits method in class Float,
        /// and then writes that int value to the underlying
        /// output stream as a 4-byte quantity, high byte first. If no
        /// exception is thrown, the counter written is
        /// incremented by 4.
        /// </summary>
        /// <param name="v"> a float value to be written. </param>
        /// <exception cref="IOException"> if an I/O error occurs. </exception>
        public void WriteFloat(float v)
        {
            WriteInt(BitConverter.SingleToInt32Bits(v));
        }

        /// <summary>
        /// Converts the double argument to a long using the
        /// doubleToLongBits method in class Double,
        /// and then writes that long value to the underlying
        /// output stream as an 8-byte quantity, high byte first. If no
        /// exception is thrown, the counter written is
        /// incremented by 8.
        /// </summary>
        /// <param name="v"> a double value to be written. </param>
        /// <exception cref="IOException"> if an I/O error occurs. </exception>
        public void WriteDouble(double v)
        {
            WriteLong(BitConverter.DoubleToInt64Bits(v));
        }

        /// <summary>
        /// Writes out the string to the underlying output stream as a
        /// sequence of bytes. Each character in the string is written out, in
        /// sequence, by discarding its high eight bits. If no exception is
        /// thrown, the counter written is incremented by the
        /// length of s.
        /// </summary>
        /// <param name="s"> a string of bytes to be written. </param>
        /// <exception cref="IOException"> if an I/O error occurs. </exception>
        public void WriteBytes(string s)
        {
            int len = s.Length;
            for (int i = 0; i < len; i++)
            {
                stream.WriteByte((byte)s[i]);
            }
            IncCount(len);
        }

        /// <summary>
        /// Writes a string to the underlying output stream as a sequence of
        /// characters. Each character is written to the data output stream as
        /// if by the writeChar method. If no exception is
        /// thrown, the counter written is incremented by twice
        /// the length of s.
        /// </summary>
        /// <param name="s"> a String value to be written. </param>
        /// <exception cref="IOException"> if an I/O error occurs. </exception>
        public void WriteChars(string s)
        {
            int len = s.Length;
            for (int i = 0; i < len; i++)
            {
                int v = s[i];
                stream.WriteByte((byte)(((int)((uint)v >> 0)) & 0xFF));
                stream.WriteByte((byte)(((int)((uint)v >> 8)) & 0xFF));
            }
            IncCount(len * 2);
        }

        /// <summary>
        /// Writes a string to the underlying output stream using
        /// modified UTF-8
        /// encoding in a machine-independent manner.
        /// <para>
        /// First, two bytes are written to the output stream as if by the
        /// writeShort method giving the number of bytes to
        /// follow. This value is the number of bytes actually written out,
        /// not the length of the string. Following the length, each character
        /// of the string is output, in sequence, using the modified UTF-8 encoding
        /// for the character. If no exception is thrown, the counter
        /// written is incremented by the total number of
        /// bytes written to the output stream. This will be at least two
        /// plus the length of str, and at most two plus
        /// thrice the length of str.
        /// 
        /// </para>
        /// </summary>
        /// <param name="str"> a string to be written. </param>
        /// <exception cref="IOException"> if an I/O error occurs. </exception>
        public void WriteUTF(string str)
        {
            byte[] bytes = str.GetBytes(Encoding.UTF8);
            WriteShort((short)bytes.Length);
            stream.Write(bytes);
            IncCount(bytes.Length);
        }

        /// <summary>
        /// Returns the current value of the counter written,
        /// the number of bytes written to this data output stream so far.
        /// If the counter overflows, it will be wrapped to Integer.MAX_VALUE.
        /// </summary>
        /// <returns> the value of the written field. </returns>
        public int Size => written;
    }

}