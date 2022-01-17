

namespace com.riscure.trs.types
{
    using TraceParameter = parameter.TraceParameter;
    using ByteArrayParameter = parameter.primitive.ByteArrayParameter;
    public class ByteTypeKey : TypedKey<byte>
    {
        public ByteTypeKey(string key) : base(typeof(byte), key)
        {
        }

        public override TraceParameter CreateParameter(byte value)
        {
            CheckLength(value);
            return new ByteArrayParameter(new byte[] { value });
        }
        protected internal override void CheckLength(byte value)
        {

        }
    }

}