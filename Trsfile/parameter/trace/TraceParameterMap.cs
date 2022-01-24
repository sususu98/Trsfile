﻿using com.riscure.trs.types;
using com.riscure.trs.parameter.trace.definition;
using com.riscure.trs.io;

namespace com.riscure.trs.parameter.trace
{
    public class TraceParameterMap : Dictionary<string, TraceParameter>, ICloneable
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
                Add(key, (TraceParameter)value.Clone());
            }
        }

        /// <returns> a new instance of a TraceParameterMap containing all the same values as this one </returns>
        public object Clone()
        {
            return new TraceParameterMap(this);
        }

        public new void Add(string s, TraceParameter t) => base.Add(s, t);

#pragma warning disable CS8601
        public virtual new bool Remove(string s, out TraceParameter t) => base.Remove(s, out t);
#pragma warning restore CS8601

        public virtual new void Clear() => base.Clear();

        /// <returns> a concatenation of all trace parameters in this map, individually converted to byte arrays </returns>
        /// <exception cref="RuntimeException"> if the map failed to serialize correctly </exception>
        public byte[] Serialize()
        {
            MemoryStream baos = new();
            try
            {
                using LittleEndianOutputStream dos = new(baos);
                foreach (TraceParameter parameter in Values)
                {
                    parameter.Serialize(dos);
                }
                dos.Flush();
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
        public static TraceParameterMap Deserialize(byte[] bytes, TraceParameterDefinitionMap definitions)
        {
            TraceParameterMap result = new();
            if (bytes != null)
            {
                if (bytes.Length != definitions.TotalByteSize())
                {
                    throw new ArgumentException(string.Format(DATA_LENGTH_DEFINITIONS_MISMATCH, bytes.Length, definitions.TotalByteSize()));
                }
                try
                {
                    MemoryStream bais = new(bytes);
                    using LittleEndianInputStream dis = new(bais);
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
                throw new ArgumentException(EMPTY_DATA_BUT_NONEMPTY_DEFINITIONS);
            }
            return UnmodifiableTraceParameterMap.Of(result);
        }

        /// <summary>
        /// Add a new parameter to the map </summary>
        /// <param name="typedKey"> the <seealso cref="TypedKey"/> defining the name and the type of the added value </param>
        /// <param name="value"> the value of the parameter to add </param>
        /// @param <T> the type of the parameter </param>
        /// <exception cref="IllegalArgumentException"> if the value is not valid </exception>
        public void Add<T>(TypedKey<T> typedKey, T value)
        {
            Add(typedKey.Key, typedKey.CreateParameter(value));
        }

        /// <summary>
        /// Get a parameter scalarvalue from the map </summary>
        /// <param name="typedKey"> the <seealso cref="TypedKey<typeparamref name="T"/>"/> defining the name and the type of the value to retrieve </param>
        /// @param <T> the type of the parameter </param>
        /// <returns> the value of the requested parameter </returns>
        public T? Get<T>(TypedKey<T> typedKey, out bool isNull, T? defaultValue = default, bool throwIfNull = false)
        {
            TraceParameter parameter = this[typedKey.Key];
            if (parameter is null || parameter is not TraceParameter<T> Tparameter)
            {
                isNull = true;
                if (throwIfNull) throw new ArgumentNullException(
                    nameof(parameter) + " or " + nameof(Tparameter));
                else return defaultValue;
            }
            if (!parameter.IsArray)
            {
                isNull = false;
                return Tparameter.ScalarValue;
            }
            else
            {
                isNull = true;
                if (throwIfNull) throw new ArgumentException($"{nameof(parameter)} is array ! Should use GetArray instead.");
                else return defaultValue;
            }
        }

        /// <summary>
        /// Get a parameter scalarvalue from the map </summary>
        /// <param name="typedKey"> the <seealso cref="TypedKey<typeparamref name="T"/>"/> defining the name and the type of the value to retrieve </param>
        /// @param <T> the type of the parameter </param>
        /// <returns> the value of the requested parameter </returns>
        /// <exception cref="ClassCastException"> if the requested value is not of the expected type </exception>
        public T[]? GetArray<T>(TypedKey<T> typedKey, out bool isNull, T[]? defaultValue = default, bool throwIfNull = false)
        {
            TraceParameter parameter = this[typedKey.Key];
            if (parameter is null || parameter is not TraceParameter<T> Tparameter)
            {
                isNull = true;
                if (throwIfNull) throw new ArgumentNullException(
                    nameof(parameter) + " or " + nameof(Tparameter));
                else return defaultValue;
            }
            if (parameter.IsArray)
            {
                isNull = false;
                return Tparameter.Value;
            }
            else
            {
                isNull = true;
                if (throwIfNull) throw new ArgumentException($"{nameof(parameter)} is array ! Should use GetArray instead.");
                else return defaultValue;
            }
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

        public byte[]? GetByteArray(string key) => Get(new ByteArrayTypeKey(key), out _);


        public short GetShort(string key) => Get(new ShortTypeKey(key), out _);

        public short[]? GetShortArray(string key) => Get(new ShortArrayTypeKey(key), out _);


        public int GetInt(string key) => Get(new IntegerTypeKey(key), out _);

        public int[]? GetIntArray(string key) => Get(new IntegerArrayTypeKey(key), out _);

        public float GetFloat(string key) => Get(new FloatTypeKey(key), out _);

        public float[]? GetFloatArray(string key) => Get(new FloatArrayTypeKey(key), out _);


        public long GetLong(string key) => Get(new LongTypeKey(key), out _);

        public long[]? GetLongArray(string key) => Get(new LongArrayTypeKey(key), out _);

        public double GetDouble(string key) => Get(new DoubleTypeKey(key), out _);

        public double[]? GetDoubleArray(string key) => Get(new DoubleArrayTypeKey(key), out _);


        public string? GetString(string key) => Get(new StringTypeKey(key), out _);


        public bool GetBool(string key) => Get(new BoolTypeKey(key), out _);

        public bool[]? GetBoolArray(string key) => Get(new BoolArrayTypeKey(key), out _);


        public override bool Equals(object? o)
        {
            if (this == o)
            {
                return true;
            }
            if (o == null || GetType() != o.GetType())
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