using System;

namespace com.riscure.trs.types
{
	using ParameterType = com.riscure.trs.enums.ParameterType;
	using TraceParameter = com.riscure.trs.parameter.TraceParameter;


	public abstract class TypedKey<T>
	{
		public const string INCORRECT_TYPE = "Tried to retrieve a value of type %s, but the actual value was of type %s";

		private readonly Type cls = typeof(T);
		private readonly string key;

		protected internal TypedKey(Type cls, string key)
		{
			this.cls = cls;
			this.key = key;
		}

		public virtual Type<T> Cls
		{
			get
			{
				return cls;
			}
		}

		public virtual string Key
		{
			get
			{
				return key;
			}
		}

		public virtual T cast(object value)
		{
			if (cls.IsAssignableFrom(value.GetType()))
			{
				return cls.cast(value);
			}
//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
			throw new System.InvalidCastException(String.format(INCORRECT_TYPE, cls.FullName, value.GetType().FullName));
		}

		public abstract TraceParameter createParameter(T value);

		public virtual ParameterType Type
		{
			get
			{
				return ParameterType.fromClass(cls);
			}
		}

		protected internal virtual void checkLength(T value)
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

			if (!Objects.equals(cls, typedKey.cls))
			{
				return false;
			}
			return Objects.equals(key, typedKey.key);
		}

		public override int GetHashCode()
		{
			int result = cls != null ? cls.GetHashCode() : 0;
			result = 31 * result + (!string.ReferenceEquals(key, null) ? key.GetHashCode() : 0);
			return result;
		}

		public override string ToString()
		{
			return "TypedKey{" + "key='" + key + "', cls=" + cls + '}';
		}
	}

}