﻿using System.IO;
using System.Text;

namespace com.riscure.trs.io
{

	public class LittleEndianOutputStream : FilterOutputStream, DataOutput
	{
		/// <summary>
		/// The number of bytes written to the data output stream so far.
		/// If this counter overflows, it will be wrapped to Integer.MAX_VALUE.
		/// </summary>
		protected internal int written;

		/// <summary>
		/// Creates an output stream filter built on top of the specified
		/// underlying output stream.
		/// </summary>
		/// <param name="out"> the underlying output stream to be assigned to
		///            the field <code>this.out</code> for later use, or
		///            <code>null</code> if this instance is to be
		///            created without an underlying stream. </param>
		public LittleEndianOutputStream(Stream @out) : base(@out)
		{
		}

		/// <summary>
		/// Increases the written counter by the specified value
		/// until it reaches Integer.MAX_VALUE.
		/// </summary>
		private void incCount(int value)
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
		/// <code>b</code>) to the underlying output stream. If no exception
		/// is thrown, the counter <code>written</code> is incremented by
		/// <code>1</code>.
		/// <para>
		/// Implements the <code>write</code> method of <code>OutputStream</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="b"> the <code>byte</code> to be written. </param>
		/// <exception cref="IOException"> if an I/O error occurs. </exception>
		/// <seealso cref="java.io.FilterOutputStream.out"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override public synchronized void write(int b) throws IOException
		public override void write(int b)
		{
			lock (this)
			{
				@out.write(b);
				incCount(1);
			}
		}

		/// <summary>
		/// Writes <code>len</code> bytes from the specified byte array
		/// starting at offset <code>off</code> to the underlying output stream.
		/// If no exception is thrown, the counter <code>written</code> is
		/// incremented by <code>len</code>.
		/// </summary>
		/// <param name="b">   the data. </param>
		/// <param name="off"> the start offset in the data. </param>
		/// <param name="len"> the number of bytes to write. </param>
		/// <exception cref="IOException"> if an I/O error occurs. </exception>
		/// <seealso cref="java.io.FilterOutputStream.out"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override public synchronized void write(byte[] b, int off, int len) throws IOException
		public override void write(sbyte[] b, int off, int len)
		{
			lock (this)
			{
				@out.write(b, off, len);
				incCount(len);
			}
		}

		/// <summary>
		/// Flushes this data output stream. This forces any buffered output
		/// bytes to be written out to the stream.
		/// <para>
		/// The <code>flush</code> method of <code>DataOutputStream</code>
		/// calls the <code>flush</code> method of its underlying output stream.
		/// 
		/// </para>
		/// </summary>
		/// <exception cref="IOException"> if an I/O error occurs. </exception>
		/// <seealso cref="java.io.FilterOutputStream.out"/>
		/// <seealso cref="java.io.OutputStream.flush()"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override public void flush() throws IOException
		public override void flush()
		{
			@out.flush();
		}

		/// <summary>
		/// Writes a <code>boolean</code> to the underlying output stream as
		/// a 1-byte value. The value <code>true</code> is written out as the
		/// value <code>(byte)1</code>; the value <code>false</code> is
		/// written out as the value <code>(byte)0</code>. If no exception is
		/// thrown, the counter <code>written</code> is incremented by
		/// <code>1</code>.
		/// </summary>
		/// <param name="v"> a <code>boolean</code> value to be written. </param>
		/// <exception cref="IOException"> if an I/O error occurs. </exception>
		/// <seealso cref="java.io.FilterOutputStream.out"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override public final void writeBoolean(boolean v) throws IOException
		public override void writeBoolean(bool v)
		{
			@out.write(v ? 1 : 0);
			incCount(1);
		}

		/// <summary>
		/// Writes out a <code>byte</code> to the underlying output stream as
		/// a 1-byte value. If no exception is thrown, the counter
		/// <code>written</code> is incremented by <code>1</code>.
		/// </summary>
		/// <param name="v"> a <code>byte</code> value to be written. </param>
		/// <exception cref="IOException"> if an I/O error occurs. </exception>
		/// <seealso cref="java.io.FilterOutputStream.out"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override public final void writeByte(int v) throws IOException
		public override void writeByte(int v)
		{
			@out.write(v);
			incCount(1);
		}

		/// <summary>
		/// Writes a <code>short</code> to the underlying output stream as two
		/// bytes, high byte first. If no exception is thrown, the counter
		/// <code>written</code> is incremented by <code>2</code>.
		/// </summary>
		/// <param name="v"> a <code>short</code> to be written. </param>
		/// <exception cref="IOException"> if an I/O error occurs. </exception>
		/// <seealso cref="java.io.FilterOutputStream.out"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override public final void writeShort(int v) throws IOException
		public override void writeShort(int v)
		{
			@out.write(((int)((uint)v >> 0)) & 0xFF);
			@out.write(((int)((uint)v >> 8)) & 0xFF);
			incCount(2);
		}

		/// <summary>
		/// Writes a <code>char</code> to the underlying output stream as a
		/// 2-byte value, high byte first. If no exception is thrown, the
		/// counter <code>written</code> is incremented by <code>2</code>.
		/// </summary>
		/// <param name="v"> a <code>char</code> value to be written. </param>
		/// <exception cref="IOException"> if an I/O error occurs. </exception>
		/// <seealso cref="java.io.FilterOutputStream.out"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override public final void writeChar(int v) throws IOException
		public override void writeChar(int v)
		{
			@out.write(((int)((uint)v >> 0)) & 0xFF);
			@out.write(((int)((uint)v >> 8)) & 0xFF);
			incCount(2);
		}

		/// <summary>
		/// Writes an <code>int</code> to the underlying output stream as four
		/// bytes, high byte first. If no exception is thrown, the counter
		/// <code>written</code> is incremented by <code>4</code>.
		/// </summary>
		/// <param name="v"> an <code>int</code> to be written. </param>
		/// <exception cref="IOException"> if an I/O error occurs. </exception>
		/// <seealso cref="java.io.FilterOutputStream.out"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override public final void writeInt(int v) throws IOException
		public override void writeInt(int v)
		{
			@out.write(((int)((uint)v >> 0)) & 0xFF);
			@out.write(((int)((uint)v >> 8)) & 0xFF);
			@out.write(((int)((uint)v >> 16)) & 0xFF);
			@out.write(((int)((uint)v >> 24)) & 0xFF);
			incCount(4);
		}

		private sbyte[] writeBuffer = new sbyte[8];

		/// <summary>
		/// Writes a <code>long</code> to the underlying output stream as eight
		/// bytes, high byte first. In no exception is thrown, the counter
		/// <code>written</code> is incremented by <code>8</code>.
		/// </summary>
		/// <param name="v"> a <code>long</code> to be written. </param>
		/// <exception cref="IOException"> if an I/O error occurs. </exception>
		/// <seealso cref="java.io.FilterOutputStream.out"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override public final void writeLong(long v) throws IOException
		public override void writeLong(long v)
		{
			writeBuffer[0] = (sbyte)((long)((ulong)v >> 0));
			writeBuffer[1] = (sbyte)((long)((ulong)v >> 8));
			writeBuffer[2] = (sbyte)((long)((ulong)v >> 16));
			writeBuffer[3] = (sbyte)((long)((ulong)v >> 24));
			writeBuffer[4] = (sbyte)((long)((ulong)v >> 32));
			writeBuffer[5] = (sbyte)((long)((ulong)v >> 40));
			writeBuffer[6] = (sbyte)((long)((ulong)v >> 48));
			writeBuffer[7] = (sbyte)((long)((ulong)v >> 56));
			@out.write(writeBuffer, 0, 8);
			incCount(8);
		}

		/// <summary>
		/// Converts the float argument to an <code>int</code> using the
		/// <code>floatToIntBits</code> method in class <code>Float</code>,
		/// and then writes that <code>int</code> value to the underlying
		/// output stream as a 4-byte quantity, high byte first. If no
		/// exception is thrown, the counter <code>written</code> is
		/// incremented by <code>4</code>.
		/// </summary>
		/// <param name="v"> a <code>float</code> value to be written. </param>
		/// <exception cref="IOException"> if an I/O error occurs. </exception>
		/// <seealso cref="java.io.FilterOutputStream.out"/>
		/// <seealso cref="java.lang.Float.floatToIntBits(float)"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override public final void writeFloat(float v) throws IOException
		public override void writeFloat(float v)
		{
			writeInt(Float.floatToIntBits(v));
		}

		/// <summary>
		/// Converts the double argument to a <code>long</code> using the
		/// <code>doubleToLongBits</code> method in class <code>Double</code>,
		/// and then writes that <code>long</code> value to the underlying
		/// output stream as an 8-byte quantity, high byte first. If no
		/// exception is thrown, the counter <code>written</code> is
		/// incremented by <code>8</code>.
		/// </summary>
		/// <param name="v"> a <code>double</code> value to be written. </param>
		/// <exception cref="IOException"> if an I/O error occurs. </exception>
		/// <seealso cref="java.io.FilterOutputStream.out"/>
		/// <seealso cref="java.lang.Double.doubleToLongBits(double)"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override public final void writeDouble(double v) throws IOException
		public override void writeDouble(double v)
		{
			writeLong(System.BitConverter.DoubleToInt64Bits(v));
		}

		/// <summary>
		/// Writes out the string to the underlying output stream as a
		/// sequence of bytes. Each character in the string is written out, in
		/// sequence, by discarding its high eight bits. If no exception is
		/// thrown, the counter <code>written</code> is incremented by the
		/// length of <code>s</code>.
		/// </summary>
		/// <param name="s"> a string of bytes to be written. </param>
		/// <exception cref="IOException"> if an I/O error occurs. </exception>
		/// <seealso cref="java.io.FilterOutputStream.out"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override public final void writeBytes(String s) throws IOException
		public override void writeBytes(string s)
		{
			int len = s.Length;
			for (int i = 0; i < len; i++)
			{
				@out.write((sbyte) s[i]);
			}
			incCount(len);
		}

		/// <summary>
		/// Writes a string to the underlying output stream as a sequence of
		/// characters. Each character is written to the data output stream as
		/// if by the <code>writeChar</code> method. If no exception is
		/// thrown, the counter <code>written</code> is incremented by twice
		/// the length of <code>s</code>.
		/// </summary>
		/// <param name="s"> a <code>String</code> value to be written. </param>
		/// <exception cref="IOException"> if an I/O error occurs. </exception>
		/// <seealso cref="java.io.DataOutputStream.writeChar(int)"/>
		/// <seealso cref="java.io.FilterOutputStream.out"/>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override public final void writeChars(String s) throws IOException
		public override void writeChars(string s)
		{
			int len = s.Length;
			for (int i = 0; i < len; i++)
			{
				int v = s[i];
				@out.write(((int)((uint)v >> 0)) & 0xFF);
				@out.write(((int)((uint)v >> 8)) & 0xFF);
			}
			incCount(len * 2);
		}

		/// <summary>
		/// Writes a string to the underlying output stream using
		/// <a href="DataInput.html#modified-utf-8">modified UTF-8</a>
		/// encoding in a machine-independent manner.
		/// <para>
		/// First, two bytes are written to the output stream as if by the
		/// <code>writeShort</code> method giving the number of bytes to
		/// follow. This value is the number of bytes actually written out,
		/// not the length of the string. Following the length, each character
		/// of the string is output, in sequence, using the modified UTF-8 encoding
		/// for the character. If no exception is thrown, the counter
		/// <code>written</code> is incremented by the total number of
		/// bytes written to the output stream. This will be at least two
		/// plus the length of <code>str</code>, and at most two plus
		/// thrice the length of <code>str</code>.
		/// 
		/// </para>
		/// </summary>
		/// <param name="str"> a string to be written. </param>
		/// <exception cref="IOException"> if an I/O error occurs. </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override public final void writeUTF(String str) throws IOException
		public override void writeUTF(string str)
		{
			sbyte[] bytes = str.GetBytes(Encoding.UTF8);
			writeShort(bytes.Length);
			@out.write(bytes);
			incCount(bytes.Length);
		}

		/// <summary>
		/// Returns the current value of the counter <code>written</code>,
		/// the number of bytes written to this data output stream so far.
		/// If the counter overflows, it will be wrapped to Integer.MAX_VALUE.
		/// </summary>
		/// <returns> the value of the <code>written</code> field. </returns>
		/// <seealso cref="LittleEndianOutputStream.written"/>
		public int size()
		{
			return written;
		}
	}

}