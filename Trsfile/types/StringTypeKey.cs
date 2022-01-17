namespace com.riscure.trs.types
{
    using TraceParameter = parameter.TraceParameter;
    using StringParameter = parameter.primitive.StringParameter;

    public class StringTypeKey : TypedKey<string>
    {
        public StringTypeKey(string key) : base(typeof(string), key)
        {
        }

        public override TraceParameter CreateParameter(string value)
        {
            CheckLength(value);
            return new StringParameter(value);
        }
        protected internal override void CheckLength(string value)
        {
        }
    }

}