namespace com.riscure.trs.types
{
    using TraceParameter = parameter.TraceParameter;
    using DoubleArrayParameter = parameter.primitive.DoubleArrayParameter;

    public class DoubleTypeKey : TypedKey<double>
    {
        public DoubleTypeKey(string key) : base(typeof(double), key)
        {
        }

        public override TraceParameter CreateParameter(double value)
        {
            CheckLength(value);
            return new DoubleArrayParameter(new double[] { value });
        }
        protected internal override void CheckLength(double value)
        {
        }
    }

}