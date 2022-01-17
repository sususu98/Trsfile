namespace com.riscure.trs.types
{
    using TraceParameter = parameter.TraceParameter;
    using FloatArrayParameter = parameter.primitive.FloatArrayParameter;

    public class FloatArrayTypeKey : TypedKey<float[]>
    {
        public FloatArrayTypeKey(string key) : base(typeof(float[]), key)
        {
        }

        public override TraceParameter CreateParameter(float[] value)
        {
            CheckLength(value);
            return new FloatArrayParameter(value);
        }
        protected internal override void CheckLength(float[] value)
        {
            if (value.Length <= 0) throw new ArgumentException("Value's length below 0");

        }
    }

}