using LittleEndianInputStream = com.riscure.trs.io.LittleEndianInputStream;
using LittleEndianOutputStream = com.riscure.trs.io.LittleEndianOutputStream;

namespace com.riscure.trs.parameter.primitive
{
    public class ByteArrayParameter : TraceParameter<byte>
    {
        private const string INVALID_LENGTH = "Error parsing byte array: Expected (%d) bytes but found (%d)";

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

        public override bool Equals(object? o)
        {
            if (this == o)
            {
                return true;
            }
            if (o == null || this.GetType() != o.GetType())
            {
                return false;
            }

            ByteArrayParameter that = (ByteArrayParameter)o;

            return Value.SequenceEqual(that.Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

    }

}