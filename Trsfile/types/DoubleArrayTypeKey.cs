namespace com.riscure.trs.types
{
    using TraceParameter = parameter.TraceParameter;
    using DoubleArrayParameter = parameter.primitive.DoubleArrayParameter;

    public class DoubleArrayTypeKey : TypedKey<double[]>
    {
        public DoubleArrayTypeKey(string key) : base(typeof(double[]), key)
        {
        }

        public override TraceParameter CreateParameter(double[] value)
        {
            CheckLength(value);
            return new DoubleArrayParameter(value);
        }
        protected internal override void CheckLength(double[] value)
        {
            if (value.Length <= 0) throw new ArgumentException("Value's length below 0");
        }
    }

}