using System.Collections.Generic;

namespace com.riscure.trs.parameter.trace.definition
{
    using TraceParameter = com.riscure.trs.parameter.TraceParameter;


    /// <summary>
    /// This class represents the header definitions of all user-added local parameters of the trace set
    /// 
    /// This map is read from the file, and is therefore unmodifiable.
    /// </summary>
    public class UnmodifiableTraceParameterDefinitionMap : TraceParameterDefinitionMap
    {
        private const string UNABLE_TO_SET_PARAMETER = "Unable to set parameter `%s` to `%s`: This trace set is in read mode and cannot be modified.";
        private const string REMOVAL_NOT_SUPPORTED_EXCEPTION = "Unable to remove parameter `%s`: This trace set is in read mode and cannot be modified.";
        private const string MODIFICATION_NOT_SUPPORTED_EXCEPTION = "Unable to modify: This trace set is in read mode and cannot be modified.";

        private const string UNABLE_TO_ADD_ALL_OF_S_THIS_TRACE_SET_IS_IN_READ_MODE_AND_CANNOT_BE_MODIFIED = "Unable to add all of `%s` : This trace set is in read mode and cannot be modified.";


        private UnmodifiableTraceParameterDefinitionMap(TraceParameterDefinitionMap t) : base((TraceParameterDefinitionMap)t.Clone())
        {

        }

        public static TraceParameterDefinitionMap Of(TraceParameterDefinitionMap t)
        {
            return new UnmodifiableTraceParameterDefinitionMap(t);
        }

        public new void Add(string key, TraceParameterDefinition value)
        {
            throw new NotSupportedException(string
                .Format(UNABLE_TO_SET_PARAMETER, key, value.ToString()));
        }
        

        public void Remove(object key)
        {
            throw new NotSupportedException(string.Format(REMOVAL_NOT_SUPPORTED_EXCEPTION, key));
        }


        public new void Clear()
        {
            throw new NotSupportedException(MODIFICATION_NOT_SUPPORTED_EXCEPTION);
        }


    }

}