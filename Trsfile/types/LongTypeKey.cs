namespace com.riscure.trs.types
{
    using TraceParameter = com.riscure.trs.parameter.TraceParameter;
    using LongArrayParameter = com.riscure.trs.parameter.primitive.LongArrayParameter;

    public class LongTypeKey : TypedKey<long>
    {
        public LongTypeKey(string key) : base(typeof(long), key)
        {
        }

        public override TraceParameter CreateParameter(long value)
        {
            CheckLength(value);
            return new LongArrayParameter(value);
        }
        protected internal override void CheckLength(long value)
        {
        }
    }

}