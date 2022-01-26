namespace com.riscure.trs.enums
{
    public sealed class ParameterType
    {
        public static readonly ParameterType BYTE
            = new("BYTE", ParameterTypeEnum.BYTE, 0x01, sizeof(byte), typeof(byte), typeof(byte[]));
        public static readonly ParameterType SHORT
            = new("SHORT", ParameterTypeEnum.SHORT, 0x02, sizeof(short), typeof(short), typeof(short[]));
        public static readonly ParameterType INT
            = new("INT", ParameterTypeEnum.INT, 0x04, sizeof(int), typeof(int), typeof(int[]));
        public static readonly ParameterType FLOAT
            = new("FLOAT", ParameterTypeEnum.FLOAT, 0x14, sizeof(float), typeof(float), typeof(float[]));
        public static readonly ParameterType LONG
            = new("LONG", ParameterTypeEnum.LONG, 0x08, sizeof(long), typeof(long), typeof(long[]));
        public static readonly ParameterType DOUBLE
            = new("DOUBLE", ParameterTypeEnum.DOUBLE, 0x18, sizeof(double), typeof(double), typeof(double[]));
        public static readonly ParameterType STRING
            = new("STRING", ParameterTypeEnum.STRING, 0x20, sizeof(byte), typeof(string), typeof(string));
        public static readonly ParameterType BOOL
            = new("BOOL", ParameterTypeEnum.BOOL, 0x31, sizeof(byte), typeof(bool), typeof(bool[]));

        public static readonly ParameterType[] Values = new ParameterType[]
        {
            BYTE,
            SHORT,
            INT,
            FLOAT,
            LONG,
            DOUBLE,
            STRING,
            BOOL
        };

        static ParameterType()
        {

        }

        public enum ParameterTypeEnum
        {
            BYTE,
            SHORT,
            INT,
            FLOAT,
            LONG,
            DOUBLE,
            STRING,
            BOOL
        }

        public ParameterTypeEnum TypeEnum { get; }
        private readonly string nameValue;
        public int Ordinal { get; }
        private static int nextOrdinal = 0;

        public Type Cls { get; }
        public Type ArrayCls { get; }

        internal ParameterType(string name, ParameterTypeEnum innerEnum, int value, int byteSize, Type cls, Type arrayCls)
        {
            Value = (byte)value;
            ByteSize = byteSize;
            Cls = cls;
            ArrayCls = arrayCls;

            nameValue = name;
            Ordinal = nextOrdinal++;
            TypeEnum = innerEnum;
        }

        public byte Value { get; }

        public int ByteSize { get; }

        public static ParameterType FromValue(sbyte value)
        {
            foreach (var parameterType in Values)
            {
                if (parameterType.Value == value)
                {
                    return parameterType;
                }
            }
            throw new ArgumentException("Unknown parameter type: " + value);
        }

        public static ParameterType FromClass(Type cls)
        {
            foreach (var parameterType in Values)
            {
                if (parameterType.Cls.IsAssignableFrom(cls) || parameterType.ArrayCls.IsAssignableFrom(cls))
                {
                    return parameterType;
                }
            }
            throw new ArgumentException("Unknown parameter type: " + cls);
        }

        public override string ToString()
        {
            return nameValue;
        }

        public static ParameterType ValueOf(string name)
        {
            foreach (var enumInstance in Values)
            {
                if (enumInstance.nameValue == name)
                {
                    return enumInstance;
                }
            }
            throw new System.ArgumentException(name);
        }
    }

}