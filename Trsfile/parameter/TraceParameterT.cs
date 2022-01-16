using com.riscure.trs.enums;
using com.riscure.trs.io;

namespace com.riscure.trs.parameter
{
    /// <summary>
    /// This interface represents a parameter with generic that is used in the trace data or the trace set header
    /// </summary>
    public abstract class TraceParameter<T> : TraceParameter
    {
        public T[] Value { get; }

        public T ScalarValue { get; }

        public TraceParameter(T value) : base(null, false)
        {
            Type = typeof(T) switch
            {
                T _ when value is bool => ParameterType.BOOL,
                T _ when value is byte => ParameterType.BYTE,
                T _ when value is short => ParameterType.SHORT,
                T _ when value is int => ParameterType.INT,
                T _ when value is float => ParameterType.FLOAT,
                T _ when value is double => ParameterType.DOUBLE,
                T _ when value is string => ParameterType.STRING,
                T _ when value is long => ParameterType.LONG,
                _ => throw new InvalidDataException("Not supported Data Type")
            };
            Value = new T[] { value };
            ScalarValue = value;
        }

        public TraceParameter(T[] value) : base(null, true)
        {
            _ = value ?? throw new ArgumentNullException(nameof(value) + " is null");
            Type = value.ElementAtOrDefault(0) switch
            {
                T t when t is bool => ParameterType.BOOL,
                T t when t is byte => ParameterType.BYTE,
                T t when t is short => ParameterType.SHORT,
                T t when t is int => ParameterType.INT,
                T t when t is float => ParameterType.FLOAT,
                T t when t is double => ParameterType.DOUBLE,
                T t when t is long => ParameterType.LONG,
                _ => throw new InvalidDataException("Not supported Data Type")
            };
            Value = value;
#pragma warning disable CS8601
            ScalarValue = value.ElementAtOrDefault(0); // Here it wouldn't be null :)
#pragma warning restore CS8601
        }

        public override int Length => Value.Length;

    }

}
