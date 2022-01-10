namespace com.riscure.trs.parameter
{
	using ParameterType = com.riscure.trs.enums.ParameterType;
	using LittleEndianInputStream = com.riscure.trs.io.LittleEndianInputStream;
	using LittleEndianOutputStream = com.riscure.trs.io.LittleEndianOutputStream;
	using com.riscure.trs.parameter.primitive;

	/// <summary>
	/// This interface represents a parameter that is used in the trace data or the trace set header
	/// </summary>
	public abstract class TraceParameter
	{
		public const string SAMPLES = "SAMPLES";
		public const string TITLE = "TITLE";

		/// <summary>
		/// The number of values of this type in this parameter
		/// </summary>
		/// <returns> the number of values of this type in this parameter </returns>
		public abstract int length();

		/// <returns> The type of the parameter. </returns>
		public abstract ParameterType Type {get;}

		/// <returns> The value of the parameter. </returns>
		public abstract object Value {get;}

		/// <returns> a newly created parameter containing the same information as this one. </returns>
		public abstract TraceParameter copy();

		/// <returns> The value of the parameter as a simple value. Will cause an exception if called on an array type. </returns>
		public abstract object ScalarValue {get;}

		/// <summary>
		/// Write this TraceParameter to the specified output stream
		/// </summary>
		/// <param name="dos"> the OutputStream to write to </param>
		/// <exception cref="IOException"> if any problems arise from writing to the stream </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public abstract void serialize(com.riscure.trs.io.LittleEndianOutputStream dos) throws java.io.IOException;
		public abstract void serialize(LittleEndianOutputStream dos);

		/// <summary>
		/// Read a new TraceParameter from the specified input stream
		/// </summary>
		/// <param name="type">   the type of the parameter to read </param>
		/// <param name="length"> the number of values to read </param>
		/// <param name="dis">    the input stream to read from </param>
		/// <returns> a new TraceParameter of the specified type and length </returns>
		/// <exception cref="IOException"> if any problems arise from reading from the stream </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static TraceParameter deserialize(com.riscure.trs.enums.ParameterType type, int length, com.riscure.trs.io.LittleEndianInputStream dis) throws java.io.IOException
		public static TraceParameter deserialize(ParameterType type, int length, LittleEndianInputStream dis)
		{
            return type.innerEnumValue switch
            {
                ParameterType.InnerEnum.BYTE => ByteArrayParameter.deserialize(dis, length),
                ParameterType.InnerEnum.SHORT => ShortArrayParameter.deserialize(dis, length),
                ParameterType.InnerEnum.INT => IntegerArrayParameter.deserialize(dis, length),
                ParameterType.InnerEnum.FLOAT => FloatArrayParameter.deserialize(dis, length),
                ParameterType.InnerEnum.LONG => LongArrayParameter.deserialize(dis, length),
                ParameterType.InnerEnum.DOUBLE => DoubleArrayParameter.deserialize(dis, length),
                ParameterType.InnerEnum.STRING => StringParameter.deserialize(dis, length),
                ParameterType.InnerEnum.BOOL => BooleanArrayParameter.deserialize(dis, length),
                _ => throw new System.ArgumentException("Unknown parameter type: " + type.ToString()),
            };
        }
	}

}