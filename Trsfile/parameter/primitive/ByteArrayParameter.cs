using System.Linq;

namespace com.riscure.trs.parameter.primitive
{
	using HexUtils = com.riscure.trs.HexUtils;
	using ParameterType = com.riscure.trs.enums.ParameterType;
	using LittleEndianInputStream = com.riscure.trs.io.LittleEndianInputStream;
	using LittleEndianOutputStream = com.riscure.trs.io.LittleEndianOutputStream;
	using TraceParameter = com.riscure.trs.parameter.TraceParameter;


	public class ByteArrayParameter : TraceParameter
	{
		private const string INVALID_LENGTH = "Error parsing byte array: Expected (%d) bytes but found (%d)";
		private readonly sbyte[] value;

		public ByteArrayParameter(int length)
		{
			this.value = new sbyte[length];
		}

		public ByteArrayParameter(sbyte[] value)
		{
			this.value = value;
		}

		public ByteArrayParameter(ByteArrayParameter toCopy) : this((sbyte[])toCopy.Value.Clone())
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void serialize(com.riscure.trs.io.LittleEndianOutputStream dos) throws java.io.IOException
		public override void serialize(LittleEndianOutputStream dos)
		{
			dos.write(value);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static ByteArrayParameter deserialize(com.riscure.trs.io.LittleEndianInputStream dis, int length) throws java.io.IOException
		public static ByteArrayParameter deserialize(LittleEndianInputStream dis, int length)
		{
			ByteArrayParameter result = new ByteArrayParameter(length);
			int bytesRead = dis.read(result.value);
			if (bytesRead != length)
			{
				throw new IOException(String.format(INVALID_LENGTH, length, bytesRead));
			}
			return result;
		}

		public override int length()
		{
			return value.Length;
		}

		public override ParameterType Type
		{
			get
			{
				return ParameterType.BYTE;
			}
		}

		public override sbyte[] Value
		{
			get
			{
				return value;
			}
		}

		public override ByteArrayParameter copy()
		{
			return new ByteArrayParameter(this);
		}

		public override sbyte? ScalarValue
		{
			get
			{
				if (length() > 1)
				{
					throw new System.ArgumentException("Parameter represents an array value of length " + length());
				}
				return Value[0];
			}
		}

		public override string ToString()
		{
			return HexUtils.toHexString(value);
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

			ByteArrayParameter that = (ByteArrayParameter) o;

			return value.SequenceEqual(that.value);
		}

		public override int GetHashCode()
		{
			return Arrays.hashCode(value);
		}
	}

}