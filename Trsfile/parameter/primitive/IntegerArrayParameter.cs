using System.Linq;

namespace com.riscure.trs.parameter.primitive
{
	using ParameterType = com.riscure.trs.enums.ParameterType;
	using LittleEndianInputStream = com.riscure.trs.io.LittleEndianInputStream;
	using LittleEndianOutputStream = com.riscure.trs.io.LittleEndianOutputStream;
	using TraceParameter = com.riscure.trs.parameter.TraceParameter;


	public class IntegerArrayParameter : TraceParameter
	{
		private readonly int[] value;

		public IntegerArrayParameter(int length)
		{
			value = new int[length];
		}

		public IntegerArrayParameter(int[] value)
		{
			this.value = value;
		}

		public IntegerArrayParameter(IntegerArrayParameter toCopy) : this((int[])toCopy.Value.Clone())
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void serialize(com.riscure.trs.io.LittleEndianOutputStream dos) throws java.io.IOException
		public override void serialize(LittleEndianOutputStream dos)
		{
			foreach (int i in value)
			{
				dos.writeInt(i);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static IntegerArrayParameter deserialize(com.riscure.trs.io.LittleEndianInputStream dis, int length) throws java.io.IOException
		public static IntegerArrayParameter deserialize(LittleEndianInputStream dis, int length)
		{
			IntegerArrayParameter result = new IntegerArrayParameter(length);
			for (int k = 0; k < length; k++)
			{
				result.value[k] = dis.readInt();
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
				return ParameterType.INT;
			}
		}

		public override int[] Value
		{
			get
			{
				return value;
			}
		}

		public override IntegerArrayParameter copy()
		{
			return new IntegerArrayParameter(this);
		}

		public override int? ScalarValue
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

			IntegerArrayParameter that = (IntegerArrayParameter) o;

			return value.SequenceEqual(that.value);
		}

		public override int GetHashCode()
		{
			return Arrays.hashCode(value);
		}
	}

}