using System;
using com.riscure.trs.enums;
using com.riscure.trs.parameter;

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

		public virtual T Cast<T2>(T2 value) where T2 : struct
		{
			if (Cls.IsAssignableFrom(value.GetType()))
			{
				return (T)value;
			}
			if (typeof(T) == typeof(T2)) return (T)value;
			throw new InvalidCastException(string.Format(INCORRECT_TYPE, Cls.FullName, value.GetType().FullName));
		}

		public virtual T Cast<T2>(T2[] value) where T2 : struct
		{
			_ = value ?? throw new ArgumentNullException();
			if (Cls.IsAssignableFrom(value.GetType()))
			{
				return (T)value;
			}
			throw new InvalidCastException(string.Format(INCORRECT_TYPE, Cls.FullName, value.GetType().FullName));
		}

		public virtual T Cast(string value)
		{
			_ = value ?? throw new ArgumentNullException();
			if (Cls.IsAssignableFrom(value.GetType()))
			{
				return (T)value;
			}
			throw new InvalidCastException(string.Format(INCORRECT_TYPE, Cls.FullName, value.GetType().FullName));
		}

		public abstract TraceParameter CreateParameter(T value);

		public virtual ParameterType Type
		{
			get
			{
				return ParameterType.FromClass(Cls);
			}
		}

		protected internal virtual void CheckLength(T value)
		{
			if (Cls.IsArray && Array.getLength(value) <= 0)
			{
				throw new System.ArgumentException("Array length must be positive and non-zero.");
			}
		}

		public override bool Equals(object o)
		{
			if (this == o)
			{
				return true;
			}
			if (o == null || this.GetType() != o.GetType())
			{
				return false;
			}

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: TypedKey<?> typedKey = (TypedKey<?>) o;
			TypedKey<object> typedKey = (TypedKey<object>) o;

			if (!Objects.equals(Cls, typedKey.Cls))
			{
				return false;
			}
			return Objects.equals(Key, typedKey.Key);
		}

		public override int GetHashCode()
		{
			int result = Cls != null ? Cls.GetHashCode() : 0;
			result = 31 * result + (!string.ReferenceEquals(Key, null) ? Key.GetHashCode() : 0);
			return result;
		}

		public override string ToString()
		{
			return "TypedKey{" + "key='" + Key + "', cls=" + Cls + '}';
		}
	}

}