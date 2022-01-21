using com.riscure.trs.parameter.trace;

namespace com.riscure.trs
{
    using Encoding = enums.Encoding;

    /// <summary>
    /// Trace contains the data related to one consecutive array of samples,
    /// including potential associated data and a title
    /// </summary>
    public class Trace
    {
        private const string TO_STRING_FORMAT = "Trace{Title='%s', numberOfSamples=%d, shifted=%d, " + "aggregatesValid=%b, hasIllegalValues=%b, isReal=%b, max=%f, min=%f%n%s}";

        /// <summary>
        /// Factory method. This will copy the sample array for stability. </summary>
        /// <param name="sample"> the sample array </param>
        /// <returns> a new trace object holding the provided samples </returns>
        public static Trace Create(float[] sample)
        {
            return new Trace((float[])sample.Clone());
        }

        /// <summary>
        /// Factory method. This will copy the provided arrays for stability. </summary>
        /// <param name="title"> the trace title </param>
        /// <param name="sample"> the sample array </param>
        /// <param name="parameters"> the parameters to be saved with every trace </param>
        /// <returns> a new trace object holding the provided information </returns>
        public static Trace Create(string title, float[] sample, TraceParameterMap parameters)
        {
            return new Trace(title, (float[])sample.Clone(), parameters);
        }

        /// <summary>
        /// Creates a new instance of Trace which contains only a sample array
        /// Do not modify the sample array, it may be used in the core!
        /// </summary>
        /// <param name="sample"> Sample values. Do not modify </param>
        public Trace(float[] sample)
        {
            Sample = (float[])sample.Clone();
        }

        /// <summary>
        /// Creates a new instance of Trace containing title, (crypto) data and sample array
        /// Do not modify the sample array, it may be used in the core!
        /// </summary>
        /// <param name="title"> Local title for this trace </param>
        /// <param name="sample"> Sample values. Do not modify </param>
        /// <param name="parameters"> the parameters to be saved with every trace. For backwards compatibility,
        ///                   these values are also stored as a raw byte array </param>
        public Trace(string title, float[] sample, TraceParameterMap parameters)
        {
            this.title = title;
            Sample = (float[])sample.Clone();
            Parameters = parameters;
        }

        /// <summary>
        /// Get the sample array, no shift corrections are done. </summary>
        /// <returns> the sample array </returns>
        public float[] Sample { get; }

        /// <summary>
        /// Force float sample coding
        /// </summary>
        public void ForceFloatCoding()
        {
            isReal = true;
        }

        /// <summary>
        /// Get the preferred data type to store samples
        /// </summary>
        /// <returns> the preferred data type to store samples
        ///  </returns>
        public int PreferredCoding
        {
            get
            {
                if (!aggregatesValid)
                {
                    UpdateAggregates();
                }

                Encoding preferredCoding;
                if (hasIllegalValues)
                {
                    preferredCoding = Encoding.ILLEGAL;
                }
                else if (isReal)
                {
                    preferredCoding = Encoding.FLOAT;
                }
                else if (max > short.MaxValue || min < short.MinValue)
                {
                    preferredCoding = Encoding.INT;
                }
                else if (max > sbyte.MaxValue || min < sbyte.MinValue)
                {
                    preferredCoding = Encoding.SHORT;
                }
                else
                {
                    preferredCoding = Encoding.BYTE;
                }

                return preferredCoding.Value;
            }
        }

        private void UpdateAggregates()
        {
            foreach (float f in Sample)
            {
                max = Math.Max(f, max);
                min = Math.Min(f, min);
                isReal |= f != (int)f;
                hasIllegalValues |= float.IsNaN(f) || float.IsInfinity(f) || f == float.PositiveInfinity;
            }

            aggregatesValid = true;
        }

        /// <summary>
        /// Get the trace title
        /// </summary>
        /// <returns> The title </returns>
        public string? Title { get; set; }

        /// <summary>
        /// Get the supplementary (crypto) data of this trace.
        /// </summary>
        /// <returns> the supplementary data of this trace </returns>
        public byte[]? Data
        {
            get => Parameters.Serialize();
        }

        /// <summary>
        /// A map of all custom named trace parameters
        /// Get the parameters of this trace
        /// </summary>
        /// <returns> the parameters of this trace </returns>
        public TraceParameterMap Parameters { get; } = new();


        /// <summary>
        /// Get the supplementary (crypto) data of this trace as a hexadecimal string.
        /// </summary>
        /// <returns> the supplementary (crypto) data of this trace as a hexadecimal string </returns>
        public string DataString
        {
            get
            {
                if (Data == null) return string.Empty;
                return BitConverter.ToString(Data).Replace("-", "");
            }
        }

        /// <summary>
        /// Get the number of samples that this trace is shifted.
        /// </summary>
        /// <returns> the number of samples that this trace is shifted </returns>
        public int Shifted { get; set; }

        /// <summary>
        /// Get the length of the sample array.
        /// </summary>
        /// <returns> the length of the sample array </returns>
        public int NumberOfSamples => Sample.Length;

        /// <returns> the trace set containing this trace, or null if not set </returns>
        public TraceSet? TraceSet { get; set; }


        public override string ToString()
        {
            return string.Format(TO_STRING_FORMAT, title, Sample.Length, shifted, aggregatesValid, 
                hasIllegalValues, isReal, max, min, Parameters);
        }
        
        /// <summary>
        /// trace title </summary>
        private readonly string title = string.Empty;
        /// <summary>
        /// number of samples shifted </summary>
        private readonly int shifted = 0;
        /// <summary>
        /// Indicates whether the aggregates (<seealso cref="hasIllegalValues"/>, 
        /// <seealso cref="isReal"/> <seealso cref="max"/>, <seealso cref="min"/>) 
        /// are valid. </summary>
        private bool aggregatesValid = false;
        /// <summary>
        /// whether the trace contains illegal float values </summary>
        private bool hasIllegalValues = false;
        /// <summary>
        /// whether the trace contains real values </summary>
        private bool isReal = false;
        /// <summary>
        /// maximal value used in trace </summary>
        private float max = 0;
        /// <summary>
        /// minimal value used in trace </summary>
        private float min = 0;
    }

}