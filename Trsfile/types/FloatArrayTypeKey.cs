namespace com.riscure.trs.types
{
	using TraceParameter = com.riscure.trs.parameter.TraceParameter;
	using FloatArrayParameter = com.riscure.trs.parameter.primitive.FloatArrayParameter;

	public class FloatArrayTypeKey : TypedKey<float[]>
	{
		public FloatArrayTypeKey(string key) : base(typeof(float[]), key)
		{
		}

		public override TraceParameter CreateParameter(float[] value)
		{
			CheckLength(value);
			return new FloatArrayParameter(value);
		}
	}

}