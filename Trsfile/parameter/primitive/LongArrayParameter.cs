using System.Linq;

namespace com.riscure.trs.parameter.primitive
{
    using ParameterType = enums.ParameterType;
    using LittleEndianInputStream = io.LittleEndianInputStream;
    using LittleEndianOutputStream = io.LittleEndianOutputStream;
    using TraceParameter = TraceParameter;


    public class LongArrayParameter : TraceParameter<long>
    {

        public LongArrayParameter(int length) : this(new long[length])
        {

        }

        public LongArrayParameter(long[] value) : base(value)
        {

        }

        public LongArrayParameter(LongArrayParameter toCopy) : this((long[])toCopy.Value.Clone())
        {
        }

        public override void Serialize(LittleEndianOutputStream dos)
        {
            foreach (long i in Value)
            {
                dos.writeLong(i);
            }
        }

        public static LongArrayParameter Deserialize(LittleEndianInputStream dis, int length)
        {
            LongArrayParameter result = new LongArrayParameter(length);
            for (int k = 0; k < length; k++)
            {
                result.Value[k] = dis.readLong();
            }
            return result;
        }



        public override LongArrayParameter Clone()
        {
            return new LongArrayParameter(this);
        }

        public override string ToString()
        {
            return Value.ToString();
        }

    }

}