namespace Trsfile.Parameter.Traceset
{

    /// <summary>
    /// This class represents the header definitions of all user-added global parameters of the trace set
    /// 
    /// This map is read from the file, and is therefore unmodifiable.
    /// </summary>
    public class UnmodifiableTraceSetParameterMap : TraceSetParameterMap
    {
        private const string UNABLE_TO_SET_PARAMETER = "Unable to set parameter `{0}` to `[{1}]`: This trace set is in read mode and cannot be modified.";
        private const string REMOVAL_NOT_SUPPORTED_EXCEPTION = "Unable to remove parameter `{0}`: This trace set is in read mode and cannot be modified.";
        private const string MODIFICATION_NOT_SUPPORTED_EXCEPTION = "Unable to modify: This trace set is in read mode and cannot be modified.";

        private UnmodifiableTraceSetParameterMap(TraceSetParameterMap m) : base((TraceSetParameterMap)m.Clone())
        {

        }

        public static TraceSetParameterMap Of(TraceSetParameterMap m)
        {
            return new UnmodifiableTraceSetParameterMap(m);
        }

        public override void Add(string key, TraceSetParameter value)
        {
            throw new NotSupportedException(string.Format(UNABLE_TO_SET_PARAMETER, key, value.Value));
        }

        public override bool Remove(string key)
        {
            throw new NotSupportedException(string.Format(REMOVAL_NOT_SUPPORTED_EXCEPTION, key));
        }

        public override bool Remove(string key, out TraceSetParameter t)
        {
            throw new NotSupportedException(string.Format(REMOVAL_NOT_SUPPORTED_EXCEPTION, key));
        }

        public override void Clear()
        {
            throw new NotSupportedException(MODIFICATION_NOT_SUPPORTED_EXCEPTION);
        }
        public override TraceSetParameter this[string key]
        {
            get => base[key];
            set => throw new NotSupportedException(UNABLE_TO_SET_PARAMETER);
        }
    }

}