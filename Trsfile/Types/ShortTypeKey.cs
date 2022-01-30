using Trsfile.Parameter;
using Trsfile.Parameter.Primitive;

namespace Trsfile.Types
{
    public class ShortTypeKey : TypedKey<short>
    {
        public ShortTypeKey(string key) : base(typeof(short), key, false) { }
        public override TraceParameter CreateParameter(short value)
        {
            return new ShortArrayParameter(value);
        }
        public override TraceParameter CreateParameter(short[] value)
        {
            throw new NotSupportedException("This is single type key, this method is not supported.");
        }
    }

}