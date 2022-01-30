using Trsfile.Parameter;
using Trsfile.Parameter.Primitive;

namespace Trsfile.Types
{
	public class BoolTypeKey : TypedKey<bool>
	{
		public BoolTypeKey(string key) : base(typeof(bool), key, false)
		{
		}

		public override TraceParameter CreateParameter(bool value)
		{
			return new BoolArrayParameter(value);
		}

        public override TraceParameter CreateParameter(bool[] value)
        {
			throw new NotSupportedException("This is single type key, this method is not supported.");
		}
    }

}