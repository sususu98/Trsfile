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
using com.riscure.trs.parameter;


namespace Trsfile.Test
{
    [TestClass]
    public class TestTraceSet
    {
        private static string tempDir = Path.Combine(Path.GetTempPath(), "TestTraceSet");
        private const string BYTES_TRS = "bytes.trs";
        private const string SHORTS_TRS = "shorts.trs";
        private const string INTS_TRS = "ints.trs";
        private const string FLOATS_TRS = "floats.trs";
        private const string TRS = ".trs";
        private const int NUMBER_OF_TRACES = 1024;
        private static readonly float[] BYTE_SAMPLES = new float[] { 1, 2, 3, 4, 5 };
        private static readonly float[] SHORT_SAMPLES = new float[] { 1, 2, 3, 4, byte.MaxValue + 1 };
        private static readonly float[] INT_SAMPLES = new float[] { 1, 2, 3, 4, short.MaxValue + 1 };
        private static readonly float[] FLOAT_SAMPLES = new float[] { 1, 2, 3, 4, 5.1f };
        private const string TVLA_STRING_VALUE = "Trace set contains the following TVLA sets: Random, R5S-Box_Out\n"
            + "AES-128 ENCRYPT (Input -> Output) Round 5 S-Box Out:HW(3~7)";

        public static bool AssertArrayEquals(float[] a1, float[] a2, float precision)
        {
            if (a1.Length != a2.Length) Assert.Fail("Array Not Equal");
            for (int i = 0; i < a1.Length; i++)
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

        [TestInitialize]
        public void CreateTempDir()
        {
            tempDir = Path.Combine(Path.GetTempPath(), "TestTraceSet");
            Directory.CreateDirectory(tempDir);

            using (TraceSet writable = TraceSet.Create(tempDir + Path.DirectorySeparatorChar + BYTES_TRS))
            {
                for (int k = 0; k < NUMBER_OF_TRACES; k++)
                {
                    writable.Add(Trace.Create(BYTE_SAMPLES));
                }
            }

            using (TraceSet writable = TraceSet.Create(tempDir + Path.DirectorySeparatorChar + SHORTS_TRS))
            {
                for (int k = 0; k < NUMBER_OF_TRACES; k++)
                {
                    writable.Add(Trace.Create(SHORT_SAMPLES));
                }
            }

            using (TraceSet writable = TraceSet.Create(tempDir + Path.DirectorySeparatorChar + INTS_TRS))
            {
                for (int k = 0; k < NUMBER_OF_TRACES; k++)
                {
                    writable.Add(Trace.Create(INT_SAMPLES));
                }
            }

            using (TraceSet writable = TraceSet.Create(tempDir + Path.DirectorySeparatorChar + FLOATS_TRS))
            {
                for (int k = 0; k < NUMBER_OF_TRACES; k++)
                {
                    writable.Add(Trace.Create(FLOAT_SAMPLES));
                }
            }
        }

        [TestCleanup]
        public void Cleanup()
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

        [TestMethod]
        public void TestOpenBytes()
        {
            using TraceSet readable = TraceSet.Open(tempDir + Path.DirectorySeparatorChar + BYTES_TRS);
            int numberOfTracesRead = readable.MetaData.GetInt(TRSTag.NUMBER_OF_TRACES);
            Encoding encoding = Encoding.FromValue(readable.MetaData.GetInt(TRSTag.SAMPLE_CODING));
            Assert.AreEqual(Encoding.BYTE, encoding);
            Assert.AreEqual(NUMBER_OF_TRACES, numberOfTracesRead);
            for (int k = 0; k < NUMBER_OF_TRACES; k++)
            {
                Trace t = readable.Get(k);
                Assert.AreEqual(Encoding.BYTE.Value, t.PreferredCoding);
                AssertArrayEquals(BYTE_SAMPLES, readable.Get(k).Sample, 0.01f);
            }
        }

        [TestMethod]
        public void TestOpenShorts()
        {
            using (TraceSet readable = TraceSet.Open(tempDir + Path.DirectorySeparatorChar + SHORTS_TRS))
            {
                int numberOfTracesRead = readable.MetaData.GetInt(TRSTag.NUMBER_OF_TRACES);
                Encoding encoding = Encoding.FromValue(readable.MetaData.GetInt(TRSTag.SAMPLE_CODING));
                Assert.AreEqual(Encoding.SHORT, encoding);
                Assert.AreEqual(NUMBER_OF_TRACES, numberOfTracesRead);
                for (int k = 0; k < NUMBER_OF_TRACES; k++)
                {
                    Trace t = readable.Get(k);
                    Assert.AreEqual(Encoding.SHORT.Value, t.PreferredCoding);
                    AssertArrayEquals(SHORT_SAMPLES, readable.Get(k).Sample, 0.01f);
                }
            }
        }

        [TestMethod]
        public void TestOpenInts()
        {
            using (TraceSet readable = TraceSet.Open(tempDir + Path.DirectorySeparatorChar + INTS_TRS))
            {
                int numberOfTracesRead = readable.MetaData.GetInt(TRSTag.NUMBER_OF_TRACES);
                Encoding encoding = Encoding.FromValue(readable.MetaData.GetInt(TRSTag.SAMPLE_CODING));
                Assert.AreEqual(Encoding.INT, encoding);
                Assert.AreEqual(NUMBER_OF_TRACES, numberOfTracesRead);
                for (int k = 0; k < NUMBER_OF_TRACES; k++)
                {
                    Trace t = readable.Get(k);
                    Assert.AreEqual(Encoding.INT.Value, t.PreferredCoding);
                    AssertArrayEquals(INT_SAMPLES, readable.Get(k).Sample, 0.01f);
                }
            }
        }

        [TestMethod]
        public void TestOpenFloats()
        {
            using TraceSet readable = TraceSet.Open(tempDir + Path.DirectorySeparatorChar + FLOATS_TRS);
            int numberOfTracesRead = readable.MetaData.GetInt(TRSTag.NUMBER_OF_TRACES);
            Encoding encoding = Encoding.FromValue(readable.MetaData.GetInt(TRSTag.SAMPLE_CODING));
            Assert.AreEqual(Encoding.FLOAT, encoding);
            Assert.AreEqual(NUMBER_OF_TRACES, numberOfTracesRead);
            for (int k = 0; k < NUMBER_OF_TRACES; k++)
            {
                Trace t = readable.Get(k);
                Assert.AreEqual(Encoding.FLOAT.Value, t.PreferredCoding);
                AssertArrayEquals(FLOAT_SAMPLES, readable.Get(k).Sample, 0.01f);
            }
        }

        [TestMethod]
        public void TestUTF8Title()
        {
            string title = "씨브 크레그스만";
            string name = Guid.NewGuid().ToString() + TRS;
            try
            {
                using TraceSet ts = TraceSet.Create(tempDir + Path.DirectorySeparatorChar + name);
                ts.Add(Trace.Create(title, Array.Empty<float>(), new TraceParameterMap()));
            }
            catch (TRSFormatException e)
            {
                throw e;
            }
            using TraceSet readable = TraceSet.Open(tempDir + Path.DirectorySeparatorChar + name);
            Assert.AreEqual(title, readable.Get(0).Title);
        }

        /// <summary>
        /// This tests adding several different types of information to the trace set header. The three parameters are chosen
        /// to match the three major cases: Strings, primitives, and arbitrary (serializable) objects.
        /// </summary>
        /// <exception cref="IOException"> </exception>
        /// <exception cref="TRSFormatException"> </exception>
        [TestMethod]
        public void TestWriteTraceSetParameters()
        {
            TRSMetaData metaData = TRSMetaData.Create();
            TraceSetParameterMap parameters = new();
            parameters.Add("BYTE", (byte)1);
            parameters.Add("SHORT", (short)2);
            parameters.Add("INT", 3);
            parameters.Add("FLOAT", (float)4);
            parameters.Add("LONG", (long)5);
            parameters.Add("DOUBLE", (double)6);
            parameters.Add("STRING", string.Format("{0,3:D}", 7));
            parameters.Add("BOOLEAN", true);
            parameters.Add("BYTEARRAY", new byte[] { 8, 9, 0 });
            parameters.Add("SHORTARRAY", new short[] { 1, 2, 3 });
            parameters.Add("INTARRAY", new int[] { 4, 5, 6 });
            parameters.Add("FLOATARRAY", new float[] { 7, 8, 9 });
            parameters.Add("LONGARRAY", new long[] { 0, 1, 2 });
            parameters.Add("DOUBLEARRAY", new double[] { 3, 4, 5 });
            parameters.Add("BOOLEANARRAY", new bool[] { true, false, true, false, true, true });
            parameters.Add("TVLA", TVLA_STRING_VALUE);
            //parameters.Add("XYZ offset", XYZ_TEST_VALUE);
            metaData.Add(TRSTag.TRACE_SET_PARAMETERS, parameters);
            //CREATE TRACE
            string name = Guid.NewGuid().ToString() + TRS;
            TraceSet.Create(tempDir + Path.DirectorySeparatorChar + name, metaData).Close();
            //READ BACK AND CHECK RESULT
            using (TraceSet readable = TraceSet.Open(tempDir + Path.DirectorySeparatorChar + name))
            {
                TraceSetParameterMap readTraceSetParameterMap = readable.MetaData.TraceSetParameters;
                foreach (var (s, traceSetParameter) in parameters)
                {
                    Assert.AreEqual(traceSetParameter, readTraceSetParameterMap[s]);
                }
            }
        }

        /// <summary>
        /// This tests adding a parameter with a name of 100000 characters
        /// </summary>
        /// <exception cref="IOException"> </exception>
        /// <exception cref="TRSFormatException"> </exception>

        [TestMethod]
        public void TestWriteTraceParametersInvalidName()
        {
            TRSMetaData metaData = TRSMetaData.Create();
            string parameterName = string.Format("{0,100000}", "XYZ");
            //CREATE TRACE
            string name = Guid.NewGuid().ToString() + TRS;
            using (TraceSet traceWithParameters = TraceSet.Create(tempDir + Path.DirectorySeparatorChar + name, metaData))
            {
                TraceParameterMap parameters = new();
                parameters.Add(parameterName, 1);
                traceWithParameters.Add(Trace.Create("", FLOAT_SAMPLES, parameters));
            }
            //READ BACK AND CHECK RESULT
            using (TraceSet readable = TraceSet.Open(tempDir + Path.DirectorySeparatorChar + name))
            {
                TraceParameterDefinitionMap parameterDefinitions = readable.MetaData.TraceParameterDefinitions;
                foreach (var (key, parameter) in parameterDefinitions)
                {
                    Assert.AreEqual(parameterName, key);
                }
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


        [TestMethod]
        public void TestWriteTraceParametersVaryingStringLength()
        {
            TRSMetaData metaData = TRSMetaData.Create();
            IList<TraceParameterMap> testParameters = new List<TraceParameterMap>();
            IList<string> strings = new List<string>
        {
            "abcd",
            "abcdefgh",
            "ab",
            "abcdefgh汉字"
        };
            //CREATE TRACE
            string name = Guid.NewGuid().ToString() + TRS;
            using (TraceSet traceWithParameters = TraceSet.Create(tempDir + Path.DirectorySeparatorChar + name, metaData))
            {
                for (int k = 0; k < 25; k++)
                {
                    TraceParameterMap parameters = new TraceParameterMap();
                    parameters.Add("BYTEARRAY", new byte[] { (byte)k, (byte)k, (byte)k });
                    parameters.Add(TraceParameter.SAMPLES, new float[] { (float)k, (float)k, (float)k });
                    parameters.Add(TraceParameter.TITLE, strings[k % strings.Count]);
                    traceWithParameters.Add(Trace.Create(strings[k % strings.Count], FLOAT_SAMPLES, parameters));
                    testParameters.Add(parameters);
                }
            }
            //READ BACK AND CHECK RESULT
            ReadBackGeneric(testParameters, name);
        }

        /// <summary>
        /// This tests whether all getters are working as expected
        /// </summary>
        /// <exception cref="IOException"> </exception>
        /// <exception cref="TRSFormatException"> </exception>

        [TestMethod]
        public void TestReadTraceParametersTyped()
        {
            TRSMetaData metaData = TRSMetaData.Create();
            IList<TraceParameterMap> testParameters = new List<TraceParameterMap>();
            //CREATE TRACE
            string name = Guid.NewGuid().ToString() + TRS;
            using (TraceSet traceWithParameters = TraceSet.Create(tempDir + Path.DirectorySeparatorChar + name, metaData))
            {
                for (int k = 0; k < 25; k++)
                {
                    TraceParameterMap parameters = new();
                    parameters.Add("BYTE", (byte)k);
                    parameters.Add("SHORT", (short)k);
                    parameters.Add("INT", k);
                    parameters.Add("FLOAT", (float)k);
                    parameters.Add("LONG", (long)k);
                    parameters.Add("DOUBLE", (double)k);
                    parameters.Add("STRING", string.Format("{0,3:D}", k));
                    parameters.Add("BOOLEAN", true);
                    parameters.Add("BYTEARRAY", new byte[] { (byte)k, (byte)k, (byte)k });
                    parameters.Add("SHORTARRAY", new short[] { (short)k, (short)k, (short)k });
                    parameters.Add("INTARRAY", new int[] { k, k, k });
                    parameters.Add("FLOATARRAY", new float[] { (float)k, (float)k, (float)k });
                    parameters.Add("LONGARRAY", new long[] { k, k, k });
                    parameters.Add("DOUBLEARRAY", new double[] { k, k, k });
                    parameters.Add("BOOLEANARRAY", new bool[] { true, false, true, false, true, true });
                    traceWithParameters.Add(Trace.Create("", FLOAT_SAMPLES, parameters));
                    testParameters.Add(parameters);
                }
            }
            ReadBackGeneric(testParameters, name);
            ReadBackTyped(testParameters, name);
            ReadBackTypedKeys(testParameters, name);
        }
        private void ReadBackGeneric(IList<TraceParameterMap> testParameters, string name)
        {
            using (TraceSet readable = TraceSet.Open(tempDir + Path.DirectorySeparatorChar + name))
            {
                TraceParameterDefinitionMap parameterDefinitions = readable.MetaData.TraceParameterDefinitions;
                for (int k = 0; k < 25; k++)
                {
                    Assert.AreEqual(parameterDefinitions.Count, testParameters[k].Count);
                    Trace trace = readable.Get(k);
                    TraceParameterMap correctValue = testParameters[k];
                    foreach (var (key, parameter) in parameterDefinitions)
                    {
                        TraceParameter traceParameter = trace.Parameters[key];
                        Assert.AreEqual(traceParameter, correctValue[key]);
                    }

                }
            }
        }
        private static void ReadBackTyped(IList<TraceParameterMap> testParameters, string name)
        {
            using (TraceSet readable = TraceSet.Open(tempDir + Path.DirectorySeparatorChar + name))
            {
                TraceParameterDefinitionMap parameterDefinitions = readable.MetaData.TraceParameterDefinitions;
                for (int k = 0; k < 25; k++)
                {
                    Assert.AreEqual(parameterDefinitions.Count, testParameters[k].Count);
                    Trace trace = readable.Get(k);
                    TraceParameterMap correctValue = testParameters[k];
                    foreach (var (key, parameter) in parameterDefinitions)
                    {
                        if (parameter.Type == ParameterType.BYTE)
                        {
                            if (parameter.Length == 1)
                            {
                                Assert.AreEqual(correctValue.GetByte(key), trace.Parameters.GetByte(key));
                            }
                            else
                            {
                                Assert.AreEqual(correctValue.GetByteArray(key), trace.Parameters.GetByteArray(key));
                            }
                        }
                        if (parameter.Type == ParameterType.SHORT)
                        {
                            if (parameter.Length == 1)
                            {
                                Assert.AreEqual(correctValue.GetShort(key), trace.Parameters.GetShort(key));
                            }
                            else
                            {
                                Assert.AreEqual(correctValue.GetShortArray(key), trace.Parameters.GetShortArray(key));
                            }
                        }
                        if (parameter.Type == ParameterType.INT)
                        {
                            if (parameter.Length == 1)
                            {
                                Assert.AreEqual(correctValue.GetInt(key), trace.Parameters.GetInt(key));
                            }
                            else
                            {
                                Assert.AreEqual(correctValue.GetIntArray(key), trace.Parameters.GetIntArray(key));
                            }
                        }
                        if (parameter.Type == ParameterType.FLOAT)
                        {
                            if (parameter.Length == 1)
                            {
                                Assert.AreEqual(correctValue.GetFloat(key), trace.Parameters.GetFloat(key));
                            }
                            else
                            {
                                Assert.AreEqual(correctValue.GetFloatArray(key), trace.Parameters.GetFloatArray(key));
                            }
                        }
                        if (parameter.Type == ParameterType.LONG)
                        {
                            if (parameter.Length == 1)
                            {
                                Assert.AreEqual(correctValue.GetLong(key), trace.Parameters.GetLong(key));
                            }
                            else
                            {
                                Assert.AreEqual(correctValue.GetLongArray(key), trace.Parameters.GetLongArray(key));
                            }
                        }
                        if (parameter.Type == ParameterType.DOUBLE)
                        {
                            if (parameter.Length == 1)
                            {
                                Assert.AreEqual(correctValue.GetDouble(key), trace.Parameters.GetDouble(key));
                            }
                            else
                            {
                                Assert.AreEqual(correctValue.GetDoubleArray(key), trace.Parameters.GetDoubleArray(key));
                            }
                        }
                        if (parameter.Type == ParameterType.STRING)
                        {
                            Assert.AreEqual(correctValue.GetString(key), trace.Parameters.GetString(key));
                        }
                        if (parameter.Type == ParameterType.BOOL)
                        {
                            if (parameter.Length == 1)
                            {
                                Assert.AreEqual(correctValue.GetBool(key), trace.Parameters.GetBool(key));
                            }
                            else
                            {
                                Assert.AreEqual(correctValue.GetBoolArray(key), trace.Parameters.GetBoolArray(key));
                            }
                        }
                        else throw new Exception("Unexpected type: " + parameter.Type);

                    }
                }

            }
        }
        private void ReadBackTypedKeys(IList<TraceParameterMap> testParameters, string name)
        {
            using (TraceSet readable = TraceSet.Open(tempDir + Path.DirectorySeparatorChar + name))
            {
                TraceParameterDefinitionMap parameterDefinitions = readable.MetaData.TraceParameterDefinitions;
                for (int k = 0; k < 25; k++)
                {
                    Assert.AreEqual(parameterDefinitions.Count, testParameters[k].Count);
                    Trace trace = readable.Get(k);
                    TraceParameterMap correctValue = testParameters[k];
                    foreach (var (key, parameter) in parameterDefinitions)
                    {
                        if (parameter.Type == ParameterType.BYTE)
                        {
                            if (parameter.Length == 1)
                            {
                                Assert.AreEqual(correctValue.GetByte(key), trace.Parameters.GetByte(key));
                            }
                            else
                            {
                                Assert.AreEqual(correctValue.GetByteArray(key), trace.Parameters.GetByteArray(key));
                            }
                        }
                        if (parameter.Type == ParameterType.SHORT)
                        {
                            if (parameter.Length == 1)
                            {
                                Assert.AreEqual(correctValue.GetShort(key), trace.Parameters.GetShort(key));
                            }
                            else
                            {
                                Assert.AreEqual(correctValue.GetShortArray(key), trace.Parameters.GetShortArray(key));
                            }
                        }
                        if (parameter.Type == ParameterType.INT)
                        {
                            if (parameter.Length == 1)
                            {
                                Assert.AreEqual(correctValue.GetInt(key), trace.Parameters.GetInt(key));
                            }
                            else
                            {
                                Assert.AreEqual(correctValue.GetIntArray(key), trace.Parameters.GetIntArray(key));
                            }
                        }
                        if (parameter.Type == ParameterType.FLOAT)
                        {
                            if (parameter.Length == 1)
                            {
                                Assert.AreEqual(correctValue.GetFloat(key), trace.Parameters.GetFloat(key));
                            }
                            else
                            {
                                Assert.AreEqual(correctValue.GetFloatArray(key), trace.Parameters.GetFloatArray(key));
                            }
                        }
                        if (parameter.Type == ParameterType.LONG)
                        {
                            if (parameter.Length == 1)
                            {
                                Assert.AreEqual(correctValue.GetLong(key), trace.Parameters.GetLong(key));
                            }
                            else
                            {
                                Assert.AreEqual(correctValue.GetLongArray(key), trace.Parameters.GetLongArray(key));
                            }
                        }
                        if (parameter.Type == ParameterType.DOUBLE)
                        {
                            if (parameter.Length == 1)
                            {
                                Assert.AreEqual(correctValue.GetDouble(key), trace.Parameters.GetDouble(key));
                            }
                            else
                            {
                                Assert.AreEqual(correctValue.GetDoubleArray(key), trace.Parameters.GetDoubleArray(key));
                            }
                        }
                        if (parameter.Type == ParameterType.STRING)
                        {
                            Assert.AreEqual(correctValue.GetString(key), trace.Parameters.GetString(key));
                        }
                        if (parameter.Type == ParameterType.BOOL)
                        {
                            if (parameter.Length == 1)
                            {
                                Assert.AreEqual(correctValue.GetBool(key), trace.Parameters.GetBool(key));
                            }
                            else
                            {
                                Assert.AreEqual(correctValue.GetBoolArray(key), trace.Parameters.GetBoolArray(key));
                            }
                        }
                        else throw new Exception("Unexpected type: " + parameter.Type);

                    }
                }

            }
        }

        [TestMethod]
        public void TestExceptionWrongType()
        {
            TRSMetaData metaData = TRSMetaData.Create();
            //CREATE trsfile
            string name = Guid.NewGuid().ToString() + TRS;
            using (TraceSet traceWithParameters = TraceSet.Create(tempDir + Path.DirectorySeparatorChar + name, metaData))
            {
                TraceParameterMap parameters = new();
                parameters.Add("BYTE", (byte)1);
                traceWithParameters.Add(Trace.Create("", FLOAT_SAMPLES, parameters));
            }
            //READ BACK AND CHECK RESULT
            using (TraceSet readable = TraceSet.Open(tempDir + Path.DirectorySeparatorChar + name))
            {
                Assert.ThrowsException<ArgumentNullException>(() => readable.Get(0).Parameters.GetDouble("BYTE"));
            }
        }

        /// <summary>
        /// This </summary>
        /// <exception cref="IOException"> </exception>
        /// <exception cref="TRSFormatException"> </exception>


        [TestMethod]
        public void TestContainsNonArray()
        {
            ByteTypeKey byteKey = new("BYTE");
            string name = Guid.NewGuid().ToString() + TRS;
            using (TraceSet traceWithParameters = TraceSet.Create(tempDir + Path.DirectorySeparatorChar + name))
            {
                TraceParameterMap parameters = new();
                parameters.Add(byteKey, (byte)1);
                traceWithParameters.Add(Trace.Create("", FLOAT_SAMPLES, parameters));
            }
            //READ BACK AND CHECK RESULT
            using TraceSet readable = TraceSet.Open(tempDir + Path.DirectorySeparatorChar + name);
            Assert.IsFalse(readable.Get(0).Parameters[byteKey.Key].Equals(null));
        }

        /// <summary>
        /// This test checks whether all deserialize methods throw an exception if the inputstream does not contain enough data
        /// Introduced to test #11: TraceParameter.deserialize does not check the actual returned length
        /// </summary> 

        [TestMethod]
        public void TestInvalidParameterLength()
        {
            int errors = 0;
            byte[] empty = new byte[1];
            foreach (ParameterType type in ParameterType.Values)
            {
                try
                {
                    using (LittleEndianInputStream dis = new(new MemoryStream(empty)))
                    {
                        TraceParameter.Deserialize(type, (short)4, dis);
                    }
                }
                catch (IOException)
                {
                    errors++;
                }
            }
            Assert.AreEqual(ParameterType.Values.Length, errors);
        }

        /// <summary>
        /// This test was added to test #26: Trace(Set)ParameterMap is modifiable in read only mode
        /// </summary>

        [TestMethod]
        public void TestModificationAfterReadback()
        {
            using (TraceSet readable = TraceSet.Open(tempDir + Path.DirectorySeparatorChar + BYTES_TRS))
            {
                Assert.ThrowsException<NotSupportedException>(() => readable.MetaData.TraceSetParameters.Add("SHOULD_FAIL", 0));
                //Assert.ThrowsException<NotSupportedException>(() => readable.MetaData.TraceParameterDefinitions.Add("SHOULD_FAIL", new TraceParameterDefinition<TraceParameter>(ParameterType.BYTE, (short)1, (short)1)));
                for (int k = 0; k < NUMBER_OF_TRACES; k++)
                {
                    Trace t = readable.Get(k);
                    Assert.ThrowsException<NotSupportedException>(() => t.Parameters.Add("SHOULD_FAIL", 0));
                }
            }
        }

        /// <summary>
        /// This test checks whether an empty array is serialized and deserialized correctly
        /// Expected: an exception is thrown when adding an empty parameter
        /// </summary>

        [TestMethod]
        public void TestEmptyArrayParameter()
        {
            Assert.ThrowsException<ArgumentException>(() => (new TraceParameterMap()).Add("EMPTY", Array.Empty<byte>()));

        }

        /// <summary>
        /// This test checks whether a Trace(Set)ParameterMap correctly works with typed keys
        /// </summary>

        [TestMethod]
        public void TestContainsTypedKey()
        {
            TraceParameterMap tpm = new();
            TraceSetParameterMap tspm = new();
            string rawKey = "BYTE";
            ByteTypeKey typedKey = new(rawKey);
            tpm.Add(typedKey, (byte)1);
            tspm.Add(typedKey, (byte)2);

            Assert.IsFalse(tpm[typedKey.Key].Equals(null));
            Assert.IsFalse(tspm[typedKey.Key].Equals(null));
        }

        /// <summary>
        /// This test checks whether you can get an array type of a simple value
        /// </summary>

        [TestMethod]
        public void TestGetArrayOfLengthOne()
        {
            TraceParameterMap tpm = new();
            TraceSetParameterMap tspm = new();

            byte rawValue = 1;
            string rawKey = "BYTE";
            ByteTypeKey typedKey = new(rawKey);
            tpm.Add(typedKey, rawValue);
            tspm.Add(typedKey, rawValue);

            ByteArrayTypeKey arrayTypeKey = new(rawKey);
            Assert.IsTrue(tpm[arrayTypeKey.Key] is not null);
            Assert.IsTrue(tspm[arrayTypeKey.Key] is not null);
            var a = tpm.GetByteArray(arrayTypeKey.Key);
            var b = tspm.GetByteArray(arrayTypeKey.Key);
            Assert.AreEqual(new byte[] { rawValue }, tpm.GetByteArray(arrayTypeKey.Key));
            Assert.AreEqual(new byte[] { rawValue }, tspm.GetByteArray(arrayTypeKey.Key));
        }

        /// <summary>
        /// This test checks whether a Trace(Set)ParameterMap correctly fails with a typed key of the incorrect type
        /// </summary>

        [TestMethod]
        public void TestContainsWrongTypedKey()
        {
            TraceParameterMap tpm = new();
            TraceSetParameterMap tspm = new();
            string rawKey = "BYTE";
            ByteTypeKey typedKey = new(rawKey);
            tpm.Add(rawKey, new byte[] { 1, 2 }); //actually a byte array
            tspm.Add(rawKey, 2); //actually an int'
            Assert.ThrowsException<InvalidCastException>(() => tpm[typedKey.Key]);
            Assert.ThrowsException<InvalidCastException>(() => tspm[typedKey.Key]);
        }

        /// <summary>
        /// This test ensure that the underlying data of a copied map cannot change underlying map itself
        /// </summary>
        [TestMethod]
        public void TestCopyConstructor()
        {
            byte[] ba = new byte[] { 1, 2, 3, 4, 5 };
            ByteArrayParameter bap = new(ba);
            TraceParameterMap tpm = new();
            tpm.Add("BA", bap);
            TraceParameterMap copy = (TraceParameterMap)tpm.Clone();
            ba[1] = 6;
            var t = copy["BA"];
            if (t is TraceParameter<byte> Tparameter)
            {
                Assert.IsFalse(Tparameter.Value.Equals(ba));
            }

        }

        [TestMethod]
        public void TestEmptyString()
        {
            TraceParameterMap tpm = new();
            tpm.Add("EMPTY_STRING", "");
            TraceParameterDefinitionMap tpdm = TraceParameterDefinitionMap.CreateFrom(tpm);
            byte[] serialized = tpm.Serialize();

            TraceParameterMap deserialized = TraceParameterMap.Deserialize(serialized, tpdm);
            string empty_string = deserialized["EMPTY_STRING"].ToString();
            Assert.IsTrue("" == empty_string);
        }

        [TestMethod]
        public void TestTooLargeArraySetParameter()
        {
            int arrayLength = 65536;
            IntegerArrayTypeKey key = new("TOO_LARGE_ARRAY");
            TraceSetParameterMap tspm = new();
            tspm.Add(key, new int[arrayLength]);
            Assert.ThrowsException<IOException>(() => tspm.Serialize());
        }


        [TestMethod]
        public void TestLargeArraySetParameter()
        {
            int arrayLength = 65535;
            IntegerArrayTypeKey key = new("JUST_SMALL_ENOUGH_ARRAY");
            TraceSetParameterMap tspm = new();
            tspm.Add(key, new int[arrayLength]);
            byte[] serialize = tspm.Serialize();
            TraceSetParameterMap deserialize = TraceSetParameterMap.Deserialize(serialize);
            Assert.AreEqual(arrayLength, deserialize.GetByteArray(key.Key).Length);
        }


    }

}



