namespace com.riscure.trs.types
{
    using TraceParameter = parameter.TraceParameter;
    using DoubleArrayParameter = parameter.primitive.DoubleArrayParameter;

    public class DoubleArrayTypeKey : TypedKey<double>
    {
        public DoubleArrayTypeKey(string key) : base(typeof(double[]), key, true)
        {
        }

        public override TraceParameter CreateParameter(double[] value)
        {
            if (value.Length <= 0) throw new ArgumentException("Value's length below 0");
            if (value.Length == 1) IsArray = false;
            return new DoubleArrayParameter(value);
        }
    }

}