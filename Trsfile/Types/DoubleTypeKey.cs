using Trsfile.Parameter;
using Trsfile.Parameter.Primitive;

namespace Trsfile.Types
{
    public class DoubleTypeKey : TypedKey<double>
    {
        public DoubleTypeKey(string key) : base(typeof(double), key, false)
        {
        }

        public override TraceParameter CreateParameter(double value)
        {
            return new DoubleArrayParameter(value);
        }

        public override TraceParameter CreateParameter(double[] value)
        {
            throw new NotSupportedException("This is single type key, this method is not supported.");
        }
    }

}