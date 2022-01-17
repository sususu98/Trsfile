using System;
using System.IO;
using System.IO.MemoryMappedFiles;
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
            if (fos.Position != 0)
            {
                Console.Error.WriteLine(REWINDING_STREAM);
                fos.Position = 0;
            }
            foreach (TRSTag tag in TRSTag.Values())
            {
                if (tag.Equals(TRSTag.TRACE_BLOCK))
                {
                    continue; //TRACE BLOCK should be the last write
                }
                if (!tag.Required && metaData.hasDefaultValue(tag))
                {
                    continue; //ignore if default and not required
                }
                fos.WriteByte(tag.Value);
                if (tag.Type == typeof(string))
                {
                    string s = metaData.getString(tag);
                    byte[] stringBytes = Encoding.UTF8.GetBytes(s);
                    writeLength(fos, stringBytes.Length);
                    fos.Write(stringBytes, 0, stringBytes.Length);
                }
                else if (tag.Type == typeof(float))
                {
                    float f = metaData.getFloat(tag);
                    writeLength(fos, tag.Length);
                    for (int i = 0; i < tag.Length; i++)
                    {
                        fos.Write(BitConverter.GetBytes(f));
                    }
                }
                else if (tag.Type == typeof(bool))
                {
                    int value = metaData.getBoolean(tag) ? 1 : 0;
                    writeLength(fos, tag.Length);
                    writeInt(fos, value, tag.Length);
                }
                else if (tag.Type == typeof(int))
                {
                    writeLength(fos, tag.Length);
                    writeInt(fos, metaData.getInt(tag), tag.Length);
                }
                else if (tag.Type == typeof(TraceSetParameterMap))
                {
                    byte[] serialized = metaData.TraceSetParameters.Serialize();
                    writeLength(fos, serialized.Length);
                    fos.Write(serialized, 0, serialized.Length);
                }
                else if (tag.Type == typeof(TraceParameterDefinitionMap))
                {
                    byte[] serialized = metaData.TraceParameterDefinitions.Serialize();
                    writeLength(fos, serialized.Length);
                    fos.Write(serialized, 0, serialized.Length);
                }
                else
                {
                    throw new TRSFormatException(string.Format(UNSUPPORTED_TAG_TYPE, tag.Name, tag.Type.Name));
                }
            }
            fos.WriteByte(TRSTag.TRACE_BLOCK.Value);
            fos.WriteByte((byte)TRSTag.TRACE_BLOCK.Length); // it's safe here
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
        //ORIGINAL LINE: private static void writeInt(java.io.FileOutputStream fos, int value, int length) throws java.io.IOException
        private static void writeInt(FileStream fos, int value, int length)
        {
            for (int i = 0; i < length; i++)
            {
                fos.WriteByte((byte)(value >> (i * 8)));
            }
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
        //ORIGINAL LINE: private static void writeLength(java.io.FileOutputStream fos, long length) throws java.io.IOException
        private static void writeLength(FileStream fos, long length)
        {
            if (length > 0x7F)
            {
                int lenlen = 1 + (int)(Math.Log(length) / Math.Log(256));
                fos.WriteByte(unchecked((byte)(0x80 + lenlen)));
                for (int i = 0; i < lenlen; i++)
                {
                    fos.WriteByte((byte)(length >> (i * 8)));
                }
            }
            else
            {
                fos.WriteByte((byte)length);
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
        public static TRSMetaData readTRSMetaData(MemoryMappedViewStream buffer)
        {
            TRSMetaData trs = TRSMetaData.create();
            byte tag;

            //We keep on reading meta data until we hit tag TB=0x5f
            do
            {
                // read meta data items and put them in trs
                tag = (byte)buffer.ReadByte();
                int length = buffer.ReadByte();
                if ((length & 0x80) != 0)
                {
                    int addlen = length & 0x7F;
                    length = 0;
                    for (int i = 0; i < addlen; i++)
                    {
                        length |= (buffer.ReadByte() & 0xFF) << (i * 8);
                    }
                }
                ReadAndStoreData(buffer, tag, length, trs);
            } while (tag != TRSTag.TRACE_BLOCK.Value);
            return trs;
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
        //ORIGINAL LINE: public static String readName(com.riscure.trs.io.LittleEndianInputStream dis) throws java.io.IOException
        public static string readName(LittleEndianInputStream dis)
        {
            //Read NL
            short nameLength = dis.ReadShort();
            byte[] nameBytes = new byte[nameLength];
            int read = dis.Read(nameBytes, 0, nameLength);
            if (read != nameLength)
            {
                throw new IOException("Error reading parameter name");
            }
            //Read N
            return StringHelper.NewString(nameBytes, Encoding.UTF8);
        }

        //JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in C#:
        //ORIGINAL LINE: private static void readAndStoreData(ByteBuffer buffer, byte tag, int length, TRSMetaData trsMD) throws TRSFormatException
        private static void ReadAndStoreData(MemoryMappedViewStream buffer, byte tag, int length, TRSMetaData trsMD)
        {
            bool hasValidLength = 0 <= length && length <= 0xffff;
            TRSTag trsTag;
            try
            {
                trsTag = TRSTag.FromValue(tag);
            }
            catch (TRSFormatException)
            {
                //unknown tag, but read it anyway
                buffer.Position = buffer.Position + length;
                Console.Error.WriteLine(string.Format(IGNORED_UNKNOWN_TAG, tag));
                return;
            }
            if (!hasValidLength)
            {
                throw new TRSFormatException(string.Format(TAG_LENGTH_INVALID, trsTag.ToString(), length));
            }

            if (trsTag == TRSTag.TRACE_BLOCK)
            {
                return; //If we read TRACE_BLOCK, we've reached the end of the metadata
            }

            if (trsTag.Type == typeof(string))
            {
                trsMD.put(trsTag, ReadString(buffer, length));
            }
            else if (trsTag.Type == typeof(float))
            {
                trsMD.put(trsTag, ReadFloat(buffer));
            }
            else if (trsTag.Type == typeof(bool))
            {
                trsMD.put(trsTag, ReadBoolean(buffer));
            }
            else if (trsTag.Type == typeof(int))
            {
                trsMD.put(trsTag, ReadInt(buffer, length));
            }
            else if (trsTag.Type == typeof(TraceSetParameterMap))
            {
                trsMD.put(trsTag, ReadTraceSetParameters(buffer, length));
            }
            else if (trsTag.Type == typeof(TraceParameterDefinitionMap))
            {
                trsMD.put(trsTag, ReadTraceParameterDefinitions(buffer, length));
            }
            else
            {
                throw new TRSFormatException(string.Format(UNSUPPORTED_TAG_TYPE, trsTag.Name, trsTag.Type.Name));
            }
        }

        private static bool ReadBoolean(MemoryMappedViewStream buffer)
        {
            return (buffer.ReadByte() & 0xFF) > 0;
        }

        private static int ReadInt(MemoryMappedViewStream buffer, int length)
        {
            long result = 0;
            for (int i = 0; i < length; i++)
            {
                result += (buffer.ReadByte() & 0xFF) << (8 * i);
            }
            return (int)result;
        }

        // Always reads 4 (four) bytes
        private static float ReadFloat(MemoryMappedViewStream buffer)
        {
            int intValue = ReadInt(buffer, 4);
            return BitConverter.Int32BitsToSingle(intValue);
        }

        private static string ReadString(MemoryMappedViewStream buffer, int length)
        {
            byte[] ba = new byte[length];
            buffer.Read(ba);
            return StringHelper.NewString(ba, Encoding.UTF8);
        }

        private static TraceSetParameterMap ReadTraceSetParameters(MemoryMappedViewStream buffer, int length)
        {
            byte[] ba = new byte[length];
            buffer.Read(ba);
            return TraceSetParameterMap.Deserialize(ba);
        }

        private static TraceParameterDefinitionMap ReadTraceParameterDefinitions(MemoryMappedViewStream buffer, int length)
        {
            byte[] ba = new byte[length];
            buffer.Read(ba);
            return TraceParameterDefinitionMap.Deserialize(ba);
        }
    }

}