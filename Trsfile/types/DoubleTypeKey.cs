﻿namespace com.riscure.trs.types
{
    using TraceParameter = parameter.TraceParameter;
    using DoubleArrayParameter = parameter.primitive.DoubleArrayParameter;

    public class DoubleTypeKey : TypedKey<double>
    {
        public DoubleTypeKey(string key) : base(typeof(double), key, false)
        {
        }

        public override TraceParameter CreateParameter(double value)
        {
            return new DoubleArrayParameter(value);
        }

        public override TraceParameter CreateParameter(double[] value)
        {
            throw new NotSupportedException("This is single type key, this method is not supported.");
        }
    }

}