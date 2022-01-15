using System;
using System.Collections.Generic;

namespace com.riscure.trs.enums
{
    using TRSFormatException = TRSFormatException;
    using TraceParameterDefinitionMap = parameter.trace.definition.TraceParameterDefinitionMap;
    using UnmodifiableTraceParameterDefinitionMap = parameter.trace.definition.UnmodifiableTraceParameterDefinitionMap;
    using TraceSetParameterMap = parameter.traceset.TraceSetParameterMap;
    using UnmodifiableTraceSetParameterMap = parameter.traceset.UnmodifiableTraceSetParameterMap;

    public sealed class TRSTag
    {
        public static readonly TRSTag NUMBER_OF_TRACES = new TRSTag("NUMBER_OF_TRACES", InnerEnum.NUMBER_OF_TRACES, 0x41, "NT", true, typeof(int), 4, 0, "Number of traces");
        public static readonly TRSTag NUMBER_OF_SAMPLES = new TRSTag("NUMBER_OF_SAMPLES", InnerEnum.NUMBER_OF_SAMPLES, 0x42, "NS", true, typeof(int), 4, 0, "Number of samples per trace");
        public static readonly TRSTag SAMPLE_CODING = new TRSTag("SAMPLE_CODING", InnerEnum.SAMPLE_CODING, 0x43, "SC", true, typeof(int), 1, Encoding.FLOAT.Value, "Sample Coding (see SampleCoding class),");
        public static readonly TRSTag DATA_LENGTH = new TRSTag("DATA_LENGTH", InnerEnum.DATA_LENGTH, 0x44, "DS", false, typeof(int), 2, 0, "Length of cryptographic data included in trace");
        public static readonly TRSTag TITLE_SPACE = new TRSTag("TITLE_SPACE", InnerEnum.TITLE_SPACE, 0x45, "TS", false, typeof(int), 1, 0, "Title space reserved per trace");
        public static readonly TRSTag GLOBAL_TITLE = new TRSTag("GLOBAL_TITLE", InnerEnum.GLOBAL_TITLE, 0x46, "GT", false, typeof(string), 0, "trace", "Global trace title");
        public static readonly TRSTag DESCRIPTION = new TRSTag("DESCRIPTION", InnerEnum.DESCRIPTION, 0x47, "DC", false, typeof(string), 0, "", "Description");
        public static readonly TRSTag OFFSET_X = new TRSTag("OFFSET_X", InnerEnum.OFFSET_X, 0x48, "XO", false, typeof(int), 4, 0, "Offset in X-axis for trace representation");
        public static readonly TRSTag LABEL_X = new TRSTag("LABEL_X", InnerEnum.LABEL_X, 0x49, "XL", false, typeof(string), 0, "", "Label of X-axis");
        public static readonly TRSTag LABEL_Y = new TRSTag("LABEL_Y", InnerEnum.LABEL_Y, 0x4A, "YL", false, typeof(string), 0, "", "Label of Y-axis");
        public static readonly TRSTag SCALE_X = new TRSTag("SCALE_X", InnerEnum.SCALE_X, 0x4B, "XS", false, typeof(float), 4, 1f, "Scale value for X-axis");
        public static readonly TRSTag SCALE_Y = new TRSTag("SCALE_Y", InnerEnum.SCALE_Y, 0x4C, "YS", false, typeof(float), 4, 1f, "Scale value for Y-axis");
        public static readonly TRSTag TRACE_OFFSET = new TRSTag("TRACE_OFFSET", InnerEnum.TRACE_OFFSET, 0x4D, "TO", false, typeof(int), 4, 0, "Trace offset for displaying trace numbers");
        public static readonly TRSTag LOGARITHMIC_SCALE = new TRSTag("LOGARITHMIC_SCALE", InnerEnum.LOGARITHMIC_SCALE, 0x4E, "LS", false, typeof(Boolean), 1, false, "Logarithmic scale");
        public static readonly TRSTag TRS_VERSION = new TRSTag("TRS_VERSION", InnerEnum.TRS_VERSION, 0x4F, "VS", false, typeof(int), 1, 0, "The version of the traceset format");
        public static readonly TRSTag ACQUISITION_RANGE_OF_SCOPE = new TRSTag("ACQUISITION_RANGE_OF_SCOPE", InnerEnum.ACQUISITION_RANGE_OF_SCOPE, 0x55, "RG", false, typeof(float), 4, 0f, "Range of the scope used to perform acquisition");
        public static readonly TRSTag ACQUISITION_COUPLING_OF_SCOPE = new TRSTag("ACQUISITION_COUPLING_OF_SCOPE", InnerEnum.ACQUISITION_COUPLING_OF_SCOPE, 0x56, "CL", false, typeof(int), 4, 0, "Coupling of the scope used to perform acquisition");
        public static readonly TRSTag ACQUISITION_OFFSET_OF_SCOPE = new TRSTag("ACQUISITION_OFFSET_OF_SCOPE", InnerEnum.ACQUISITION_OFFSET_OF_SCOPE, 0x57, "OS", false, typeof(float), 4, 0f, "Offset of the scope used to perform acquisition");
        public static readonly TRSTag ACQUISITION_INPUT_IMPEDANCE = new TRSTag("ACQUISITION_INPUT_IMPEDANCE", InnerEnum.ACQUISITION_INPUT_IMPEDANCE, 0x58, "II", false, typeof(float), 4, 0f, "Input impedance of the scope used to perform acquisition");
        public static readonly TRSTag ACQUISITION_DEVICE_ID = new TRSTag("ACQUISITION_DEVICE_ID", InnerEnum.ACQUISITION_DEVICE_ID, 0x59, "AI", false, typeof(string), 0, "", "Device ID of the scope used to perform acquisition");
        public static readonly TRSTag ACQUISITION_TYPE_FILTER = new TRSTag("ACQUISITION_TYPE_FILTER", InnerEnum.ACQUISITION_TYPE_FILTER, 0x5A, "FT", false, typeof(int), 4, 0, "The type of filter used during acquisition");
        public static readonly TRSTag ACQUISITION_FREQUENCY_FILTER = new TRSTag("ACQUISITION_FREQUENCY_FILTER", InnerEnum.ACQUISITION_FREQUENCY_FILTER, 0x5B, "FF", false, typeof(float), 4, 0f, "Frequency of the filter used during acquisition");
        public static readonly TRSTag ACQUISITION_RANGE_FILTER = new TRSTag("ACQUISITION_RANGE_FILTER", InnerEnum.ACQUISITION_RANGE_FILTER, 0x5C, "FR", false, typeof(float), 4, 0f, "Range of the filter used during acquisition");
        public static readonly TRSTag TRACE_BLOCK = new TRSTag("TRACE_BLOCK", InnerEnum.TRACE_BLOCK, 0x5F, "TB", true, typeof(int), 0, 0, "Trace block marker: an empty TLV that marks the end of the header");
        public static readonly TRSTag EXTERNAL_CLOCK_USED = new TRSTag("EXTERNAL_CLOCK_USED", InnerEnum.EXTERNAL_CLOCK_USED, 0x60, "EU", false, typeof(Boolean), 1, false, "External clock used");
        public static readonly TRSTag EXTERNAL_CLOCK_THRESHOLD = new TRSTag("EXTERNAL_CLOCK_THRESHOLD", InnerEnum.EXTERNAL_CLOCK_THRESHOLD, 0x61, "ET", false, typeof(float), 4, 0f, "External clock threshold");
        public static readonly TRSTag EXTERNAL_CLOCK_MULTIPLIER = new TRSTag("EXTERNAL_CLOCK_MULTIPLIER", InnerEnum.EXTERNAL_CLOCK_MULTIPLIER, 0x62, "EM", false, typeof(int), 4, 0, "External clock multiplier");
        public static readonly TRSTag EXTERNAL_CLOCK_PHASE_SHIFT = new TRSTag("EXTERNAL_CLOCK_PHASE_SHIFT", InnerEnum.EXTERNAL_CLOCK_PHASE_SHIFT, 0x63, "EP", false, typeof(int), 4, 0, "External clock phase shift");
        public static readonly TRSTag EXTERNAL_CLOCK_RESAMPLER_MASK = new TRSTag("EXTERNAL_CLOCK_RESAMPLER_MASK", InnerEnum.EXTERNAL_CLOCK_RESAMPLER_MASK, 0x64, "ER", false, typeof(int), 4, 0, "External clock resampler mask");
        public static readonly TRSTag EXTERNAL_CLOCK_RESAMPLER_ENABLED = new TRSTag("EXTERNAL_CLOCK_RESAMPLER_ENABLED", InnerEnum.EXTERNAL_CLOCK_RESAMPLER_ENABLED, 0x65, "RE", false, typeof(Boolean), 1, false, "External clock resampler enabled");
        public static readonly TRSTag EXTERNAL_CLOCK_FREQUENCY = new TRSTag("EXTERNAL_CLOCK_FREQUENCY", InnerEnum.EXTERNAL_CLOCK_FREQUENCY, 0x66, "EF", false, typeof(float), 4, 0f, "External clock frequency");
        public static readonly TRSTag EXTERNAL_CLOCK_BASE = new TRSTag("EXTERNAL_CLOCK_BASE", InnerEnum.EXTERNAL_CLOCK_BASE, 0x67, "EB", false, typeof(int), 4, 0, "External clock time base");
        public static readonly TRSTag NUMBER_VIEW = new TRSTag("NUMBER_VIEW", InnerEnum.NUMBER_VIEW, 0x68, "VT", false, typeof(int), 4, 0, "View number of traces: number of traces to show on opening");
        public static readonly TRSTag TRACE_OVERLAP = new TRSTag("TRACE_OVERLAP", InnerEnum.TRACE_OVERLAP, 0x69, "OV", false, typeof(Boolean), 1, false, "Overlap: whether to overlap traces in case of multi trace view");
        public static readonly TRSTag GO_LAST_TRACE = new TRSTag("GO_LAST_TRACE", InnerEnum.GO_LAST_TRACE, 0x6A, "GL", false, typeof(Boolean), 1, false, "Go to last trace on opening");
        public static readonly TRSTag INPUT_OFFSET = new TRSTag("INPUT_OFFSET", InnerEnum.INPUT_OFFSET, 0x6B, "IO", false, typeof(int), 4, 0, "Input data offset in trace data");
        public static readonly TRSTag OUTPUT_OFFSET = new TRSTag("OUTPUT_OFFSET", InnerEnum.OUTPUT_OFFSET, 0x6C, "OO", false, typeof(int), 4, 0, "Output data offset in trace data");
        public static readonly TRSTag KEY_OFFSET = new TRSTag("KEY_OFFSET", InnerEnum.KEY_OFFSET, 0x6D, "KO", false, typeof(int), 4, 0, "Key data offset in trace data");
        public static readonly TRSTag INPUT_LENGTH = new TRSTag("INPUT_LENGTH", InnerEnum.INPUT_LENGTH, 0x6E, "IL", false, typeof(int), 4, 0, "Input data length in trace data");
        public static readonly TRSTag OUTPUT_LENGTH = new TRSTag("OUTPUT_LENGTH", InnerEnum.OUTPUT_LENGTH, 0x6F, "OL", false, typeof(int), 4, 0, "Output data length in trace data");
        public static readonly TRSTag KEY_LENGTH = new TRSTag("KEY_LENGTH", InnerEnum.KEY_LENGTH, 0x70, "KL", false, typeof(int), 4, 0, "Key data length in trace data");
        public static readonly TRSTag NUMBER_OF_ENABLED_CHANNELS = new TRSTag("NUMBER_OF_ENABLED_CHANNELS", InnerEnum.NUMBER_OF_ENABLED_CHANNELS, 0x71, "CH", false, typeof(int), 4, 0, "Number of oscilloscope channels used for measurement");
        public static readonly TRSTag NUMBER_OF_USED_OSCILLOSCOPES = new TRSTag("NUMBER_OF_USED_OSCILLOSCOPES", InnerEnum.NUMBER_OF_USED_OSCILLOSCOPES, 0x72, "NO", false, typeof(int), 4, 0, "Number of oscilloscopes used for measurement");
        public static readonly TRSTag XY_SCAN_WIDTH = new TRSTag("XY_SCAN_WIDTH", InnerEnum.XY_SCAN_WIDTH, 0x73, "WI", false, typeof(int), 4, 0, "Number of steps in the \"x\" direction during XY scan");
        public static readonly TRSTag XY_SCAN_HEIGHT = new TRSTag("XY_SCAN_HEIGHT", InnerEnum.XY_SCAN_HEIGHT, 0x74, "HE", false, typeof(int), 4, 0, "Number of steps in the \"y\" direction during XY scan");
        public static readonly TRSTag XY_MEASUREMENTS_PER_SPOT = new TRSTag("XY_MEASUREMENTS_PER_SPOT", InnerEnum.XY_MEASUREMENTS_PER_SPOT, 0x75, "ME", false, typeof(int), 4, 0, "Number of consecutive measurements done per spot during XY scan");
        public static readonly TRSTag TRACE_SET_PARAMETERS = new TRSTag("TRACE_SET_PARAMETERS", InnerEnum.TRACE_SET_PARAMETERS, 0x76, "GP", false, typeof(parameter.traceset.TraceSetParameterMap), 0, parameter.traceset.UnmodifiableTraceSetParameterMap.of(new parameter.traceset.TraceSetParameterMap()), "The set of custom global trace set parameters");
        public static readonly TRSTag TRACE_PARAMETER_DEFINITIONS = new TRSTag("TRACE_PARAMETER_DEFINITIONS", InnerEnum.TRACE_PARAMETER_DEFINITIONS, 0x77, "LP", false, typeof(parameter.trace.definition.TraceParameterDefinitionMap), 0, parameter.trace.definition.UnmodifiableTraceParameterDefinitionMap.of(new parameter.trace.definition.TraceParameterDefinitionMap()), "The set of custom local trace parameters");

        private static readonly List<TRSTag> valueList = new List<TRSTag>();

        static TRSTag()
        {
            valueList.Add(NUMBER_OF_TRACES);
            valueList.Add(NUMBER_OF_SAMPLES);
            valueList.Add(SAMPLE_CODING);
            valueList.Add(DATA_LENGTH);
            valueList.Add(TITLE_SPACE);
            valueList.Add(GLOBAL_TITLE);
            valueList.Add(DESCRIPTION);
            valueList.Add(OFFSET_X);
            valueList.Add(LABEL_X);
            valueList.Add(LABEL_Y);
            valueList.Add(SCALE_X);
            valueList.Add(SCALE_Y);
            valueList.Add(TRACE_OFFSET);
            valueList.Add(LOGARITHMIC_SCALE);
            valueList.Add(TRS_VERSION);
            valueList.Add(ACQUISITION_RANGE_OF_SCOPE);
            valueList.Add(ACQUISITION_COUPLING_OF_SCOPE);
            valueList.Add(ACQUISITION_OFFSET_OF_SCOPE);
            valueList.Add(ACQUISITION_INPUT_IMPEDANCE);
            valueList.Add(ACQUISITION_DEVICE_ID);
            valueList.Add(ACQUISITION_TYPE_FILTER);
            valueList.Add(ACQUISITION_FREQUENCY_FILTER);
            valueList.Add(ACQUISITION_RANGE_FILTER);
            valueList.Add(TRACE_BLOCK);
            valueList.Add(EXTERNAL_CLOCK_USED);
            valueList.Add(EXTERNAL_CLOCK_THRESHOLD);
            valueList.Add(EXTERNAL_CLOCK_MULTIPLIER);
            valueList.Add(EXTERNAL_CLOCK_PHASE_SHIFT);
            valueList.Add(EXTERNAL_CLOCK_RESAMPLER_MASK);
            valueList.Add(EXTERNAL_CLOCK_RESAMPLER_ENABLED);
            valueList.Add(EXTERNAL_CLOCK_FREQUENCY);
            valueList.Add(EXTERNAL_CLOCK_BASE);
            valueList.Add(NUMBER_VIEW);
            valueList.Add(TRACE_OVERLAP);
            valueList.Add(GO_LAST_TRACE);
            valueList.Add(INPUT_OFFSET);
            valueList.Add(OUTPUT_OFFSET);
            valueList.Add(KEY_OFFSET);
            valueList.Add(INPUT_LENGTH);
            valueList.Add(OUTPUT_LENGTH);
            valueList.Add(KEY_LENGTH);
            valueList.Add(NUMBER_OF_ENABLED_CHANNELS);
            valueList.Add(NUMBER_OF_USED_OSCILLOSCOPES);
            valueList.Add(XY_SCAN_WIDTH);
            valueList.Add(XY_SCAN_HEIGHT);
            valueList.Add(XY_MEASUREMENTS_PER_SPOT);
            valueList.Add(TRACE_SET_PARAMETERS);
            valueList.Add(TRACE_PARAMETER_DEFINITIONS);
        }

        public enum InnerEnum
        {
            NUMBER_OF_TRACES,
            NUMBER_OF_SAMPLES,
            SAMPLE_CODING,
            DATA_LENGTH,
            TITLE_SPACE,
            GLOBAL_TITLE,
            DESCRIPTION,
            OFFSET_X,
            LABEL_X,
            LABEL_Y,
            SCALE_X,
            SCALE_Y,
            TRACE_OFFSET,
            LOGARITHMIC_SCALE,
            TRS_VERSION,
            ACQUISITION_RANGE_OF_SCOPE,
            ACQUISITION_COUPLING_OF_SCOPE,
            ACQUISITION_OFFSET_OF_SCOPE,
            ACQUISITION_INPUT_IMPEDANCE,
            ACQUISITION_DEVICE_ID,
            ACQUISITION_TYPE_FILTER,
            ACQUISITION_FREQUENCY_FILTER,
            ACQUISITION_RANGE_FILTER,
            TRACE_BLOCK,
            EXTERNAL_CLOCK_USED,
            EXTERNAL_CLOCK_THRESHOLD,
            EXTERNAL_CLOCK_MULTIPLIER,
            EXTERNAL_CLOCK_PHASE_SHIFT,
            EXTERNAL_CLOCK_RESAMPLER_MASK,
            EXTERNAL_CLOCK_RESAMPLER_ENABLED,
            EXTERNAL_CLOCK_FREQUENCY,
            EXTERNAL_CLOCK_BASE,
            NUMBER_VIEW,
            TRACE_OVERLAP,
            GO_LAST_TRACE,
            INPUT_OFFSET,
            OUTPUT_OFFSET,
            KEY_OFFSET,
            INPUT_LENGTH,
            OUTPUT_LENGTH,
            KEY_LENGTH,
            NUMBER_OF_ENABLED_CHANNELS,
            NUMBER_OF_USED_OSCILLOSCOPES,
            XY_SCAN_WIDTH,
            XY_SCAN_HEIGHT,
            XY_MEASUREMENTS_PER_SPOT,
            TRACE_SET_PARAMETERS,
            TRACE_PARAMETER_DEFINITIONS
        }

        public readonly InnerEnum innerEnumValue;
        private readonly string nameValue;
        private readonly int ordinalValue;
        private static int nextOrdinal = 0;

        private const string UNKNOWN_TAG = "Unknown tag: 0x%X";

        private readonly byte value;
        private readonly string name;
        private readonly bool required;
        private readonly Type type;
        private readonly int length;
        private readonly object defaultValue;
        private readonly string description;

        internal TRSTag(string nameValue, InnerEnum innerEnum, int value, string name, bool required, Type type, int length, object defaultValue, string description)
        {
            this.value = (byte)value;
            this.name = name;
            this.required = required;
            this.type = type;
            this.length = length;
            this.defaultValue = defaultValue;
            this.description = description;

            this.nameValue = nameValue;
            ordinalValue = nextOrdinal++;
            innerEnumValue = innerEnum;
        }

        public byte Value
        {
            get
            {
                return value;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public bool Required
        {
            get
            {
                return required;
            }
        }

        public Type Type
        {
            get
            {
                return type;
            }
        }

        public int Length
        {
            get
            {
                return length;
            }
        }

        public object DefaultValue
        {
            get
            {
                return defaultValue;
            }
        }

        public string Description
        {
            get
            {
                return description;
            }
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
        //ORIGINAL LINE: public static TRSTag fromValue(byte value) throws TRSFormatException
        public static TRSTag FromValue(byte value)
        {
            foreach (TRSTag tag in TRSTag.Values())
            {
                if (tag.Value == value)
                {
                    return tag;
                }
            }
            throw new TRSFormatException(string.Format(UNKNOWN_TAG, value));
        }

        public override string ToString()
        {
            return string.Format("{0} (value={1:X}, name={2}, {3}required, type={4}, length={5:D}, default={6}, description={7})", nameValue, Value, Name, Required ? "" : "not ", Type.FullName, Length, DefaultValue, Description);
        }

        public static TRSTag[] Values()
        {
            return valueList.ToArray();
        }

        public int Ordinal()
        {
            return ordinalValue;
        }

        public static TRSTag ValueOf(string name)
        {
            foreach (TRSTag enumInstance in TRSTag.valueList)
            {
                if (enumInstance.nameValue == name)
                {
                    return enumInstance;
                }
            }
            throw new System.ArgumentException(name);
        }
    }

}