using System.IO;
using System.Text;

namespace FfntTool.Ffnt
{
    public class FfntEntryHeader
    {
        public const int FfntEntryHeaderSize = 12;
        public string FfntEntrySignature { get; set; }
        public int Offset { get; set; }
        public int Size { get; set; }

        public static FfntEntryHeader ReadFfntEntryHeader(Stream inputStream)
        {
            FfntEntryHeader ffntEntryHeader = new FfntEntryHeader();
            ffntEntryHeader.Read(inputStream);
            return ffntEntryHeader;
        }

        public void Read(Stream inputStream)
        {
            BinaryReader reader = new BinaryReader(inputStream, Encoding.Default, true);
            FfntEntrySignature = reader.ReadString(4);
            Offset = reader.ReadInt32();
            Size = reader.ReadInt32();
        }

        public FfntEntry ReadData(Stream inputStream)
        {
            inputStream.Position = Offset;
            FfntEntry data;
            switch (FfntEntrySignature)
            {
                case GlyphMap.GlyphSignature:
                    data = GlyphMap.ReadGlyphMap(inputStream);
                    break;
                case FontData.FontSignature:
                    data = FontData.ReadFontData(inputStream);
                    break;
                default:
                    data = null;
                    break;
            }
            return data;
        }

        public void Write(Stream outputStream)
        {
            BinaryWriter writer = new BinaryWriter(outputStream, Encoding.Default, true);
            writer.Write(Encoding.Default.GetBytes(FfntEntrySignature));
            writer.Write(Offset);
            writer.Write(Size);
        }
    }
}
