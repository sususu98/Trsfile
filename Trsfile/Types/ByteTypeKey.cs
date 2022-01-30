using Trsfile.Parameter;
using Trsfile.Parameter.Primitive;

namespace Trsfile.Types
{
    public class ByteTypeKey : TypedKey<byte>
    {
        public ByteTypeKey(string key) : base(typeof(byte), key, false)
        {
        }

        public override TraceParameter CreateParameter(byte value)
        {
            return new ByteArrayParameter(value);
        }

        public override TraceParameter CreateParameter(byte[] value)
        {
            throw new NotSupportedException("This is single type key, this method is not supported.");
        }
    }

}