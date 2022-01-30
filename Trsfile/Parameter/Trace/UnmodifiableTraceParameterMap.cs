namespace Trsfile.Parameter.Trace
{
    /// <summary>
    /// This is an unmodifiable version of a trace parameter map. This should always be used for a trace read back from a file.
    /// </summary>
    public class UnmodifiableTraceParameterMap : TraceParameterMap
    {
        private const string UNABLE_TO_SET_PARAMETER = "Unable to set parameter `{0}` to `[{1}]`: This trace set is in read mode and cannot be modified.";
        private const string REMOVAL_NOT_SUPPORTED_EXCEPTION = "Unable to remove parameter `{0}`: This trace set is in read mode and cannot be modified.";
        private const string MODIFICATION_NOT_SUPPORTED_EXCEPTION = "Unable to modify: This trace set is in read mode and cannot be modified.";

        private UnmodifiableTraceParameterMap(TraceParameterMap t) : base((TraceParameterMap)t.Clone())
        {
            ;
        }

        public static TraceParameterMap Of(TraceParameterMap t)
        {
            return new UnmodifiableTraceParameterMap(t);
        }

        public override void Add(string key, TraceParameter value)
        {
            throw new NotSupportedException(string.Format(UNABLE_TO_SET_PARAMETER, key, value.ToString()));
        }

        public override void Clear()
        {
            throw new NotSupportedException(MODIFICATION_NOT_SUPPORTED_EXCEPTION);
        }

        public override bool Remove(string key)
        {
            throw new NotSupportedException(string.Format(REMOVAL_NOT_SUPPORTED_EXCEPTION, key));
        }

        public override bool Remove(string key, out TraceParameter t)
        {
            throw new NotSupportedException(string.Format(REMOVAL_NOT_SUPPORTED_EXCEPTION, key));
        }
        public override TraceParameter this[string key]
        {
            get => base[key];
            set => throw new NotSupportedException(string.Format(UNABLE_TO_SET_PARAMETER, key, value.ToString()));
        }

        public override object Clone() => new UnmodifiableTraceParameterMap(this);
    }

}