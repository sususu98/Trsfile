namespace com.riscure.trs.types
{
	using TraceParameter = com.riscure.trs.parameter.TraceParameter;
	using IntegerArrayParameter = com.riscure.trs.parameter.primitive.IntegerArrayParameter;

	public class IntegerTypeKey : TypedKey<int>
	{
		public IntegerTypeKey(string key) : base(typeof(Integer), key)
		{
		}

		public override TraceParameter createParameter(int? value)
		{
			checkLength(value.Value);
			return new IntegerArrayParameter(new int[]{value.Value});
		}
	}

}