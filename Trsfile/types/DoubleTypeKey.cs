namespace com.riscure.trs.types
{
	using TraceParameter = com.riscure.trs.parameter.TraceParameter;
	using DoubleArrayParameter = com.riscure.trs.parameter.primitive.DoubleArrayParameter;

	public class DoubleTypeKey : TypedKey<double>
	{
		public DoubleTypeKey(string key) : base(typeof(Double), key)
		{
		}

		public override TraceParameter createParameter(double? value)
		{
			checkLength(value.Value);
			return new DoubleArrayParameter(new double[]{value.Value});
		}
	}

}