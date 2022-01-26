

namespace com.riscure.trs.types
{
    using TraceParameter = parameter.TraceParameter;
    using ByteArrayParameter = parameter.primitive.ByteArrayParameter;
    public class ByteTypeKey : TypedKey<byte>
    {
        public ByteTypeKey(string key) : base(typeof(byte), key, false)
        {
        }

        public override TraceParameter CreateParameter(byte value)
        {
            return new ByteArrayParameter(value);
        }

        public override TraceParameter CreateParameter(byte[] value)
        {
            throw new NotSupportedException("This is single type key, this method is not supported.");
        }
    }

}