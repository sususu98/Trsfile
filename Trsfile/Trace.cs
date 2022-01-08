using System;
using System.Numerics;

namespace com.riscure.trs
{
	using Encoding = com.riscure.trs.enums.Encoding;
	using TraceParameterMap = com.riscure.trs.parameter.trace.TraceParameterMap;


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
		public static Trace create(float[] sample)
		{
			return new Trace((float[])sample.Clone());
		}

		/// <summary>
		/// Factory method. This will copy the provided arrays for stability. </summary>
		/// <param name="title"> the trace title </param>
		/// <param name="sample"> the sample array </param>
		/// <param name="parameters"> the parameters to be saved with every trace </param>
		/// <returns> a new trace object holding the provided information </returns>
		public static Trace create(string title, float[] sample, TraceParameterMap parameters)
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
			this.sample = FloatBuffer.wrap(sample);
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
			this.sample = FloatBuffer.wrap(sample);
			this.parameters = parameters;
		}

		/// <summary>
		/// Get the sample array, no shift corrections are done. </summary>
		/// <returns> the sample array </returns>
		public virtual float[] Sample
		{
			get
			{
				return sample.array();
			}
		}

		/// <summary>
		/// Force float sample coding
		/// </summary>
		public virtual void forceFloatCoding()
		{
			isReal = true;
		}

		/// <summary>
		/// Get the preferred data type to store samples
		/// </summary>
		/// <returns> the preferred data type to store samples
		///  </returns>
		public virtual int PreferredCoding
		{
			get
			{
				if (!aggregatesValid)
				{
					updateAggregates();
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

		private void updateAggregates()
		{
			foreach (float f in Sample)
			{
				max = Math.Max(f, max);
				min = Math.Min(f, min);
				isReal |= f != (int) f;
				hasIllegalValues |= float.IsNaN(f) || float.IsInfinity(f) || f == float.PositiveInfinity;
			}

			aggregatesValid = true;
		}

		/// <summary>
		/// Get the trace title
		/// </summary>
		/// <returns> The title </returns>
		public virtual string Title
		{
			get
			{
				return title;
			}
			set
			{
				this.title = value;
			}
		}


		/// <summary>
		/// Get the supplementary (crypto) data of this trace.
		/// </summary>
		/// <returns> the supplementary data of this trace </returns>
		public virtual sbyte[] Data
		{
			get
			{
				return parameters.toByteArray();
			}
		}

		/// <summary>
		/// Get the parameters of this trace
		/// </summary>
		/// <returns> the parameters of this trace </returns>
		public virtual TraceParameterMap Parameters
		{
			get
			{
				return parameters;
			}
		}

		/// <summary>
		/// Get the supplementary (crypto) data of this trace as a hexadecimal string.
		/// </summary>
		/// <returns> the supplementary (crypto) data of this trace as a hexadecimal string </returns>
		public virtual string DataString
		{
			get
			{
				return (new BigInteger(Data)).toString(16);
			}
		}

		/// <summary>
		/// Get the number of samples that this trace is shifted.
		/// </summary>
		/// <returns> the number of samples that this trace is shifted </returns>
		public virtual int Shifted
		{
			get
			{
				return shifted;
			}
			set
			{
				this.shifted = value;
			}
		}


		/// <summary>
		/// Get the length of the sample array.
		/// </summary>
		/// <returns> the length of the sample array </returns>
		public virtual int NumberOfSamples
		{
			get
			{
				return sample.limit();
			}
		}

		/// <returns> the trace set containing this trace, or null if not set </returns>
		public virtual TraceSet TraceSet
		{
			get
			{
				return traceSet;
			}
			set
			{
				this.traceSet = value;
			}
		}


		public override string ToString()
		{
			return string.Format(TO_STRING_FORMAT, title, sample.array().length, shifted, aggregatesValid, hasIllegalValues, isReal, max, min, parameters);
		}

		/// <summary>
		/// A map of all custom named trace parameters </summary>
		private TraceParameterMap parameters = new TraceParameterMap();
		/// <summary>
		/// list of samples </summary>
		private readonly FloatBuffer sample;
		/// <summary>
		/// trace title </summary>
		private string title = null;
		/// <summary>
		/// number of samples shifted </summary>
		private int shifted = 0;
		/// <summary>
		/// trace set including this trace </summary>
		private TraceSet traceSet = null;
		/// <summary>
		/// Indicates whether the aggregates (<seealso cref="Trace.hasIllegalValues"/>, <seealso cref="Trace.isReal"/> <seealso cref="Trace.max"/>, <seealso cref="Trace.min"/>) are valid. </summary>
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