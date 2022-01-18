namespace com.riscure.trs.parameter.trace
{
    /// <summary>
    /// This is an unmodifiable version of a trace parameter map. This should always be used for a trace read back from a file.
    /// </summary>
    public class UnmodifiableTraceParameterMap : TraceParameterMap
	{
		private const string UNABLE_TO_SET_PARAMETER = "Unable to set parameter `%s` to `%s`: This trace set is in read mode and cannot be modified.";
		private const string REMOVAL_NOT_SUPPORTED_EXCEPTION = "Unable to remove parameter `%s`: This trace set is in read mode and cannot be modified.";
		private const string MODIFICATION_NOT_SUPPORTED_EXCEPTION = "Unable to modify: This trace set is in read mode and cannot be modified.";

		private const string UNABLE_TO_ADD_ALL_OF_S_THIS_TRACE_SET_IS_IN_READ_MODE_AND_CANNOT_BE_MODIFIED = "Unable to add all of `%s` : This trace set is in read mode and cannot be modified.";

		private UnmodifiableTraceParameterMap(TraceParameterMap t) : base((TraceParameterMap)t.Clone())
		{
			;
		}

		public static TraceParameterMap Of(TraceParameterMap t)
		{
			return new UnmodifiableTraceParameterMap(t);
		}

		public new TraceParameter Add(string key, TraceParameter value)
		{
			throw new System.NotSupportedException(string.Format(UNABLE_TO_SET_PARAMETER, key, value.ToString()));
		}

		public new TraceParameter Remove(string key)
		{
			throw new System.NotSupportedException(string.Format(REMOVAL_NOT_SUPPORTED_EXCEPTION, key));
		}

		public void AddRange(IDictionary<string, TraceParameter> m)
		{
			throw new System.NotSupportedException(string.Format(UNABLE_TO_ADD_ALL_OF_S_THIS_TRACE_SET_IS_IN_READ_MODE_AND_CANNOT_BE_MODIFIED, m.ToString()));
		}

		public new void Clear()
		{
			throw new System.NotSupportedException(MODIFICATION_NOT_SUPPORTED_EXCEPTION);
		}

		public new bool Remove(string key, out TraceParameter value)
		{
			throw new System.NotSupportedException(string.Format(REMOVAL_NOT_SUPPORTED_EXCEPTION, key));
		}

		public bool Replace(string key, TraceParameter oldValue, TraceParameter newValue)
		{
			throw new System.NotSupportedException(string.Format(UNABLE_TO_SET_PARAMETER, key, newValue.ToString()));
		}

		public TraceParameter Replace(string key, TraceParameter value)
		{
			throw new System.NotSupportedException(string.Format(UNABLE_TO_SET_PARAMETER, key, value.ToString()));
		}

	}

}