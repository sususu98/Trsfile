namespace com.riscure.trs.types
{
	using TraceParameter = com.riscure.trs.parameter.TraceParameter;
	using LongArrayParameter = com.riscure.trs.parameter.primitive.LongArrayParameter;

	public class LongTypeKey : TypedKey<long>
	{
		public LongTypeKey(string key) : base(typeof(Long), key)
		{
		}

		public override TraceParameter createParameter(long? value)
		{
			CheckLength(value.Value);
			return new LongArrayParameter(new long[]{value.Value});
		}
	}

}