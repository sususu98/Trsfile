using LittleEndianInputStream = com.riscure.trs.io.LittleEndianInputStream;
using LittleEndianOutputStream = com.riscure.trs.io.LittleEndianOutputStream;

namespace com.riscure.trs.parameter.primitive
{
    public class DoubleArrayParameter : TraceParameter<double>
    {

        public DoubleArrayParameter(double value) : base(value)
        {

        }

        public DoubleArrayParameter(double[] value) : base(value)
        {

        }

        public DoubleArrayParameter(DoubleArrayParameter toCopy) : this((double[])toCopy.Value.Clone())
        {
        }


        public override void Serialize(LittleEndianOutputStream dos)
        {
            foreach (double i in Value)
            {
                dos.WriteDouble(i);
            }
        }


        public static DoubleArrayParameter Deserialize(LittleEndianInputStream dis, int length)
        {
            DoubleArrayParameter result = new(new double[length]);
            for (int k = 0; k < length; k++)
            {
                result.Value[k] = dis.ReadDouble();
            }
            return result;
        }

        public override DoubleArrayParameter Clone()
        {
            return new DoubleArrayParameter(this);
        }

        public override string ToString()
        {
            return string.Join(", ", Value);
        }
    }

}