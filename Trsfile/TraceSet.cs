using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace com.riscure.trs
{
	using Encoding = com.riscure.trs.enums.Encoding;
	using ParameterType = com.riscure.trs.enums.ParameterType;
	using TraceParameter = com.riscure.trs.parameter.TraceParameter;
	using StringParameter = com.riscure.trs.parameter.primitive.StringParameter;
	using TraceParameterMap = com.riscure.trs.parameter.trace.TraceParameterMap;
	using com.riscure.trs.parameter.trace.definition;
	using TraceParameterDefinitionMap = com.riscure.trs.parameter.trace.definition.TraceParameterDefinitionMap;


	using static com.riscure.trs.enums.TRSTag;

	public class TraceSet : AutoCloseable
	{
		private const string ERROR_READING_FILE = "Error reading TRS file: file size (%d) != meta data (%d) + trace size (%d) * nr of traces (%d)";
		private const string TRACE_SET_NOT_OPEN = "TraceSet has not been opened or has been closed.";
		private const string TRACE_SET_IN_WRITE_MODE = "TraceSet is in write mode. Please open the TraceSet in read mode.";
		private const string TRACE_INDEX_OUT_OF_BOUNDS = "Requested trace index (%d) is larger than the total number of available traces (%d).";
		private const string TRACE_SET_IN_READ_MODE = "TraceSet is in read mode. Please open the TraceSet in write mode.";
		private const string TRACE_LENGTH_DIFFERS = "All traces in a set need to be the same length, but current trace length (%d) differs from the previous trace(s) (%d)";
		private const string TRACE_DATA_LENGTH_DIFFERS = "All traces in a set need to have the same data length, but current trace data length (%d) differs from the previous trace(s) (%d)";
		private const string UNKNOWN_SAMPLE_CODING = "Error reading TRS file: unknown sample coding '%d'";
		private static readonly long MAX_BUFFER_SIZE = int.MaxValue;
		private const string PARAMETER_NOT_DEFINED = "Parameter %s is saved in the trace, but was not found in the header definition";
		private static readonly CharsetDecoder UTF8_DECODER = StandardCharsets.UTF_8.newDecoder();

		//Reading variables
		private int metaDataSize;
		private FileStream readStream;
		private FileChannel channel;

		private ByteBuffer buffer;

		private long bufferStart; //the byte index of the file where the buffer window starts
		private long bufferSize; //the number of bytes that are in the buffer window
		private long fileSize; //the total number of bytes in the underlying file

		//Writing variables
		private FileStream writeStream;

		private bool firstTrace = true;

		//Shared variables
		private readonly TRSMetaData metaData;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods of the current type:
		private bool open_Conflict;
		private readonly bool writing; //whether the trace is opened in write mode
		private readonly Path path;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private TraceSet(String inputFileName) throws IOException, TRSFormatException
		private TraceSet(string inputFileName)
		{
			this.writing = false;
			this.open_Conflict = true;
			this.path = Paths.get(inputFileName);
			this.readStream = new FileStream(inputFileName, FileMode.Open, FileAccess.Read);
			this.channel = readStream.getChannel();

			//the file might be bigger than the buffer, in which case we partially buffer it in memory
			this.fileSize = this.channel.size();
			this.bufferStart = 0L;
			this.bufferSize = Math.Min(fileSize, MAX_BUFFER_SIZE);

			mapBuffer();
			this.metaData = TRSMetaDataUtils.readTRSMetaData(buffer);
			this.metaDataSize = buffer.position();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private TraceSet(String outputFileName, TRSMetaData metaData) throws java.io.FileNotFoundException
		private TraceSet(string outputFileName, TRSMetaData metaData)
		{
			this.open_Conflict = true;
			this.writing = true;
			this.metaData = metaData;
			this.path = Paths.get(outputFileName);
			this.writeStream = new FileStream(outputFileName, FileMode.Create, FileAccess.Write);
		}

		/// <returns> the Path on disk of this trace set </returns>
		public virtual Path Path
		{
			get
			{
				return path;
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void mapBuffer() throws java.io.IOException
		private void mapBuffer()
		{
			this.buffer = this.channel.map(FileChannel.MapMode.READ_ONLY, this.bufferStart, this.bufferSize);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void moveBufferIfNecessary(int traceIndex) throws java.io.IOException
		private void moveBufferIfNecessary(int traceIndex)
		{
			long traceSize = calculateTraceSize();
			long start = metaDataSize + (long) traceIndex * traceSize;
			long end = start + traceSize;

			bool moveRequired = start < this.bufferStart || this.bufferStart + this.bufferSize < end;
			if (moveRequired)
			{
				this.bufferStart = start;
				this.bufferSize = Math.Min(this.fileSize - start, MAX_BUFFER_SIZE);
				this.mapBuffer();
			}
		}

		private long calculateTraceSize()
		{
			int sampleSize = Encoding.fromValue(metaData.getInt(SAMPLE_CODING)).getSize();
			long sampleSpace = metaData.getInt(NUMBER_OF_SAMPLES) * (long) sampleSize;
			return sampleSpace + metaData.getInt(DATA_LENGTH) + metaData.getInt(TITLE_SPACE);
		}

		/// <summary>
		/// Get a trace from the set at the specified index </summary>
		/// <param name="index"> the index of the Trace to read from the file </param>
		/// <returns> the Trace at the requested trace index </returns>
		/// <exception cref="IOException"> if a read error occurs </exception>
		/// <exception cref="IllegalArgumentException"> if this TraceSet is not ready be read from </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public Trace get(int index) throws java.io.IOException
		public virtual Trace get(int index)
		{
			if (!open_Conflict)
			{
				throw new System.ArgumentException(TRACE_SET_NOT_OPEN);
			}
			if (writing)
			{
				throw new System.ArgumentException(TRACE_SET_IN_WRITE_MODE);
			}

			moveBufferIfNecessary(index);

			long traceSize = calculateTraceSize();
			long nrOfTraces = this.metaData.getInt(NUMBER_OF_TRACES);
			if (index >= nrOfTraces)
			{
				string msg = String.format(TRACE_INDEX_OUT_OF_BOUNDS, index, nrOfTraces);
				throw new System.ArgumentException(msg);
			}

			long calculatedFileSize = metaDataSize + traceSize * nrOfTraces;
			if (fileSize != calculatedFileSize)
			{
				string msg = String.format(ERROR_READING_FILE, fileSize, metaDataSize, traceSize, nrOfTraces);
				throw new System.InvalidOperationException(msg);
			}
			long absolutePosition = metaDataSize + index * traceSize;
			buffer.position((int)(absolutePosition - this.bufferStart));

			string traceTitle = this.readTraceTitle();
			if (traceTitle.Trim().Length == 0)
			{
				traceTitle = string.Format("{0} {1:D}", metaData.getString(GLOBAL_TITLE), index);
			}

			try
			{
				TraceParameterMap traceParameterMap;
				if (metaData.getInt(TRS_VERSION) > 1)
				{
					TraceParameterDefinitionMap traceParameterDefinitionMap = metaData.TraceParameterDefinitions;
					int size = traceParameterDefinitionMap.totalSize();
					sbyte[] data = new sbyte[size];
					buffer.get(data);
					traceParameterMap = TraceParameterMap.deserialize(data, traceParameterDefinitionMap);
				}
				else
				{
					//legacy mode
					sbyte[] data = readData();
					traceParameterMap = new TraceParameterMap();
					traceParameterMap.put("LEGACY_DATA", data);
				}

				float[] samples = readSamples();
				return new Trace(traceTitle, samples, traceParameterMap);
			}
			catch (TRSFormatException ex)
			{
				throw new IOException(ex);
			}
		}

		/// <summary>
		/// Add a trace to a writable TraceSet </summary>
		/// <param name="trace"> the Trace object to add </param>
		/// <exception cref="IOException"> if any write error occurs </exception>
		/// <exception cref="TRSFormatException"> if the formatting of the trace is invalid </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public void add(Trace trace) throws IOException, TRSFormatException
		public virtual void add(Trace trace)
		{
			if (!open_Conflict)
			{
				throw new System.ArgumentException(TRACE_SET_NOT_OPEN);
			}
			if (!writing)
			{
				throw new System.ArgumentException(TRACE_SET_IN_READ_MODE);
			}
			if (firstTrace)
			{
				int dataLength = trace.Data == null ? 0 : trace.Data.Length;
				int titleLength = string.ReferenceEquals(trace.Title, null) ? 0 : trace.Title.GetBytes(Encoding.UTF8).length;
				metaData.put(NUMBER_OF_SAMPLES, trace.NumberOfSamples, false);
				metaData.put(DATA_LENGTH, dataLength, false);
				metaData.put(TITLE_SPACE, titleLength, false);
				metaData.put(SAMPLE_CODING, trace.PreferredCoding, false);
				metaData.put(TRACE_PARAMETER_DEFINITIONS, TraceParameterDefinitionMap.createFrom(trace.Parameters));
				TRSMetaDataUtils.writeTRSMetaData(writeStream, metaData);
				firstTrace = false;
			}
			truncateStrings(trace, metaData);
			checkValid(trace);

			trace.TraceSet = this;
			writeTrace(trace);

			int numberOfTraces = metaData.getInt(NUMBER_OF_TRACES);
			metaData.put(NUMBER_OF_TRACES, numberOfTraces + 1);
		}

		/// <summary>
		/// This method makes sure that the trace title and any added string parameters adhere to the preset maximum length </summary>
		/// <param name="trace"> the trace to update </param>
		/// <param name="metaData"> the metadata specifying the maximum string lengths </param>
		private void truncateStrings(Trace trace, TRSMetaData metaData)
		{
			int titleSpace = metaData.getInt(TITLE_SPACE);
			trace.Title = fitUtf8StringToByteLength(trace.Title, titleSpace);
			TraceParameterDefinitionMap traceParameterDefinitionMap = metaData.TraceParameterDefinitions;
			foreach (KeyValuePair<string, TraceParameterDefinition<TraceParameter>> definition in traceParameterDefinitionMap.entrySet())
			{
				TraceParameterDefinition<TraceParameter> value = definition.Value;
				string key = definition.Key;
				if (value.Type == ParameterType.STRING)
				{
					short stringLength = value.Length;
					string stringValue = ((StringParameter) trace.Parameters.get(key)).Value;
					if (stringLength != stringValue.GetBytes(Encoding.UTF8).length)
					{
						trace.Parameters.put(key, fitUtf8StringToByteLength(stringValue, stringLength));
					}
				}
			}
		}

		/// <summary>
		/// Fits a string to the number of characters that fit in X bytes avoiding multi byte characters being cut in
		/// half at the cut off point. Also handles surrogate pairs where 2 characters in the string is actually one literal
		/// character. If the string is too long, it is truncated. If it's too short, it's padded with NUL characters. </summary>
		/// <param name="s"> the string to fit </param>
		/// <param name="maxBytes"> the number of bytes required </param>
		private string fitUtf8StringToByteLength(string s, int maxBytes)
		{
			if (string.ReferenceEquals(s, null))
			{
				return null;
			}
			sbyte[] sba = s.GetBytes(Encoding.UTF8);
			if (sba.Length <= maxBytes)
			{
				return new string(Arrays.CopyOf(sba, maxBytes));
			}
			// Ensure truncation by having byte buffer = maxBytes
			ByteBuffer bb = ByteBuffer.wrap(sba, 0, maxBytes);
			CharBuffer cb = CharBuffer.allocate(maxBytes);
			// Ignore an incomplete character
			UTF8_DECODER.reset();
			UTF8_DECODER.onMalformedInput(CodingErrorAction.IGNORE);
			UTF8_DECODER.decode(bb, cb, true);
			UTF8_DECODER.flush(cb);
			return new string(cb.array(), 0, cb.position());
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void writeTrace(Trace trace) throws TRSFormatException, java.io.IOException
		private void writeTrace(Trace trace)
		{
			string title = string.ReferenceEquals(trace.Title, null) ? "" : trace.Title;
			writeStream.WriteByte(title.GetBytes(Encoding.UTF8));
			sbyte[] data = trace.Data == null ? new sbyte[0] : trace.Data;
			writeStream.Write(data, 0, data.Length);
			Encoding encoding = Encoding.fromValue(metaData.getInt(SAMPLE_CODING));
			writeStream.Write(toByteArray(trace.Sample, encoding), 0, toByteArray(trace.Sample, encoding).Length);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private byte[] toByteArray(float[] samples, com.riscure.trs.enums.Encoding encoding) throws TRSFormatException
		private sbyte[] toByteArray(float[] samples, Encoding encoding)
		{
			sbyte[] result;
			switch (encoding.innerEnumValue)
			{
				case Encoding.InnerEnum.ILLEGAL:
					throw new TRSFormatException("Illegal sample encoding");
				case Encoding.InnerEnum.BYTE:
					result = new sbyte[samples.Length];
					for (int k = 0; k < samples.Length; k++)
					{
						if (samples[k] != (sbyte)samples[k])
						{
							throw new System.ArgumentException("Byte sample encoding too small");
						}
						result[k] = (sbyte) samples[k];
					}
					break;
				case Encoding.InnerEnum.SHORT:
					result = new sbyte[samples.Length * 2];
					for (int k = 0; k < samples.Length; k++)
					{
						if (samples[k] != (short)samples[k])
						{
							throw new System.ArgumentException("Short sample encoding too small");
						}
						short value = (short) samples[k];
						result[2 * k] = (sbyte) value;
						result[2 * k + 1] = (sbyte)(value >> 8);
					}
					break;
				case Encoding.InnerEnum.INT:
					result = new sbyte[samples.Length * 4];
					for (int k = 0; k < samples.Length; k++)
					{
						int value = (int) samples[k];
						result[4 * k] = (sbyte) value;
						result[4 * k + 1] = (sbyte)(value >> 8);
						result[4 * k + 2] = (sbyte)(value >> 16);
						result[4 * k + 3] = (sbyte)(value >> 24);
					}
					break;
				case Encoding.InnerEnum.FLOAT:
					result = new sbyte[samples.Length * 4];
					for (int k = 0; k < samples.Length; k++)
					{
						int value = Float.floatToIntBits(samples[k]);
						result[4 * k] = (sbyte) value;
						result[4 * k + 1] = (sbyte)(value >> 8);
						result[4 * k + 2] = (sbyte)(value >> 16);
						result[4 * k + 3] = (sbyte)(value >> 24);
					}
					break;
				default:
					throw new TRSFormatException(string.Format("Sample encoding not supported: {0}", encoding.ToString()));
			}
			return result;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: @Override public void close() throws IOException, TRSFormatException
		public override void close()
		{
			open_Conflict = false;
			if (writing)
			{
				closeWriter();
			}
			else
			{
				closeReader();
			}
		}

		private void checkValid(Trace trace)
		{
			int numberOfSamples = metaData.getInt(NUMBER_OF_SAMPLES);
			if (metaData.getInt(NUMBER_OF_SAMPLES) != trace.NumberOfSamples)
			{
				throw new System.ArgumentException(String.format(TRACE_LENGTH_DIFFERS, trace.NumberOfSamples, numberOfSamples));
			}

			int dataLength = metaData.getInt(DATA_LENGTH);
			int traceDataLength = trace.Data == null ? 0 : trace.Data.Length;
			if (metaData.getInt(DATA_LENGTH) != traceDataLength)
			{
				throw new System.ArgumentException(String.format(TRACE_DATA_LENGTH_DIFFERS, traceDataLength, dataLength));
			}

			foreach (KeyValuePair<string, TraceParameter> entry in trace.Parameters.entrySet())
			{
				if (!metaData.TraceParameterDefinitions.containsKey(entry.Key))
				{
					throw new System.ArgumentException(String.format(PARAMETER_NOT_DEFINED, entry.Key));
				}
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void closeReader() throws java.io.IOException
		private void closeReader()
		{
			readStream.Close();
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private void closeWriter() throws IOException, TRSFormatException
		private void closeWriter()
		{
			try
			{
				//reset writer to start of file and overwrite header
				writeStream.getChannel().position(0);
				TRSMetaDataUtils.writeTRSMetaData(writeStream, metaData);
				writeStream.Flush();
			}
			finally
			{
				writeStream.Close();
			}
		}

		/// <summary>
		/// Get the metadata associated with this trace set </summary>
		/// <returns> the metadata associated with this trace set </returns>
		public virtual TRSMetaData MetaData
		{
			get
			{
				return metaData;
			}
		}

		protected internal virtual string readTraceTitle()
		{
			sbyte[] titleArray = new sbyte[metaData.getInt(TITLE_SPACE)];
			buffer.get(titleArray);
			return StringHelper.NewString(titleArray);
		}

		protected internal virtual sbyte[] readData()
		{
			int inputSize = metaData.getInt(DATA_LENGTH);
			sbyte[] comDataArray = new sbyte[inputSize];
			buffer.get(comDataArray);
			return comDataArray;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: protected float[] readSamples() throws TRSFormatException
		protected internal virtual float[] readSamples()
		{
			buffer.order(ByteOrder.LITTLE_ENDIAN);
			int numberOfSamples = metaData.getInt(NUMBER_OF_SAMPLES);
			float[] samples;
			switch (Encoding.fromValue(metaData.getInt(SAMPLE_CODING)))
			{
				case BYTE:
					sbyte[] byteData = new sbyte[numberOfSamples];
					buffer.get(byteData);
					samples = toFloatArray(byteData);
					break;
				case SHORT:
					ShortBuffer shortView = buffer.asShortBuffer();
					short[] shortData = new short[numberOfSamples];
					shortView.get(shortData);
					samples = toFloatArray(shortData);
					break;
				case FLOAT:
					FloatBuffer floatView = buffer.asFloatBuffer();
					samples = new float[numberOfSamples];
					floatView.get(samples);
					break;
				case INT:
					IntBuffer intView = buffer.asIntBuffer();
					int[] intData = new int[numberOfSamples];
					intView.get(intData);
					samples = toFloatArray(intData);
					break;
				default:
					throw new TRSFormatException(String.format(UNKNOWN_SAMPLE_CODING, metaData.getInt(SAMPLE_CODING)));
			}

			return samples;
		}

		private float[] toFloatArray(sbyte[] numbers)
		{
			float[] result = new float[numbers.Length];
			for (int k = 0; k < numbers.Length; k++)
			{
				result[k] = numbers[k];
			}
			return result;
		}

		private float[] toFloatArray(int[] numbers)
		{
			float[] result = new float[numbers.Length];
			for (int k = 0; k < numbers.Length; k++)
			{
				result[k] = (float) numbers[k];
			}
			return result;
		}

		private float[] toFloatArray(short[] numbers)
		{
			float[] result = new float[numbers.Length];
			for (int k = 0; k < numbers.Length; k++)
			{
				result[k] = numbers[k];
			}
			return result;
		}

		/// <summary>
		/// Factory method. This creates a new open TraceSet for reading.
		/// The resulting TraceSet is a live view on the file, and loads from the file directly.
		/// Remember to close the TraceSet when done. </summary>
		/// <param name="file"> the path to the TRS file to open </param>
		/// <returns> the TraceSet representation of the file </returns>
		/// <exception cref="IOException"> when any read exception is encountered </exception>
		/// <exception cref="TRSFormatException"> when any incorrect formatting of the TRS file is encountered </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static TraceSet open(String file) throws IOException, TRSFormatException
		public static TraceSet open(string file)
		{
			return new TraceSet(file);
		}

		/// <summary>
		/// A one-shot creator of a TRS file. The metadata not related to the trace list is assumed to be default. </summary>
		/// <param name="file"> the path to the file to save </param>
		/// <param name="traces"> the list of traces to save in the file </param>
		/// <exception cref="IOException"> when any write exception is encountered </exception>
		/// <exception cref="TRSFormatException"> when any TRS formatting issues arise from saving the provided traces </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static void save(String file, java.util.List<Trace> traces) throws IOException, TRSFormatException
		public static void save(string file, IList<Trace> traces)
		{
			TRSMetaData trsMetaData = TRSMetaData.create();
			save(file, traces, trsMetaData);
		}

		/// <summary>
		/// A one-shot creator of a TRS file. Any unfilled fields of metadata are assumed to be default. </summary>
		/// <param name="file"> the path to the file to save </param>
		/// <param name="traces"> the list of traces to save in the file </param>
		/// <param name="metaData"> the metadata associated with the set to create </param>
		/// <exception cref="IOException"> when any write exception is encountered </exception>
		/// <exception cref="TRSFormatException"> when any TRS formatting issues arise from saving the provided traces </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static void save(String file, java.util.List<Trace> traces, TRSMetaData metaData) throws IOException, TRSFormatException
		public static void save(string file, IList<Trace> traces, TRSMetaData metaData)
		{
			TraceSet traceSet = create(file, metaData);
			foreach (Trace trace in traces)
			{
				traceSet.add(trace);
			}
			traceSet.close();
		}

		/// <summary>
		/// Create a new traceset file at the specified location. <br>
		/// NOTE: The metadata is fully defined by the first added trace. <br>
		/// Every next trace is expected to adhere to the following parameters: <br>
		/// NUMBER_OF_SAMPLES is equal to the number of samples in the first trace <br>
		/// DATA_LENGTH is equal to the binary data size of the first trace <br>
		/// TITLE_SPACE is defined by the length of the first trace title (including spaces) <br>
		/// SCALE_X is defined for the whole set based on the sampling frequency of the first trace <br>
		/// SAMPLE_CODING is defined for the whole set based on the values of the first trace <br> </summary>
		/// <param name="file"> the path to the file to be created </param>
		/// <returns> a writable trace set object </returns>
		/// <exception cref="IOException"> if the file creation failed </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static TraceSet create(String file) throws java.io.IOException
		public static TraceSet create(string file)
		{
			TRSMetaData trsMetaData = TRSMetaData.create();
			return create(file, trsMetaData);
		}

		/// <summary>
		/// Create a new traceset file at the specified location. <br>
		/// NOTE: The supplied metadata is leading, and is not overwritten.
		/// Please make sure that the supplied values are correct <br>
		/// Every next trace is expected to adhere to the following parameters: <br>
		/// NUMBER_OF_SAMPLES is equal to the number of samples in the first trace <br>
		/// DATA_LENGTH is equal to the binary data size of the first trace <br>
		/// TITLE_SPACE is defined by the length of the first trace title (including spaces) <br>
		/// SCALE_X is defined for the whole set based on the sampling frequency of the first trace <br>
		/// SAMPLE_CODING is defined for the whole set based on the values of the first trace <br> </summary>
		/// <param name="file"> the path to the file to be created </param>
		/// <param name="metaData"> the user-supplied meta data </param>
		/// <returns> a writable trace set object </returns>
		/// <exception cref="IOException"> if the file creation failed </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static TraceSet create(String file, TRSMetaData metaData) throws java.io.IOException
		public static TraceSet create(string file, TRSMetaData metaData)
		{
			metaData.put(TRS_VERSION, 2, false);
			return new TraceSet(file, metaData);
		}
	}

}