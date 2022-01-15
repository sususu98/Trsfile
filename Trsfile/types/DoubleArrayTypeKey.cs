﻿namespace com.riscure.trs.types
{
	using TraceParameter = com.riscure.trs.parameter.TraceParameter;
	using DoubleArrayParameter = com.riscure.trs.parameter.primitive.DoubleArrayParameter;

	public class DoubleArrayTypeKey : TypedKey<double[]>
	{
		public DoubleArrayTypeKey(string key) : base(typeof(double[]), key)
		{
		}

		public override TraceParameter CreateParameter(double[] value)
		{
			CheckLength(value);
			return new DoubleArrayParameter(value);
		}
	}

}