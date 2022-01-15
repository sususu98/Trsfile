namespace com.riscure.trs.types
{
	using TraceParameter = com.riscure.trs.parameter.TraceParameter;
	using FloatArrayParameter = com.riscure.trs.parameter.primitive.FloatArrayParameter;

	public class FloatTypeKey : TypedKey<float>
	{
		public FloatTypeKey(string key) : base(typeof(Float), key)
		{
		}

		public override TraceParameter createParameter(float? value)
		{
			CheckLength(value.Value);
			return new FloatArrayParameter(new float[]{value.Value});
		}
	}

}