using Trsfile.Parameter;
using Trsfile.Parameter.Primitive;

namespace Trsfile.Types
{
    public class ByteArrayTypeKey : TypedKey<byte>
    {
        public ByteArrayTypeKey(string key) : base(typeof(byte[]), key, true)
        {
        }

        public override TraceParameter CreateParameter(byte[] value)
        {
            if (value.Length <= 0) throw new ArgumentException("Value's length below 0");
            if (value.Length == 1) IsArray = false;
            return new ByteArrayParameter(value);
        }
    }

}