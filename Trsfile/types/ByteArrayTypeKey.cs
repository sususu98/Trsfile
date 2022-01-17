namespace com.riscure.trs.types
{
    using TraceParameter = parameter.TraceParameter;
    using ByteArrayParameter = parameter.primitive.ByteArrayParameter;

    public class ByteArrayTypeKey : TypedKey<byte[]>
    {
        public ByteArrayTypeKey(string key) : base(typeof(byte[]), key)
        {
        }

        public override TraceParameter CreateParameter(byte[] value)
        {
            CheckLength(value);
            return new ByteArrayParameter(value);
        }
        protected internal override void CheckLength(byte[] value)
        {
            if (value.Length <= 0) throw new ArgumentException("Value's length below 0");
        }
    }

}