using TraceParameter = Trsfile.Parameter.TraceParameter;
using LongArrayParameter = Trsfile.Parameter.Primitive.LongArrayParameter;

namespace Trsfile.Types
{
    public class LongTypeKey : TypedKey<long>
    {
        public LongTypeKey(string key) : base(typeof(long), key, false) { }
        public override TraceParameter CreateParameter(long value)
        {
            return new LongArrayParameter(value);
        }
        public override TraceParameter CreateParameter(long[] value)
        {
            throw new NotSupportedException("This is single type key, this method is not supported.");
        }
    }

}