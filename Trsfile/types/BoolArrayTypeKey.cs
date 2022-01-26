namespace com.riscure.trs.types
{
	using TraceParameter = parameter.TraceParameter;
	using BoolArrayParameter = parameter.primitive.BoolArrayParameter;
	public class BoolArrayTypeKey : TypedKey<bool>
	{
		public BoolArrayTypeKey(string key) : base(typeof(bool[]), key, true)
		{
		}

        public override TraceParameter CreateParameter(bool[] value)
		{
			if (value.Length <= 0) throw new ArgumentException("Value's length below 0");
			if (value.Length == 1) IsArray = false;
			return new BoolArrayParameter(value);
		}
    }

}