namespace com.riscure.trs.parameter.primitive
{
    using LittleEndianInputStream = io.LittleEndianInputStream;
    using LittleEndianOutputStream = io.LittleEndianOutputStream;


    public class ShortArrayParameter : TraceParameter<short>
    {

        public ShortArrayParameter(int length) : this(new short[length])
        {

        }

        public ShortArrayParameter(short[] value) : base(value)
        {
        }

        public ShortArrayParameter(ShortArrayParameter toCopy) : this((short[])toCopy.Value.Clone())
        {
        }
        public override void Serialize(LittleEndianOutputStream dos)
        {
            foreach (short i in Value)
            {
                dos.writeShort(i);
            }
        }

        public static ShortArrayParameter Deserialize(LittleEndianInputStream dis, int length)
        {
            ShortArrayParameter result = new ShortArrayParameter(length);
            for (int k = 0; k < length; k++)
            {
                result.Value[k] = dis.readShort();
            }
            return result;
        }
        public override ShortArrayParameter Clone()
        {
            return new ShortArrayParameter(this);
        }


        public override string ToString()
        {
            return Value.ToString();
        }


    }

}