namespace com.riscure.trs.types
{
    using TraceParameter = parameter.TraceParameter;
    using ShortArrayParameter = parameter.primitive.ShortArrayParameter;

    public class ShortArrayTypeKey : TypedKey<short>
    {
        public ShortArrayTypeKey(string key) : base(typeof(short[]), key, true)
        {
        }

        public override TraceParameter CreateParameter(short[] value)
        {
            if (value.Length <= 0) throw new ArgumentException("Value's length below 0");
            if (value.Length == 1) IsArray = false;
            return new ShortArrayParameter(value);
        }
    }

}