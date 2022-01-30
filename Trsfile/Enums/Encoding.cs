namespace Trsfile.Enums
{
    public sealed record Encoding
    {
        public static readonly Encoding ILLEGAL = new("ILLEGAL", EncodingEnum.ILLEGAL, 0x00, -1);
        public static readonly Encoding BYTE = new("BYTE", EncodingEnum.BYTE, 0x01, 1);
        public static readonly Encoding SHORT = new("SHORT", EncodingEnum.SHORT, 0x02, 2);
        public static readonly Encoding INT = new("INT", EncodingEnum.INT, 0x04, 4);
        public static readonly Encoding FLOAT = new("FLOAT", EncodingEnum.FLOAT, 0x14, 4);

        private static readonly Encoding[] valueArray 
            = new Encoding[] { ILLEGAL, BYTE, SHORT, INT, FLOAT };

        public enum EncodingEnum
        {
            ILLEGAL,
            BYTE,
            SHORT,
            INT,
            FLOAT
        }

        public EncodingEnum InnerEnumValue { get; }
        private readonly string nameValue;
        private static int nextOrdinal = 0;


        internal Encoding(string name, EncodingEnum innerEnum, int value, int size)
        {
            Value = value;
            Size = size;

            nameValue = name;
            Ordinal = nextOrdinal++;
            InnerEnumValue = innerEnum;
        }

        public int Size { get; }

        public int Value { get; }
        public int Ordinal { get; }

        public static Encoding FromValue(int value)
        {
            foreach (Encoding encoding in Values)
            {
                if (encoding.Value == value)
                {
                    return encoding;
                }
            }
            throw new ArgumentException("Unknown Trace encoding: " + value);
        }

        public static Encoding[] Values => valueArray;

        public override string ToString()
        {
            return nameValue;
        }

        public static Encoding ValueOf(string name)
        {
            foreach (var enumInstance in valueArray)
            {
                if (enumInstance.nameValue == name)
                {
                    return enumInstance;
                }
            }
            throw new ArgumentException(name);
        }
    }

}