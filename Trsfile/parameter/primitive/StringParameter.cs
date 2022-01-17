using System.Text;

namespace com.riscure.trs.parameter.primitive
{
    using LittleEndianInputStream = io.LittleEndianInputStream;
    using LittleEndianOutputStream = io.LittleEndianOutputStream;


    public class StringParameter : TraceParameter<string>
    {
        private const string INVALID_LENGTH = "Error parsing string: Expected (%d) bytes but found (%d)";

        public StringParameter(string value) : base(value)
        {
        }

        public StringParameter(StringParameter toCopy) : this(toCopy.ScalarValue)
        {
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
        //ORIGINAL LINE: public void serialize(com.riscure.trs.io.LittleEndianOutputStream dos) throws java.io.IOException
        public override void Serialize(LittleEndianOutputStream dos)
        {
            dos.write(ScalarValue.GetBytes(Encoding.UTF8));
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
        //ORIGINAL LINE: public static StringParameter deserialize(com.riscure.trs.io.LittleEndianInputStream dis, int length) throws java.io.IOException
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