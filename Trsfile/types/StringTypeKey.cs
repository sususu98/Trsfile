namespace com.riscure.trs.types
{
	using TraceParameter = com.riscure.trs.parameter.TraceParameter;
	using StringParameter = com.riscure.trs.parameter.primitive.StringParameter;

	public class StringTypeKey : TypedKey<string>
	{
		public StringTypeKey(string key) : base(typeof(string), key)
		{
		}

		public override TraceParameter createParameter(string value)
		{
			checkLength(value);
			return new StringParameter(value);
		}
	}

}