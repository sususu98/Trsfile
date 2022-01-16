using com.riscure.trs.parameter;
using com.riscure.trs.parameter.primitive;

namespace com.riscure.trs.types
{
	public class BooleanTypeKey : TypedKey<bool>
	{
		public BooleanTypeKey(string key) : base(typeof(Boolean), key)
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