namespace com.riscure.trs.parameter.traceset
{
	using ParameterType = com.riscure.trs.enums.ParameterType;
	using LittleEndianInputStream = com.riscure.trs.io.LittleEndianInputStream;
	using LittleEndianOutputStream = com.riscure.trs.io.LittleEndianOutputStream;
	using TraceParameter = com.riscure.trs.parameter.TraceParameter;


	public class TraceSetParameter
	{
		private const string LENGTH_ERROR = "length of parameter (%d) exceeds maximum length (%d)";
		private const int MAX_LENGTH = 0x0FFFF;

		private readonly TraceParameter value;

		public override string ToString()
		{
			return this.value.ToString();
		}

		public TraceSetParameter(TraceParameter instance)
		{
			this.value = instance;
		}

		public TraceSetParameter(TraceSetParameter toCopy) : this(toCopy.Value)
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void serialize(com.riscure.trs.io.LittleEndianOutputStream dos) throws java.io.IOException
		public virtual void serialize(LittleEndianOutputStream dos)
		{
			if (value.Length() != (value.Length() & MAX_LENGTH))
			{
				throw new IOException(String.format(LENGTH_ERROR, value.Length(), MAX_LENGTH));
			}
			dos.writeByte(value.Type.getValue());
			dos.writeShort(value.Length());
			value.Serialize(dos);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static TraceSetParameter deserialize(com.riscure.trs.io.LittleEndianInputStream dis) throws java.io.IOException
		public static TraceSetParameter deserialize(LittleEndianInputStream dis)
		{
			ParameterType type = ParameterType.FromValue(dis.readByte());
			int length = dis.readShort() & MAX_LENGTH;
			return new TraceSetParameter(TraceParameter.Deserialize(type, length, dis));
		}

		public virtual TraceParameter Value
		{
			get
			{
				return value;
			}
		}

		/// <returns> a new instance of a TraceSetParameter containing a copy of its trace parameter </returns>
		public virtual TraceSetParameter copy()
		{
			return new TraceSetParameter(this);
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

			TraceSetParameter that = (TraceSetParameter) o;

			return Objects.equals(value, that.value);
		}

		public override int GetHashCode()
		{
			return value != null ? value.GetHashCode() : 0;
		}
	}

}