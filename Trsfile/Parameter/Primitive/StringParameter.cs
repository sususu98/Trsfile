using System.Text;
using Trsfile.IO;

namespace Trsfile.Parameter.Primitive
{
    public class StringParameter : TraceParameter<string>
    {
        private const string INVALID_LENGTH = "Error parsing string: Expected ({0:d}) bytes but found ({1:d})";

        public StringParameter(string value) : base(value)
        {
        }

        public StringParameter(StringParameter toCopy) : this(toCopy.ScalarValue)
        {
        }

        public override int Length => ScalarValue.Length;
        public override void Serialize(LittleEndianOutputStream dos)
        {
            dos.Write(ScalarValue.GetBytes(Encoding.UTF8));
        }

        public static StringParameter Deserialize(LittleEndianInputStream dis, int length)
        {
            byte[] bytes = new byte[length];
            if (length > 0)
            {
                int bytesRead = dis.Read(bytes);
                if (bytesRead != length)
                {
                    throw new IOException(string.Format(INVALID_LENGTH, length, bytesRead));
                }
            }
            return new StringParameter(StringHelper.NewString(bytes, Encoding.UTF8));
        }

        public override StringParameter Clone()
        {
            return new StringParameter(this);
        }


        public override string ToString()
        {
            return ScalarValue;
        }

    }

}