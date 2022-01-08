using System.Linq;

namespace com.riscure.trs.parameter.primitive
{
	using ParameterType = com.riscure.trs.enums.ParameterType;
	using LittleEndianInputStream = com.riscure.trs.io.LittleEndianInputStream;
	using LittleEndianOutputStream = com.riscure.trs.io.LittleEndianOutputStream;
	using TraceParameter = com.riscure.trs.parameter.TraceParameter;


	public class LongArrayParameter : TraceParameter
	{
		private readonly long[] value;

		public LongArrayParameter(int length)
		{
			value = new long[length];
		}

		public LongArrayParameter(long[] value)
		{
			this.value = value;
		}

		public LongArrayParameter(LongArrayParameter toCopy) : this((long[])toCopy.Value.Clone())
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void serialize(com.riscure.trs.io.LittleEndianOutputStream dos) throws java.io.IOException
		public override void serialize(LittleEndianOutputStream dos)
		{
			foreach (long i in value)
			{
				dos.writeLong(i);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static LongArrayParameter deserialize(com.riscure.trs.io.LittleEndianInputStream dis, int length) throws java.io.IOException
		public static LongArrayParameter deserialize(LittleEndianInputStream dis, int length)
		{
			LongArrayParameter result = new LongArrayParameter(length);
			for (int k = 0; k < length; k++)
			{
				result.value[k] = dis.readLong();
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
				return ParameterType.LONG;
			}
		}

		public override long[] Value
		{
			get
			{
				return value;
			}
		}

		public override LongArrayParameter copy()
		{
			return new LongArrayParameter(this);
		}

		public override long? ScalarValue
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
			return Arrays.toString(value);
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

			LongArrayParameter that = (LongArrayParameter) o;

			return value.SequenceEqual(that.value);
		}

		public override int GetHashCode()
		{
			return Arrays.hashCode(value);
		}
	}

}