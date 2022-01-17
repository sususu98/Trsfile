namespace com.riscure.trs.types
{
	using TraceParameter = parameter.TraceParameter;
	using BoolArrayParameter = parameter.primitive.BoolArrayParameter;
	public class BoolTypeKey : TypedKey<bool>
	{
		public BoolTypeKey(string key) : base(typeof(bool), key)
		{
		}

		public override TraceParameter CreateParameter(bool value)
		{
			CheckLength(value);
			return new BoolArrayParameter(value);
		}

		protected internal override void CheckLength(bool value)
		{
			
		}
	}

}