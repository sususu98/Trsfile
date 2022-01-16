using System;
using System.Collections.Generic;
using System.IO;

namespace com.riscure.trs.parameter.trace
{
	using LittleEndianInputStream = com.riscure.trs.io.LittleEndianInputStream;
	using LittleEndianOutputStream = com.riscure.trs.io.LittleEndianOutputStream;
	using TraceParameter = com.riscure.trs.parameter.TraceParameter;
	using com.riscure.trs.parameter.trace.definition;
	using TraceParameterDefinitionMap = com.riscure.trs.parameter.trace.definition.TraceParameterDefinitionMap;
	using com.riscure.trs.types;


	public class TraceParameterMap : Dictionary<string, TraceParameter>
	{
		private const string KEY_NOT_FOUND = "TraceParameter %s was not found in the trace.";
		private const string EMPTY_DATA_BUT_NONEMPTY_DEFINITIONS = "The provided byte array is null or empty, but the provided definitions are not";
		private const string DATA_LENGTH_DEFINITIONS_MISMATCH = "The provided byte array (%d bytes) does not match the total definitions length (%d bytes)";

		public TraceParameterMap() : base()
		{
		}

		public TraceParameterMap(TraceParameterMap toCopy) : this()
		{
            foreach (var (key, value) in toCopy)
            {
				Add(key, value.copy());
            }
		}

		/// <returns> a new instance of a TraceParameterMap containing all the same values as this one </returns>
		public virtual TraceParameterMap copy()
		{
			return new TraceParameterMap(this);
		}

		/// <returns> a concatenation of all trace parameters in this map, individually converted to byte arrays </returns>
		/// <exception cref="RuntimeException"> if the map failed to serialize correctly </exception>
		public virtual byte[] ToByteArray()
		{
			MemoryStream baos = new MemoryStream();
			try
			{
                using LittleEndianOutputStream dos = new LittleEndianOutputStream(baos);
                foreach (TraceParameter parameter in Values)
                {
                    parameter.Serialize(dos);
                }
                dos.flush();
                return baos.ToArray();
            }
			catch (IOException ex)
			{
				throw new Exception(ex.Message, ex);
			}
		}

		/// <param name="bytes"> a raw byte array representing the values defined by the definition map </param>
		/// <param name="definitions"> the type and length information describing the provided byte array </param>
		/// <returns> a new TraceParameterMap, created from the provided byte array based on the provided definitions </returns>
		/// <exception cref="RuntimeException"> if the provided byte array does not represent a valid parameter map </exception>
		public static TraceParameterMap deserialize(byte[] bytes, TraceParameterDefinitionMap definitions)
		{
			TraceParameterMap result = new TraceParameterMap();
			if (bytes != null)
			{
				if (bytes.Length != definitions.TotalByteSize())
				{
					throw new ArgumentException(string.Format(DATA_LENGTH_DEFINITIONS_MISMATCH, bytes.Length, definitions.TotalByteSize()));
				}
				try
				{
                    using MemoryStream bais = new MemoryStream(bytes);
                    LittleEndianInputStream dis = new LittleEndianInputStream(bais);
                    foreach (var entry in definitions)
                    {
                        TraceParameter traceParameter = TraceParameter.Deserialize(entry.Value.Type, entry.Value.Length, dis);
                        result.Add(entry.Key, traceParameter);
                    }
                }
				catch (IOException ex)
				{
					throw new Exception(ex.Message, ex);
				}
			}
			else if (definitions.TotalByteSize() != 0)
			{
				throw new System.ArgumentException(EMPTY_DATA_BUT_NONEMPTY_DEFINITIONS);
			}
			return UnmodifiableTraceParameterMap.of(result);
		}

		/// <summary>
		/// Add a new parameter to the map </summary>
		/// <param name="typedKey"> the <seealso cref="TypedKey"/> defining the name and the type of the added value </param>
		/// <param name="value"> the value of the parameter to add </param>
		/// @param <T> the type of the parameter </param>
		/// <exception cref="IllegalArgumentException"> if the value is not valid </exception>
		public virtual void Add<T>(TypedKey<T> typedKey, T value) where T : struct
		{
			Add(typedKey.Key, typedKey.CreateParameter(value));
		}

		/// <summary>
		/// Get a parameter from the map </summary>
		/// <param name="typedKey"> the <seealso cref="TypedKey"/> defining the name and the type of the value to retrieve </param>
		/// @param <T> the type of the parameter </param>
		/// <returns> the value of the requested parameter </returns>
		/// <exception cref="ClassCastException"> if the requested value is not of the expected type </exception>
		public virtual T? get<T>(TypedKey<T> typedKey) where T : struct
		{
			TraceParameter parameter = this[typedKey.Key];
			if (parameter != null)
			{
				if (parameter.Length() == 1 && !typedKey.Cls.IsArray)
				{
					return typedKey.Cast(parameter.ScalarValue);
				}
				else
				{
					return typedKey.Cast(parameter.Value);
				}
			}
			return default(T);
		}

		/// <summary>
		/// Get a parameter from the map </summary>
		/// <param name="typedKey"> the <seealso cref="TypedKey"/> defining the name and the type of the value to retrieve </param>
		/// @param <T> the type of the parameter </param>
		/// <returns> the value of the requested parameter </returns>
		/// <exception cref="ClassCastException"> if the requested value is not of the expected type </exception>
		/// <exception cref="NoSuchElementException"> if the requested value does not exist in the map </exception>
		public virtual T getOrElseThrow<T>(TypedKey<T> typedKey)
		{
			return get(typedKey).orElseThrow(() => new NoSuchElementException(String.format(KEY_NOT_FOUND, typedKey.Key)));
		}

		/// <summary>
		/// Get a parameter from the map </summary>
		/// <param name="typedKey"> the <seealso cref="TypedKey"/> defining the name and the type of the value to retrieve </param>
		/// @param <T> the type of the parameter </param>
		/// <param name="defaultValue"> the value to use if the value is not present in the map </param>
		/// <returns> the value of the requested parameter </returns>
		/// <exception cref="ClassCastException"> if the requested value is not of the expected type </exception>
		public virtual T getOrDefault<T>(TypedKey<T> typedKey, T defaultValue)
		{
			return get(typedKey).orElse(defaultValue);
		}

		public virtual void put(string key, byte value)
		{
			put(new ByteTypeKey(key), value);
		}

		public virtual void put(string key, byte[] value)
		{
			put(new ByteArrayTypeKey(key), value);
		}

		public virtual void put(string key, short value)
		{
			put(new ShortTypeKey(key), value);
		}

		public virtual void put(string key, short[] value)
		{
			put(new ShortArrayTypeKey(key), value);
		}

		public virtual void put(string key, int value)
		{
			put(new IntegerTypeKey(key), value);
		}

		public virtual void put(string key, int[] value)
		{
			put(new IntegerArrayTypeKey(key), value);
		}

		public virtual void put(string key, float value)
		{
			put(new FloatTypeKey(key), value);
		}

		public virtual void put(string key, float[] value)
		{
			put(new FloatArrayTypeKey(key), value);
		}

		public virtual void put(string key, long value)
		{
			put(new LongTypeKey(key), value);
		}

		public virtual void put(string key, long[] value)
		{
			put(new LongArrayTypeKey(key), value);
		}

		public virtual void put(string key, double value)
		{
			put(new DoubleTypeKey(key), value);
		}

		public virtual void put(string key, double[] value)
		{
			put(new DoubleArrayTypeKey(key), value);
		}

		public virtual void put(string key, string value)
		{
			put(new StringTypeKey(key), value);
		}

		public virtual void put(string key, bool value)
		{
			put(new BoolTypeKey(key), value);
		}

		public virtual void put(string key, bool[] value)
		{
			put(new BoolArrayTypeKey(key), value);
		}

		public virtual byte getByte(string key)
		{
			return getOrElseThrow(new ByteTypeKey(key));
		}

		public virtual byte[] getByteArray(string key)
		{
			return getOrElseThrow(new ByteArrayTypeKey(key));
		}

		public virtual short getShort(string key)
		{
			return getOrElseThrow(new ShortTypeKey(key));
		}

		public virtual short[] getShortArray(string key)
		{
			return getOrElseThrow(new ShortArrayTypeKey(key));
		}

		public virtual int getInt(string key)
		{
			return getOrElseThrow(new IntegerTypeKey(key));
		}

		public virtual int[] getIntArray(string key)
		{
			return getOrElseThrow(new IntegerArrayTypeKey(key));
		}

		public virtual float getFloat(string key)
		{
			return getOrElseThrow(new FloatTypeKey(key));
		}

		public virtual float[] getFloatArray(string key)
		{
			return getOrElseThrow(new FloatArrayTypeKey(key));
		}

		public virtual long getLong(string key)
		{
			return getOrElseThrow(new LongTypeKey(key));
		}

		public virtual long[] getLongArray(string key)
		{
			return getOrElseThrow(new LongArrayTypeKey(key));
		}

		public virtual double getDouble(string key)
		{
			return getOrElseThrow(new DoubleTypeKey(key));
		}

		public virtual double[] getDoubleArray(string key)
		{
			return getOrElseThrow(new DoubleArrayTypeKey(key));
		}

		public virtual string getString(string key)
		{
			return getOrElseThrow(new StringTypeKey(key));
		}

		public virtual bool getBoolean(string key)
		{
			return getOrElseThrow(new BoolTypeKey(key));
		}

		public virtual bool[] getBooleanArray(string key)
		{
			return getOrElseThrow(new BoolArrayTypeKey(key));
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

			TraceParameterMap that = (TraceParameterMap)o;
			if (Count != that.Count)
			{
				return false;
			}

			foreach (var (key, item) in this)
			{
				if (!that.ContainsKey(key) || that[key] != item)
					return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

}