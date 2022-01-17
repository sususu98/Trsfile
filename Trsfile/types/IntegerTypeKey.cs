namespace com.riscure.trs.types
{
    using TraceParameter = parameter.TraceParameter;
    using IntegerArrayParameter = parameter.primitive.IntegerArrayParameter;

    public class IntegerTypeKey : TypedKey<int>
    {
        public IntegerTypeKey(string key) : base(typeof(int), key)
        {
        }

        public override TraceParameter CreateParameter(int value)
        {
            CheckLength(value);
            return new IntegerArrayParameter(new int[] { value });
        }
        protected internal override void CheckLength(int value)
        {
        }
    }

}