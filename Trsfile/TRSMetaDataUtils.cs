using System;
using System.IO;
using System.Text;

namespace com.riscure.trs
{
	using TRSTag = com.riscure.trs.enums.TRSTag;
	using LittleEndianInputStream = com.riscure.trs.io.LittleEndianInputStream;
	using TraceParameterDefinitionMap = com.riscure.trs.parameter.trace.definition.TraceParameterDefinitionMap;
	using TraceSetParameterMap = com.riscure.trs.parameter.traceset.TraceSetParameterMap;


	public class TRSMetaDataUtils
	{
		private const string IGNORED_UNKNOWN_TAG = "ignored unknown metadata tag '%02X' while reading a TRS file\n";
		private const string TAG_LENGTH_INVALID = "The length field following tag '%s' has value '%X', which is not between 0 and 0xffff";
		private const string UNSUPPORTED_TAG_TYPE = "Unsupported tag type for tag '%s': %s";
		private const string REWINDING_STREAM = "The output stream is not at the start of the file. Rewinding stream.";

		/// <summary>
		/// Writes the provided TRS metadata to the stream.
		/// </summary>
		/// <param name="fos">      the file output stream </param>
		/// <param name="metaData"> the metadata to write </param>
		/// <exception cref="IOException">        if any write error occurs </exception>
		/// <exception cref="TRSFormatException"> if the metadata contains unsupported tags </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static void writeTRSMetaData(java.io.FileOutputStream fos, TRSMetaData metaData) throws IOException, TRSFormatException
		public static void writeTRSMetaData(FileStream fos, TRSMetaData metaData)
		{
			if (fos.getChannel().position() != 0)
			{
				Console.Error.WriteLine(REWINDING_STREAM);
				fos.getChannel().position(0);
			}
			foreach (TRSTag tag in TRSTag.Values())
			{
				if (tag.Equals(TRSTag.TRACE_BLOCK))
				{
					continue; //TRACE BLOCK should be the last write
				}
				if (!tag.isRequired() && metaData.hasDefaultValue(tag))
				{
					continue; //ignore if default and not required
				}
				fos.WriteByte(tag.getValue());
				if (tag.getType() == typeof(string))
				{
					string s = metaData.getString(tag);
					sbyte[] stringBytes = s.GetBytes(Encoding.UTF8);
					writeLength(fos, stringBytes.Length);
					fos.Write(stringBytes, 0, stringBytes.Length);
				}
				else if (tag.getType() == typeof(Float))
				{
					float f = metaData.getFloat(tag);
					writeLength(fos, tag.getLength());
					writeInt(fos, Float.floatToIntBits(f), tag.getLength());
				}
				else if (tag.getType() == typeof(Boolean))
				{
					int value = metaData.getBoolean(tag) ? 1 : 0;
					writeLength(fos, tag.getLength());
					writeInt(fos, value, tag.getLength());
				}
				else if (tag.getType() == typeof(Integer))
				{
					writeLength(fos, tag.getLength());
					writeInt(fos, metaData.getInt(tag), tag.getLength());
				}
				else if (tag.getType() == typeof(TraceSetParameterMap))
				{
					sbyte[] serialized = metaData.TraceSetParameters.serialize();
					writeLength(fos, serialized.Length);
					fos.Write(serialized, 0, serialized.Length);
				}
				else if (tag.getType() == typeof(TraceParameterDefinitionMap))
				{
					sbyte[] serialized = metaData.TraceParameterDefinitions.serialize();
					writeLength(fos, serialized.Length);
					fos.Write(serialized, 0, serialized.Length);
				}
				else
				{
					throw new TRSFormatException(String.format(UNSUPPORTED_TAG_TYPE, tag.getName(), tag.getType()));
				}
			}
			fos.WriteByte(TRSTag.TRACE_BLOCK.getValue());
			fos.WriteByte(TRSTag.TRACE_BLOCK.getLength());
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private static void writeInt(java.io.FileOutputStream fos, int value, int length) throws java.io.IOException
		private static void writeInt(FileStream fos, int value, int length)
		{
			for (int i = 0; i < length; i++)
			{
				fos.WriteByte((sbyte)(value >> (i * 8)));
			}
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private static void writeLength(java.io.FileOutputStream fos, long length) throws java.io.IOException
		private static void writeLength(FileStream fos, long length)
		{
			if (length > 0x7F)
			{
				int lenlen = 1 + (int)(Math.Log(length) / Math.Log(256));
				fos.WriteByte(unchecked((sbyte)(0x80 + lenlen)));
				for (int i = 0; i < lenlen; i++)
				{
					fos.WriteByte((sbyte)(length >> (i * 8)));
				}
			}
			else
			{
				fos.WriteByte((sbyte) length);
			}
		}

		/// <summary>
		/// Reads the meta data of a TRS file. The {@code ByteBuffer} is assumed to be positioned at the start of the file; A
		/// {@code TRSFormatException} will probably be thrown otherwise, since it cannot be parsed.
		/// </summary>
		/// <param name="buffer"> The buffer which wraps the TRS file (should be positioned at the first byte of the file) </param>
		/// <returns> the meta data of a TRS file </returns>
		/// <exception cref="TRSFormatException"> If either the file is corrupt or the reader is not positioned at the start of the file </exception>
//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static TRSMetaData readTRSMetaData(ByteBuffer buffer) throws TRSFormatException
		public static TRSMetaData readTRSMetaData(ByteBuffer buffer)
		{
			TRSMetaData trs = TRSMetaData.create();
			sbyte tag;

			//We keep on reading meta data until we hit tag TB=0x5f
			do
			{
				// read meta data items and put them in trs
				tag = buffer.get();
				int length = buffer.get();
				if ((length & 0x80) != 0)
				{
					int addlen = length & 0x7F;
					length = 0;
					for (int i = 0; i < addlen; i++)
					{
						length |= (buffer.get() & 0xFF) << (i * 8);
					}
				}
				readAndStoreData(buffer, tag, length, trs);
			} while (tag != TRSTag.TRACE_BLOCK.getValue());
			return trs;
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: public static String readName(com.riscure.trs.io.LittleEndianInputStream dis) throws java.io.IOException
		public static string readName(LittleEndianInputStream dis)
		{
			//Read NL
			short nameLength = dis.readShort();
			sbyte[] nameBytes = new sbyte[nameLength];
			int read = dis.read(nameBytes, 0, nameLength);
			if (read != nameLength)
			{
				throw new IOException("Error reading parameter name");
			}
			//Read N
			return StringHelper.NewString(nameBytes, StandardCharsets.UTF_8);
		}

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
//ORIGINAL LINE: private static void readAndStoreData(ByteBuffer buffer, byte tag, int length, TRSMetaData trsMD) throws TRSFormatException
		private static void readAndStoreData(ByteBuffer buffer, sbyte tag, int length, TRSMetaData trsMD)
		{
			bool hasValidLength = (0 <= length & length <= 0xffff);
			TRSTag trsTag;
			try
			{
				trsTag = TRSTag.FromValue(tag);
			}
			catch (TRSFormatException)
			{
				//unknown tag, but read it anyway
				buffer.position(buffer.position() + length);
				System.err.printf(IGNORED_UNKNOWN_TAG, tag);
				return;
			}
			if (!hasValidLength)
			{
				throw new TRSFormatException(String.format(TAG_LENGTH_INVALID, trsTag.ToString(), length));
			}

			if (trsTag == TRSTag.TRACE_BLOCK)
			{
				return; //If we read TRACE_BLOCK, we've reached the end of the metadata
			}

			if (trsTag.getType() == typeof(string))
			{
				trsMD.put(trsTag, readString(buffer, length));
			}
			else if (trsTag.getType() == typeof(Float))
			{
				trsMD.put(trsTag, readFloat(buffer));
			}
			else if (trsTag.getType() == typeof(Boolean))
			{
				trsMD.put(trsTag, readBoolean(buffer));
			}
			else if (trsTag.getType() == typeof(Integer))
			{
				trsMD.put(trsTag, readInt(buffer, length));
			}
			else if (trsTag.getType() == typeof(TraceSetParameterMap))
			{
				trsMD.put(trsTag, readTraceSetParameters(buffer, length));
			}
			else if (trsTag.getType() == typeof(TraceParameterDefinitionMap))
			{
				trsMD.put(trsTag, readTraceParameterDefinitions(buffer, length));
			}
			else
			{
				throw new TRSFormatException(String.format(UNSUPPORTED_TAG_TYPE, trsTag.getName(), trsTag.getType()));
			}
		}

		private static bool readBoolean(ByteBuffer buffer)
		{
			return (buffer.get() & 0xFF) > 0;
		}

		private static int readInt(ByteBuffer buffer, int length)
		{
			long result = 0;
			for (int i = 0; i < length; i++)
			{
				result += (((int) buffer.get()) & 0xFF) << (8 * i);
			}
			return (int) result;
		}

		// Always reads 4 (four) bytes
		private static float readFloat(ByteBuffer buffer)
		{
			int intValue = readInt(buffer, 4);
			return Float.intBitsToFloat(intValue);
		}

		private static string readString(ByteBuffer buffer, int length)
		{
			sbyte[] ba = new sbyte[length];
			buffer.get(ba);
			return StringHelper.NewString(ba, StandardCharsets.UTF_8);
		}

		private static TraceSetParameterMap readTraceSetParameters(ByteBuffer buffer, int length)
		{
			sbyte[] ba = new sbyte[length];
			buffer.get(ba);
			return TraceSetParameterMap.deserialize(ba);
		}

		private static TraceParameterDefinitionMap readTraceParameterDefinitions(ByteBuffer buffer, int length)
		{
			sbyte[] ba = new sbyte[length];
			buffer.get(ba);
			return TraceParameterDefinitionMap.deserialize(ba);
		}
	}

}