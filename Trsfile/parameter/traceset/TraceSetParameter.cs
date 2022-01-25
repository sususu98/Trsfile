namespace com.riscure.trs.parameter.traceset
{
    using ParameterType = enums.ParameterType;
    using LittleEndianInputStream = io.LittleEndianInputStream;
    using LittleEndianOutputStream = io.LittleEndianOutputStream;
    using TraceParameter = TraceParameter;


    public class TraceSetParameter: ICloneable
    {
        private const string LENGTH_ERROR = "length of parameter {0:d} exceeds maximum length {1:d}";
        private const int MAX_LENGTH = 0x0FFFF;


        public override string? ToString()
        {
            return Value.ToString();
        }

        public TraceSetParameter(TraceParameter instance)
        {
            this.Value = instance;
        }

        public TraceSetParameter(TraceSetParameter toCopy) : this(toCopy.Value)
        {
        }

        public virtual void Serialize(LittleEndianOutputStream dos)
        {
            if (Value.Length != (Value.Length & MAX_LENGTH))
            {
                throw new IOException(string.Format(LENGTH_ERROR, Value.Length, MAX_LENGTH));
            }
            dos.WriteByte(Value.Type.Value);
            dos.WriteShort(Value.Length);
            Value.Serialize(dos);
        }

        public static TraceSetParameter Deserialize(LittleEndianInputStream dis)
        {
            ParameterType type = ParameterType.FromValue(dis.ReadByte());
            int length = dis.ReadShort() & MAX_LENGTH;
            return new TraceSetParameter(TraceParameter.Deserialize(type, length, dis));
        }

        public virtual TraceParameter Value { get; }

        /// <returns> a new instance of a TraceSetParameter containing a copy of its trace parameter </returns>
        public virtual object Clone()
        {
            return new TraceSetParameter(this);
        }


        public override int GetHashCode()
        {
            return Value != null ? Value.GetHashCode() : 0;
        }
    }

}