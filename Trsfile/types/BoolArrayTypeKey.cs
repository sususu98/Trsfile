using com.riscure.trs.parameter;
using com.riscure.trs.parameter.primitive;

namespace com.riscure.trs.types
{

	public class BoolArrayTypeKey : TypedKey<bool[]>
	{
		public BoolArrayTypeKey(string key) : base(typeof(bool[]), key)
		{
		}

        public override TraceParameter CreateParameter(bool[] value)
		{
			CheckLength(value);
			return new BoolArrayParameter(value);
		}

        protected internal override void CheckLength(bool[] value)
        {
			if (value.Length <= 0) throw new ArgumentException("Value's length below 0");
        }
    }

}