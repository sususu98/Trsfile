﻿using Trsfile.Types;
using Trsfile.Parameter.Trace.Definition;
using Trsfile.IO;

namespace Trsfile.Parameter.Trace
{
    public class TraceParameterMap : Dictionary<string, TraceParameter>, ICloneable
    {
        private const string KEY_NOT_FOUND = "TraceParameter {0}s was not found in the trace.";
        private const string EMPTY_DATA_BUT_NONEMPTY_DEFINITIONS = "The provided byte array is null or empty, but the provided definitions are not";
        private const string DATA_LENGTH_DEFINITIONS_MISMATCH = "The provided byte array ({0:d} bytes) does not match the total definitions length ({1:d} bytes)";

        public TraceParameterMap() : base()
        {
        }

        public TraceParameterMap(TraceParameterMap toCopy) : this()
        {
            foreach (var (key, value) in toCopy)
            {
                base.Add(key, (TraceParameter)value.Clone());
            }
        }

        /// <returns> a new instance of a TraceParameterMap containing all the same values as this one </returns>
        public virtual object Clone() => new TraceParameterMap(this);

        public virtual new void Add(string s, TraceParameter t)
        {
            this[s] = t;
        }

#pragma warning disable CS8601
        public virtual new bool Remove(string s, out TraceParameter t) => base.Remove(s, out t);
#pragma warning restore CS8601

        public virtual new bool Remove(string s) => base.Remove(s);

        public virtual new void Clear() => base.Clear();
        public virtual new TraceParameter this[string key]
        {
            get => base[key];
            set => base[key] = value;
        }
        /// <returns> a concatenation of all trace parameters in this map, individually converted to byte arrays </returns>
        /// <exception cref="RuntimeException"> if the map failed to serialize correctly </exception>
        public byte[] Serialize()
        {
            try
            {
                MemoryStream baos = new();
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
        /// <param name="typedKey"> the <seealso cref="TypedKey<typeparamref name="T"/>"/> defining the name and the type of the added value </param>
        /// <param name="value"> the value of the parameter to add </param>
        /// <typeparam name="T"> the type of the parameter </typeparam>
        /// <exception cref="IllegalArgumentException"> if the value is not valid </exception>
        public void Add<T>(TypedKey<T> typedKey, T value)
        {
            Add(typedKey.Key, typedKey.CreateParameter(value));
        }

        /// <summary>
        /// Add a new array parameter to the map </summary>
        /// <param name="typedKey"> the <seealso cref="TypedKey<typeparamref name="T"/>"/> defining the name and the type of the added value </param>
        /// <param name="value"> the array value of the parameter to add </param>
        /// <typeparam name="T"> the type of the array's parameter</typeparam>
        /// <exception cref="IllegalArgumentException"> if the value is not valid </exception>
        public void Add<T>(TypedKey<T> typedKey, T[] value)
        {
            Add(typedKey.Key, typedKey.CreateParameter(value));
        }

        /// <summary>
        /// Get a parameter scalarvalue from the map </summary>
        /// <param name="typedKey"> the <seealso cref="TypedKey<typeparamref name="T"/>"/> defining the name and the type of the value to retrieve </param>
        /// <typeparam name="T"> the type of the parameter </typeparam>
        /// <returns> the value of the requested parameter </returns>
        public T? Get<T>(TypedKey<T> typedKey, out bool isNull, T? defaultValue = default, bool throwIfNull = true)
        {
            TraceParameter parameter = this[typedKey.Key];
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
            TraceParameter parameter = this[typedKey.Key];
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

        public double GetDouble(string key) => Get(new DoubleTypeKey(key), out bool isNull);

        public double[]? GetDoubleArray(string key) => GetArray(new DoubleArrayTypeKey(key), out _);


        public string? GetString(string key) => Get(new StringTypeKey(key), out _);


        public bool GetBool(string key) => Get(new BoolTypeKey(key), out _);

        public bool[]? GetBoolArray(string key) => GetArray(new BoolArrayTypeKey(key), out _);


        public override bool Equals(object? o)
        {
            if ((object)this == o) return true;
            if (o is not TraceParameterMap that)
                return false;
            if (Count != that.Count) return false;
            foreach (var (key, item) in this)
            {
                if (!that.ContainsKey(key) || that[key] != item)
                    return false;
            }
            return true;
        }

        public static bool operator ==(TraceParameterMap a, TraceParameterMap b)
            => a.Equals(b);

        public static bool operator !=(TraceParameterMap a, TraceParameterMap b)
            => !a.Equals(b);

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

}