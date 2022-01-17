namespace com.riscure.trs.types
{
    using TraceParameter = parameter.TraceParameter;
    using IntegerArrayParameter = parameter.primitive.IntegerArrayParameter;

    public class IntegerArrayTypeKey : TypedKey<int[]>
    {
        public IntegerArrayTypeKey(string key) : base(typeof(int[]), key)
        {
        }

        public override TraceParameter CreateParameter(int[] value)
        {
            CheckLength(value);
            return new IntegerArrayParameter(value);
        }
        protected internal override void CheckLength(int[] value)
        {
            if (value.Length <= 0) throw new ArgumentException("Value's length below 0");
        }
    }

}