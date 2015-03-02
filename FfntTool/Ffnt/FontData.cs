using System.IO;
using System.Text;

namespace FfntTool.Ffnt
{
    public class FontData : FfntEntry
    {
        public const string FontSignature = "FTDT";
        public byte[] Data { get; set; }

        public static FontData ReadFontData(Stream inputStream)
        {
            FontData fontData = new FontData();
            fontData.Read(inputStream);
            return fontData;
        }

        public void Read(Stream inputStream)
        {
            BinaryReader reader = new BinaryReader(inputStream, Encoding.Default, true);
            reader.Skip(4);
            int size = reader.ReadInt32();
            reader.Skip(8);
            Data = reader.ReadBytes(size);
        }

        public override void Write(Stream outputStream)
        {
            BinaryWriter writer = new BinaryWriter(outputStream, Encoding.Default, true);
            writer.AlignWrite(16, 0x00);
            writer.WriteZeros(4);
            writer.Write(Data.Length);
            writer.WriteZeros(8);
            writer.Write(Data);
        }

        public override FfntEntryHeader GetHeader(Stream outputStream)
        {
            FfntEntryHeader header = new FfntEntryHeader
            {
                FfntEntrySignature = FontSignature,
                Offset = (int) outputStream.Position,
                Size = Data.Length + 16
            };
            return header;
        }
    }
}
