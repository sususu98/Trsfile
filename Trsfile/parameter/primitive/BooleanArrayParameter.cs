using System.Linq;

namespace com.riscure.trs.parameter.primitive
{
	using ParameterType = com.riscure.trs.enums.ParameterType;
	using LittleEndianInputStream = com.riscure.trs.io.LittleEndianInputStream;
	using LittleEndianOutputStream = com.riscure.trs.io.LittleEndianOutputStream;
	using TraceParameter = com.riscure.trs.parameter.TraceParameter;


	public class BooleanArrayParameter : TraceParameter
	{
		private readonly bool[] value;

		public BooleanArrayParameter(int length)
		{
			value = new bool[length];
		}

		public BooleanArrayParameter(bool[] value)
		{
			this.value = value;
		}

		public BooleanArrayParameter(BooleanArrayParameter toCopy) : this((bool[])toCopy.Value.Clone())
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void serialize(com.riscure.trs.io.LittleEndianOutputStream dos) throws java.io.IOException
		public override void Serialize(LittleEndianOutputStream dos)
		{
			foreach (bool i in value)
			{
				dos.writeByte(i ? 1 : 0);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static BooleanArrayParameter deserialize(com.riscure.trs.io.LittleEndianInputStream dis, int length) throws java.io.IOException
		public static BooleanArrayParameter deserialize(LittleEndianInputStream dis, int length)
		{
			BooleanArrayParameter result = new BooleanArrayParameter(length);
			for (int k = 0; k < length; k++)
			{
				result.value[k] = dis.readByte() != 0;
			}
			return result;
		}

		public override int Length()
		{
			return value.Length;
		}

		public override ParameterType Type
		{
			get
			{
				return ParameterType.BOOL;
			}
		}

		public override bool[] Value
		{
			get
			{
				return value;
			}
		}

		public override BooleanArrayParameter copy()
		{
			return new BooleanArrayParameter(this);
		}

		public override bool? ScalarValue
		{
			get
			{
				if (Length() > 1)
				{
					throw new System.ArgumentException("Parameter represents an array value of length " + Length());
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

			BooleanArrayParameter that = (BooleanArrayParameter) o;

			return value.SequenceEqual(that.value);
		}

		public override int GetHashCode()
		{
			return Arrays.hashCode(value);
		}
	}

}