using System.Text;

namespace com.riscure.trs.parameter.primitive
{
	using ParameterType = com.riscure.trs.enums.ParameterType;
	using LittleEndianInputStream = com.riscure.trs.io.LittleEndianInputStream;
	using LittleEndianOutputStream = com.riscure.trs.io.LittleEndianOutputStream;
	using TraceParameter = com.riscure.trs.parameter.TraceParameter;


	public class StringParameter : TraceParameter
	{
		private const string INVALID_LENGTH = "Error parsing string: Expected (%d) bytes but found (%d)";
		private readonly string value;

		public StringParameter(string value)
		{
			this.value = value;
		}

		public StringParameter(StringParameter toCopy) : this(toCopy.Value)
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void serialize(com.riscure.trs.io.LittleEndianOutputStream dos) throws java.io.IOException
		public override void Serialize(LittleEndianOutputStream dos)
		{
			dos.write(value.GetBytes(Encoding.UTF8));
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static StringParameter deserialize(com.riscure.trs.io.LittleEndianInputStream dis, int length) throws java.io.IOException
		public static StringParameter deserialize(LittleEndianInputStream dis, int length)
		{
			sbyte[] bytes = new sbyte[length];
			if (length > 0)
			{
				int bytesRead = dis.read(bytes);
				if (bytesRead != length)
				{
					throw new IOException(String.format(INVALID_LENGTH, length, bytesRead));
				}
			}
			return new StringParameter(StringHelper.NewString(bytes, StandardCharsets.UTF_8));
		}

		public override int Length()
		{
			return value.GetBytes(Encoding.UTF8).length;
		}

		public override ParameterType Type
		{
			get
			{
				return ParameterType.STRING;
			}
		}

		public override string Value
		{
			get
			{
				return value;
			}
		}

		public override StringParameter copy()
		{
			return new StringParameter(this);
		}

		public override string ScalarValue
		{
			get
			{
				return Value;
			}
		}

		public override string ToString()
		{
			return Value;
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

			StringParameter that = (StringParameter) o;

			return Objects.equals(value, that.value);
		}

		public override int GetHashCode()
		{
			return !string.ReferenceEquals(value, null) ? value.GetHashCode() : 0;
		}
	}

}