using Trsfile.Parameter;

namespace Trsfile.Types
{
    /// <summary>
    /// Basic Type Key for Map
    /// </summary>
    /// <typeparam name="T">Type of single value</typeparam>
    public abstract class TypedKey<T>
	{
		public const string INCORRECT_TYPE = "Tried to retrieve a value of type {0}, but the actual value was of type {1}";

		protected TypedKey(Type cls, string key, bool isArray)
		{
			Cls = cls;
			Key = key;
			IsArray = isArray;
		}

		public Type Cls { get; }

		public string Key { get; }

		public bool IsArray { get; protected set; }

        public virtual T Cast(object value)
        {
            _ = value ?? throw new ArgumentNullException();
            if (Cls.IsAssignableFrom(value.GetType()))
            {
                return (T)value;
            }
            throw new InvalidCastException(string.Format(INCORRECT_TYPE, Cls.FullName, value.GetType().FullName));
        }

        public virtual TraceParameter CreateParameter(T value)
        {
			return CreateParameter(new T[] { value });
        }

		public abstract TraceParameter CreateParameter(T[] value);

		// T, T[], string

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