namespace com.riscure.trs.parameter.trace.definition
{
	using ParameterType = com.riscure.trs.enums.ParameterType;
	using LittleEndianInputStream = com.riscure.trs.io.LittleEndianInputStream;
	using LittleEndianOutputStream = com.riscure.trs.io.LittleEndianOutputStream;
	using TraceParameter = com.riscure.trs.parameter.TraceParameter;


	public class TraceParameterDefinition: ICloneable
	{
		private readonly ParameterType type;
		private readonly short offset;
		private readonly short length;

		public TraceParameterDefinition(TraceParameter instance, short offset)
		{
			this.type = instance.Type;
			this.offset = offset;
			this.length = (short) instance.length();
		}

		public TraceParameterDefinition(ParameterType type, short length, short offset)
		{
			this.type = type;
			this.offset = offset;
			this.length = length;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void serialize(com.riscure.trs.io.LittleEndianOutputStream dos) throws java.io.IOException
		public virtual void serialize(LittleEndianOutputStream dos)
		{
			dos.writeByte(type.Value);
			dos.writeShort(length);
			dos.writeShort(offset);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static TraceParameterDefinition<com.riscure.trs.parameter.TraceParameter> deserialize(com.riscure.trs.io.LittleEndianInputStream dis) throws java.io.IOException
		public static TraceParameterDefinition deserialize(LittleEndianInputStream dis)
		{
			ParameterType type = ParameterType.fromValue(dis.readByte());
			short length = dis.readShort();
			short offset = dis.readShort();
			return new TraceParameterDefinition(type, length, offset);
		}

		public virtual ParameterType Type
		{
			get
			{
				return type;
			}
		}

		public virtual short Offset
		{
			get
			{
				return offset;
			}
		}

		public virtual short Length
		{
			get
			{
				return length;
			}
		}

		public override bool Equals(object o)
		{
			if (this == o)
			{
				return true;
			}
			if (o == null || this.GetType() != o.GetType())
			{
				return false;
			}

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: TraceParameterDefinition<?> that = (TraceParameterDefinition<?>) o;
			TraceParameterDefinition that = (TraceParameterDefinition) o;

			if (offset != that.offset)
			{
				return false;
			}
			if (length != that.length)
			{
				return false;
			}
			return type == that.type;
		}

		public override int GetHashCode()
		{
			int result = type != null ? type.GetHashCode() : 0;
			result = 31 * result + (int) offset;
			result = 31 * result + (int) length;
			return result;
		}

		public override string ToString()
		{
			return string.Format("TraceParameterDefinition{{Type={0}, Offset={1:D}, Length={2:D}}}", Type, Offset, Length);
		}

        public object Clone()
        {
			return new TraceParameterDefinition(Type, Length, Offset);
        }
    }

}