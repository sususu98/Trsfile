using com.riscure.trs.enums;
using com.riscure.trs.io;
using com.riscure.trs.parameter.primitive;

namespace com.riscure.trs.parameter
{
    /// <summary>
    /// This interface represents a parameter that is used in the trace data or the trace set header
    /// </summary>
    public abstract partial class TraceParameter : ICloneable
    {
        public const string SAMPLES = nameof(SAMPLES);
        public const string TITLE = nameof(TITLE);

        protected TraceParameter(ParameterType type)
        {
            Type = type;
        }

        /// <summary>
        /// The number of values of this type in this parameter
        /// </summary>
        /// <returns> the number of values of this type in this parameter </returns>
        public abstract int Length { get; }

        public virtual bool IsArray { get; }

        /// <returns> The type of the parameter. </returns>
        public ParameterType Type { get; protected set; }

        /// <returns> The value of the parameter. </returns>
        //public abstract object Value { get; }

        /// <returns> The value of the parameter as a simple value. Will cause an exception if called on an array type. </returns>
        //public abstract object ScalarValue { get; }

        /// <summary>
        /// Write this TraceParameter to the specified output stream
        /// </summary>
        /// <param name="dos"> the OutputStream to write to </param>
        /// <exception cref="IOException"> if any problems arise from writing to the stream </exception>
        public abstract void Serialize(LittleEndianOutputStream dos);

        /// <summary>
        /// Read a new TraceParameter from the specified input stream
        /// </summary>
        /// <param name="type">   the type of the parameter to read </param>
        /// <param name="length"> the number of values to read </param>
        /// <param name="dis">    the input stream to read from </param>
        /// <returns> a new TraceParameter of the specified type and length </returns>
        /// <exception cref="IOException"> if any problems arise from reading from the stream </exception>
        public static TraceParameter Deserialize(ParameterType type, int length, LittleEndianInputStream dis)
        {
            return type.TypeEnum switch
            {
                ParameterType.ParameterTypeEnum.BYTE => ByteArrayParameter.Deserialize(dis, length),
                ParameterType.ParameterTypeEnum.SHORT => ShortArrayParameter.Deserialize(dis, length),
                ParameterType.ParameterTypeEnum.INT => IntegerArrayParameter.Deserialize(dis, length),
                ParameterType.ParameterTypeEnum.FLOAT => FloatArrayParameter.Deserialize(dis, length),
                ParameterType.ParameterTypeEnum.LONG => LongArrayParameter.Deserialize(dis, length),
                ParameterType.ParameterTypeEnum.DOUBLE => DoubleArrayParameter.Deserialize(dis, length),
                ParameterType.ParameterTypeEnum.STRING => StringParameter.Deserialize(dis, length),
                ParameterType.ParameterTypeEnum.BOOL => BoolArrayParameter.Deserialize(dis, length),
                _ => throw new System.ArgumentException("Unknown parameter type: " + type.ToString()),
            };
        }

        public abstract object Clone();

    }

}