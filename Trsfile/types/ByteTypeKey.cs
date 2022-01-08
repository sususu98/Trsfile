namespace com.riscure.trs.types
{
	using TraceParameter = com.riscure.trs.parameter.TraceParameter;
	using ByteArrayParameter = com.riscure.trs.parameter.primitive.ByteArrayParameter;

	public class ByteTypeKey : TypedKey<sbyte>
	{
		public ByteTypeKey(string key) : base(typeof(Byte), key)
		{
		}

		public override TraceParameter createParameter(sbyte? value)
		{
			checkLength(value.Value);
			return new ByteArrayParameter(new sbyte[]{value});
		}
	}

}