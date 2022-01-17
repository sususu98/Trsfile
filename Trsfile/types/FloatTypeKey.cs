namespace com.riscure.trs.types
{
    using TraceParameter = parameter.TraceParameter;
    using FloatArrayParameter = parameter.primitive.FloatArrayParameter;

    public class FloatTypeKey : TypedKey<float>
    {
        public FloatTypeKey(string key) : base(typeof(float), key)
        {
        }

        public override TraceParameter CreateParameter(float value)
        {
            CheckLength(value);
            return new FloatArrayParameter(new float[] { value });
        }
        protected internal override void CheckLength(float value)
        {
        }
    }

}