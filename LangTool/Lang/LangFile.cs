using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using LangTool.Utility;

namespace LangTool.Lang
{
    [XmlRoot("LangFile")]
    public class LangFile
    {
        private const int LittleEndianConstant = 0x0000454C; // LE
        private const int BigEndianConstant = 0x00004542; // BE

        public LangFile()
        {
            Entries = new List<LangEntry>();
        }

        [XmlArray("Entries")]
        public List<LangEntry> Entries { get; set; }

        [XmlAttribute("Endianess")]
        public Endianess Endianess { get; set; }

        public static LangFile ReadLangFile(Stream inputStream)
        {
            LangFile langFile = new LangFile();
            langFile.Read(inputStream);
            return langFile;
        }

        public void Read(Stream inputStream)
        {
            BinaryReader headerReader = new BinaryReader(inputStream, Encoding.UTF8, true);
            BinaryReader reader;
            
            int magicNumber = headerReader.ReadInt32();
            int version = headerReader.ReadInt32(); // GZ 2, TPP 3
            int endianess = headerReader.ReadInt32(); // LE, BE
            switch (endianess)
            {
                case LittleEndianConstant: // LE
                    Endianess = Endianess.LittleEndian;
                    reader = headerReader;
                    break;
                case BigEndianConstant: // BE
                    Endianess = Endianess.BigEndian;
                    version = EndianessBitConverter.FlipEndianess(version);
                    reader = new BigEndianBinaryReader(inputStream, Encoding.UTF8, true);
                    break;
                default:
                    throw new Exception(string.Format("Unknown endianess: {0:X}", endianess));
            }

            int entryCount = reader.ReadInt32();
            int valuesOffset = reader.ReadInt32();
            int keysOffset = reader.ReadInt32();

            inputStream.Position = valuesOffset;
            Dictionary<int, LangEntry> offsetEntryDictionary = new Dictionary<int, LangEntry>();
            for (int i = 0; i < entryCount; i++)
            {
                int valuePosition = (int)inputStream.Position - valuesOffset;
                short valueConstant = headerReader.ReadInt16();
                Debug.Assert(valueConstant == 1);
                string value = reader.ReadNullTerminatedString();
                offsetEntryDictionary.Add(valuePosition, new LangEntry
                {
                    Value = value
                });
            }

            inputStream.Position = keysOffset;
            for (int i = 0; i < entryCount; i++)
            {
                uint key = reader.ReadUInt32();
                int offset = reader.ReadInt32();

                offsetEntryDictionary[offset].Key = key;
            }

            Entries = offsetEntryDictionary.Values.ToList();
        }

        public void Write(Stream outputStream)
        {
            BinaryWriter headerWriter = new BinaryWriter(outputStream, Encoding.UTF8, true);
            BinaryWriter writer;
            switch (Endianess)
            {
                case Endianess.LittleEndian:
                    writer = headerWriter;
                    break;
                case Endianess.BigEndian:
                    writer = new BigEndianBinaryWriter(outputStream, Encoding.UTF8, true);
                    break;
                default:
                    throw new Exception("Unknown endianess " + Endianess);
            }
            
            long headerPosition = outputStream.Position;
            outputStream.Position += 24;

            int valuesPosition = (int)outputStream.Position;
            foreach (var entry in Entries)
            {
                entry.Offset = (int)outputStream.Position - valuesPosition;
                headerWriter.Write((short)0x0001);
                writer.WriteNullTerminatedString(entry.Value);
            }

            writer.AlignWrite(4, 0x00);

            int keysPosition = (int)outputStream.Position;
            foreach (var entry in Entries.OrderBy(e => e.Key).ThenByDescending(e => e.Offset))
            {
                writer.Write(entry.Key);
                writer.Write(entry.Offset);
            }

            long endPosition = outputStream.Position;

            outputStream.Position = headerPosition;

            headerWriter.Write(0x474e414c); // LANG
            writer.Write(0x0000003);
            headerWriter.Write(Endianess == Endianess.LittleEndian ? LittleEndianConstant : BigEndianConstant);

            writer.Write(Entries.Count);
            writer.Write(valuesPosition);
            writer.Write(keysPosition);

            outputStream.Position = endPosition;
        }
    }
}
