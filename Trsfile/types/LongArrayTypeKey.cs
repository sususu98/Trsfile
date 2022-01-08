namespace com.riscure.trs.types
{
	using TraceParameter = com.riscure.trs.parameter.TraceParameter;
	using LongArrayParameter = com.riscure.trs.parameter.primitive.LongArrayParameter;

	public class LongArrayTypeKey : TypedKey<long[]>
	{
		public LongArrayTypeKey(string key) : base(typeof(long[]), key)
		{
		}

		public override TraceParameter createParameter(long[] value)
		{
			checkLength(value);
			return new LongArrayParameter(value);
		}
	}

}