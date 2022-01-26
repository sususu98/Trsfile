namespace com.riscure.trs.types
{
    using TraceParameter = parameter.TraceParameter;
    using IntegerArrayParameter = parameter.primitive.IntegerArrayParameter;

    public class IntegerTypeKey : TypedKey<int>
    {
        public IntegerTypeKey(string key) : base(typeof(int), key, false)
        {
        }

        public override TraceParameter CreateParameter(int value)
        {
            return new IntegerArrayParameter(value);
        }

        public override TraceParameter CreateParameter(int[] value)
        {
            throw new NotSupportedException("This is single type key, this method is not supported.");
        }
    }

}