namespace com.riscure.trs.types
{
	using TraceParameter = com.riscure.trs.parameter.TraceParameter;
	using BooleanArrayParameter = com.riscure.trs.parameter.primitive.BoolArrayParameter;

	public class BooleanTypeKey : TypedKey<bool>
	{
		public BooleanTypeKey(string key) : base(typeof(Boolean), key)
		{
		}

		public override TraceParameter createParameter(bool? value)
		{
			CheckLength(value.Value);
			return new BooleanArrayParameter(new bool[]{value.Value});
		}
	}

}