using com.riscure.trs.io;

namespace com.riscure.trs.parameter.trace.definition
{

    /// <summary>
    /// This class represents the header definitions of all user-added local parameters in the trace format
    /// This explicitly implements LinkedHashMap to ensure that the data is retrieved in the same order as it was added
    /// </summary>
    public class TraceParameterDefinitionMap : Dictionary<string, TraceParameterDefinition>, ICloneable
	{

		public TraceParameterDefinitionMap() : base()
		{
		}

		public TraceParameterDefinitionMap(TraceParameterDefinitionMap toCopy) : this()
		{
			foreach (var (key, value) in toCopy)
			{
				Add(key, value with { });
			}
		}

		/// <returns> a new instance of a TraceParameterDefinitionMap containing all the same values as this one </returns>
		public object Clone()
		{
			return new TraceParameterDefinitionMap(this);
		}

		/// <summary>
		/// Sum of bytes in the map
		/// </summary>
		/// <returns>total byte size</returns>
		public virtual int TotalByteSize()
		{
			return Values.Select(definition => definition.Length * definition.Type.ByteSize).Sum();
		}

		/// <returns> this map converted to a byte array, serialized according to the TRS V2 standard definition </returns>
		/// <exception cref="RuntimeException"> if the map failed to serialize correctly </exception>
		public virtual byte[] Serialize()
		{
			MemoryStream baos = new MemoryStream();
			try
			{
                using LittleEndianOutputStream dos = new LittleEndianOutputStream(baos);
                //Write NE
                dos.writeShort(Count);
                foreach (var entry in this)
                {
                    byte[] nameBytes = entry.Key.GetBytes(System.Text.Encoding.UTF8);
                    //Write NL
                    dos.writeShort((short)nameBytes.Length);
                    //Write N
                    dos.write(nameBytes);
                    TraceParameterDefinition value = entry.Value;
                    value.Serialize(dos);
                }
                dos.flush();
                return baos.ToArray();
            }
			catch (IOException ex)
			{
				throw new Exception(ex.Message, ex);
			}
		}

		/// <param name="bytes"> a valid serialized Trace parameter definition map </param>
		/// <returns> a new populated Trace parameter definition map as represented by the provided byte array </returns>
		/// <exception cref="RuntimeException"> if the provided byte array does not represent a valid parameter definition map </exception>
		public static TraceParameterDefinitionMap Deserialize(byte[] bytes)
		{
			TraceParameterDefinitionMap result = new TraceParameterDefinitionMap();
			if (bytes != null && bytes.Length > 0)
			{
				try
				{
                    using MemoryStream bais = new MemoryStream(bytes);
                    LittleEndianInputStream dis = new LittleEndianInputStream(bais);
                    //Read NE
                    short numberOfEntries = dis.readShort();
                    for (int k = 0; k < numberOfEntries; k++)
                    {
                        string name = TRSMetaDataUtils.readName(dis);
                        //Read definition
                        result.Add(name, TraceParameterDefinition.Deserialize(dis));
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
		public static TraceParameterDefinitionMap CreateFrom(TraceParameterMap parameters)
		{
			TraceParameterDefinitionMap definitions = new ();
			if (parameters.Count != 0)
			{
				short offset = 0;
				foreach (var entry in parameters)
				{
					definitions.Add(entry.Key, new TraceParameterDefinition(entry.Value, offset));
					offset += (short)(entry.Value.Length * entry.Value.Type.ByteSize);
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