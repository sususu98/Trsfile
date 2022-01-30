using Trsfile.Parameter;
using Trsfile.Parameter.Primitive;

namespace Trsfile.Types
{
    public class StringTypeKey : TypedKey<string>
    {
        public StringTypeKey(string key) : base(typeof(string), key, false) { }
        public override TraceParameter CreateParameter(string value)
        {
            return new StringParameter(value);
        }
        public override TraceParameter CreateParameter(string[] value)
        {
            throw new NotSupportedException("This is single type key, this method is not supported.");
        }
    }

}