using System.IO.MemoryMappedFiles;
using Trsfile.Enums;
using Trsfile.IO;
using Trsfile.Parameter.Trace.Definition;
using Trsfile.Parameter.Traceset;

namespace Trsfile
{
    public class TRSMetaDataUtils
    {
        private const string IGNORED_UNKNOWN_TAG = "ignored unknown metadata tag '{0:2X}' while reading a TRS file\n";
        private const string TAG_LENGTH_INVALID = "The length field following tag '{0}' has value '{1}', which is not between 0 and 0xffff";
        private const string UNSUPPORTED_TAG_TYPE = "Unsupported tag type for tag '{0}': {1}";
        private const string REWINDING_STREAM = "The output stream is not at the start of the file. Rewinding stream.";

        /// <summary>
        /// Writes the provided TRS metadata to the stream.
        /// </summary>
        /// <param name="fos">      the file output stream </param>
        /// <param name="metaData"> the metadata to write </param>
        public static void WriteTRSMetaData(FileStream fos, TRSMetaData metaData)
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
                if (!tag.Required && metaData.HasDefaultValue(tag))
                {
                    continue; //ignore if default and not required
                }
                fos.WriteByte(tag.Value);
                if (tag.Type == typeof(string))
                {
                    string s = metaData.GetString(tag);
                    byte[] stringBytes = System.Text.Encoding.UTF8.GetBytes(s);
                    WriteLength(fos, stringBytes.Length);
                    fos.Write(stringBytes, 0, stringBytes.Length);
                }
                else if (tag.Type == typeof(float))
                {
                    float f = metaData.GetFloat(tag);
                    WriteLength(fos, tag.Length);
                    for (int i = 0; i < tag.Length; i++)
                    {
                        fos.Write(BitConverter.GetBytes(f));
                    }
                }
                else if (tag.Type == typeof(bool))
                {
                    int value = metaData.GetBoolean(tag) ? 1 : 0;
                    WriteLength(fos, tag.Length);
                    WriteInt(fos, value, tag.Length);
                }
                else if (tag.Type == typeof(int))
                {
                    WriteLength(fos, tag.Length);
                    WriteInt(fos, metaData.GetInt(tag), tag.Length);
                }
                else if (tag.Type == typeof(TraceSetParameterMap))
                {
                    byte[] serialized = metaData.TraceSetParameters.Serialize();
                    WriteLength(fos, serialized.Length);
                    fos.Write(serialized, 0, serialized.Length);
                }
                else if (tag.Type == typeof(TraceParameterDefinitionMap))
                {
                    byte[] serialized = metaData.TraceParameterDefinitions.Serialize();
                    WriteLength(fos, serialized.Length);
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
        private static void WriteInt(FileStream fos, int value, int length)
        {
            for (int i = 0; i < length; i++)
            {
                fos.WriteByte((byte)(value >> (i * 8)));
            }
        }

        private static void WriteLength(FileStream fos, long length)
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
        public static TRSMetaData ReadTRSMetaData(MemoryMappedViewStream buffer)
        {
            TRSMetaData trs = TRSMetaData.Create();
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

        public static string ReadName(LittleEndianInputStream dis)
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
            return StringHelper.NewString(nameBytes, System.Text.Encoding.UTF8);
        }


        private static void ReadAndStoreData(MemoryMappedViewStream buffer, byte tag, int length, TRSMetaData trsMD)
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
                buffer.Position += length;
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
                trsMD.Add(trsTag, ReadString(buffer, length));
            }
            else if (trsTag.Type == typeof(float))
            {
                trsMD.Add(trsTag, ReadFloat(buffer));
            }
            else if (trsTag.Type == typeof(bool))
            {
                trsMD.Add(trsTag, ReadBoolean(buffer));
            }
            else if (trsTag.Type == typeof(int))
            {
                trsMD.Add(trsTag, ReadInt(buffer, length));

            }
            else if (trsTag.Type == typeof(TraceSetParameterMap))
            {
                trsMD.Add(trsTag, ReadTraceSetParameters(buffer, length));
            }
            else if (trsTag.Type == typeof(TraceParameterDefinitionMap))
            {
                trsMD.Add(trsTag, ReadTraceParameterDefinitions(buffer, length));
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
            return StringHelper.NewString(ba, System.Text.Encoding.UTF8);
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