namespace com.riscure.trs.types
{
    using TraceParameter = parameter.TraceParameter;
    using LongArrayParameter = parameter.primitive.LongArrayParameter;

    public class LongArrayTypeKey : TypedKey<long>
    {
        public LongArrayTypeKey(string key) : base(typeof(long[]), key, true)
        {
        }

        public override TraceParameter CreateParameter(long[] value)
        {
            if (value.Length <= 0) throw new ArgumentException("Value's length below 0");
            if (value.Length == 1) IsArray = false;
            return new LongArrayParameter(value);
        }
    }

}