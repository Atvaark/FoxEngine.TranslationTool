using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FfntTool.Ffnt
{
    public class FfntFile
    {
        private const string Signature = "FXFT";
        private const short FfntHeaderSize = 10;
        private const short LittleEndianMagicNumber = 1;
        private readonly List<FfntEntry> _entries;

        public FfntFile()
        {
            _entries = new List<FfntEntry>();
        }

        public List<FfntEntry> Entries
        {
            get { return _entries; }
        }

        public static FfntFile ReadFfntFile(Stream inputStream)
        {
            FfntFile ffntFile = new FfntFile();
            ffntFile.Read(inputStream);
            return ffntFile;
        }

        public void Read(Stream inputStream)
        {
            BinaryReader reader = new BinaryReader(inputStream, Encoding.Default, true);
            string magicNumber = reader.ReadString(4);
            short endianess = reader.ReadInt16();
            byte entryCount = reader.ReadByte();
            reader.Skip(1);
            short headerSize = reader.ReadInt16();
            inputStream.AlignRead(16);
            List<FfntEntryHeader> ffntEntryHeaders = new List<FfntEntryHeader>();

            for (int i = 0; i < entryCount; i++)
            {
                ffntEntryHeaders.Add(FfntEntryHeader.ReadFfntEntryHeader(inputStream));
            }

            foreach (var header in ffntEntryHeaders)
            {
                _entries.Add(header.ReadData(inputStream));
            }
        }

        public void Write(Stream outputStream)
        {
            BinaryWriter writer = new BinaryWriter(outputStream, Encoding.Default, true);
            writer.Write(Encoding.Default.GetBytes(Signature));
            writer.Write(LittleEndianMagicNumber);
            writer.Write((byte) Entries.Count);
            writer.WriteZeros(1);
            writer.Write(FfntHeaderSize);
            writer.AlignWrite(16, 0x00);

            long entryHeaderPosition = outputStream.Position;

            outputStream.Position += Entries.Count*FfntEntryHeader.FfntEntryHeaderSize;
            writer.AlignWrite(16, 0x00);

            List<FfntEntryHeader> ffntEntryHeaders = new List<FfntEntryHeader>();

            foreach (var entry in Entries)
            {
                ffntEntryHeaders.Add(entry.GetHeader(outputStream));
                entry.Write(outputStream);
                writer.AlignWrite(16, 0x00);
            }

            outputStream.Position = entryHeaderPosition;

            foreach (var header in ffntEntryHeaders)
            {
                header.Write(outputStream);
            }
        }
    }
}
