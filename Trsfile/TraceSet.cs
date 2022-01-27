using Trsfile.Enums;
using Trsfile.Parameter.Primitive;
using Trsfile.Parameter.Trace;
using Trsfile.Parameter.Trace.Definition;
using System.Text;
using System.IO.MemoryMappedFiles;
using static Trsfile.Enums.TRSTag;
using Encoding = Trsfile.Enums.Encoding;

namespace Trsfile
{
#pragma warning disable CS8604, CS8618
    public class TraceSet : IDisposable
    {
        private const string ERROR_READING_FILE = "Error reading TRS file: file size ({0:d}) != meta data ({1:d}) + trace size ({2:d}) * nr of traces ({3:d})";
        private const string TRACE_SET_NOT_OPEN = "TraceSet has not been opened or has been closed.";
        private const string TRACE_SET_IN_WRITE_MODE = "TraceSet is in write mode. Please open the TraceSet in read mode.";
        private const string TRACE_INDEX_OUT_OF_BOUNDS = "Requested trace index ({0:d}) is larger than the total number of available traces ({1:d}).";
        private const string TRACE_SET_IN_READ_MODE = "TraceSet is in read mode. Please open the TraceSet in write mode.";
        private const string TRACE_LENGTH_DIFFERS = "All traces in a set need to be the same length, but current trace length ({0:d}) differs from the previous trace(s) ({1:d})";
        private const string TRACE_DATA_LENGTH_DIFFERS = "All traces in a set need to have the same data length, but current trace data length ({0:d}) differs from the previous trace(s) ({1:d})";
        private const string UNKNOWN_SAMPLE_CODING = "Error reading TRS file: unknown sample coding '{0:d}'";
        private static readonly long MAX_BUFFER_SIZE = int.MaxValue;
        private const string PARAMETER_NOT_DEFINED = "Parameter {0} is saved in the trace, but was not found in the header definition";
        private static readonly System.Text.Encoding UTF8_IGNORE_DECODER =
            System.Text.Encoding.GetEncoding(System.Text.Encoding.UTF8.CodePage,
                System.Text.Encoding.UTF8.EncoderFallback,
                new DecoderReplacementFallback(""));

        //Reading variables
        private int metaDataSize;
        //private FileChannel channel;
        private FileInfo info;
        private MemoryMappedFile mmf;

        private MemoryMappedViewStream buffer;

        private long bufferStart; //the byte index of the file where the buffer window starts
        private long bufferSize; //the number of bytes that are in the buffer window
        private long fileSize; //the total number of bytes in the underlying file

        //Writing variables
        private FileStream writeStream;

        private bool firstTrace = true;
        private bool open_Conflict;
        private readonly bool writing; //whether the trace is opened in write mode
        private readonly string filePath;

        private TraceSet(string inputFileName)
        {
            this.writing = false;
            this.open_Conflict = true;
            this.filePath = Path.GetFullPath(inputFileName);
            this.info = new FileInfo(inputFileName);

            //the file might be bigger than the buffer, in which case we partially buffer it in memory
            this.fileSize = this.info.Length;
            this.bufferStart = 0L;
            this.bufferSize = Math.Min(fileSize, MAX_BUFFER_SIZE);

            MapBuffer();
            this.MetaData = TRSMetaDataUtils.ReadTRSMetaData(buffer); // buffer is not null through MapBuffer()
            this.metaDataSize = (int)buffer.Position;
        }

        private TraceSet(string outputFileName, TRSMetaData metaData)
        {
            this.open_Conflict = true;
            this.writing = true;
            this.MetaData = metaData;
            this.filePath = Path.GetFullPath(outputFileName);
            this.writeStream = new FileStream(outputFileName, FileMode.Create, FileAccess.Write);
        }

        /// <returns> the Path on disk of this trace set </returns>
        public virtual string FilePath { get { return filePath; } }
        private void MapBuffer()
        {
            mmf = MemoryMappedFile.CreateFromFile(filePath, FileMode.Open);
            this.buffer = mmf.CreateViewStream(bufferStart, bufferSize, MemoryMappedFileAccess.Read);

            //this.buffer = this.channel.map(FileChannel.MapMode.READ_ONLY, this.bufferStart, this.bufferSize);

        }

        private void MoveBufferIfNecessary(int traceIndex)
        {
            long traceSize = CalculateTraceSize();
            long start = metaDataSize + (long)traceIndex * traceSize;
            long end = start + traceSize;

            bool moveRequired = start < this.bufferStart || this.bufferStart + this.bufferSize < end;
            if (moveRequired)
            {
                this.bufferStart = start;
                this.bufferSize = Math.Min(this.fileSize - start, MAX_BUFFER_SIZE);
                this.MapBuffer();
            }
        }

        private long CalculateTraceSize()
        {
            int sampleSize = Encoding.FromValue(MetaData.GetInt(SAMPLE_CODING)).Size;
            long sampleSpace = MetaData.GetInt(NUMBER_OF_SAMPLES) * (long)sampleSize;
            return sampleSpace + MetaData.GetInt(DATA_LENGTH) + MetaData.GetInt(TITLE_SPACE);
        }

        /// <summary>
        /// Get a trace from the set at the specified index </summary>
        /// <param name="index"> the index of the Trace to read from the file </param>
        /// <returns> the Trace at the requested trace index </returns>
        public virtual Trace Get(int index)
        {
            if (!open_Conflict)
            {
                throw new ArgumentException(TRACE_SET_NOT_OPEN);
            }
            if (writing)
            {
                throw new ArgumentException(TRACE_SET_IN_WRITE_MODE);
            }


            long traceSize = CalculateTraceSize();
            long nrOfTraces = MetaData.GetInt(NUMBER_OF_TRACES);
            if (index >= nrOfTraces)
            {
                string msg = string.Format(TRACE_INDEX_OUT_OF_BOUNDS, index, nrOfTraces);
                throw new ArgumentException(msg);
            }

            long calculatedFileSize = metaDataSize + traceSize * nrOfTraces;
            if (fileSize != calculatedFileSize)
            {
                string msg = string.Format(ERROR_READING_FILE, fileSize, metaDataSize, traceSize, nrOfTraces);
                throw new InvalidOperationException(msg);
            }
            MoveBufferIfNecessary(index);

            long absolutePosition = metaDataSize + index * traceSize;
            buffer.Position = absolutePosition - this.bufferStart;

            string traceTitle = ReadTraceTitle();
            if (traceTitle.Trim().Length == 0)
            {
                traceTitle = string.Format("{0} {1:D}", MetaData.GetString(GLOBAL_TITLE), index);
            }

            try
            {
                TraceParameterMap traceParameterMap;
                if (MetaData.GetInt(TRS_VERSION) > 1)
                {
                    TraceParameterDefinitionMap traceParameterDefinitionMap = MetaData.TraceParameterDefinitions;
                    int size = traceParameterDefinitionMap.TotalByteSize();
                    byte[] data = new byte[size];
                    buffer.Read(data, 0, size);
                    traceParameterMap = TraceParameterMap.Deserialize(data, traceParameterDefinitionMap);
                }
                else
                {
                    //legacy mode
                    byte[] data = ReadData();
                    traceParameterMap = new TraceParameterMap
                    {
                        { "LEGACY_DATA", data }
                    };
                }

                float[] samples = ReadSamples();
                return new Trace(traceTitle, samples, traceParameterMap);
            }
            catch (TRSFormatException ex)
            {
                throw new IOException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Add a trace to a writable TraceSet </summary>
        /// <param name="trace"> the Trace object to add </param>
        public void Add(Trace trace)
        {
            if (!open_Conflict)
            {
                throw new ArgumentException(TRACE_SET_NOT_OPEN);
            }
            if (!writing)
            {
                throw new ArgumentException(TRACE_SET_IN_READ_MODE);
            }
            if (firstTrace)
            {
                int dataLength = trace.Data == null ? 0 : trace.Data.Length;
                int titleLength = string.ReferenceEquals(trace.Title, null) ? 0 : trace.Title.GetBytes(System.Text.Encoding.UTF8).Length;
                MetaData.Add(NUMBER_OF_SAMPLES, trace.NumberOfSamples, false);
                MetaData.Add(DATA_LENGTH, dataLength, false);
                MetaData.Add(TITLE_SPACE, titleLength, false);
                MetaData.Add(SAMPLE_CODING, trace.PreferredCoding, false);
                MetaData.Add(TRACE_PARAMETER_DEFINITIONS, TraceParameterDefinitionMap.CreateFrom(trace.Parameters));
                TRSMetaDataUtils.WriteTRSMetaData(writeStream, MetaData);
                firstTrace = false;
            }
            TruncateStrings(trace, MetaData);
            CheckValid(trace);

            trace.TraceSet = this;
            WriteTrace(trace);

            int numberOfTraces = MetaData.GetInt(NUMBER_OF_TRACES);
            MetaData.Add(NUMBER_OF_TRACES, numberOfTraces + 1);
        }

        /// <summary>
        /// This method makes sure that the trace title and any added string parameters adhere to the preset maximum length </summary>
        /// <param name="trace"> the trace to update </param>
        /// <param name="metaData"> the metadata specifying the maximum string lengths </param>
        private static void TruncateStrings(Trace trace, TRSMetaData metaData)
        {
            int titleSpace = metaData.GetInt(TITLE_SPACE);
            trace.Title = FitUtf8StringToByteLength(trace.Title, titleSpace);
            TraceParameterDefinitionMap traceParameterDefinitionMap = metaData.TraceParameterDefinitions;
            foreach (var definition in traceParameterDefinitionMap)
            {
                var value = definition.Value;
                string key = definition.Key;
                if (value.Type == ParameterType.STRING)
                {
                    short stringLength = value.Length;
                    if (trace.Parameters[key] is not StringParameter sp)
                        throw new ArgumentException(nameof(trace.Parameters));
                    string stringValue = sp.ScalarValue;
                    if (stringLength != stringValue.GetBytes(System.Text.Encoding.UTF8).Length)
                    {
                        trace.Parameters.Add(key, FitUtf8StringToByteLength(stringValue, stringLength));
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
        private static string? FitUtf8StringToByteLength(string s, int maxBytes)
        {
            if (s is null)
            {
                return null;
            }
            byte[] sba = s.GetBytes(System.Text.Encoding.UTF8);
            if (sba.Length <= maxBytes)
            {
                // return System.Text.Encoding.UTF8.GetString(sba[..maxBytes]);
                byte[] bytes = new byte[maxBytes];
                Array.Copy(sba, 0, bytes, 0, sba.Length);
                return System.Text.Encoding.UTF8.GetString(bytes);
            }
            // Ensure truncation by having byte buffer = maxBytes
            var bb = sba.AsSpan(0, maxBytes);
            return UTF8_IGNORE_DECODER.GetString(bb);
        }

        private void WriteTrace(Trace trace)
        {
            string title = trace.Title is null ? "" : trace.Title;
            writeStream.Write(title.GetBytes(System.Text.Encoding.UTF8));
            byte[] data = trace.Data is null ? Array.Empty<byte>() : trace.Data;
            writeStream.Write(data, 0, data.Length);
            Encoding encoding = Encoding.FromValue(MetaData.GetInt(SAMPLE_CODING));
            writeStream.Write(ToByteArray(trace.Sample, encoding), 0, ToByteArray(trace.Sample, encoding).Length);
        }
        private byte[] ToByteArray(float[] samples, Encoding encoding)
        {
            byte[] result;
            switch (encoding.innerEnumValue)
            {
                case Encoding.InnerEnum.ILLEGAL:
                    throw new TRSFormatException("Illegal sample encoding");
                case Encoding.InnerEnum.BYTE:
                    result = new byte[samples.Length];
                    for (int k = 0; k < samples.Length; k++)
                    {
                        if (samples[k] != (byte)samples[k])
                        {
                            throw new System.ArgumentException("Byte sample encoding too small");
                        }
                        result[k] = (byte)samples[k];
                    }
                    break;
                case Encoding.InnerEnum.SHORT:
                    result = new byte[samples.Length * 2];
                    for (int k = 0; k < samples.Length; k++)
                    {
                        if (samples[k] != (short)samples[k])
                        {
                            throw new System.ArgumentException("Short sample encoding too small");
                        }
                        short value = (short)samples[k];
                        result[2 * k] = (byte)value;
                        result[2 * k + 1] = (byte)(value >> 8);
                    }
                    break;
                case Encoding.InnerEnum.INT:
                    result = new byte[samples.Length * 4];
                    for (int k = 0; k < samples.Length; k++)
                    {
                        int value = (int)samples[k];
                        result[4 * k] = (byte)value;
                        result[4 * k + 1] = (byte)(value >> 8);
                        result[4 * k + 2] = (byte)(value >> 16);
                        result[4 * k + 3] = (byte)(value >> 24);
                    }
                    break;
                case Encoding.InnerEnum.FLOAT:
                    result = new byte[samples.Length * 4];
                    for (int k = 0; k < samples.Length; k++)
                    {
                        int value = BitConverter.SingleToInt32Bits(samples[k]);
                        result[4 * k] = (byte)value;
                        result[4 * k + 1] = (byte)(value >> 8);
                        result[4 * k + 2] = (byte)(value >> 16);
                        result[4 * k + 3] = (byte)(value >> 24);
                    }
                    break;
                default:
                    throw new TRSFormatException(string.Format("Sample encoding not supported: {0}", encoding.ToString()));
            }
            return result;
        }

        private bool closed = false;
        public void Close()
        {
            open_Conflict = false;
            if (writing)
            {
                CloseWriter();
            }
            else
            {
                CloseReader();
            }
            closed = true;
        }

        private void CheckValid(Trace trace)
        {
            int numberOfSamples = MetaData.GetInt(NUMBER_OF_SAMPLES);
            if (MetaData.GetInt(NUMBER_OF_SAMPLES) != trace.NumberOfSamples)
            {
                throw new System.ArgumentException(string.Format(TRACE_LENGTH_DIFFERS, trace.NumberOfSamples, numberOfSamples));
            }

            int dataLength = MetaData.GetInt(DATA_LENGTH);
            int traceDataLength = trace.Data == null ? 0 : trace.Data.Length;
            if (MetaData.GetInt(DATA_LENGTH) != traceDataLength)
            {
                throw new System.ArgumentException(string.Format(TRACE_DATA_LENGTH_DIFFERS, traceDataLength, dataLength));
            }

            foreach (var entry in trace.Parameters)
            {
                if (!MetaData.TraceParameterDefinitions.ContainsKey(entry.Key))
                {
                    throw new System.ArgumentException(string.Format(PARAMETER_NOT_DEFINED, entry.Key));
                }
            }
        }
        private void CloseReader()
        {
            mmf.Dispose();
        }

        private void CloseWriter()
        {
            try
            {
                //reset writer to start of file and overwrite header
                writeStream.Position = 0;
                TRSMetaDataUtils.WriteTRSMetaData(writeStream, MetaData);
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
        public virtual TRSMetaData MetaData { get; }

        protected internal virtual string ReadTraceTitle()
        {
            byte[] titleArray = new byte[MetaData.GetInt(TITLE_SPACE)];
            buffer.Read(titleArray);
            return StringHelper.NewString(titleArray);
        }

        protected internal virtual byte[] ReadData()
        {
            int inputSize = MetaData.GetInt(DATA_LENGTH);
            byte[] comDataArray = new byte[inputSize];
            buffer.Read(comDataArray);
            return comDataArray;
        }

        protected internal virtual float[] ReadSamples()
        {
            int numberOfSamples = MetaData.GetInt(NUMBER_OF_SAMPLES);
            float[] samples;
            byte[] temp;
            switch (Encoding.FromValue(MetaData.GetInt(SAMPLE_CODING)))
            {
                case Encoding e when e == Encoding.BYTE:
                    byte[] byteData = new byte[numberOfSamples];
                    buffer.Read(byteData);
                    samples = ToFloatArray(byteData);
                    break;
                case Encoding e when e == Encoding.SHORT:
                    short[] shortData = new short[numberOfSamples];
                    temp = new byte[numberOfSamples * sizeof(short)];
                    buffer.Read(temp);
                    Buffer.BlockCopy(temp, 0, shortData, 0, temp.Length);
                    samples = ToFloatArray(shortData);
                    break;
                case Encoding e when e == Encoding.FLOAT:
                    samples = new float[numberOfSamples];
                    temp = new byte[numberOfSamples * sizeof(float)];
                    buffer.Read(temp);
                    Buffer.BlockCopy(temp, 0, samples, 0, temp.Length);
                    break;
                case Encoding e when e == Encoding.INT:
                    int[] intData = new int[numberOfSamples];
                    temp = new byte[numberOfSamples * sizeof(int)];
                    buffer.Read(temp);
                    Buffer.BlockCopy(temp, 0, intData, 0, temp.Length);
                    samples = ToFloatArray(intData);
                    break;
                default:
                    throw new TRSFormatException(string.Format(UNKNOWN_SAMPLE_CODING, MetaData.GetInt(SAMPLE_CODING)));
            }

            return samples;
        }

        private float[] ToFloatArray(byte[] numbers)
        {
            float[] result = new float[numbers.Length];
            for (int k = 0; k < numbers.Length; k++)
            {
                result[k] = numbers[k];
            }
            return result;
        }

        private float[] ToFloatArray(int[] numbers)
        {
            float[] result = new float[numbers.Length];
            for (int k = 0; k < numbers.Length; k++)
            {
                result[k] = (float)numbers[k];
            }
            return result;
        }

        private float[] ToFloatArray(short[] numbers)
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
        public static TraceSet Open(string file)
        {
            return new TraceSet(file);
        }

        /// <summary>
        /// A one-shot creator of a TRS file. The metadata not related to the trace list is assumed to be default. </summary>
        /// <param name="file"> the path to the file to save </param>
        /// <param name="traces"> the list of traces to save in the file </param>
        public static void Save(string file, IList<Trace> traces)
        {
            TRSMetaData trsMetaData = TRSMetaData.Create();
            Save(file, traces, trsMetaData);
        }

        /// <summary>
        /// A one-shot creator of a TRS file. Any unfilled fields of metadata are assumed to be default. </summary>
        /// <param name="file"> the path to the file to save </param>
        /// <param name="traces"> the list of traces to save in the file </param>
        /// <param name="metaData"> the metadata associated with the set to create </param>
        public static void Save(string file, IList<Trace> traces, TRSMetaData metaData)
        {
            TraceSet traceSet = Create(file, metaData);
            foreach (Trace trace in traces)
            {
                traceSet.Add(trace);
            }
            traceSet.Close();
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
        public static TraceSet Create(string file)
        {
            TRSMetaData trsMetaData = TRSMetaData.Create();
            return Create(file, trsMetaData);
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
        public static TraceSet Create(string file, TRSMetaData metaData)
        {
            metaData.Add(TRS_VERSION, 2, false);
            return new TraceSet(file, metaData);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!closed)
                {
                    Close();
                    closed = true;
                }
            }
        }
    }
#pragma warning restore CS8604, CS8618
}