﻿namespace com.riscure.trs.types
{
	using TraceParameter = com.riscure.trs.parameter.TraceParameter;
	using IntegerArrayParameter = com.riscure.trs.parameter.primitive.IntegerArrayParameter;

	public class IntegerArrayTypeKey : TypedKey<int[]>
	{
		public IntegerArrayTypeKey(string key) : base(typeof(int[]), key)
		{
		}

		public override TraceParameter CreateParameter(int[] value)
		{
			CheckLength(value);
			return new IntegerArrayParameter(value);
		}
	}

}