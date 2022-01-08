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


		private UnmodifiableTraceParameterDefinitionMap(TraceParameterDefinitionMap @delegate)
		{
			base.putAll(@delegate.copy());
		}

		public static TraceParameterDefinitionMap of(TraceParameterDefinitionMap @delegate)
		{
			return new UnmodifiableTraceParameterDefinitionMap(@delegate);
		}

		public override TraceParameterDefinition<TraceParameter> put(string key, TraceParameterDefinition<TraceParameter> value)
		{
			throw new System.NotSupportedException(String.format(UNABLE_TO_SET_PARAMETER, key, value.ToString()));
		}

		public override TraceParameterDefinition<TraceParameter> remove(object key)
		{
			throw new System.NotSupportedException(String.format(REMOVAL_NOT_SUPPORTED_EXCEPTION, key));
		}

		public override void putAll<T1, T2>(IDictionary<T1, T2> m) where T1 : string where T2 : TraceParameterDefinition<com.riscure.trs.parameter.TraceParameter>
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

		public override ICollection<TraceParameterDefinition<TraceParameter>> values()
		{
			return Collections.unmodifiableCollection(base.values());
		}

		public override ISet<KeyValuePair<string, TraceParameterDefinition<TraceParameter>>> entrySet()
		{
			return Collections.unmodifiableSet(base.entrySet());
		}

//JAVA TO C# CONVERTER TODO TASK: There is no C# equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public void replaceAll(java.util.function.BiFunction<? super String, ? super TraceParameterDefinition<com.riscure.trs.parameter.TraceParameter>, ? extends TraceParameterDefinition<com.riscure.trs.parameter.TraceParameter>> function)
		public override void replaceAll<T1, T2, T3>(System.Func<T1, T2, T3> function) where T1 : TraceParameterDefinition<com.riscure.trs.parameter.TraceParameter>
		{
			throw new System.NotSupportedException(MODIFICATION_NOT_SUPPORTED_EXCEPTION);
		}

		public override TraceParameterDefinition<TraceParameter> putIfAbsent(string key, TraceParameterDefinition<TraceParameter> value)
		{
			throw new System.NotSupportedException(String.format(UNABLE_TO_SET_PARAMETER, key, value.ToString()));
		}

		public override bool remove(object key, object value)
		{
			throw new System.NotSupportedException(String.format(REMOVAL_NOT_SUPPORTED_EXCEPTION, key));
		}

		public override bool replace(string key, TraceParameterDefinition<TraceParameter> oldValue, TraceParameterDefinition<TraceParameter> newValue)
		{
			throw new System.NotSupportedException(String.format(UNABLE_TO_SET_PARAMETER, key, newValue.ToString()));
		}

		public override TraceParameterDefinition<TraceParameter> replace(string key, TraceParameterDefinition<TraceParameter> value)
		{
			throw new System.NotSupportedException(String.format(UNABLE_TO_SET_PARAMETER, key, value.ToString()));
		}

//JAVA TO C# CONVERTER TODO TASK: There is no C# equivalent to the Java 'super' constraint:
//ORIGINAL LINE: @Override public TraceParameterDefinition<com.riscure.trs.parameter.TraceParameter> merge(String key, TraceParameterDefinition<com.riscure.trs.parameter.TraceParameter> value, java.util.function.BiFunction<? super TraceParameterDefinition<com.riscure.trs.parameter.TraceParameter>, ? super TraceParameterDefinition<com.riscure.trs.parameter.TraceParameter>, ? extends TraceParameterDefinition<com.riscure.trs.parameter.TraceParameter>> remappingFunction)
		public override TraceParameterDefinition<TraceParameter> merge<T1, T2, T3>(string key, TraceParameterDefinition<TraceParameter> value, System.Func<T1, T2, T3> remappingFunction) where T1 : TraceParameterDefinition<com.riscure.trs.parameter.TraceParameter>
		{
			throw new System.NotSupportedException(MODIFICATION_NOT_SUPPORTED_EXCEPTION);
		}
	}

}