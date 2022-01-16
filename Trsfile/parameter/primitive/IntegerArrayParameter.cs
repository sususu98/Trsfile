namespace com.riscure.trs.parameter.primitive
{
    using LittleEndianInputStream = io.LittleEndianInputStream;
    using LittleEndianOutputStream = io.LittleEndianOutputStream;


    public class IntegerArrayParameter : TraceParameter<int>
    {
        public IntegerArrayParameter(int value) : base(value)
        {

        }

        public IntegerArrayParameter(int[] value) : base(value)
        {

        }

        public IntegerArrayParameter(IntegerArrayParameter toCopy) : this((int[])toCopy.Value.Clone())
        {
        }

        public override void Serialize(LittleEndianOutputStream dos)
        {
            foreach (int i in Value)
            {
                dos.writeInt(i);
            }
        }
        public static IntegerArrayParameter Deserialize(LittleEndianInputStream dis, int length)
        {
            IntegerArrayParameter result = new(new int[length]);
            for (int k = 0; k < length; k++)
            {
                result.Value[k] = dis.readInt();
            }
            return result;
        }
        public override IntegerArrayParameter Clone()
        {
            return new IntegerArrayParameter(this);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

}