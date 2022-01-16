using com.riscure.trs.io;

namespace com.riscure.trs.parameter.primitive
{

    public class BoolArrayParameter : TraceParameter<bool>
    {
        public BoolArrayParameter(int length) : this(new bool[length])
        {

        }

        public BoolArrayParameter(bool[] value) : base(value)
        {

        }

        public BoolArrayParameter(BoolArrayParameter toCopy) : this((bool[])toCopy.Value.Clone())
        {
        }


        public override void Serialize(LittleEndianOutputStream dos)
        {
            foreach (bool i in Value)
            {
                dos.writeByte(i ? 1 : 0);
            }
        }

        public static BoolArrayParameter Deserialize(LittleEndianInputStream dis, int length)
        {
            BoolArrayParameter result = new(length);
            for (int k = 0; k < length; k++)
            {
                result.Value[k] = dis.readByte() != 0;
            }
            return result;
        }

        public override BoolArrayParameter Clone()
        {
            return new BoolArrayParameter(this);
        }

        public override string ToString()
        {
            return string.Join(", ", Value);
        }        
    }

}