using System.Linq;

namespace com.riscure.trs.parameter.primitive
{
	using ParameterType = com.riscure.trs.enums.ParameterType;
	using LittleEndianInputStream = com.riscure.trs.io.LittleEndianInputStream;
	using LittleEndianOutputStream = com.riscure.trs.io.LittleEndianOutputStream;
	using TraceParameter = com.riscure.trs.parameter.TraceParameter;


	public class DoubleArrayParameter : TraceParameter
	{
		private readonly double[] value;

		public DoubleArrayParameter(int length)
		{
			value = new double[length];
		}

		public DoubleArrayParameter(double[] value)
		{
			this.value = value;
		}

		public DoubleArrayParameter(DoubleArrayParameter toCopy) : this((double[])toCopy.Value.Clone())
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void serialize(com.riscure.trs.io.LittleEndianOutputStream dos) throws java.io.IOException
		public override void Serialize(LittleEndianOutputStream dos)
		{
			foreach (double i in value)
			{
				dos.writeDouble(i);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static DoubleArrayParameter deserialize(com.riscure.trs.io.LittleEndianInputStream dis, int length) throws java.io.IOException
		public static DoubleArrayParameter deserialize(LittleEndianInputStream dis, int length)
		{
			DoubleArrayParameter result = new DoubleArrayParameter(length);
			for (int k = 0; k < length; k++)
			{
				result.value[k] = dis.readDouble();
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
				return ParameterType.DOUBLE;
			}
		}

		public override double[] Value
		{
			get
			{
				return value;
			}
		}

		public override DoubleArrayParameter copy()
		{
			return new DoubleArrayParameter(this);
		}

		public override double? ScalarValue
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

			DoubleArrayParameter that = (DoubleArrayParameter) o;

			return value.SequenceEqual(that.value);
		}

		public override int GetHashCode()
		{
			return Arrays.hashCode(value);
		}
	}

}