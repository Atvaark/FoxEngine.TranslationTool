using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace LangTool.Lang
{
    [XmlRoot("LangFile")]
    public class LangFile
    {
        public LangFile()
        {
            Entries = new List<LangEntry>();
        }

        [XmlArray("Entries")]
        public List<LangEntry> Entries { get; set; }

        public static LangFile ReadLangFile(Stream inputStream)
        {
            LangFile langFile = new LangFile();
            langFile.Read(inputStream);
            return langFile;
        }

        public void Read(Stream inputStream)
        {
            BinaryReader reader = new BinaryReader(inputStream, Encoding.UTF8, true);
            int magicNumber = reader.ReadInt32();
            int version = reader.ReadInt32(); // GZ 2, TPP 3
            int endianess = reader.ReadInt32(); // LE, BE
            int entryCount = reader.ReadInt32();
            int valuesOffset = reader.ReadInt32();
            int keysOffset = reader.ReadInt32();

            inputStream.Position = valuesOffset;
            Dictionary<int, LangEntry> offsetEntryDictionary = new Dictionary<int, LangEntry>();
            for (int i = 0; i < entryCount; i++)
            {
                int valuePosition = (int)inputStream.Position - valuesOffset;
                short valueConstant = reader.ReadInt16();
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
            BinaryWriter writer = new BinaryWriter(outputStream, Encoding.UTF8, true);

            long headerPosition = outputStream.Position;

            outputStream.Position += 24;
            int valuesPosition = (int) outputStream.Position;
            foreach (var entry in Entries)
            {
                entry.Offset = (int) outputStream.Position - valuesPosition;
                writer.Write((short) 0x0001);
                writer.WriteNullTerminatedString(entry.Value);
            }

            writer.AlignWrite(4, 0x00);

            int keysPosition = (int) outputStream.Position;
            foreach (var entry in Entries.OrderBy(e => e.Key).ThenByDescending(e => e.Offset))
            {
                writer.Write(entry.Key);
                writer.Write(entry.Offset);
            }

            long endPosition = outputStream.Position;

            outputStream.Position = headerPosition;
            writer.Write(0x474e414c); // LANG
            writer.Write(3);
            writer.Write(0x0000454c); // LE
            writer.Write(Entries.Count);
            writer.Write(valuesPosition);
            writer.Write(keysPosition);

            outputStream.Position = endPosition;
        }
    }
}
