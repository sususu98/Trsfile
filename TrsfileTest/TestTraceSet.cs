using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using TRSFormatException = com.riscure.trs.TRSFormatException;
using TRSMetaData = com.riscure.trs.TRSMetaData;
using Trace = com.riscure.trs.Trace;
using TraceSet = com.riscure.trs.TraceSet;
using Encoding = com.riscure.trs.enums.Encoding;
using ParameterType = com.riscure.trs.enums.ParameterType;
using TRSTag = com.riscure.trs.enums.TRSTag;
using LittleEndianInputStream = com.riscure.trs.io.LittleEndianInputStream;
using TraceParameter = com.riscure.trs.parameter.TraceParameter;
using ByteArrayParameter = com.riscure.trs.parameter.primitive.ByteArrayParameter;
using TraceParameterMap = com.riscure.trs.parameter.trace.TraceParameterMap;
using com.riscure.trs.parameter.trace.definition;
using TraceParameterDefinitionMap = com.riscure.trs.parameter.trace.definition.TraceParameterDefinitionMap;
using TraceSetParameter = com.riscure.trs.parameter.traceset.TraceSetParameter;
using TraceSetParameterMap = com.riscure.trs.parameter.traceset.TraceSetParameterMap;
using com.riscure.trs.types;


//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//import static org.junit.Assert.*;

public class TestTraceSet
{
	private static string tempDir = string.Empty;
	private const string BYTES_TRS = "bytes.trs";
	private const string SHORTS_TRS = "shorts.trs";
	private const string INTS_TRS = "ints.trs";
	private const string FLOATS_TRS = "floats.trs";
	private const string TRS = ".trs";
	private const int NUMBER_OF_TRACES = 1024;
	private static readonly float[] BYTE_SAMPLES = new float[]{1, 2, 3, 4, 5};
	private static readonly float[] SHORT_SAMPLES = new float[]{1, 2, 3, 4, byte.MaxValue + 1};
	private static readonly float[] INT_SAMPLES = new float[]{1, 2, 3, 4, short.MaxValue + 1};
	private static readonly float[] FLOAT_SAMPLES = new float[]{1, 2, 3, 4, 5.1f};
	private const string TVLA_STRING_VALUE = "Trace set contains the following TVLA sets: Random, R5S-Box_Out\n"
		+ "AES-128 ENCRYPT (Input -> Output) Round 5 S-Box Out:HW(3~7)";

	public static bool AssertArrayEquals(float[] a1, float[] a2, float precision)
    {
		if (a1.Length != a2.Length) Assert.Fail("Array Not Equal");
		for (int i=0;i<a1.Length; i++)
        {
			if (Math.Abs(a1[i] - a2[i]) > precision) Assert.Fail("Array Not Equal");
        }
		return true;
    }

	public static bool AssertArrayEquals(double[] a1, double[] a2, float precision)
	{
		if (a1.Length != a2.Length) Assert.Fail("Array Not Equal");
		for (int i = 0; i < a1.Length; i++)
		{
			if (Math.Abs(a1[i] - a2[i]) > precision) Assert.Fail("Array Not Equal");
		}
		return true;
	}

	//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
	//ORIGINAL LINE: @BeforeClass public static void createTempDir() throws IOException, com.riscure.trs.TRSFormatException
	//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
	public static void createTempDir()
	{
		tempDir = Path.Combine(Path.GetTempPath(), "TestTraceSet");

		using (TraceSet writable = TraceSet.Create(tempDir + Path.PathSeparator + BYTES_TRS))
		{
			for (int k = 0; k < NUMBER_OF_TRACES; k++)
			{
				writable.add(Trace.Create(BYTE_SAMPLES));
			}
		}

		using (TraceSet writable = TraceSet.Create(tempDir + Path.PathSeparator + SHORTS_TRS))
		{
			for (int k = 0; k < NUMBER_OF_TRACES; k++)
			{
				writable.add(Trace.Create(SHORT_SAMPLES));
			}
		}

		using (TraceSet writable = TraceSet.Create(tempDir + Path.PathSeparator + INTS_TRS))
		{
			for (int k = 0; k < NUMBER_OF_TRACES; k++)
			{
				writable.add(Trace.Create(INT_SAMPLES));
			}
		}

		using (TraceSet writable = TraceSet.Create(tempDir + Path.PathSeparator + FLOATS_TRS))
		{
			for (int k = 0; k < NUMBER_OF_TRACES; k++)
			{
				writable.add(Trace.Create(FLOAT_SAMPLES));
			}
		}
	}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @AfterClass public static void cleanup() throws InterruptedException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
	public static void cleanup()
	{
		//We need to allow a little time for java to release all handles
		GC.Collect();
		Thread.Sleep(100);
		foreach (var file in Directory.GetFiles(tempDir))
		{
			try
			{
				File.Delete(file);
			}
			catch (IOException e)
			{
				Console.Error.WriteLine($"Failed to delete temporary file {file}");
				Console.WriteLine(e.ToString());
				Console.Write(e.StackTrace);
			}
		}
		try
		{
			Directory.Delete(tempDir);
		}
		catch (IOException e)
		{
			Console.Error.WriteLine($"Failed to delete temporary folder {tempDir}");
			Console.WriteLine(e.ToString());
			Console.Write(e.StackTrace);
		}
	}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testOpenBytes() throws IOException, com.riscure.trs.TRSFormatException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
	public virtual void testOpenBytes()
	{
        using TraceSet readable = TraceSet.Open(tempDir + Path.PathSeparator + BYTES_TRS);
        int numberOfTracesRead = readable.MetaData.GetInt(TRSTag.NUMBER_OF_TRACES);
        Encoding encoding = Encoding.fromValue(readable.MetaData.GetInt(TRSTag.SAMPLE_CODING));
        Assert.Equals(Encoding.BYTE, encoding);
        Assert.Equals(NUMBER_OF_TRACES, numberOfTracesRead);
        for (int k = 0; k < NUMBER_OF_TRACES; k++)
        {
            Trace t = readable.get(k);
            Assert.Equals(Encoding.BYTE.Value, t.PreferredCoding);
            AssertArrayEquals(BYTE_SAMPLES, readable.get(k).Sample, 0.01f);
        }
    }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testOpenShorts() throws IOException, com.riscure.trs.TRSFormatException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
	public virtual void testOpenShorts()
	{
		using (TraceSet readable = TraceSet.Open(tempDir + Path.PathSeparator + SHORTS_TRS))
		{
			int numberOfTracesRead = readable.MetaData.GetInt(TRSTag.NUMBER_OF_TRACES);
			Encoding encoding = Encoding.fromValue(readable.MetaData.GetInt(TRSTag.SAMPLE_CODING));
			Assert.Equals(Encoding.SHORT, encoding);
			Assert.Equals(NUMBER_OF_TRACES, numberOfTracesRead);
			for (int k = 0; k < NUMBER_OF_TRACES; k++)
			{
				Trace t = readable.get(k);
				Assert.Equals(Encoding.SHORT.Value, t.PreferredCoding);
				AssertArrayEquals(SHORT_SAMPLES, readable.get(k).Sample, 0.01f);
			}
		}
	}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testOpenInts() throws IOException, com.riscure.trs.TRSFormatException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
	public virtual void testOpenInts()
	{
		using (TraceSet readable = TraceSet.Open(tempDir + Path.PathSeparator + INTS_TRS))
		{
			int numberOfTracesRead = readable.MetaData.GetInt(TRSTag.NUMBER_OF_TRACES);
			Encoding encoding = Encoding.fromValue(readable.MetaData.GetInt(TRSTag.SAMPLE_CODING));
			Assert.Equals(Encoding.INT, encoding);
			Assert.Equals(NUMBER_OF_TRACES, numberOfTracesRead);
			for (int k = 0; k < NUMBER_OF_TRACES; k++)
			{
				Trace t = readable.get(k);
				Assert.Equals(Encoding.INT.Value, t.PreferredCoding);
				AssertArrayEquals(INT_SAMPLES, readable.get(k).Sample, 0.01f);
			}
		}
	}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testOpenFloats() throws IOException, com.riscure.trs.TRSFormatException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
	public virtual void testOpenFloats()
	{
        using TraceSet readable = TraceSet.Open(tempDir + Path.PathSeparator + FLOATS_TRS);
        int numberOfTracesRead = readable.MetaData.GetInt(TRSTag.NUMBER_OF_TRACES);
        Encoding encoding = Encoding.fromValue(readable.MetaData.GetInt(TRSTag.SAMPLE_CODING));
        Assert.Equals(Encoding.FLOAT, encoding);
        Assert.Equals(NUMBER_OF_TRACES, numberOfTracesRead);
        for (int k = 0; k < NUMBER_OF_TRACES; k++)
        {
            Trace t = readable.get(k);
            Assert.Equals(Encoding.FLOAT.Value, t.PreferredCoding);
            AssertArrayEquals(FLOAT_SAMPLES, readable.get(k).Sample, 0.01f);
        }
    }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testUTF8Title() throws IOException, com.riscure.trs.TRSFormatException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
	public virtual void testUTF8Title()
	{
		string title = "씨브 크레그스만";
		string name = Guid.NewGuid().ToString() + TRS;
		try
		{
            using TraceSet ts = TraceSet.Create(tempDir + Path.PathSeparator + name);
            ts.add(Trace.create(title, new float[0], new TraceParameterMap()));
        }
		catch (TRSFormatException e)
		{
			throw e;
		}
        using TraceSet readable = TraceSet.Open(tempDir + Path.PathSeparator + name);
        Assert.Equals(title, readable.get(0).Title);
    }

	/// <summary>
	/// This tests adding several different types of information to the trace set header. The three parameters are chosen
	/// to match the three major cases: Strings, primitives, and arbitrary (serializable) objects.
	/// </summary>
	/// <exception cref="IOException"> </exception>
	/// <exception cref="TRSFormatException"> </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testWriteTraceSetParameters() throws IOException, com.riscure.trs.TRSFormatException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
	public virtual void testWriteTraceSetParameters()
	{
		TRSMetaData metaData = TRSMetaData.create();
		TraceSetParameterMap parameters = new TraceSetParameterMap();
		parameters.put("BYTE", (byte) 1);
        parameters.put("SHORT", (short)2);
        parameters.put("INT", 3);
        parameters.put("FLOAT", (float)4);
        parameters.put("LONG", (long)5);
        parameters.put("DOUBLE", (double)6);
        parameters.put("STRING", string.Format("{0,3:D}", 7));
        parameters.put("BOOLEAN", true);
        parameters.Add("BYTEARRAY", new byte[] { 8, 9, 0 });
        parameters.put("SHORTARRAY", new short[] { 1, 2, 3 });
        parameters.put("INTARRAY", new int[] { 4, 5, 6 });
        parameters.put("FLOATARRAY", new float[] { 7, 8, 9 });
        parameters.put("LONGARRAY", new long[] { 0, 1, 2 });
        parameters.put("DOUBLEARRAY", new double[] { 3, 4, 5 });
        parameters.put("BOOLEANARRAY", new bool[] { true, false, true, false, true, true });
        parameters.put("TVLA", TVLA_STRING_VALUE);
        //parameters.put("XYZ offset", XYZ_TEST_VALUE);
        metaData.put(TRSTag.TRACE_SET_PARAMETERS, parameters);
		//CREATE TRACE
		string name = Guid.NewGuid().ToString() + TRS;
		TraceSet.Create(tempDir + Path.PathSeparator + name, metaData).close();
		//READ BACK AND CHECK RESULT
		using (TraceSet readable = TraceSet.Open(tempDir + Path.PathSeparator + name))
		{
			TraceSetParameterMap readTraceSetParameterMap = readable.MetaData.TraceSetParameters;
			parameters.forEach((s, traceSetParameter) => Assert.Equals(traceSetParameter, readTraceSetParameterMap.get(s)));
		}
	}

	/// <summary>
	/// This tests adding a parameter with a name of 100000 characters
	/// </summary>
	/// <exception cref="IOException"> </exception>
	/// <exception cref="TRSFormatException"> </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test(expected = com.riscure.trs.TRSFormatException.class) public void testWriteTraceParametersInvalidName() throws IOException, com.riscure.trs.TRSFormatException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
	public virtual void testWriteTraceParametersInvalidName()
	{
		TRSMetaData metaData = TRSMetaData.create();
		string parameterName = string.Format("{0,100000}", "XYZ");
		//CREATE TRACE
		string name = Guid.NewGuid().ToString() + TRS;
		using (TraceSet traceWithParameters = TraceSet.Create(tempDir + Path.PathSeparator + name, metaData))
		{
			TraceParameterMap parameters = new TraceParameterMap();
			parameters.Add(parameterName, 1);
			traceWithParameters.add(Trace.create("", FLOAT_SAMPLES, parameters));
		}
		//READ BACK AND CHECK RESULT
		using (TraceSet readable = TraceSet.Open(tempDir + Path.PathSeparator + name))
		{
			TraceParameterDefinitionMap parameterDefinitions = readable.MetaData.TraceParameterDefinitions;
			parameterDefinitions.forEach((key, parameter) => Assert.Equals(parameterName, key));
		}
	}

	/// <summary>
	/// This tests whether any strings added to the trace are handled as specified:
	/// - if no length is specified, the first string is leading
	/// - if a string is longer than the length specified, it should be truncated
	/// - when truncated, a string should still be valid UTF-8 (truncated at character level, not byte level)
	/// </summary>
	/// <exception cref="IOException"> </exception>
	/// <exception cref="TRSFormatException"> </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testWriteTraceParametersVaryingStringLength() throws IOException, com.riscure.trs.TRSFormatException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
	public virtual void testWriteTraceParametersVaryingStringLength()
	{
		TRSMetaData metaData = TRSMetaData.create();
		IList<TraceParameterMap> testParameters = new List<TraceParameterMap>();
		IList<string> strings = new List<string>();
		strings.Add("abcd");
		strings.Add("abcdefgh");
		strings.Add("ab");
		strings.Add("abcdefgh汉字");
		//CREATE TRACE
		string name = Guid.NewGuid().ToString() + TRS;
		using (TraceSet traceWithParameters = TraceSet.Create(tempDir + Path.PathSeparator + name, metaData))
		{
			for (int k = 0; k < 25; k++)
			{
				TraceParameterMap parameters = new TraceParameterMap();
				parameters.Add("BYTEARRAY", new byte[]{(byte) k, (byte) k, (byte) k});
				parameters.Add(TraceParameter.SAMPLES, new float[]{(float) k, (float) k, (float) k});
				parameters.Add(TraceParameter.TITLE, strings[k % strings.Count]);
				traceWithParameters.add(Trace.create(strings[k % strings.Count], FLOAT_SAMPLES, parameters));
				testParameters.Add(parameters);
			}
		}
		//READ BACK AND CHECK RESULT
		readBackGeneric(testParameters, name);
	}

	/// <summary>
	/// This tests whether all getters are working as expected
	/// </summary>
	/// <exception cref="IOException"> </exception>
	/// <exception cref="TRSFormatException"> </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testReadTraceParametersTyped() throws IOException, com.riscure.trs.TRSFormatException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
	public virtual void testReadTraceParametersTyped()
	{
		TRSMetaData metaData = TRSMetaData.create();
		IList<TraceParameterMap> testParameters = new List<TraceParameterMap>();
		//CREATE TRACE
		string name = Guid.NewGuid().ToString() + TRS;
		using (TraceSet traceWithParameters = TraceSet.Create(tempDir + Path.PathSeparator + name, metaData))
		{
			for (int k = 0; k < 25; k++)
			{
				TraceParameterMap parameters = new TraceParameterMap();
				parameters.Add("BYTE", (byte) k);
				parameters.Add("SHORT", (short) k);
				parameters.Add("INT", k);
				parameters.Add("FLOAT", (float) k);
				parameters.Add("LONG", (long) k);
				parameters.Add("DOUBLE", (double) k);
				parameters.Add("STRING", string.Format("{0,3:D}", k));
				parameters.Add("BOOLEAN", true);
				parameters.Add("BYTEARRAY", new byte[]{(byte) k, (byte) k, (byte) k});
				parameters.Add("SHORTARRAY", new short[]{(short) k, (short) k, (short) k});
				parameters.Add("INTARRAY", new int[]{k, k, k});
				parameters.Add("FLOATARRAY", new float[]{(float) k, (float) k, (float) k});
				parameters.Add("LONGARRAY", new long[]{k, k, k});
				parameters.Add("DOUBLEARRAY", new double[]{k, k, k});
				parameters.Add("BOOLEANARRAY", new bool[]{true, false, true, false, true, true});
				traceWithParameters.add(Trace.create("", FLOAT_SAMPLES, parameters));
				testParameters.Add(parameters);
			}
		}
		readBackGeneric(testParameters, name);
		readBackTyped(testParameters, name);
		readBackTypedKeys(testParameters, name);
	}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void readBackGeneric(List<com.riscure.trs.parameter.trace.TraceParameterMap> testParameters, String name) throws IOException, com.riscure.trs.TRSFormatException
	private void readBackGeneric(IList<TraceParameterMap> testParameters, string name)
	{
		using (TraceSet readable = TraceSet.Open(tempDir + Path.PathSeparator + name))
		{
			TraceParameterDefinitionMap parameterDefinitions = readable.MetaData.TraceParameterDefinitions;
			for (int k = 0; k < 25; k++)
			{
				Assert.Equals(parameterDefinitions.size(), testParameters[k].size());
				Trace trace = readable.get(k);
				TraceParameterMap correctValue = testParameters[k];
				parameterDefinitions.ForEach((key, parameter) =>
				{
					TraceParameter traceParameter = trace.Parameters.get(key);
					Assert.Equals(traceParameter, correctValue.get(key));
				});
			}
		}
	}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void readBackTyped(List<com.riscure.trs.parameter.trace.TraceParameterMap> testParameters, String name) throws IOException, com.riscure.trs.TRSFormatException
	private void readBackTyped(IList<TraceParameterMap> testParameters, string name)
	{
		using (TraceSet readable = TraceSet.Open(tempDir + Path.PathSeparator + name))
		{
			TraceParameterDefinitionMap parameterDefinitions = readable.MetaData.TraceParameterDefinitions;
			for (int k = 0; k < 25; k++)
			{
				Assert.Equals(parameterDefinitions.size(), testParameters[k].size());
				Trace trace = readable.get(k);
				TraceParameterMap correctValue = testParameters[k];
				parameterDefinitions.forEach((key, parameter) => { parameter.getType() switch
				{
						BYTE =>
							if (parameter.getLength() == 1)
							{
								Assert.Equals(correctValue.getByte(key), trace.Parameters.getByte(key));
							}
							else
							{
								AssertArrayEquals(correctValue.getByteArray(key), trace.Parameters.getByteArray(key));
							}
							break;
						SHORT =>
							if (parameter.getLength() == 1)
							{
								Assert.Equals(correctValue.getShort(key), trace.Parameters.getShort(key));
							}
							else
							{
								AssertArrayEquals(correctValue.getShortArray(key), trace.Parameters.getShortArray(key));
							}
							break;
						INT =>
							if (parameter.getLength() == 1)
							{
								Assert.Equals(correctValue.getInt(key), trace.Parameters.getInt(key));
							}
							else
							{
								AssertArrayEquals(correctValue.getIntArray(key), trace.Parameters.getIntArray(key));
							}
							break;
						FLOAT =>
							if (parameter.getLength() == 1)
							{
								Assert.Equals(correctValue.getFloat(key), trace.Parameters.getFloat(key), 0.01f);
							}
							else
							{
								AssertArrayEquals(correctValue.getFloatArray(key), trace.Parameters.getFloatArray(key), 0.01f);
							}
							break;
						LONG =>
							if (parameter.getLength() == 1)
							{
								Assert.Equals(correctValue.getLong(key), trace.Parameters.getLong(key));
							}
							else
							{
								AssertArrayEquals(correctValue.getLongArray(key), trace.Parameters.getLongArray(key));
							}
							break;
						DOUBLE =>
							if (parameter.getLength() == 1)
							{
								Assert.Equals(correctValue.getDouble(key), trace.Parameters.getDouble(key), 0.01);
							}
							else
							{
								AssertArrayEquals(correctValue.getDoubleArray(key), trace.Parameters.getDoubleArray(key), 0.01);
							}
							break;
						STRING =>
							Assert.Equals(correctValue.getString(key), trace.Parameters.getString(key));
							break;
						BOOL =>
							if (parameter.getLength() == 1)
							{
								Assert.Equals(correctValue.getBoolean(key), trace.Parameters.getBoolean(key));
							}
							else
							{
								AssertArrayEquals(correctValue.getBooleanArray(key), trace.Parameters.getBooleanArray(key));
							}
							break;
						_ =>
							throw new Exception("Unexpected type: " + parameter.getType());
				}
			}
				);
		}
	}
}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void readBackTypedKeys(List<com.riscure.trs.parameter.trace.TraceParameterMap> testParameters, String name) throws IOException, com.riscure.trs.TRSFormatException
	private void readBackTypedKeys(IList<TraceParameterMap> testParameters, string name)
	{
		using (TraceSet readable = TraceSet.open(tempDir + Path.PathSeparator + name))
		{
			TraceParameterDefinitionMap parameterDefinitions = readable.MetaData.TraceParameterDefinitions;
			for (int k = 0; k < 25; k++)
			{
				Assert.Equals(parameterDefinitions.size(), testParameters[k].size());
				Trace trace = readable.get(k);
				TraceParameterMap correctValue = testParameters[k];
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: parameterDefinitions.forEach((key, parameter) -> { TypedKey<?> typedKey; switch(parameter.getType())
				parameterDefinitions.forEach((key, parameter) => { TypedKey<object> typedKey; parameter.getType() switch
				{
						BYTE =>
							typedKey = parameter.getLength() > 1 ? new ByteArrayTypeKey(key) : new ByteTypeKey(key);
							break;
						SHORT =>
							typedKey = parameter.getLength() > 1 ? new ShortArrayTypeKey(key) : new ShortTypeKey(key);
							break;
						INT =>
							typedKey = parameter.getLength() > 1 ? new IntegerArrayTypeKey(key) : new IntegerTypeKey(key);
							break;
						FLOAT =>
							typedKey = parameter.getLength() > 1 ? new FloatArrayTypeKey(key) : new FloatTypeKey(key);
							break;
						LONG =>
							typedKey = parameter.getLength() > 1 ? new LongArrayTypeKey(key) : new LongTypeKey(key);
							break;
						DOUBLE =>
							typedKey = parameter.getLength() > 1 ? new DoubleArrayTypeKey(key) : new DoubleTypeKey(key);
							break;
						STRING =>
							typedKey = new StringTypeKey(key);
							break;
						BOOL =>
							typedKey = parameter.getLength() > 1 ? new BooleanArrayTypeKey(key) : new BooleanTypeKey(key);
							break;
						_ =>
							throw new Exception("Unexpected type: " + parameter.getType());
				}
					if (parameter.getLength() > 1 && typedKey.getCls().isArray())
					{
						AssertArrayEquals(java.util.Arrays.asList(correctValue.getOrElseThrow(typedKey)).toArray(), java.util.Arrays.asList(trace.Parameters.getOrElseThrow(typedKey)).toArray());
					}
					else
					{
						Assert.Equals(correctValue.get(typedKey), trace.Parameters.get(typedKey));
					}
			}
				);
		}
	}
	}

	/// <summary>
	/// This tests getting a value of the wrong type correctly throws an exception
	/// </summary>
	/// <exception cref="IOException"> </exception>
	/// <exception cref="TRSFormatException"> </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testExceptionWrongType() throws IOException, com.riscure.trs.TRSFormatException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
	public virtual void testExceptionWrongType()
	{
		TRSMetaData metaData = TRSMetaData.create();
		//CREATE TRACE
		string name = Guid.NewGuid().ToString() + TRS;
		using (TraceSet traceWithParameters = TraceSet.create(tempDir + Path.PathSeparator + name, metaData))
		{
			TraceParameterMap parameters = new TraceParameterMap();
			parameters.Add("BYTE", (byte) 1);
			traceWithParameters.add(Trace.create("", FLOAT_SAMPLES, parameters));
		}
		//READ BACK AND CHECK RESULT
		using (TraceSet readable = TraceSet.open(tempDir + Path.PathSeparator + name))
		{
			assertThrows(typeof(System.InvalidCastException), () => readable.get(0).getParameters().getDouble("BYTE"));
		}
	}

	/// <summary>
	/// This </summary>
	/// <exception cref="IOException"> </exception>
	/// <exception cref="TRSFormatException"> </exception>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testContainsNonArray() throws IOException, com.riscure.trs.TRSFormatException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
	public virtual void testContainsNonArray()
	{
		ByteTypeKey byteKey = new ByteTypeKey("BYTE");
		string name = Guid.NewGuid().ToString() + TRS;
		using (TraceSet traceWithParameters = TraceSet.create(tempDir + Path.PathSeparator + name))
		{
			TraceParameterMap parameters = new TraceParameterMap();
			parameters.put(byteKey, (byte) 1);
			traceWithParameters.add(Trace.create("", FLOAT_SAMPLES, parameters));
		}
		//READ BACK AND CHECK RESULT
		using (TraceSet readable = TraceSet.open(tempDir + Path.PathSeparator + name))
		{
			assertTrue(readable.get(0).getParameters().get(byteKey).isPresent());
		}
	}

	/// <summary>
	/// This test checks whether all deserialize methods throw an exception if the inputstream does not contain enough data
	/// Introduced to test #11: TraceParameter.deserialize does not check the actual returned length
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testInvalidParameterLength()
	public virtual void testInvalidParameterLength()
	{
		int errors = 0;
		byte[] empty = new byte[1];
		foreach (ParameterType type in ParameterType.Values())
		{
			try
			{
				using (LittleEndianInputStream dis = new LittleEndianInputStream(new MemoryStream(empty)))
				{
					TraceParameter.Deserialize(type, (short) 4, dis);
				}
			}
			catch (IOException)
			{
				errors++;
			}
		}
		Assert.Equals(ParameterType.Values().Length, errors);
	}

	/// <summary>
	/// This test was added to test #26: Trace(Set)ParameterMap is modifiable in read only mode
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testModificationAfterReadback() throws IOException, com.riscure.trs.TRSFormatException
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
	public virtual void testModificationAfterReadback()
	{
		using (TraceSet readable = TraceSet.open(tempDir + Path.PathSeparator + BYTES_TRS))
		{
			assertThrows(typeof(System.NotSupportedException), () => readable.MetaData.TraceSetParameters.put("SHOULD_FAIL", 0));
			assertThrows(typeof(System.NotSupportedException), () => readable.MetaData.TraceParameterDefinitions.put("SHOULD_FAIL", new TraceParameterDefinition<TraceParameter>(ParameterType.BYTE, (short)1, (short)1)));
			for (int k = 0; k < NUMBER_OF_TRACES; k++)
			{
				Trace t = readable.get(k);
				assertThrows(typeof(System.NotSupportedException), () => t.Parameters.Add("SHOULD_FAIL", 0));
			}
		}
	}

	/// <summary>
	/// This test checks whether an empty array is serialized and deserialized correctly
	/// Expected: an exception is thrown when adding an empty parameter
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyArrayParameter()
	public virtual void testEmptyArrayParameter()
	{
		assertThrows(typeof(System.ArgumentException), () => (new TraceParameterMap()).Add("EMPTY", new byte[0]));
	}

	/// <summary>
	/// This test checks whether a Trace(Set)ParameterMap correctly works with typed keys
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testContainsTypedKey()
	public virtual void testContainsTypedKey()
	{
		TraceParameterMap tpm = new TraceParameterMap();
		TraceSetParameterMap tspm = new TraceSetParameterMap();
		string rawKey = "BYTE";
		ByteTypeKey typedKey = new ByteTypeKey(rawKey);
		tpm.put(typedKey, (byte)1);
		tspm.Add(typedKey, (byte)2);

		assertTrue(tpm.Get(typedKey).isPresent());
		assertTrue(tspm.Get(typedKey).isPresent());
	}

	/// <summary>
	/// This test checks whether you can get an array type of a simple value
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGetArrayOfLengthOne()
	public virtual void testGetArrayOfLengthOne()
	{
		TraceParameterMap tpm = new TraceParameterMap();
		TraceSetParameterMap tspm = new TraceSetParameterMap();

		byte rawValue = 1;
		string rawKey = "BYTE";
		ByteTypeKey typedKey = new ByteTypeKey(rawKey);
		tpm.put(typedKey, rawValue);
		tspm.Add(typedKey, rawValue);

		ByteArrayTypeKey arrayTypeKey = new ByteArrayTypeKey(rawKey);
		assertTrue(tpm.get(arrayTypeKey).isPresent());
		assertTrue(tspm.Get(arrayTypeKey).isPresent());
		AssertArrayEquals(new byte[]{rawValue}, tpm.getByteArray(rawKey));
		AssertArrayEquals(new byte[]{rawValue}, tspm.getByteArray(rawKey));
	}

	/// <summary>
	/// This test checks whether a Trace(Set)ParameterMap correctly fails with a typed key of the incorrect type
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testContainsWrongTypedKey()
	public virtual void testContainsWrongTypedKey()
	{
		TraceParameterMap tpm = new TraceParameterMap();
		TraceSetParameterMap tspm = new TraceSetParameterMap();
		string rawKey = "BYTE";
		ByteTypeKey typedKey = new ByteTypeKey(rawKey);
		tpm.Add(rawKey, new byte[]{1, 2}); //actually a byte array
		tspm.put(rawKey, 2); //actually an int

		assertThrows(typeof(System.InvalidCastException), () => tpm.Get(typedKey));
		assertThrows(typeof(System.InvalidCastException), () => tspm.Get(typedKey));
	}

	/// <summary>
	/// This test ensure that the underlying data of a copied map cannot change underlying map itself
	/// </summary>
//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testCopyConstructor()
	public virtual void testCopyConstructor()
	{
		byte[] ba = new byte[] {1, 2, 3, 4, 5};
		ByteArrayParameter bap = new ByteArrayParameter(ba);
		TraceParameterMap tpm = new TraceParameterMap();
		tpm.put("BA", bap);
		TraceParameterMap copy = tpm.copy();
		ba[1] = 6;
		byte[] baCopy = (byte[]) copy.get("BA").Value;
		assertFalse("Arrays should not be equal, but they are", baCopy.SequenceEqual(ba));
	}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testEmptyString()
	public virtual void testEmptyString()
	{
		TraceParameterMap tpm = new TraceParameterMap();
		tpm.Add("EMPTY_STRING", "");
		TraceParameterDefinitionMap tpdm = TraceParameterDefinitionMap.CreateFrom(tpm);
		byte[] serialized = tpm.Serialize();

		TraceParameterMap deserialized = TraceParameterMap.Deserialize(serialized, tpdm);
		string empty_string = deserialized.get("EMPTY_STRING").ToString();
		Assert.Equals("", empty_string);
	}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testTooLargeArraySetParameter()
	public virtual void testTooLargeArraySetParameter()
	{
		int arrayLength = 65536;
		IntegerArrayTypeKey key = new IntegerArrayTypeKey("TOO_LARGE_ARRAY");
		TraceSetParameterMap tspm = new TraceSetParameterMap();
		tspm.Add(key, new int[arrayLength]);
		assertThrows(typeof(Exception), tspm.serialize);
	}

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testLargeArraySetParameter()
	public virtual void testLargeArraySetParameter()
	{
		int arrayLength = 65535;
		IntegerArrayTypeKey key = new IntegerArrayTypeKey("JUST_SMALL_ENOUGH_ARRAY");
		TraceSetParameterMap tspm = new TraceSetParameterMap();
		tspm.Add(key, new int[arrayLength]);
		byte[] serialize = tspm.Serialize();
		TraceSetParameterMap deserialize = TraceSetParameterMap.Deserialize(serialize);
		Assert.Equals(arrayLength, deserialize.getOrElseThrow(key).length);
	}
}
