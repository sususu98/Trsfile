using Trsfile.Enums;
using Trsfile.IO;

namespace Trsfile.Parameter.Trace.Definition
{

    public record TraceParameterDefinition
    {
        public ParameterType Type { get; }
        public short Offset { get; }
        public short Length { get; }

        public TraceParameterDefinition(TraceParameter instance, short offset)
        {
            Type = instance.Type;
            Offset = offset;
            Length = (short)instance.Length;
        }

        public TraceParameterDefinition(ParameterType type, short length, short offset)
        {
            Type = type;
            Length = length;
            Offset = offset;
        }

        public void Serialize(LittleEndianOutputStream dos)
        {
            dos.WriteByte(Type.Value);
            dos.WriteShort(Length);
            dos.WriteShort(Offset);
        }

        public static TraceParameterDefinition Deserialize(LittleEndianInputStream dis)
        {
            ParameterType type = ParameterType.FromValue(dis.ReadByte());
            short length = dis.ReadShort();
            short offset = dis.ReadShort();
            return new TraceParameterDefinition(type, length, offset);
        }

        public override string ToString()
        {
            return string.Format("TraceParameterDefinition{{Type={0}, Offset={1:D}, Length={2:D}}}", Type, Offset, Length);
        }

    }

}