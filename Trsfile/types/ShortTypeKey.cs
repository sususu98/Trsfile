namespace com.riscure.trs.types
{
    using TraceParameter = com.riscure.trs.parameter.TraceParameter;
    using ShortArrayParameter = com.riscure.trs.parameter.primitive.ShortArrayParameter;

    public class ShortTypeKey : TypedKey<short>
    {
        public ShortTypeKey(string key) : base(typeof(short), key)
        {
        }

        public override TraceParameter CreateParameter(short value)
        {
            CheckLength(value);
            return new ShortArrayParameter(new short[] { value });
        }
        protected internal override void CheckLength(short value)
        {
        }
    }

}