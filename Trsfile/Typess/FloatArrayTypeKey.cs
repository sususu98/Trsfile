using TraceParameter = Trsfile.Parameter.TraceParameter;
using FloatArrayParameter = Trsfile.Parameter.Primitive.FloatArrayParameter;

namespace Trsfile.Types
{
    public class FloatArrayTypeKey : TypedKey<float>
    {
        public FloatArrayTypeKey(string key) : base(typeof(float[]), key, true)
        {
        }

        public override TraceParameter CreateParameter(float[] value)
        {
            if (value.Length <= 0) throw new ArgumentException("Value's length below 0");
            if (value.Length == 1) IsArray = false;
            return new FloatArrayParameter(value);
        }
    }

}