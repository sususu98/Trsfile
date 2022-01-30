using Trsfile.Parameter.Trace.Definition;
using Trsfile.Parameter.Traceset;

namespace Trsfile.Enums
{
    public sealed record TRSTag
    {
        public static readonly TRSTag NUMBER_OF_TRACES = new("NUMBER_OF_TRACES", TRSTagEnum.NUMBER_OF_TRACES, 0x41, "NT", true, typeof(int), 4, 0, "Number of traces");
        public static readonly TRSTag NUMBER_OF_SAMPLES = new("NUMBER_OF_SAMPLES", TRSTagEnum.NUMBER_OF_SAMPLES, 0x42, "NS", true, typeof(int), 4, 0, "Number of samples per trace");
        public static readonly TRSTag SAMPLE_CODING = new("SAMPLE_CODING", TRSTagEnum.SAMPLE_CODING, 0x43, "SC", true, typeof(int), 1, Encoding.FLOAT.Value, "Sample Coding (see SampleCoding class),");
        public static readonly TRSTag DATA_LENGTH = new("DATA_LENGTH", TRSTagEnum.DATA_LENGTH, 0x44, "DS", false, typeof(int), 2, 0, "Length of cryptographic data included in trace");
        public static readonly TRSTag TITLE_SPACE = new("TITLE_SPACE", TRSTagEnum.TITLE_SPACE, 0x45, "TS", false, typeof(int), 1, 0, "Title space reserved per trace");
        public static readonly TRSTag GLOBAL_TITLE = new("GLOBAL_TITLE", TRSTagEnum.GLOBAL_TITLE, 0x46, "GT", false, typeof(string), 0, "trace", "Global trace title");
        public static readonly TRSTag DESCRIPTION = new("DESCRIPTION", TRSTagEnum.DESCRIPTION, 0x47, "DC", false, typeof(string), 0, "", "Description");
        public static readonly TRSTag OFFSET_X = new("OFFSET_X", TRSTagEnum.OFFSET_X, 0x48, "XO", false, typeof(int), 4, 0, "Offset in X-axis for trace representation");
        public static readonly TRSTag LABEL_X = new("LABEL_X", TRSTagEnum.LABEL_X, 0x49, "XL", false, typeof(string), 0, "", "Label of X-axis");
        public static readonly TRSTag LABEL_Y = new("LABEL_Y", TRSTagEnum.LABEL_Y, 0x4A, "YL", false, typeof(string), 0, "", "Label of Y-axis");
        public static readonly TRSTag SCALE_X = new("SCALE_X", TRSTagEnum.SCALE_X, 0x4B, "XS", false, typeof(float), 4, 1f, "Scale value for X-axis");
        public static readonly TRSTag SCALE_Y = new("SCALE_Y", TRSTagEnum.SCALE_Y, 0x4C, "YS", false, typeof(float), 4, 1f, "Scale value for Y-axis");
        public static readonly TRSTag TRACE_OFFSET = new("TRACE_OFFSET", TRSTagEnum.TRACE_OFFSET, 0x4D, "TO", false, typeof(int), 4, 0, "Trace offset for displaying trace numbers");
        public static readonly TRSTag LOGARITHMIC_SCALE = new("LOGARITHMIC_SCALE", TRSTagEnum.LOGARITHMIC_SCALE, 0x4E, "LS", false, typeof(Boolean), 1, false, "Logarithmic scale");
        public static readonly TRSTag TRS_VERSION = new("TRS_VERSION", TRSTagEnum.TRS_VERSION, 0x4F, "VS", false, typeof(int), 1, 0, "The version of the traceset format");
        public static readonly TRSTag ACQUISITION_RANGE_OF_SCOPE = new("ACQUISITION_RANGE_OF_SCOPE", TRSTagEnum.ACQUISITION_RANGE_OF_SCOPE, 0x55, "RG", false, typeof(float), 4, 0f, "Range of the scope used to perform acquisition");
        public static readonly TRSTag ACQUISITION_COUPLING_OF_SCOPE = new("ACQUISITION_COUPLING_OF_SCOPE", TRSTagEnum.ACQUISITION_COUPLING_OF_SCOPE, 0x56, "CL", false, typeof(int), 4, 0, "Coupling of the scope used to perform acquisition");
        public static readonly TRSTag ACQUISITION_OFFSET_OF_SCOPE = new("ACQUISITION_OFFSET_OF_SCOPE", TRSTagEnum.ACQUISITION_OFFSET_OF_SCOPE, 0x57, "OS", false, typeof(float), 4, 0f, "Offset of the scope used to perform acquisition");
        public static readonly TRSTag ACQUISITION_INPUT_IMPEDANCE = new("ACQUISITION_INPUT_IMPEDANCE", TRSTagEnum.ACQUISITION_INPUT_IMPEDANCE, 0x58, "II", false, typeof(float), 4, 0f, "Input impedance of the scope used to perform acquisition");
        public static readonly TRSTag ACQUISITION_DEVICE_ID = new("ACQUISITION_DEVICE_ID", TRSTagEnum.ACQUISITION_DEVICE_ID, 0x59, "AI", false, typeof(string), 0, "", "Device ID of the scope used to perform acquisition");
        public static readonly TRSTag ACQUISITION_TYPE_FILTER = new("ACQUISITION_TYPE_FILTER", TRSTagEnum.ACQUISITION_TYPE_FILTER, 0x5A, "FT", false, typeof(int), 4, 0, "The type of filter used during acquisition");
        public static readonly TRSTag ACQUISITION_FREQUENCY_FILTER = new("ACQUISITION_FREQUENCY_FILTER", TRSTagEnum.ACQUISITION_FREQUENCY_FILTER, 0x5B, "FF", false, typeof(float), 4, 0f, "Frequency of the filter used during acquisition");
        public static readonly TRSTag ACQUISITION_RANGE_FILTER = new("ACQUISITION_RANGE_FILTER", TRSTagEnum.ACQUISITION_RANGE_FILTER, 0x5C, "FR", false, typeof(float), 4, 0f, "Range of the filter used during acquisition");
        public static readonly TRSTag TRACE_BLOCK = new("TRACE_BLOCK", TRSTagEnum.TRACE_BLOCK, 0x5F, "TB", true, typeof(int), 0, 0, "Trace block marker: an empty TLV that marks the end of the header");
        public static readonly TRSTag EXTERNAL_CLOCK_USED = new("EXTERNAL_CLOCK_USED", TRSTagEnum.EXTERNAL_CLOCK_USED, 0x60, "EU", false, typeof(Boolean), 1, false, "External clock used");
        public static readonly TRSTag EXTERNAL_CLOCK_THRESHOLD = new("EXTERNAL_CLOCK_THRESHOLD", TRSTagEnum.EXTERNAL_CLOCK_THRESHOLD, 0x61, "ET", false, typeof(float), 4, 0f, "External clock threshold");
        public static readonly TRSTag EXTERNAL_CLOCK_MULTIPLIER = new("EXTERNAL_CLOCK_MULTIPLIER", TRSTagEnum.EXTERNAL_CLOCK_MULTIPLIER, 0x62, "EM", false, typeof(int), 4, 0, "External clock multiplier");
        public static readonly TRSTag EXTERNAL_CLOCK_PHASE_SHIFT = new("EXTERNAL_CLOCK_PHASE_SHIFT", TRSTagEnum.EXTERNAL_CLOCK_PHASE_SHIFT, 0x63, "EP", false, typeof(int), 4, 0, "External clock phase shift");
        public static readonly TRSTag EXTERNAL_CLOCK_RESAMPLER_MASK = new("EXTERNAL_CLOCK_RESAMPLER_MASK", TRSTagEnum.EXTERNAL_CLOCK_RESAMPLER_MASK, 0x64, "ER", false, typeof(int), 4, 0, "External clock resampler mask");
        public static readonly TRSTag EXTERNAL_CLOCK_RESAMPLER_ENABLED = new("EXTERNAL_CLOCK_RESAMPLER_ENABLED", TRSTagEnum.EXTERNAL_CLOCK_RESAMPLER_ENABLED, 0x65, "RE", false, typeof(Boolean), 1, false, "External clock resampler enabled");
        public static readonly TRSTag EXTERNAL_CLOCK_FREQUENCY = new("EXTERNAL_CLOCK_FREQUENCY", TRSTagEnum.EXTERNAL_CLOCK_FREQUENCY, 0x66, "EF", false, typeof(float), 4, 0f, "External clock frequency");
        public static readonly TRSTag EXTERNAL_CLOCK_BASE = new("EXTERNAL_CLOCK_BASE", TRSTagEnum.EXTERNAL_CLOCK_BASE, 0x67, "EB", false, typeof(int), 4, 0, "External clock time base");
        public static readonly TRSTag NUMBER_VIEW = new("NUMBER_VIEW", TRSTagEnum.NUMBER_VIEW, 0x68, "VT", false, typeof(int), 4, 0, "View number of traces: number of traces to show on opening");
        public static readonly TRSTag TRACE_OVERLAP = new("TRACE_OVERLAP", TRSTagEnum.TRACE_OVERLAP, 0x69, "OV", false, typeof(Boolean), 1, false, "Overlap: whether to overlap traces in case of multi trace view");
        public static readonly TRSTag GO_LAST_TRACE = new("GO_LAST_TRACE", TRSTagEnum.GO_LAST_TRACE, 0x6A, "GL", false, typeof(Boolean), 1, false, "Go to last trace on opening");
        public static readonly TRSTag INPUT_OFFSET = new("INPUT_OFFSET", TRSTagEnum.INPUT_OFFSET, 0x6B, "IO", false, typeof(int), 4, 0, "Input data offset in trace data");
        public static readonly TRSTag OUTPUT_OFFSET = new("OUTPUT_OFFSET", TRSTagEnum.OUTPUT_OFFSET, 0x6C, "OO", false, typeof(int), 4, 0, "Output data offset in trace data");
        public static readonly TRSTag KEY_OFFSET = new("KEY_OFFSET", TRSTagEnum.KEY_OFFSET, 0x6D, "KO", false, typeof(int), 4, 0, "Key data offset in trace data");
        public static readonly TRSTag INPUT_LENGTH = new("INPUT_LENGTH", TRSTagEnum.INPUT_LENGTH, 0x6E, "IL", false, typeof(int), 4, 0, "Input data length in trace data");
        public static readonly TRSTag OUTPUT_LENGTH = new("OUTPUT_LENGTH", TRSTagEnum.OUTPUT_LENGTH, 0x6F, "OL", false, typeof(int), 4, 0, "Output data length in trace data");
        public static readonly TRSTag KEY_LENGTH = new("KEY_LENGTH", TRSTagEnum.KEY_LENGTH, 0x70, "KL", false, typeof(int), 4, 0, "Key data length in trace data");
        public static readonly TRSTag NUMBER_OF_ENABLED_CHANNELS = new("NUMBER_OF_ENABLED_CHANNELS", TRSTagEnum.NUMBER_OF_ENABLED_CHANNELS, 0x71, "CH", false, typeof(int), 4, 0, "Number of oscilloscope channels used for measurement");
        public static readonly TRSTag NUMBER_OF_USED_OSCILLOSCOPES = new("NUMBER_OF_USED_OSCILLOSCOPES", TRSTagEnum.NUMBER_OF_USED_OSCILLOSCOPES, 0x72, "NO", false, typeof(int), 4, 0, "Number of oscilloscopes used for measurement");
        public static readonly TRSTag XY_SCAN_WIDTH = new("XY_SCAN_WIDTH", TRSTagEnum.XY_SCAN_WIDTH, 0x73, "WI", false, typeof(int), 4, 0, "Number of steps in the \"x\" direction during XY scan");
        public static readonly TRSTag XY_SCAN_HEIGHT = new("XY_SCAN_HEIGHT", TRSTagEnum.XY_SCAN_HEIGHT, 0x74, "HE", false, typeof(int), 4, 0, "Number of steps in the \"y\" direction during XY scan");
        public static readonly TRSTag XY_MEASUREMENTS_PER_SPOT = new("XY_MEASUREMENTS_PER_SPOT", TRSTagEnum.XY_MEASUREMENTS_PER_SPOT, 0x75, "ME", false, typeof(int), 4, 0, "Number of consecutive measurements done per spot during XY scan");
        public static readonly TRSTag TRACE_SET_PARAMETERS = new("TRACE_SET_PARAMETERS", TRSTagEnum.TRACE_SET_PARAMETERS, 0x76, "GP", false, typeof(TraceSetParameterMap), 0, Parameter.Traceset.UnmodifiableTraceSetParameterMap.Of(new TraceSetParameterMap()), "The set of custom global trace set parameters");
        public static readonly TRSTag TRACE_PARAMETER_DEFINITIONS = new("TRACE_PARAMETER_DEFINITIONS", TRSTagEnum.TRACE_PARAMETER_DEFINITIONS, 0x77, "LP", false, typeof(Parameter.Trace.Definition.TraceParameterDefinitionMap), 0, UnmodifiableTraceParameterDefinitionMap.Of(new Parameter.Trace.Definition.TraceParameterDefinitionMap()), "The set of custom local trace parameters");

        private static readonly TRSTag[] valueArray = new[]
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
        };

        public enum TRSTagEnum
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

        public TRSTagEnum InnerEnumValue { get; }
        private static int nextOrdinal = 0;

        private const string UNKNOWN_TAG = "Unknown tag: 0x{0:X}";


        internal TRSTag(string nameValue, TRSTagEnum innerEnum, int value, string name, bool required, Type type, int length, object defaultValue, string description)
        {
            Value = (byte)value;
            Name = name;
            Required = required;
            Type = type;
            Length = length;
            DefaultValue = defaultValue;
            Description = description;

            NameValue = nameValue;
            Ordinal = nextOrdinal++;
            InnerEnumValue = innerEnum;
        }

        public byte Value { get; }

        public string Name { get; }

        public bool Required { get; }

        public Type Type { get; }

        public int Length { get; }

        public object DefaultValue { get; }

        public string Description { get; }
        public int Ordinal { get; }

        private readonly string NameValue;

        public static TRSTag FromValue(byte value)
        {
            foreach (TRSTag tag in Values)
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
            return string.Format("{0} (value={1:X}, name={2}, {3}required, type={4}, length={5:D}, default={6}, description={7})", NameValue, Value, Name, Required ? "" : "not ", Type.FullName, Length, DefaultValue, Description);
        }

        public static TRSTag[] Values => valueArray;


        public static TRSTag ValueOf(string name)
        {
            foreach (TRSTag enumInstance in valueArray)
            {
                if (enumInstance.NameValue == name)
                {
                    return enumInstance;
                }
            }
            throw new System.ArgumentException(name);
        }
    }


}

