using System.Collections.Generic;

namespace com.riscure.trs.enums
{
	public sealed class Encoding
	{
		public static readonly Encoding ILLEGAL = new Encoding("ILLEGAL", InnerEnum.ILLEGAL, 0x00, -1);
		public static readonly Encoding BYTE = new Encoding("BYTE", InnerEnum.BYTE, 0x01, 1);
		public static readonly Encoding SHORT = new Encoding("SHORT", InnerEnum.SHORT, 0x02, 2);
		public static readonly Encoding INT = new Encoding("INT", InnerEnum.INT, 0x04, 4);
		public static readonly Encoding FLOAT = new Encoding("FLOAT", InnerEnum.FLOAT, 0x14, 4);

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
		private readonly int ordinalValue;
		private static int nextOrdinal = 0;

		private readonly int value;
		private readonly int size;

		internal Encoding(string name, InnerEnum innerEnum, int value, int size)
		{
			this.value = value;
			this.size = size;

			nameValue = name;
			ordinalValue = nextOrdinal++;
			innerEnumValue = innerEnum;
		}

		public int Size
		{
			get
			{
				return size;
			}
		}

		public int Value
		{
			get
			{
				return value;
			}
		}

		public static Encoding fromValue(int value)
		{
			foreach (Encoding encoding in Encoding.values())
			{
				if (encoding.Value == value)
				{
					return encoding;
				}
			}
			throw new System.ArgumentException("Unknown Trace encoding: " + value);
		}

		public static Encoding[] values()
		{
			return valueList.ToArray();
		}

		public int ordinal()
		{
			return ordinalValue;
		}

		public override string ToString()
		{
			return nameValue;
		}

		public static Encoding valueOf(string name)
		{
			foreach (Encoding enumInstance in Encoding.valueList)
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