using Trsfile.Enums;
using Trsfile.IO;

namespace Trsfile.Parameter.Traceset
{
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
            Value = instance;
        }

        public TraceSetParameter(TraceSetParameter toCopy) : this(toCopy.Value)
        {
        }

        public void Serialize(LittleEndianOutputStream dos)
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

        public TraceParameter Value { get; }

        /// <returns> a new instance of a TraceSetParameter containing a copy of its trace parameter </returns>
        public object Clone()
        {
            return new TraceSetParameter(this);
        }


        public override int GetHashCode()
        {
            return Value != null ? Value.GetHashCode() : 0;
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (base.Equals(obj)) return true;
            if (obj is not TraceSetParameter t)
                return false;
            return t.Value.Equals(Value);
        }

        public static bool operator ==(TraceSetParameter a, TraceSetParameter b)
            => a.Value.Equals(b.Value);
        public static bool operator !=(TraceSetParameter a, TraceSetParameter b)
            => !(a == b);

    }

}