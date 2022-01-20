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
                Add(key, (TraceSetParameter)value.Clone());
            }
        }

        public virtual new void Add(string t, TraceSetParameter p) => base.Add(t, p);

        public virtual new void Remove(string t) => base.Remove(t);

        public virtual new void Clear() => base.Clear();

        /// <returns> a new instance of a TraceParameterMap containing all the same values as this one </returns>
        public virtual object Clone() => new TraceSetParameterMap(this);

        /// <returns> this map converted to a byte array, serialized according to the TRS V2 standard definition </returns>
        /// <exception cref="RuntimeException"> if the map failed to serialize correctly </exception>
        public virtual byte[] Serialize()
        {
            MemoryStream baos = new();
            try
            {
                using LittleEndianOutputStream dos = new(baos);
                //Write NE
                dos.WriteShort(Count);
                foreach (var entry in this)
                {
                    byte[] nameBytes = entry.Key.GetBytes(System.Text.Encoding.UTF8);
                    //Write NL
                    dos.WriteShort(nameBytes.Length);
                    //Write N
                    dos.Write(nameBytes);
                    //Write value
                    entry.Value.Serialize(dos);
                }
                dos.Flush();
                return baos.ToArray();
            }
            catch (IOException ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <param name="bytes"> a valid serialized Trace set parameter map </param>
        /// <returns> a new populated Trace set parameter map as represented by the provided byte array </returns>
        /// <exception cref="RuntimeException"> if the provided byte array does not represent a valid parameter map </exception>
        public static TraceSetParameterMap Deserialize(byte[] bytes)
        {
            TraceSetParameterMap result = new();
            if (bytes != null && bytes.Length > 0)
            {
                try
                {
                    using (MemoryStream bais = new(bytes))
                    {
                        LittleEndianInputStream dis = new(bais);
                        //Read NE
                        short numberOfEntries = dis.ReadShort();
                        for (int k = 0; k < numberOfEntries; k++)
                        {
                            string name = TRSMetaDataUtils.readName(dis);
                            //Read value
                            result.Add(name, TraceSetParameter.Deserialize(dis));
                        }
                    }
                }
                catch (IOException ex)
                {
                    throw new Exception(ex.Message, ex);
                }
            }
            return UnmodifiableTraceSetParameterMap.Of(result);
        }

        /// <summary>
        /// Add a new parameter to the map </summary>
        /// <param name="typedKey"> the <seealso cref="TypedKey"/> defining the name and the type of the added value </param>
        /// <param name="value"> the value of the parameter to add </param>
        /// @param <T> the type of the parameter </param>
        /// <exception cref="IllegalArgumentException"> if the value is not valid </exception>
        public virtual void Add<T>(TypedKey<T> typedKey, T value)
        {
            Add(typedKey.Key, new TraceSetParameter(typedKey.CreateParameter(value)));
        }

        /// <summary>
        /// Get a parameter from the map </summary>
        /// <param name="typedKey"> the <seealso cref="TypedKey"/> defining the name and the type of the value to retrieve </param>
        /// @param <T> the type of the parameter </param>
        /// <returns> the value of the requested parameter </returns>
        /// <exception cref="ClassCastException"> if the requested value is not of the expected type </exception>
        public T? Get<T>(TypedKey<T> typedKey)
        {
            TraceSetParameter parameter = this[typedKey.Key];
            if (parameter != null)
            {
                if (parameter.Value.Length == 1 && !typedKey.Cls.IsArray)
                {
                    return typedKey.Cast(parameter.Value.ScalarValue);
                }
                else
                {
                    return typedKey.Cast(parameter.Value.Value);
                }
            }
            return default;
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
            return Get(typedKey).orElseThrow(() => new NoSuchElementException(String.format(KEY_NOT_FOUND, typedKey.Key)));
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
            return Get(typedKey).orElse(defaultValue);
        }

        public void Add(string key, byte value) => Add(new ByteTypeKey(key), value);

        public void Add(string key, byte[] value) => Add(new ByteArrayTypeKey(key), value);

        public void Add(string key, short value) => Add(new ShortTypeKey(key), value);

        public void Add(string key, short[] value) => Add(new ShortArrayTypeKey(key), value);

        public void Add(string key, int value) => Add(new IntegerTypeKey(key), value);

        public void Add(string key, int[] value) => Add(new IntegerArrayTypeKey(key), value);

        public void Add(string key, float value) => Add(new FloatTypeKey(key), value);

        public void Add(string key, float[] value) => Add(new FloatArrayTypeKey(key), value);

        public void Add(string key, long value) => Add(new LongTypeKey(key), value);

        public void Add(string key, long[] value) => Add(new LongArrayTypeKey(key), value);

        public void Add(string key, double value) => Add(new DoubleTypeKey(key), value);

        public void Add(string key, double[] value) => Add(new DoubleArrayTypeKey(key), value);

        public void Add(string key, string value) => Add(new StringTypeKey(key), value);

        public void Add(string key, bool value) => Add(new BoolTypeKey(key), value);

        public void Add(string key, bool[] value) => Add(new BoolArrayTypeKey(key), value);

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