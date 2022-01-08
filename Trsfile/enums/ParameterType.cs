using System;
using System.Collections.Generic;

namespace com.riscure.trs.enums
{
	public sealed class ParameterType
	{
		public static readonly ParameterType BYTE = new ParameterType("BYTE", InnerEnum.BYTE, 0x01, Byte.BYTES, typeof(Byte), typeof(sbyte[]));
		public static readonly ParameterType SHORT = new ParameterType("SHORT", InnerEnum.SHORT, 0x02, Short.BYTES, typeof(Short), typeof(short[]));
		public static readonly ParameterType INT = new ParameterType("INT", InnerEnum.INT, 0x04, Integer.BYTES, typeof(Integer), typeof(int[]));
		public static readonly ParameterType FLOAT = new ParameterType("FLOAT", InnerEnum.FLOAT, 0x14, Float.BYTES, typeof(Float), typeof(float[]));
		public static readonly ParameterType LONG = new ParameterType("LONG", InnerEnum.LONG, 0x08, Long.BYTES, typeof(Long), typeof(long[]));
		public static readonly ParameterType DOUBLE = new ParameterType("DOUBLE", InnerEnum.DOUBLE, 0x18, Double.BYTES, typeof(Double), typeof(double[]));
		public static readonly ParameterType STRING = new ParameterType("STRING", InnerEnum.STRING, 0x20, Byte.BYTES, typeof(string), typeof(string));
		public static readonly ParameterType BOOL = new ParameterType("BOOL", InnerEnum.BOOL, 0x31, Byte.BYTES, typeof(Boolean), typeof(bool[]));

		private static readonly List<ParameterType> valueList = new List<ParameterType>();

		static ParameterType()
		{
			valueList.Add(BYTE);
			valueList.Add(SHORT);
			valueList.Add(INT);
			valueList.Add(FLOAT);
			valueList.Add(LONG);
			valueList.Add(DOUBLE);
			valueList.Add(STRING);
			valueList.Add(BOOL);
		}

		public enum InnerEnum
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

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal = 0;

		private readonly sbyte value;
		private readonly int byteSize;
		private readonly Type cls;
		private readonly Type arrayCls;

		internal ParameterType(string name, InnerEnum innerEnum, int value, int byteSize, Type cls, Type arrayCls)
		{
			this.value = (sbyte)value;
			this.byteSize = byteSize;
			this.cls = cls;
			this.arrayCls = arrayCls;

			nameValue = name;
			ordinalValue = nextOrdinal++;
			innerEnumValue = innerEnum;
		}

		public sbyte Value
		{
			get
			{
				return value;
			}
		}

		public int ByteSize
		{
			get
			{
				return byteSize;
			}
		}

		public static ParameterType fromValue(sbyte value)
		{
			foreach (ParameterType parameterType in ParameterType.values())
			{
				if (parameterType.value == value)
				{
					return parameterType;
				}
			}
			throw new System.ArgumentException("Unknown parameter type: " + value);
		}

		public static ParameterType fromClass(Type cls)
		{
			foreach (ParameterType parameterType in ParameterType.values())
			{
				if (parameterType.cls.IsAssignableFrom(cls) || parameterType.arrayCls.IsAssignableFrom(cls))
				{
					return parameterType;
				}
			}
			throw new System.ArgumentException("Unknown parameter type: " + cls);
		}

		public static ParameterType[] values()
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

		public static ParameterType valueOf(string name)
		{
			foreach (ParameterType enumInstance in ParameterType.valueList)
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