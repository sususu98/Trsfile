namespace com.riscure.trs.types
{
    using TraceParameter = parameter.TraceParameter;
    using ShortArrayParameter = parameter.primitive.ShortArrayParameter;

    public class ShortArrayTypeKey : TypedKey<short[]>
    {
        public ShortArrayTypeKey(string key) : base(typeof(short[]), key)
        {
        }

        public override TraceParameter CreateParameter(short[] value)
        {
            CheckLength(value);
            return new ShortArrayParameter(value);
        }
        protected internal override void CheckLength(short[] value)
        {
            if (value.Length <= 0) throw new ArgumentException("Value's length below 0");
        }
    }

}