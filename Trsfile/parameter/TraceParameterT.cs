using com.riscure.trs.enums;

namespace com.riscure.trs.parameter
{
    /// <summary>
    /// This interface represents a parameter with generic that is used in the trace data or the trace set header
    /// </summary>
    public abstract class TraceParameter<T> : TraceParameter
    {
        public T[] Value { get; }

#pragma warning disable CS8603
        public T ScalarValue { get => Value.ElementAtOrDefault(0); }
#pragma warning restore CS8603

        public TraceParameter(T value) : this(new T[]{ value })
        {
           
        }

#pragma warning disable CS8618
        public TraceParameter(T[] value) : base(ParameterType.BOOL) // set as some default
#pragma warning restore CS8618
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
                T t when t is string => value.Length == 1 ?
                    ParameterType.STRING : throw new InvalidDataException("String array is not supported."),
                _ => throw new InvalidDataException("Not supported Data Type")
            };
            Value = value;
        }

        public override int Length => Value.Length;

        public override bool IsArray => Value.Length != 1;

        public override bool Equals(object? o)
        {
            if (this == o)
            {
                return true;
            }
            if (o == null || this.GetType() != o.GetType())
            {
                return false;
            }

            TraceParameter<T> that = (TraceParameter<T>)o;
            return Value.SequenceEqual(that.Value);
        }
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

    }

}
