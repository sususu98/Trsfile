using System;
using System.Collections.Generic;
using System.IO;

namespace com.riscure.trs.parameter.trace.definition
{
	using TRSMetaDataUtils = com.riscure.trs.TRSMetaDataUtils;
	using LittleEndianInputStream = com.riscure.trs.io.LittleEndianInputStream;
	using LittleEndianOutputStream = com.riscure.trs.io.LittleEndianOutputStream;
	using TraceParameter = com.riscure.trs.parameter.TraceParameter;
	using TraceParameterMap = com.riscure.trs.parameter.trace.TraceParameterMap;


	/// <summary>
	/// This class represents the header definitions of all user-added local parameters in the trace format
	/// This explicitly implements LinkedHashMap to ensure that the data is retrieved in the same order as it was added
	/// </summary>
	public class TraceParameterDefinitionMap : Dictionary<string, TraceParameterDefinition>
	{

		public TraceParameterDefinitionMap() : base()
		{
		}

		public TraceParameterDefinitionMap(TraceParameterDefinitionMap toCopy) : this()
		{
			foreach (var (key, value) in toCopy)
			{
				Add(key, value.copy());
			}
		}

		/// <returns> a new instance of a TraceParameterDefinitionMap containing all the same values as this one </returns>
		public virtual TraceParameterDefinitionMap copy()
		{
			return new TraceParameterDefinitionMap(this);
		}

		public virtual int totalSize()
		{
			return Values.Select(definition => definition.Length * definition.Type.ByteSize).Sum();
		}

		/// <returns> this map converted to a byte array, serialized according to the TRS V2 standard definition </returns>
		/// <exception cref="RuntimeException"> if the map failed to serialize correctly </exception>
		public virtual byte[] serialize()
		{
			MemoryStream baos = new MemoryStream();
			try
			{
					using (LittleEndianOutputStream dos = new LittleEndianOutputStream(baos))
					{
					//Write NE
					dos.writeShort(Count);
					foreach (KeyValuePair<string, TraceParameterDefinition> entry in entrySet())
					{
						byte[] nameBytes = entry.Key.GetBytes(System.Text.Encoding.UTF8);
						//Write NL
						dos.writeShort((short)nameBytes.Length);
						//Write N
						dos.write(nameBytes);
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in C#:
//ORIGINAL LINE: TraceParameterDefinition<? extends com.riscure.trs.parameter.TraceParameter> value = entry.getValue();
						TraceParameterDefinition value = entry.Value;
						value.serialize(dos);
					}
					dos.flush();
					return baos.ToArray();
					}
			}
			catch (IOException ex)
			{
				throw new Exception(ex.Message, ex);
			}
		}

		/// <param name="bytes"> a valid serialized Trace parameter definition map </param>
		/// <returns> a new populated Trace parameter definition map as represented by the provided byte array </returns>
		/// <exception cref="RuntimeException"> if the provided byte array does not represent a valid parameter definition map </exception>
		public static TraceParameterDefinitionMap deserialize(byte[] bytes)
		{
			TraceParameterDefinitionMap result = new TraceParameterDefinitionMap();
			if (bytes != null && bytes.Length > 0)
			{
				try
				{
						using (MemoryStream bais = new MemoryStream(bytes))
						{
						LittleEndianInputStream dis = new LittleEndianInputStream(bais);
						//Read NE
						short numberOfEntries = dis.readShort();
						for (int k = 0; k < numberOfEntries; k++)
						{
							string name = TRSMetaDataUtils.readName(dis);
							//Read definition
							result.Add(name, TraceParameterDefinition.deserialize(dis));
						}
						}
				}
				catch (IOException ex)
				{
					throw new Exception(ex.Message, ex);
				}
			}
			return UnmodifiableTraceParameterDefinitionMap.of(result);
		}

		/// <summary>
		/// Create a set of definitions based on the parameters present in a trace.
		/// </summary>
		/// <param name="parameters"> the parameters of the trace </param>
		/// <returns> a set of definitions based on the parameters present in a trace </returns>
		public static TraceParameterDefinitionMap createFrom(TraceParameterMap parameters)
		{
			TraceParameterDefinitionMap definitions = new ();
			if (parameters.Count != 0)
			{
				short offset = 0;
				foreach (var entry in parameters)
				{
					definitions.Add(entry.Key, new TraceParameterDefinition(entry.Value, offset));
					offset += (short)(entry.Value.length() * entry.Value.Type.ByteSize);
				}
			}
			return definitions;
		}

		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}

			TraceParameterDefinitionMap that = (TraceParameterDefinitionMap) obj;
			if (Count != that.Count)
			{
				return false;
			}

			return this.All(e => e.Value == that[e.Key]);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

}