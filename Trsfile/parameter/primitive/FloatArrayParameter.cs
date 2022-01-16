namespace com.riscure.trs.parameter.primitive
{
    using LittleEndianInputStream = io.LittleEndianInputStream;
    using LittleEndianOutputStream = io.LittleEndianOutputStream;


    public class FloatArrayParameter : TraceParameter<float>
    {


        public FloatArrayParameter(int length) : this(new float[length])
        {

        }

        public FloatArrayParameter(float[] value) : base(value)
        {

        }

        public FloatArrayParameter(FloatArrayParameter toCopy) : this((float[])toCopy.Value.Clone())
        {
        }


        public override void Serialize(LittleEndianOutputStream dos)
        {
            foreach (float i in Value)
            {
                dos.writeFloat(i);
            }
        }

        public static FloatArrayParameter Deserialize(LittleEndianInputStream dis, int length)
        {
            FloatArrayParameter result = new(length);
            for (int k = 0; k < length; k++)
            {
                result.Value[k] = dis.readFloat();
            }
            return result;
        }



        public override FloatArrayParameter Clone()
        {
            return new FloatArrayParameter(this);
        }


        public override string ToString()
        {
            return Value.ToString();
        }
    }

}