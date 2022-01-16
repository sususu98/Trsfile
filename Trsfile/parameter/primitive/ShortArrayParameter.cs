using System.Linq;

namespace com.riscure.trs.parameter.primitive
{
	using ParameterType = com.riscure.trs.enums.ParameterType;
	using LittleEndianInputStream = com.riscure.trs.io.LittleEndianInputStream;
	using LittleEndianOutputStream = com.riscure.trs.io.LittleEndianOutputStream;
	using TraceParameter = com.riscure.trs.parameter.TraceParameter;


	public class ShortArrayParameter : TraceParameter
	{
		private readonly short[] value;

		public ShortArrayParameter(int length)
		{
			value = new short[length];
		}

		public ShortArrayParameter(short[] value)
		{
			this.value = value;
		}

		public ShortArrayParameter(ShortArrayParameter toCopy) : this((short[])toCopy.Value.Clone())
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void serialize(com.riscure.trs.io.LittleEndianOutputStream dos) throws java.io.IOException
		public override void Serialize(LittleEndianOutputStream dos)
		{
			foreach (short i in value)
			{
				dos.writeShort(i);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static ShortArrayParameter deserialize(com.riscure.trs.io.LittleEndianInputStream dis, int length) throws java.io.IOException
		public static ShortArrayParameter deserialize(LittleEndianInputStream dis, int length)
		{
			ShortArrayParameter result = new ShortArrayParameter(length);
			for (int k = 0; k < length; k++)
			{
				result.value[k] = dis.readShort();
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
				return ParameterType.SHORT;
			}
		}

		public override short[] Value
		{
			get
			{
				return value;
			}
		}

		public override ShortArrayParameter copy()
		{
			return new ShortArrayParameter(this);
		}

		public override short? ScalarValue
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

			ShortArrayParameter that = (ShortArrayParameter) o;

			return value.SequenceEqual(that.value);
		}

		public override int GetHashCode()
		{
			return Arrays.hashCode(value);
		}
	}

}