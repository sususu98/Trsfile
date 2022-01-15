namespace com.riscure.trs.types
{
	using TraceParameter = com.riscure.trs.parameter.TraceParameter;
	using ShortArrayParameter = com.riscure.trs.parameter.primitive.ShortArrayParameter;

	public class ShortArrayTypeKey : TypedKey<short[]>
	{
		public ShortArrayTypeKey(string key) : base(typeof(short[]), key)
		{
		}

		public override TraceParameter CreateParameter(short[] value)
		{
			CheckLength(value);
			return new ShortArrayParameter(value);
		}
	}

}