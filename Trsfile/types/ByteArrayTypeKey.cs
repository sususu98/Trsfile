namespace com.riscure.trs.types
{
	using TraceParameter = com.riscure.trs.parameter.TraceParameter;
	using ByteArrayParameter = com.riscure.trs.parameter.primitive.ByteArrayParameter;

	public class ByteArrayTypeKey : TypedKey<sbyte[]>
	{
		public ByteArrayTypeKey(string key) : base(typeof(sbyte[]), key)
		{
		}

		public override TraceParameter createParameter(sbyte[] value)
		{
			checkLength(value);
			return new ByteArrayParameter(value);
		}
	}

}