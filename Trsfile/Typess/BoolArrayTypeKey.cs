using TraceParameter = Trsfile.Parameter.TraceParameter;
using BoolArrayParameter = Trsfile.Parameter.Primitive.BoolArrayParameter;

namespace Trsfile.Types
{
	public class BoolArrayTypeKey : TypedKey<bool>
	{
		public BoolArrayTypeKey(string key) : base(typeof(bool[]), key, true)
		{
		}

        public override TraceParameter CreateParameter(bool[] value)
		{
			if (value.Length <= 0) throw new ArgumentException("Value's length below 0");
			if (value.Length == 1) IsArray = false;
			return new BoolArrayParameter(value);
		}
    }

}