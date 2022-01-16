using com.riscure.trs.enums;
using com.riscure.trs.io;
using com.riscure.trs.parameter;

namespace com.riscure.trs.parameter.trace.definition
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
		}

		public TraceParameterDefinition(ParameterType type, short length, short offset)
		{
			Type = type;
			Length = length;
			Offset = offset;
		}

		public virtual void Serialize(LittleEndianOutputStream dos)
		{
			dos.writeByte(Type.Value);
			dos.writeShort(Length);
			dos.writeShort(Offset);
		}

		public static TraceParameterDefinition Deserialize(LittleEndianInputStream dis)
		{
			ParameterType type = ParameterType.FromValue(dis.readByte());
			short length = dis.readShort();
			short offset = dis.readShort();
			return new TraceParameterDefinition(type, length, offset);
		}

		public override string ToString()
		{
			return string.Format("TraceParameterDefinition{{Type={0}, Offset={1:D}, Length={2:D}}}", Type, Offset, Length);
		}

    }

}