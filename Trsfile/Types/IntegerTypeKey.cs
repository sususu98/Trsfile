using Trsfile.Parameter;
using Trsfile.Parameter.Primitive;

namespace Trsfile.Types
{
    public class IntegerTypeKey : TypedKey<int>
    {
        public IntegerTypeKey(string key) : base(typeof(int), key, false)
        {
        }

        public override TraceParameter CreateParameter(int value)
        {
            return new IntegerArrayParameter(value);
        }

        public override TraceParameter CreateParameter(int[] value)
        {
            throw new NotSupportedException("This is single type key, this method is not supported.");
        }
    }

}