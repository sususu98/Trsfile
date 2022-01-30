using Trsfile.IO;

namespace Trsfile.Parameter.Trace.Definition
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

		public new virtual void Add(string key, TraceParameterDefinition value) 
			=> base.Add(key, value);

		public new virtual bool Remove(string key) => base.Remove(key);

		public new virtual void Clear() => base.Clear();

		/// <returns> a new instance of a TraceParameterDefinitionMap containing all the same values as this one </returns>
		public object Clone()
		{
			return new TraceParameterDefinitionMap(this);
		}

		/// <summary>
		/// Sum of bytes in the map
		/// </summary>
		/// <returns>total byte size</returns>
		public int TotalByteSize()
		{
			return Values.Select(definition => definition.Length * definition.Type.ByteSize).Sum();
		}

		/// <returns> this map converted to a byte array, serialized according to the TRS V2 standard definition </returns>
		/// <exception cref="Exception"> if the map failed to serialize correctly </exception>
		public byte[] Serialize()
		{
			MemoryStream baos = new();
			try
			{
                using LittleEndianOutputStream dos = new(baos);
                //Write NE
                dos.WriteShort(Count);
                foreach (var entry in this)
                {
					byte[] nameBytes = System.Text.Encoding.UTF8.GetBytes(entry.Key); //entry.Key.GetBytes(System.Text.Encoding.UTF8);
                    //Write NL
                    dos.WriteShort((short)nameBytes.Length);
                    //Write N
                    dos.Write(nameBytes);
                    TraceParameterDefinition value = entry.Value;
                    value.Serialize(dos);
                }
                dos.Flush();
                return baos.ToArray();
            }
			catch (IOException ex)
			{
				throw new Exception(ex.Message, ex);
			}
		}

		/// <param name="bytes"> a valid serialized Trace parameter definition map </param>
		/// <returns> a new populated Trace parameter definition map as represented by the provided byte array </returns>
		/// <exception cref="Exception"> if the provided byte array does not represent a valid parameter definition map </exception>
		public static TraceParameterDefinitionMap Deserialize(byte[] bytes)
		{
			TraceParameterDefinitionMap result = new();
			if (bytes != null && bytes.Length > 0)
			{
				try
				{
                    MemoryStream bais = new(bytes);
					using LittleEndianInputStream dis = new(bais);
                    //Read NE
                    short numberOfEntries = dis.ReadShort();
                    for (int k = 0; k < numberOfEntries; k++)
                    {
                        string name = TRSMetaDataUtils.ReadName(dis);
                        //Read definition
                        result.Add(name, TraceParameterDefinition.Deserialize(dis));
                    }
					return result;
                }
				catch (IOException ex)
				{
					throw new Exception(ex.Message, ex);
				}
			}
			return UnmodifiableTraceParameterDefinitionMap.Of(result);
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

		public override bool Equals(object? obj)
		{
			if ((object)this == obj) return true;
			if (obj is not TraceParameterDefinitionMap that)
				return false;
			if (Count != that.Count) return false;
			return this.All(e => e.Value == that[e.Key]);
		}

		public static bool operator ==(TraceParameterDefinitionMap a, TraceParameterDefinitionMap b)
			=> a.Equals(b);
		public static bool operator !=(TraceParameterDefinitionMap a, TraceParameterDefinitionMap b)
			=> !a.Equals(b);


		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

}