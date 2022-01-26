namespace com.riscure.trs.types
{
    using TraceParameter = parameter.TraceParameter;
    using FloatArrayParameter = parameter.primitive.FloatArrayParameter;

    public class FloatTypeKey : TypedKey<float>
    {
        public FloatTypeKey(string key) : base(typeof(float), key, false) { }
        public override TraceParameter CreateParameter(float value)
        {
            return new FloatArrayParameter(value);
        }
        public override TraceParameter CreateParameter(float[] value)
        {
            throw new NotSupportedException("This is single type key, this method is not supported.");
        }
    }

}