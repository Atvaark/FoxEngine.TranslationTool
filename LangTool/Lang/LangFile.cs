using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace LangTool.Lang
{
    [XmlRoot("LangFile")]
    public class LangFile
    {
        [XmlArray("Entries")]
        public List<LangEntry> Entries { get; set; }

        public LangFile()
        {
            Entries = new List<LangEntry>();
        }


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
            int constant2 = reader.ReadInt32();
            int endianess = reader.ReadInt32();
            int entryCount = reader.ReadInt32();
            int headerSize = reader.ReadInt32();
            int offsetKeys = reader.ReadInt32();
            int offsetValues = reader.ReadInt32();
            int padding = reader.ReadInt32();

            for (int i = 0; i < entryCount; i++)
            {
                int offsetKey = reader.ReadInt32();
                int offsetValue = reader.ReadInt32();

                long nextEntryPosition = inputStream.Position;

                inputStream.Position = offsetKeys + offsetKey;
                string key = reader.ReadNullTerminatedString();
                inputStream.Position = offsetValues + offsetValue;
                short valueConstant = reader.ReadInt16();
                string value = reader.ReadNullTerminatedString();

                Entries.Add(new LangEntry
                {
                    Key = key,
                    Value = value
                });

                inputStream.Position = nextEntryPosition;
            }


        }

        public void Write(Stream outputStream)
        {
            BinaryWriter writer = new BinaryWriter(outputStream, Encoding.UTF8, true);

            long headerPosition = outputStream.Position;

            outputStream.Position += 32;
            long entriesPosition = outputStream.Position;
            outputStream.Position += 8 * Entries.Count;
            int keysPosition = (int) outputStream.Position;

            foreach (var entry in Entries)
            {
                entry.KeyOffset = (int)(outputStream.Position - keysPosition);
                writer.WriteNullTerminatedString(entry.Key);
            }


            int valuesPosition = (int) outputStream.Position;

            foreach (var entry in Entries)
            {
                entry.ValueOffset = (int)(outputStream.Position - valuesPosition);
                writer.Write((short)1);
                writer.WriteNullTerminatedString(entry.Value);
            }

            long endPosition = outputStream.Position;

            outputStream.Position = headerPosition;

            writer.Write(1196310860);
            writer.Write(2);
            writer.Write(17740);
            writer.Write(Entries.Count);
            writer.Write(32);
            writer.Write((int)(keysPosition - headerPosition));
            writer.Write((int)(valuesPosition - headerPosition));
            writer.Write(0);

            foreach (var entry in Entries)
            {
                writer.Write(entry.KeyOffset);
                writer.Write(entry.ValueOffset);
            }

            outputStream.Position = endPosition;
        }
    }
}
