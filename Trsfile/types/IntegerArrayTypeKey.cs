﻿namespace com.riscure.trs.types
{
    using TraceParameter = parameter.TraceParameter;
    using IntegerArrayParameter = parameter.primitive.IntegerArrayParameter;

    public class IntegerArrayTypeKey : TypedKey<int>
    {
        public IntegerArrayTypeKey(string key) : base(typeof(int[]), key, true)
        {
        }

        public override TraceParameter CreateParameter(int[] value)
        {
            if (value.Length <= 0) throw new ArgumentException("Value's length below 0");
            if (value.Length == 1) IsArray = false;
            return new IntegerArrayParameter(value);
        }
    }

}