using System.Collections.Generic;

namespace com.riscure.trs.parameter.trace
{
	using TraceParameter = com.riscure.trs.parameter.TraceParameter;


	/// <summary>
	/// This is an unmodifiable version of a trace parameter map. This should always be used for a trace read back from a file.
	/// </summary>
	public class UnmodifiableTraceParameterMap : TraceParameterMap
	{
		private const string UNABLE_TO_SET_PARAMETER = "Unable to set parameter `%s` to `%s`: This trace set is in read mode and cannot be modified.";
		private const string REMOVAL_NOT_SUPPORTED_EXCEPTION = "Unable to remove parameter `%s`: This trace set is in read mode and cannot be modified.";
		private const string MODIFICATION_NOT_SUPPORTED_EXCEPTION = "Unable to modify: This trace set is in read mode and cannot be modified.";

		private const string UNABLE_TO_ADD_ALL_OF_S_THIS_TRACE_SET_IS_IN_READ_MODE_AND_CANNOT_BE_MODIFIED = "Unable to add all of `%s` : This trace set is in read mode and cannot be modified.";

		private UnmodifiableTraceParameterMap(TraceParameterMap @delegate)
		{
			base.putAll(@delegate.copy());
		}

		public static TraceParameterMap of(TraceParameterMap @delegate)
		{
			return new UnmodifiableTraceParameterMap(@delegate);
		}

		public override TraceParameter put(string key, TraceParameter value)
		{
			throw new System.NotSupportedException(String.format(UNABLE_TO_SET_PARAMETER, key, value.ToString()));
		}

		public override TraceParameter remove(object key)
		{
			throw new System.NotSupportedException(String.format(REMOVAL_NOT_SUPPORTED_EXCEPTION, key));
		}

		public override void putAll<T1, T2>(IDictionary<T1, T2> m) where T1 : string where T2 : com.riscure.trs.parameter.TraceParameter
		{
			throw new System.NotSupportedException(String.format(UNABLE_TO_ADD_ALL_OF_S_THIS_TRACE_SET_IS_IN_READ_MODE_AND_CANNOT_BE_MODIFIED, m.ToString()));
		}

		public override void clear()
		{
			throw new System.NotSupportedException(MODIFICATION_NOT_SUPPORTED_EXCEPTION);
		}

		public override ISet<string> keySet()
		{
			return Collections.unmodifiableSet(base.keySet());
		}

		public override ICollection<TraceParameter> values()
		{
			return Collections.unmodifiableCollection(base.values());
		}

		public override ISet<KeyValuePair<string, TraceParameter>> entrySet()
		{
			return Collections.unmodifiableSet(base.entrySet());
		}

//JAVA TO C# CONVERTER TODO TASK: There is no C# equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void replaceAll(java.util.function.BiFunction<? super String, ? super com.riscure.trs.parameter.TraceParameter, ? extends com.riscure.trs.parameter.TraceParameter> function)
		public override void replaceAll<T1, T2, T3>(System.Func<T1, T2, T3> function) where T1 : com.riscure.trs.parameter.TraceParameter
		{
			throw new System.NotSupportedException(MODIFICATION_NOT_SUPPORTED_EXCEPTION);
		}

		public override TraceParameter putIfAbsent(string key, TraceParameter value)
		{
			throw new System.NotSupportedException(String.format(UNABLE_TO_SET_PARAMETER, key, value.ToString()));
		}

		public override bool remove(object key, object value)
		{
			throw new System.NotSupportedException(String.format(REMOVAL_NOT_SUPPORTED_EXCEPTION, key));
		}

		public override bool replace(string key, TraceParameter oldValue, TraceParameter newValue)
		{
			throw new System.NotSupportedException(String.format(UNABLE_TO_SET_PARAMETER, key, newValue.ToString()));
		}

		public override TraceParameter replace(string key, TraceParameter value)
		{
			throw new System.NotSupportedException(String.format(UNABLE_TO_SET_PARAMETER, key, value.ToString()));
		}

//JAVA TO C# CONVERTER TODO TASK: There is no C# equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public com.riscure.trs.parameter.TraceParameter merge(String key, com.riscure.trs.parameter.TraceParameter value, java.util.function.BiFunction<? super com.riscure.trs.parameter.TraceParameter, ? super com.riscure.trs.parameter.TraceParameter, ? extends com.riscure.trs.parameter.TraceParameter> remappingFunction)
		public override TraceParameter merge<T1, T2, T3>(string key, TraceParameter value, System.Func<T1, T2, T3> remappingFunction) where T1 : com.riscure.trs.parameter.TraceParameter
		{
			throw new System.NotSupportedException(MODIFICATION_NOT_SUPPORTED_EXCEPTION);
		}
	}

}