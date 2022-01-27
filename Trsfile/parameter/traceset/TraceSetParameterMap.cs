using Trsfile.IO;
using Trsfile.Types;

namespace Trsfile.Parameter.Traceset
{
    /// <summary>
    /// This class represents the header definitions of all user-added global parameters of the trace set
    /// 
    /// This explicitly implements LinkedHashMap to ensure that the data is retrieved in the same order as it was added
    /// </summary>
    public class TraceSetParameterMap : Dictionary<string, TraceSetParameter>, ICloneable
    {
        private const string KEY_NOT_FOUND = "TraceSetParameter {0} was not found in the trace set.";

        public TraceSetParameterMap() : base()
        {
        }

        public TraceSetParameterMap(TraceSetParameterMap toCopy) : this()
        {
            foreach (var (key, value) in toCopy)
            {
                base.Add(key, (TraceSetParameter)value.Clone());
            }
        }

        public virtual new void Add(string t, TraceSetParameter p) => base.Add(t, p);

        public virtual new bool Remove(string t) => base.Remove(t);

#pragma warning disable CS8601
        public virtual new bool Remove(string s, out TraceSetParameter t) => base.Remove(s, out t);
#pragma warning restore CS8601

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
                throw new IOException(ex.Message);
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
                            string name = TRSMetaDataUtils.ReadName(dis);
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
        /// <typeparam name="T">the type of the parameter</typeparam>
        /// <exception cref="IllegalArgumentException"> if the value is not valid </exception>
        public virtual void Add<T>(TypedKey<T> typedKey, T value)
        {
            Add(typedKey.Key, new TraceSetParameter(typedKey.CreateParameter(value)));
        }

        /// <summary>
        /// Add a new parameter to the map </summary>
        /// <param name="typedKey"> the <seealso cref="TypedKey"/> defining the name and the type of the added value </param>
        /// <param name="value"> the value of the parameter to add </param>
        /// <typeparam name="T">the type of the array's parameter</typeparam>
        /// <exception cref="IllegalArgumentException"> if the value is not valid </exception>
        public virtual void Add<T>(TypedKey<T> typedKey, T[] value)
        {
            Add(typedKey.Key, new TraceSetParameter(typedKey.CreateParameter(value)));
        }

        /// <summary>
        /// Get a parameter scalarvalue from the map </summary>
        /// <param name="typedKey"> the <seealso cref="TypedKey<typeparamref name="T"/>"/> defining the name and the type of the value to retrieve </param>
        /// <typeparam name="T"> the type of the parameter </typeparam>
        /// <returns> the value of the requested parameter </returns>
        public T? Get<T>(TypedKey<T> typedKey, out bool isNull, T? defaultValue = default, bool throwIfNull = true)
        {
            TraceParameter parameter = this[typedKey.Key].Value;
            if (parameter is null || parameter is not TraceParameter<T> Tparameter)
            {
                isNull = true;
                if (throwIfNull) throw new InvalidCastException(
                    nameof(parameter) + " or " + nameof(Tparameter));
                else return defaultValue;
            }
            // suppose TypedKey<T> is Bool, TraceParam<T> is BoolArray, then they should be the same.
            bool typeEqual = typedKey.Cls == parameter.Type.Cls
                || typedKey.Cls == parameter.Type.ArrayCls;
            if (!typeEqual)
            {
                isNull = true;
                if (throwIfNull)
                    throw new InvalidCastException("Types are not the same");
            }
            if (Tparameter.IsArray || typedKey.IsArray) // IsArray means len > 1
            {
                isNull = true;
                if (throwIfNull)
                    throw new InvalidCastException("Both should not be array.");
            }
            if (Tparameter.IsArray != typedKey.IsArray)
            {
                isNull = true;
                if (throwIfNull)
                    throw new InvalidCastException("There's wrong type for array.");
            }
            isNull = false;
            return Tparameter.ScalarValue;
        }

        /// <summary>
        /// Get a parameter scalarvalue from the map </summary>
        /// <param name="typedKey"> the <seealso cref="TypedKey<typeparamref name="T"/>"/> defining the name and the type of the value to retrieve </param>
        /// <typeparam name="T"> the type of the array's parameter </typeparam>
        /// <returns> the value of the requested parameter </returns>
        /// <exception cref="ClassCastException"> if the requested value is not of the expected type </exception>
        public T[]? GetArray<T>(TypedKey<T> typedKey, out bool isNull, T[]? defaultValue = default, bool throwIfNull = true)
        {
            TraceParameter parameter = this[typedKey.Key].Value;
            if (parameter is null || parameter is not TraceParameter<T> Tparameter)
            {
                isNull = true;
                if (throwIfNull) throw new InvalidCastException(
                    nameof(parameter) + " or " + nameof(Tparameter));
                else return defaultValue;
            }
            bool typeEqual = typedKey.Cls == parameter.Type.Cls
                || typedKey.Cls == parameter.Type.ArrayCls;
            if (!typeEqual)
            {
                isNull = true;
                if (throwIfNull)
                    throw new InvalidCastException("Types are not the same");
            }
            if (!typedKey.IsArray && parameter.IsArray)
            {
                isNull = true;
                if (throwIfNull)
                    throw new InvalidCastException("You shouldn't use (Not Array)TypeKey to get array.");
            }
            isNull = false;
            return Tparameter.Value;
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

        public byte GetByte(string key) => Get(new ByteTypeKey(key), out _);

        public byte[]? GetByteArray(string key) => GetArray(new ByteArrayTypeKey(key), out _);


        public short GetShort(string key) => Get(new ShortTypeKey(key), out _);

        public short[]? GetShortArray(string key) => GetArray(new ShortArrayTypeKey(key), out _);


        public int GetInt(string key) => Get(new IntegerTypeKey(key), out _);

        public int[]? GetIntArray(string key) => GetArray(new IntegerArrayTypeKey(key), out _);

        public float GetFloat(string key) => Get(new FloatTypeKey(key), out _);

        public float[]? GetFloatArray(string key) => GetArray(new FloatArrayTypeKey(key), out _);


        public long GetLong(string key) => Get(new LongTypeKey(key), out _);

        public long[]? GetLongArray(string key) => GetArray(new LongArrayTypeKey(key), out _);

        public double GetDouble(string key) => Get(new DoubleTypeKey(key), out _);

        public double[]? GetDoubleArray(string key) => GetArray(new DoubleArrayTypeKey(key), out _);


        public string? GetString(string key) => Get(new StringTypeKey(key), out _);


        public bool GetBool(string key) => Get(new BoolTypeKey(key), out _);

        public bool[]? GetBoolArray(string key) => GetArray(new BoolArrayTypeKey(key), out _);


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