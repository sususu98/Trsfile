namespace Trsfile.IO
{

    public class LittleEndianInputStream : IDisposable
    {

        private Stream stream;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inS"></param>
        public LittleEndianInputStream(Stream inS)
        {
            stream = inS;
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

        public int Read(byte[] b)
        {
            return stream.Read(b, 0, b.Length);
        }


        public int Read(byte[] b, int off, int len)
        {
            return stream.Read(b, off, len);
        }


        public void ReadFully(byte[] b)
        {
            ReadFully(b, 0, b.Length);
        }
        public void ReadFully(byte[] b, int off, int len)
        {
            if (len < 0)
            {
                throw new IndexOutOfRangeException();
            }
            int n = 0;
            while (n < len)
            {
                int count = stream.Read(b, off + n, len - n);
                if (count <= 0)
                {
                    throw new EndOfStreamException();
                }
                n += count;
            }
        }
        public int SkipBytes(int n)
        {
            int total = 0;
            int cur;
            while ((total < n) && ((cur = (int)stream.Seek(n - total, SeekOrigin.Current)) > 0))
            {
                total += cur;
            }

            return total;
        }
        public bool ReadBoolean()
        {
            int ch = stream.ReadByte();
            if (ch < 0)
            {
                throw new EndOfStreamException();
            }
            return (ch != 0);
        }
        public sbyte ReadByte()
        {
            int ch = stream.ReadByte();
            if (ch < 0)
            {
                throw new EndOfStreamException();
            }
            return (sbyte)(ch);
        }
        public int ReadUnsignedByte()
        {
            int ch = stream.ReadByte();
            if (ch < 0)
            {
                throw new EndOfStreamException();
            }
            return ch;
        }
        public short ReadShort()
        {
            int ch1 = stream.ReadByte();
            int ch2 = stream.ReadByte();
            if ((ch1 | ch2) < 0)
            {
                throw new EndOfStreamException();
            }
            return (short)((ch1 << 0) + (ch2 << 8));
        }
        public int ReadUnsignedShort()
        {
            int ch1 = stream.ReadByte();
            int ch2 = stream.ReadByte();
            if ((ch1 | ch2) < 0)
            {
                throw new EndOfStreamException();
            }
            return (ch1 << 0) + (ch2 << 8);
        }
        public char ReadChar()
        {
            int ch1 = stream.ReadByte();
            int ch2 = stream.ReadByte();
            if ((ch1 | ch2) < 0)
            {
                throw new EndOfStreamException();
            }
            return (char)((ch1 << 0) + (ch2 << 8));
        }

        public int ReadInt()
        {
            int ch1 = stream.ReadByte();
            int ch2 = stream.ReadByte();
            int ch3 = stream.ReadByte();
            int ch4 = stream.ReadByte();
            if ((ch1 | ch2 | ch3 | ch4) < 0)
            {
                throw new EndOfStreamException();
            }
            return ((ch1 << 0) + (ch2 << 8) + (ch3 << 16) + (ch4 << 24));
        }

        private byte[] readBuffer = new byte[8];
        public long ReadLong()
        {
            ReadFully(readBuffer, 0, 8);
            return (((long)readBuffer[0] << 0) + ((long)(readBuffer[1] & 255) << 8) + ((long)(readBuffer[2] & 255) << 16) + ((long)(readBuffer[3] & 255) << 24) + ((long)(readBuffer[4] & 255) << 32) + ((long)(readBuffer[5] & 255) << 40) + ((long)(readBuffer[6] & 255) << 48) + ((long)(readBuffer[7] & 255) << 56));
        }
        public float ReadFloat()
        {
            return BitConverter.Int32BitsToSingle(ReadInt());
        }

        public double ReadDouble()
        {
            return BitConverter.Int64BitsToDouble(ReadLong());
        }
        public string ReadUTF()
        {
            int length = ReadUnsignedShort();
            byte[] bytes = new byte[length];
            int read = this.Read(bytes);
            if (read != length)
            {
                throw new EndOfStreamException();
            }
            return StringHelper.NewString(bytes, System.Text.Encoding.UTF8);
        }
    }

}