namespace com.riscure.trs.types
{
	using TraceParameter = com.riscure.trs.parameter.TraceParameter;
	using BooleanArrayParameter = com.riscure.trs.parameter.primitive.BooleanArrayParameter;

	public class BooleanArrayTypeKey : TypedKey<bool[]>
	{
		public BooleanArrayTypeKey(string key) : base(typeof(bool[]), key)
		{
		}

        public override bool[] Cast(bool value)
        {
            throw new NotImplementedException();
        }

        public override TraceParameter CreateParameter(bool[] value)
		{
			CheckLength(value);
			return new BooleanArrayParameter(value);
		}

        protected internal override void CheckLength(bool[] value)
        {
            throw new NotImplementedException();
        }
    }

}