using LittleEndianInputStream = Trsfile.IO.LittleEndianInputStream;
using LittleEndianOutputStream = Trsfile.IO.LittleEndianOutputStream;

namespace Trsfile.Parameter.Primitive
{
    public class ShortArrayParameter : TraceParameter<short>
    {

        public ShortArrayParameter(short value) : base(value)
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
                dos.WriteShort(i);
            }
        }

        public static ShortArrayParameter Deserialize(LittleEndianInputStream dis, int length)
        {
            ShortArrayParameter result = new(new short[length]);
            for (int k = 0; k < length; k++)
            {
                result.Value[k] = dis.ReadShort();
            }
            return result;
        }
        public override ShortArrayParameter Clone()
        {
            return new ShortArrayParameter(this);
        }


        public override string ToString()
        {
            return string.Join(", ", Value);
        }


    }

}