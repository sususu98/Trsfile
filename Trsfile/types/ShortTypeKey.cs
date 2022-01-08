namespace com.riscure.trs.types
{
	using TraceParameter = com.riscure.trs.parameter.TraceParameter;
	using ShortArrayParameter = com.riscure.trs.parameter.primitive.ShortArrayParameter;

	public class ShortTypeKey : TypedKey<short>
	{
		public ShortTypeKey(string key) : base(typeof(Short), key)
		{
		}

		public override TraceParameter createParameter(short? value)
		{
			checkLength(value.Value);
			return new ShortArrayParameter(new short[]{value.Value});
		}
	}

}