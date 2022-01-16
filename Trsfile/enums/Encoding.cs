namespace com.riscure.trs.enums
{
    public sealed class Encoding
    {
        public static readonly Encoding ILLEGAL = new("ILLEGAL", InnerEnum.ILLEGAL, 0x00, -1);
        public static readonly Encoding BYTE = new("BYTE", InnerEnum.BYTE, 0x01, 1);
        public static readonly Encoding SHORT = new("SHORT", InnerEnum.SHORT, 0x02, 2);
        public static readonly Encoding INT = new("INT", InnerEnum.INT, 0x04, 4);
        public static readonly Encoding FLOAT = new("FLOAT", InnerEnum.FLOAT, 0x14, 4);

        private static readonly List<Encoding> valueList = new List<Encoding>();

        static Encoding()
        {
            valueList.Add(ILLEGAL);
            valueList.Add(BYTE);
            valueList.Add(SHORT);
            valueList.Add(INT);
            valueList.Add(FLOAT);
        }

        public enum InnerEnum
        {
            ILLEGAL,
            BYTE,
            SHORT,
            INT,
            FLOAT
        }

        public readonly InnerEnum innerEnumValue;
        private readonly string nameValue;
        private static int nextOrdinal = 0;


        internal Encoding(string name, InnerEnum innerEnum, int value, int size)
        {
            Value = value;
            Size = size;

            nameValue = name;
            Ordinal = nextOrdinal++;
            innerEnumValue = innerEnum;
        }

        public int Size { get; }

        public int Value { get; }
        public int Ordinal { get; }

        public static Encoding FromValue(int value)
        {
            foreach (Encoding encoding in Values())
            {
                if (encoding.Value == value)
                {
                    return encoding;
                }
            }
            throw new ArgumentException("Unknown Trace encoding: " + value);
        }

        public static Encoding[] Values()
        {
            return valueList.ToArray();
        }



        public override string ToString()
        {
            return nameValue;
        }

        public static Encoding ValueOf(string name)
        {
            foreach (Encoding enumInstance in Encoding.valueList)
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