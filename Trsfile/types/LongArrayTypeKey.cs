namespace com.riscure.trs.types
{
    using TraceParameter = parameter.TraceParameter;
    using LongArrayParameter = parameter.primitive.LongArrayParameter;

    public class LongArrayTypeKey : TypedKey<long[]>
    {
        public LongArrayTypeKey(string key) : base(typeof(long[]), key)
        {
        }

        public override TraceParameter CreateParameter(long[] value)
        {
            CheckLength(value);
            return new LongArrayParameter(value);
        }
        protected internal override void CheckLength(double[] value)
        {
            if (value.Length <= 0) throw new ArgumentException("Value's length below 0");
        }
    }

}