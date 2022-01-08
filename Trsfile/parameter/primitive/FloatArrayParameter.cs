using System.Linq;

namespace com.riscure.trs.parameter.primitive
{
	using ParameterType = com.riscure.trs.enums.ParameterType;
	using LittleEndianInputStream = com.riscure.trs.io.LittleEndianInputStream;
	using LittleEndianOutputStream = com.riscure.trs.io.LittleEndianOutputStream;
	using TraceParameter = com.riscure.trs.parameter.TraceParameter;


	public class FloatArrayParameter : TraceParameter
	{
		private readonly float[] value;

		public FloatArrayParameter(int length)
		{
			value = new float[length];
		}

		public FloatArrayParameter(float[] value)
		{
			this.value = value;
		}

		public FloatArrayParameter(FloatArrayParameter toCopy) : this((float[])toCopy.Value.Clone())
		{
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void serialize(com.riscure.trs.io.LittleEndianOutputStream dos) throws java.io.IOException
		public override void serialize(LittleEndianOutputStream dos)
		{
			foreach (float i in value)
			{
				dos.writeFloat(i);
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static FloatArrayParameter deserialize(com.riscure.trs.io.LittleEndianInputStream dis, int length) throws java.io.IOException
		public static FloatArrayParameter deserialize(LittleEndianInputStream dis, int length)
		{
			FloatArrayParameter result = new FloatArrayParameter(length);
			for (int k = 0; k < length; k++)
			{
				result.value[k] = dis.readFloat();
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
				return ParameterType.FLOAT;
			}
		}

		public override float[] Value
		{
			get
			{
				return value;
			}
		}

		public override FloatArrayParameter copy()
		{
			return new FloatArrayParameter(this);
		}

		public override float? ScalarValue
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

			FloatArrayParameter that = (FloatArrayParameter) o;

			return value.SequenceEqual(that.value);
		}

		public override int GetHashCode()
		{
			return Arrays.hashCode(value);
		}
	}

}