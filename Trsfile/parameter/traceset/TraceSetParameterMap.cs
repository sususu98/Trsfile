using System;
using System.Collections.Generic;
using System.IO;
using com.riscure.trs;
using com.riscure.trs.io;
using com.riscure.trs.types;

namespace com.riscure.trs.parameter.traceset
{
	/// <summary>
	/// This class represents the header definitions of all user-added global parameters of the trace set
	/// 
	/// This explicitly implements LinkedHashMap to ensure that the data is retrieved in the same order as it was added
	/// </summary>
	public class TraceSetParameterMap : Dictionary<string, TraceSetParameter>, ICloneable
	{
		private const string KEY_NOT_FOUND = "TraceSetParameter %s was not found in the trace set.";

		public TraceSetParameterMap() : base()
		{
		}

		public TraceSetParameterMap(TraceSetParameterMap toCopy) : this()
		{
			foreach (var (key, value) in toCopy)
			{
				Add(key, value.copy());
			}
		}

		/// <returns> a new instance of a TraceParameterMap containing all the same values as this one </returns>
		public virtual object Clone() => new TraceSetParameterMap(this);

		/// <returns> this map converted to a byte array, serialized according to the TRS V2 standard definition </returns>
		/// <exception cref="RuntimeException"> if the map failed to serialize correctly </exception>
		public virtual byte[] Serialize()
		{
			MemoryStream baos = new MemoryStream();
			try
			{
                using LittleEndianOutputStream dos = new LittleEndianOutputStream(baos);
                //Write NE
                dos.writeShort(Count);
                foreach (var entry in entrySet())
                {
                    byte[] nameBytes = entry.Key.GetBytes(System.Text.Encoding.UTF8);
                    //Write NL
                    dos.writeShort(nameBytes.Length);
                    //Write N
                    dos.write(nameBytes);
                    //Write value
                    entry.Value.serialize(dos);
                }
                dos.flush();
                return baos.toByteArray();
            }
			catch (IOException ex)
			{
				throw new Exception(ex);
			}
		}

		/// <param name="bytes"> a valid serialized Trace set parameter map </param>
		/// <returns> a new populated Trace set parameter map as represented by the provided byte array </returns>
		/// <exception cref="RuntimeException"> if the provided byte array does not represent a valid parameter map </exception>
		public static TraceSetParameterMap Deserialize(byte[] bytes)
		{
			TraceSetParameterMap result = new TraceSetParameterMap();
			if (bytes != null && bytes.Length > 0)
			{
				try
				{
						using (MemoryStream bais = new MemoryStream(bytes))
						{
						LittleEndianInputStream dis = new LittleEndianInputStream(bais);
						//Read NE
						short numberOfEntries = dis.readShort();
						for (int k = 0; k < numberOfEntries; k++)
						{
							string name = TRSMetaDataUtils.readName(dis);
							//Read value
							result.put(name, TraceSetParameter.deserialize(dis));
						}
						}
				}
				catch (IOException ex)
				{
					throw new Exception(ex);
				}
			}
			return UnmodifiableTraceSetParameterMap.of(result);
		}

		/// <summary>
		/// Add a new parameter to the map </summary>
		/// <param name="typedKey"> the <seealso cref="TypedKey"/> defining the name and the type of the added value </param>
		/// <param name="value"> the value of the parameter to add </param>
		/// @param <T> the type of the parameter </param>
		/// <exception cref="IllegalArgumentException"> if the value is not valid </exception>
		public virtual void put<T>(TypedKey<T> typedKey, T value)
		{
			put(typedKey.Key, new TraceSetParameter(typedKey.CreateParameter(value)));
		}

		/// <summary>
		/// Get a parameter from the map </summary>
		/// <param name="typedKey"> the <seealso cref="TypedKey"/> defining the name and the type of the value to retrieve </param>
		/// @param <T> the type of the parameter </param>
		/// <returns> the value of the requested parameter </returns>
		/// <exception cref="ClassCastException"> if the requested value is not of the expected type </exception>
		public virtual Optional<T> get<T>(TypedKey<T> typedKey)
		{
			TraceSetParameter parameter = get(typedKey.Key);
			if (parameter != null)
			{
				if (parameter.Value.Length() == 1 && !typedKey.Cls.IsArray)
				{
					return typedKey.Cast(parameter.Value.ScalarValue);
				}
				else
				{
					return typedKey.Cast(parameter.Value.Value);
				}
			}
			return null;
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

		public virtual void put(string key, sbyte value)
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
			put(new BooleanTypeKey(key), value);
		}

		public virtual void put(string key, bool[] value)
		{
			put(new BoolArrayTypeKey(key), value);
		}

		public virtual sbyte getByte(string key)
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
			return getOrElseThrow(new BooleanTypeKey(key));
		}

		public virtual bool[] getBooleanArray(string key)
		{
			return getOrElseThrow(new BoolArrayTypeKey(key));
		}

		public override bool Equals(object? obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}

			TraceSetParameterMap that = (TraceSetParameterMap)obj;
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