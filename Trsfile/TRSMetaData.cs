﻿using System.Text;
using Trsfile.Enums;
using Trsfile.Parameter.Trace.Definition;
using Trsfile.Parameter.Traceset;

namespace Trsfile
{
    public class TRSMetaData
    {
        private const string IGNORING_NEW_VALUE = "{0}: Ignoring new value ({1}) because previously defined value is non-default ({2}) and overwrite is disabled.\n";
        private const string INCOMPATIBLE_TYPES = "Failed to add tag {0}: Expected type ({1}) does not match actual type ({2}).";

        private readonly IDictionary<TRSTag, object> metaData;

        /// <summary>
        /// Creates a TRS metadata object with all default values
        /// </summary>
        public TRSMetaData()
        {
            metaData = new Dictionary<TRSTag, object>();
            Init();
        }

        private void Init()
        {
            foreach (TRSTag tag in TRSTag.Values)
            {
                metaData[tag] = tag.DefaultValue;
            }
        }

        /// <summary>
        /// Add the data associated with the supplied tag to this metadata.
        /// This will overwrite any existing value </summary>
        /// <param name="tag"> the tag for which to save the metadata </param>
        /// <param name="data"> the data to save </param>
        public void Add(TRSTag tag, object data)
        {
            Add(tag, data, true);
        }

        /// <summary>
        /// Add the data associated with the supplied tag to this metadata </summary>
        /// <param name="tag"> the tag for which to save the metadata </param>
        /// <param name="data"> the data to save </param>
        /// <param name="overwriteNonDefault"> whether to overwrite non-default values currently saved for this tag </param>
        public void Add(TRSTag tag, object data, bool overwriteNonDefault)
        {

            if (!tag.Type.IsAssignableFrom(data.GetType()))
            {
                throw new ArgumentException(string.Format(INCOMPATIBLE_TYPES, tag.ToString(), tag.Type, data.GetType()));
            }

            if (HasDefaultValue(tag) || overwriteNonDefault)
            {
                metaData[tag] = data;
            }
            else
            {
                //System.err.printf(IGNORING_NEW_VALUE, tag.ToString(), data.ToString(), metaData[tag]);
                Console.Error.WriteLine(IGNORING_NEW_VALUE, tag.ToString(), data.ToString(), metaData[tag]);

            }
        }

        /// <summary>
        /// Get the value of the associated tag </summary>
        /// <param name="tag"> the tag to get the data for </param>
        /// <returns> the value of the supplied tag in the metadata. The type of the data is <seealso cref="typeof(TRSTag)"/>. </returns>
        public object this[TRSTag tag] => metaData[tag];

        /// <summary>
        /// Get the value of the associated tag as an int. The type of the data can be found by calling <seealso cref="TRSTag.getType()"/>. </summary>
        /// <param name="tag"> the tag to get the data for </param>
        /// <returns> the Integer value of the supplied tag in the metadata </returns>
        /// <exception cref="ClassCastException"> if the type does not match the type of the TRSTag </exception>
        public int GetInt(TRSTag tag)
        {
            return ((int?)metaData[tag]).Value;
        }

        /// <summary>
        /// Get the value of the associated tag as a String. The type of the data can be found by calling <seealso cref="TRSTag.getType()"/>. </summary>
        /// <param name="tag"> the tag to get the data for </param>
        /// <returns> the String value of the supplied tag in the metadata </returns>
        /// <exception cref="ClassCastException"> if the type does not match the type of the TRSTag </exception>
        public string GetString(TRSTag tag)
        {
            return (string)metaData[tag];
        }

        /// <summary>
        /// Get the value of the associated tag as a boolean. The type of the data can be found by calling <seealso cref="TRSTag.getType()"/>. </summary>
        /// <param name="tag"> the tag to get the data for </param>
        /// <returns> the Boolean value of the supplied tag in the metadata </returns>
        /// <exception cref="ClassCastException"> if the type does not match the type of the TRSTag </exception>
        public bool GetBoolean(TRSTag tag)
        {
            return ((bool?)metaData[tag]).Value;
        }

        /// <summary>
        /// Get the value of the associated tag as a float. The type of the data can be found by calling <seealso cref="TRSTag.getType()"/>. </summary>
        /// <param name="tag"> the tag to get the data for </param>
        /// <returns> the Float value of the supplied tag in the metadata </returns>
        /// <exception cref="ClassCastException"> if the type does not match the type of the TRSTag </exception>
        public float GetFloat(TRSTag tag)
        {
            return ((float?)metaData[tag]).Value;
        }

        /// <summary>
        /// Get the TraceSetParameters of this trace set, or an empty set if they are undefined </summary>
        /// <returns> the TraceSetParameters of this trace set </returns>
        public TraceSetParameterMap TraceSetParameters
        {
            get
            {
                object o = metaData[TRSTag.TRACE_SET_PARAMETERS];
                return o as TraceSetParameterMap ??
                    UnmodifiableTraceSetParameterMap.Of(new TraceSetParameterMap());
            }
        }

        /// <summary>
        /// Get the TraceParameterDefinitions of this trace set, or an empty set if they are undefined </summary>
        /// <returns> the TraceParameterDefinitions of this trace set </returns>
        public TraceParameterDefinitionMap TraceParameterDefinitions
        {
            get
            {
                object o = metaData[TRSTag.TRACE_PARAMETER_DEFINITIONS];
                return o as TraceParameterDefinitionMap ??
                    UnmodifiableTraceParameterDefinitionMap.Of(new TraceParameterDefinitionMap());
            }
        }

        /// <summary>
        /// Check whether the supplied tag has the default value in this metadata </summary>
        /// <param name="tag"> the tag to check the data for </param>
        /// <returns> true if the value of the supplied tag is default, false otherwise </returns>
        public bool HasDefaultValue(TRSTag tag)
        {
            return tag.DefaultValue.Equals(metaData[tag]);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (TRSTag tag in TRSTag.Values)
            {
                builder.Append(tag).Append("\n\tValue = ").Append(this[tag]).Append('\n');
            }
            return builder.ToString();
        }

        /// <summary>
        /// Factory method for convenience </summary>
        /// <returns> a new TRSMetaData instance with all default values </returns>
        public static TRSMetaData Create()
        {
            return new TRSMetaData();
        }
    }

}