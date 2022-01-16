using System;
using com.riscure.trs.enums;
using com.riscure.trs.parameter;
using System.Text.Json;

namespace com.riscure.trs.types
{
	public abstract class TypedKey<T>
	{
		public const string INCORRECT_TYPE = "Tried to retrieve a value of type %s, but the actual value was of type %s";

		protected TypedKey(Type cls, string key)
		{
			Cls = cls;
			Key = key;
		}

		public virtual Type Cls { get; } = typeof(T);

		public virtual string Key { get; }

        public virtual T Cast(object value)
        {
            _ = value ?? throw new ArgumentNullException();
            if (Cls.IsAssignableFrom(value.GetType()))
            {
                return (T)value;
            }
            throw new InvalidCastException(string.Format(INCORRECT_TYPE, Cls.FullName, value.GetType().FullName));
        }

        public abstract TraceParameter CreateParameter(T value);

		public virtual ParameterType Type { get => ParameterType.FromClass(Cls); }

		// T, T[], string

		protected internal virtual void CheckLength(T value)
        {

        }

		public override bool Equals(object? o)
		{
			if (o is null) return false;
			if (this == o) return true;
			if (GetType() != o.GetType()) return false;
            if (o is not TypedKey<T> typedKey) return false;
            return typedKey.Key == Key;
		}

		public override int GetHashCode()
		{
			int result = Cls != null ? Cls.GetHashCode() : 0;
			result = 31 * result + Key.GetHashCode();
			return result;
		}

		public override string ToString()
		{
			return "TypedKey{" + "key='" + Key + "', cls=" + Cls + '}';
		}
	}

}