namespace Trsfile.Parameter.Trace.Definition
{
    /// <summary>
    /// This class represents the header definitions of all user-added local parameters of the trace set
    /// 
    /// This map is read from the file, and is therefore unmodifiable.
    /// </summary>
    public class UnmodifiableTraceParameterDefinitionMap : TraceParameterDefinitionMap
    {
        private const string UNABLE_TO_SET_PARAMETER = "Unable to set parameter `{0}` to `{1}`: This trace set is in read mode and cannot be modified.";
        private const string REMOVAL_NOT_SUPPORTED_EXCEPTION = "Unable to remove parameter `{0}`: This trace set is in read mode and cannot be modified.";
        private const string MODIFICATION_NOT_SUPPORTED_EXCEPTION = "Unable to modify: This trace set is in read mode and cannot be modified.";

        private UnmodifiableTraceParameterDefinitionMap(TraceParameterDefinitionMap t) : base((TraceParameterDefinitionMap)t.Clone())
        {

        }

        public static TraceParameterDefinitionMap Of(TraceParameterDefinitionMap t)
        {
            return new UnmodifiableTraceParameterDefinitionMap(t);
        }

        public override void Add(string key, TraceParameterDefinition value)
        {
            throw new NotSupportedException(string
                .Format(UNABLE_TO_SET_PARAMETER, key, value?.ToString()));
        }

        public override bool Remove(string key)
        {
            throw new NotSupportedException(string.Format(REMOVAL_NOT_SUPPORTED_EXCEPTION, key));
        }

        public override void Clear()
        {
            throw new NotSupportedException(MODIFICATION_NOT_SUPPORTED_EXCEPTION);
        }
        public override TraceParameterDefinition this[string key]
        {
            get => base[key];
            set => throw new NotSupportedException(UNABLE_TO_SET_PARAMETER);
        }
    }

}