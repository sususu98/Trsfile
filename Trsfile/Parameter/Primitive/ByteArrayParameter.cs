﻿using Trsfile.IO;

namespace Trsfile.Parameter.Primitive
{
    public class ByteArrayParameter : TraceParameter<byte>
    {
        private const string INVALID_LENGTH = "Error parsing byte array: Expected ({0:d}) bytes but found ({1:d})";

        public ByteArrayParameter(byte value) : base(value)
        {

        }

        public ByteArrayParameter(byte[] value) : base(value)
        {

        }

        public ByteArrayParameter(ByteArrayParameter toCopy) : this((byte[])toCopy.Value.Clone())
        {
        }
        public override void Serialize(LittleEndianOutputStream dos)
        {
            dos.Write(Value);
        }

        public static ByteArrayParameter Deserialize(LittleEndianInputStream dis, int length)
        {
            ByteArrayParameter result = new(new byte[length]);
            int bytesRead = dis.Read(result.Value);
            if (bytesRead != length)
            {
                throw new IOException(string.Format(INVALID_LENGTH, length, bytesRead));
            }
            return result;
        }
        public override ByteArrayParameter Clone()
        {
            return new ByteArrayParameter(this);
        }

        public override string ToString()
        {
            return HexUtils.ToHexString(Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

    }

}